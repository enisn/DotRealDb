using DotRealDb.AspNetCore.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DotRealDb.AspNetCore
{
    public static class Startup
    {
        public static IServiceCollection AddDotRealDb(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddTransient<IDotRealChangeTracker, DotRealChangeTracker>();
            return services;
        }

        public static IEndpointRouteBuilder MapDotRealDbHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<DotRealHub>("/hubs/DotRealHub");
            return endpoints;
        }
    }
}
