using DotRealDb.AspNetCore.Hubs;
using DotRealDb.AspNetCore.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore
{
    public class DotRealChangeTracker : IDotRealChangeTracker
    {
        private readonly IHubContext<DotRealHub, IDotRealHubClient> hubContext;

        public DotRealChangeTracker(IHubContext<DotRealHub, IDotRealHubClient> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async void TrackAndPublish(DbContext dbContext)
        {
            await PublishAsync(dbContext);
        }

        public Task TrackAndPublishAsync(DbContext dbContext)
        {
            TrackAndPublish(dbContext);
            return Task.CompletedTask;
        }

        private async Task PublishAsync(DbContext dbContext)
        {
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                var groupName = dbContext.GetType().Name + "_" + entry.Entity.GetType().Name;
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        await hubContext.Clients.Group(groupName).Deleted(entry.Entity);
                        break;
                    case EntityState.Modified:
                        await hubContext.Clients.Group(groupName).Modified(entry.Entity);
                        break;
                    case EntityState.Added:
                        await hubContext.Clients.Group(groupName).Added(entry.Entity);
                        break;
                }
            }
        }
    }
}
