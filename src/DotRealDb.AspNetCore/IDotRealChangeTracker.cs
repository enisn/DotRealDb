using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore
{
    public interface IDotRealChangeTracker
    {

        Task<T> TrackAndPublishAfterAsync<T>(DbContext dbContext, Func<Task<T>> saveChanges);
        T TrackAndPublishAfter<T>(DbContext dbContext, Func<T> saveChanges);
    }
}
