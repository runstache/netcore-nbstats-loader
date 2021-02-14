using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class RosterItem
    {
        [JsonProperty(PropertyName = "teamCode")]
        public string TeamCode { get; set; }

        [JsonProperty(PropertyName = "players")]
        public List<string> Players { get; set; }

        public RosterItem()
        {
            Players = new List<string>();
        }
    }
}
