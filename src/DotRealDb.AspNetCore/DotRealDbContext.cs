using Microsoft.EntityFrameworkCore;
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
            return tracker.TrackAndPublishAfter(this, () => base.SaveChanges(acceptAllChangesOnSuccess));
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await tracker.TrackAndPublishAfterAsync(this, () => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
        }
    }
}
