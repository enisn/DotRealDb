using DotRealDb.AspNetCore.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore.Hubs
{
    public class DotRealHub : Hub<IDotRealHubClient>
    {
        public async Task ConnectToEntity(string dbContextName, string entityName)
        {
            var groupName = dbContextName + "_" + entityName;

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
