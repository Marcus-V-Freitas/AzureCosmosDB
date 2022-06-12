using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCosmosDB
{

    public class Location
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Volcano
    {
        [JsonProperty("Volcano Name")]
        public string VolcanoName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public Location Location { get; set; }
        public int? Elevation { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        [JsonProperty("Last Known Eruption")]
        public string LastKnownEruption { get; set; }
        public string id { get; set; }
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }
    }
}
