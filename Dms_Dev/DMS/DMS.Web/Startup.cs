using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DMS.Web.Startup))]
namespace DMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
