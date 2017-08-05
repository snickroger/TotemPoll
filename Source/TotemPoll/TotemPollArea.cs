using System;
using System.ComponentModel.Composition;
using Autofac;
using Nancy.Authentication.Basic;
using Totem.Http;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Timeline;
using Totem.Web;
using Totem.Web.Push;
using TotemPoll.Web;

namespace TotemPoll
{
  [DeployedResource("UI")]
  public class TotemPollArea : RuntimeArea
  {
    protected override void RegisterArea()
    {
      RegisterType<TotemPollApi>().InstancePerRequest();
      RegisterType<TotemPollSecureApi>().InstancePerRequest();

      Register(c => new TotemPollApp(new WebUIContext(
        HttpLink.From("http://+:8080/"),
        c.Resolve<ILifetimeScope>(),
        enableCors: false,
        contentFolder: ReadContentFolder())))
      .As<IWebApp>()
      .SingleInstance();

      Register(c => new PushApp(
        new WebAppContext(ReadPushBinding(), c.Resolve<ILifetimeScope>(), enableCors: true),
        enableDetailedErrors: true))
      .As<IWebApp>()
      .SingleInstance();

      Register(c => new ViewExchange(
        c.Resolve<IViewDb>(),
        c.Resolve<IPushChannel>(),
        TimeSpan.FromMilliseconds(300)))
      .As<IViewExchange>()
      .SingleInstance();

      Register(c => new MockUserValidator()).As<IMockUserValidator, IUserValidator>().SingleInstance();
      Register(c => new MockUserDb(c.Resolve<IMockUserValidator>())).As<IWebUserDb>().SingleInstance();

      RegisterType<TimelineScope>().As<ITimelineScope>().SingleInstance();
      RegisterType<MemoryTimelineDb>().As<MemoryTimelineDb, ITimelineDb, IViewDb>().SingleInstance();
    }

    public override IConnectable Compose(ILifetimeScope scope)
    {
      return scope.Resolve<ITimelineScope>();
    }

    private FolderLink ReadContentFolder() =>
          AreaType.Package.DeploymentFolder.Link.Then(FolderResource.From("UI"));

    private HttpLink ReadPushBinding() => HttpLink.From("http://+:8080/push");
  }
}
