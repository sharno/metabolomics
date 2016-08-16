using System.Web.Http;
using Metabol.Api;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Metabol.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureAuth(app);

            //app.UseWebApi(GlobalConfiguration.Configuration);
        }
    }
}
