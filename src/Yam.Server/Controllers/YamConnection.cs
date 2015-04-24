using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Yam.Server.Controllers
{
    public class YamConnection : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            var parts = data.Split(':');
            if (parts[0] == "set-world" && parts.Length == 2)
            {
                var worldId = parts[1];
                Groups.Add(connectionId, worldId);
            }

            return base.OnReceived(request, connectionId, data);
        }

        public void SendNotification(int worldId)
        {
            GetContext().Groups.Send(worldId.ToString(), "notify");
        }

        private static IPersistentConnectionContext GetContext()
        {
            return GlobalHost.ConnectionManager.GetConnectionContext<YamConnection>();
        }
    }
}