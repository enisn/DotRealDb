using DotRealDb.AspNetCore.Hubs;
using DotRealDb.AspNetCore.Hubs.Clients;
using DotRealDb.AspNetCore.Internals;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
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


        public async Task<T> TrackAndPublishAfterAsync<T>(DbContext dbContext, Func<Task<T>> saveChanges)
        {
            var entries = dbContext.ChangeTracker.Entries().Select(EntrySelector).ToArray();
            var result = await saveChanges();
            Publish(entries);
            return result;
        }

        public T TrackAndPublishAfter<T>(DbContext dbContext, Func<T> saveChanges)
        {
            var entries = dbContext.ChangeTracker.Entries().Select(EntrySelector).ToArray();
            var result = saveChanges();
            Publish(entries);
            return result;
        }

        private EntityStateSummary EntrySelector(EntityEntry s, int i) => new EntityStateSummary(s.Entity.GetType().Name, s.Context.GetType().Name, s.Entity, s.State);

        private async void Publish(EntityStateSummary[] entries)
        {
            await PublishAsync(entries);
        }

        private async Task PublishAsync(EntityStateSummary[] entries)
        {
            foreach (var entry in entries)
            {
                var groupName = entry.DbContextName + "_" + entry.EntityName;
                switch (entry.EntityState)
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
