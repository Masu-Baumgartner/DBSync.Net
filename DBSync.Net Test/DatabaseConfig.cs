using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net_Test
{
    internal class DatabaseConfig
    {
        internal static DatabaseConfig Get
        {
            get
            {
                return JsonConvert.DeserializeObject<DatabaseConfig>(File.ReadAllText("../../../testing.json"));
            }
        }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("pwd")]
        public string Pwd { get; set; }

        [JsonProperty("database")]
        public string Database { get; set; }
    }
}
