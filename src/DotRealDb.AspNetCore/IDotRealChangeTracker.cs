using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore
{
    public interface IDotRealChangeTracker
    {
        
        Task<int> TrackAndPublishSaveChangesAsync(DbContext dbContext, bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default);
        int TrackAndPublishSaveChanges(DbContext dbContext, bool acceptAllChangesOnSuccess = true);
    }
}
