using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore
{
    public class DotRealDbContext : DbContext
    {
        private readonly IDotRealChangeTracker tracker;

        protected DotRealDbContext() : base()
        {
        }

        public DotRealDbContext(DbContextOptions options, IDotRealChangeTracker tracker) : base(options)
        {
            this.tracker = tracker;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            tracker.TrackAndPublish(this);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            await tracker.TrackAndPublishAsync(this);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
