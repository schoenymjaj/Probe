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
        }
    }
}
