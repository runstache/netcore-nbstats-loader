using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class InjuredPlayer
    {

        [JsonProperty(PropertyName = "playerName")]
        public string PlayerName { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "position")]
        public string Position { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

    }
}
