using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Yammerly.Service.Startup))]

namespace Yammerly.Service
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}