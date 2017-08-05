using System;
using Topshelf;
using Topshelf.HostConfigurators;
using Totem.Runtime.Hosting;

namespace TotemPoll
{
  internal class Program : IRuntimeProgram
  {
    [STAThread]
    static int Main(string[] args)
    {
      return RuntimeHost.Run<Program>(args);
    }

    public void ConfigureHost(HostConfigurator host)
    {
      host.SetServiceName("TotemPoll");
      host.SetDisplayName("TotemPoll");
      host.SetDescription("Hosts the elements of a simple polling application using Totem");

      host.RunAsPrompt();

      host.StartAutomatically();

      host.SetStartTimeout(TimeSpan.FromSeconds(10));
      host.SetStopTimeout(TimeSpan.FromSeconds(10));
    }

  }
}
