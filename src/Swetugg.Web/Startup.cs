using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Swetugg.Web.Startup))]
namespace Swetugg.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
