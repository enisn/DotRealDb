using DotRealDb.Client.Configurations;
using DotRealDb.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.Client
{
    public class DotRealChangeHandler : IDotRealChangeHandler
    {
        private readonly HubConnection connection;
        public DotRealChangeHandler(DotRealDbClientOptions options)
        {
            var builder = new HubConnectionBuilder()
                .WithUrl(options.ServerBaseUrl + "/hubs/DotRealHub")
                .WithAutomaticReconnect();

            options?.ConfigureBuilder?.Invoke(builder);

            connection = builder.Build();
        }

        [SuppressMessage("Wrong Usage", "DF0001:Marks undisposed anonymous objects from method invocations.", Justification = "<Pending>")]
        public async Task StartTrackingAsync<T>(IList<T> source, string dbContextName, string entityName = null)
        {
            if (connection.State != HubConnectionState.Connected)
                await connection.StartAsync();

            await connection.SendAsync("ConnectToEntity", dbContextName, (entityName ?? typeof(T).Name));

            connection.On<T>("Added", entity => source.Add(entity));
            connection.On<T>("Deleted", entity => source.Remove(entity));
            connection.On<T>("Modified", entity =>
            {
                var existing = source.FirstOrDefault(x => x.Equals(entity));
                if (existing == null)
                    return;

                existing.CopyPropertiesFrom(entity);
            });
        }
    }
}
