using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class GameInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "teams")]
        public List<TeamEntry> Teams { get; set; }

        public GameInfo()
        {
            Teams = new List<TeamEntry>();            
        }
    }
}
