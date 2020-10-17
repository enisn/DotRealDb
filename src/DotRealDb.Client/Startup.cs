using DotRealDb.Client.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotRealDb.Client
{
    public static class Startup
    {
        public static IServiceCollection AddDotRealDbClient(this IServiceCollection services, Action<DotRealDbClientOptions> configureAction)
        {
            var options = new DotRealDbClientOptions();
            configureAction?.Invoke(options);
            services.AddSingleton(options);
            services.AddTransient<IDotRealChangeHandler, DotRealChangeHandler>();

            return services;
        }
    }
}
