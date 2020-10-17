using AutoFilterer.Types;
using DotRealDb.AspNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/_api/{dbContext}/{entityName}")]
    public class DotRealController : ControllerBase
    {
        private readonly ITypeFromNameResolver resolver;

        public DotRealController(ITypeFromNameResolver resolver)
        {
            this.resolver = resolver;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string dbContext, string entityName, [FromQuery]PaginationFilterBase filter)
        {
            var context = resolver.Resolve<DbContext>(dbContext);

            var entityType = resolver.GetTypeFromName(entityName);

            var dbSetProperty = context.GetType().GetProperties().FirstOrDefault(x => x.PropertyType == typeof(DbSet<>).MakeGenericType(entityType));

            dynamic dbSet = dbSetProperty.GetValue(context);

            var filtered = filter.ApplyFilterTo(dbSet);

            return Ok(await EntityFrameworkQueryableExtensions.ToListAsync(filtered));
        }
    }
}
