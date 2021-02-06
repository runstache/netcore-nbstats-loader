using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class TeamStatCollection
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "stats")]
        public List<TeamStatEntry> Stats { get; set; }

        public TeamStatCollection()
        {
            Stats = new List<TeamStatEntry>();
        }

        public void AddStat(TeamStatEntry stat)
        {
            Stats.Add(stat);
        }
    }
}
