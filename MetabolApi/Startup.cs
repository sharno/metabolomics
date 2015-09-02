using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MetabolApi.Startup))]

namespace MetabolApi
{
    using Microsoft.AspNet.SignalR;

    using Newtonsoft.Json;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR();
            var config = new HubConfiguration { EnableDetailedErrors = true };
            app.MapSignalR("/signalr", config);
        }
    }
}
