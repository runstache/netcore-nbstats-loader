using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class InjuryEntry
    {
        [JsonProperty(PropertyName = "name")]
        public string TeamName { get; set; }

        [JsonProperty(PropertyName = "injuries")]
        public List<InjuredPlayer> Players { get; set; }

        public InjuryEntry()
        {
            Players = new List<InjuredPlayer>();
        }

    }
}
