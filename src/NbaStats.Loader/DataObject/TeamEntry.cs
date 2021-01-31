using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class TeamEntry
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "lineScores")]
        public List<LineScore> LineScores { get; set; }

        public TeamEntry()
        {
            LineScores = new List<LineScore>();
        }

        public void AddLineScore(LineScore score)
        {
            LineScores.Add(score);
        }
    }
}
