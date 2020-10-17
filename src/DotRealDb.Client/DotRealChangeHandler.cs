using DotRealDb.Client.Configurations;
using DotRealDb.Client.Extensions;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.Client
{
    public class DotRealChangeHandler : IDotRealChangeHandler
    {
        private readonly HubConnection connection;
        private readonly DotRealDbClientOptions options;

        public DotRealChangeHandler(DotRealDbClientOptions options)
        {
            this.options = options;

            var builder = new HubConnectionBuilder()
                .WithUrl(options.ServerBaseUrl + "/hubs/DotRealHub")
                .WithAutomaticReconnect();

            options?.ConfigureBuilder?.Invoke(builder);

            connection = builder.Build();
        }

        public async Task<ObservableCollection<T>> ConnectAndTrackAsync<T>(string dbContextName, string entityName = null, uint limit = 32)
        {
            using (var client = new HttpClient())
            {
                foreach (var header in options.HttpHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var json = await client.GetStringAsync($"{options.ServerBaseUrl}/_api/{dbContextName}/{entityName}?page=1&perPage={limit}");

                var items = JsonConvert.DeserializeObject<ObservableCollection<T>>(json);
                await StartTrackingAsync(items, dbContextName, entityName);
                return items;
            }
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
