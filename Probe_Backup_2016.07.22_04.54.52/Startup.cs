using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Probe.Startup))]
namespace Probe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR(); //ATTENTION:THIS NEEDS TO BE BELOW ConfigureAuth(app) - MNS

        }
    }
}
