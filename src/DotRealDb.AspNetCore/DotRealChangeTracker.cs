﻿using DotRealDb.AspNetCore.Hubs;
using DotRealDb.AspNetCore.Hubs.Clients;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public int TrackAndPublishSaveChanges(DbContext dbContext, bool acceptAllChangesOnSuccess = true)
        {
            var entries = dbContext.ChangeTracker.Entries().ToArray();
            var result = dbContext.SaveChanges(acceptAllChangesOnSuccess);
            Publish(entries);
            return result;
        }

        public async Task<int> TrackAndPublishSaveChangesAsync(DbContext dbContext, bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default)
        {
            var entries = dbContext.ChangeTracker.Entries().ToArray();
            var result = await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            Publish(entries);
            return result;
        }

        private async void Publish(EntityEntry[] entries)
        {
            await PublishAsync(entries);
        }

        private async Task PublishAsync(EntityEntry[] entries)
        {
            foreach (var entry in entries)
            {
                var groupName = entry.Context.GetType().Name + "_" + entry.Entity.GetType().Name;
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
