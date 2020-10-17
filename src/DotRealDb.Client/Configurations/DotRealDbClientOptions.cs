using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.Client.Configurations
{
    public class DotRealDbClientOptions
    {
        public string ServerBaseUrl { get; set; }

        public Action<IHubConnectionBuilder> ConfigureBuilder { get; set; }
    }
}
