using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Yam.Server.Controllers;

[assembly: OwinStartup(typeof(Yam.Server.App_Start.SignalRConfig))]

namespace Yam.Server.App_Start
{
    public class SignalRConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR<YamConnection>("/api/notifications");
        }
    }
}
