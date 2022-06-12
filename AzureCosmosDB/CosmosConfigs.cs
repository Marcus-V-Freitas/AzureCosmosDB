using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB
{
    public class CosmosConfigs
    {
        private readonly List<Volcano> _volcanos;

        public CosmosConfigs()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "VolcanoData.json");
            _volcanos = JsonConvert.DeserializeObject<List<Volcano>>(File.ReadAllText(file));
        }       

        public string AccountEndpoint { get; set; }
        public string AuthKeyOrResourceToken { get; set; }
        public string Database { get; set; }
        public string Container { get; set; }
        public string ContainerLease { get; set; }
        public string PartitionKey { get; set; }
        public List<Volcano> Volcanos => _volcanos;
    }
}
