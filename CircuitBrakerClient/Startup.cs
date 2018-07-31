using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CircuitBrakerClient.Startup))]
namespace CircuitBrakerClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
