using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class PlayerEntry
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "position")]
        public string Position { get; set; }

        [JsonProperty(PropertyName = "fullUrl")]
        public string FullUrl { get; set; }

        [JsonProperty(PropertyName = "team")]
        public string Team {get; set;}

        [JsonProperty(PropertyName = "stats")]
        public List<StatEntry> Stats { get; set; }

        public PlayerEntry()
        {
            Stats = new List<StatEntry>();
        }

    }
}
