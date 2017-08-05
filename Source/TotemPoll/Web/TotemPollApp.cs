using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Bootstrapper;
using Totem.Web;

namespace TotemPoll.Web
{
  public class TotemPollApp : WebUIApp
  {
    public TotemPollApp(WebUIContext context) : base(context)
		{ }

    protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
    {
      return
        from region in Runtime.Regions
        from package in region.Packages
        from webApi in package.WebApis
        select (INancyModule)container.Resolve(webApi.DeclaredType);
    }

    protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
    {
      base.RequestStartup(container, pipelines, context);
      pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(container.Resolve<IUserValidator>(), "Totem Poll"));
    }

  }
}
