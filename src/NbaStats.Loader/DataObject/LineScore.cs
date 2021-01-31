using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class LineScore
    {
        [JsonProperty(PropertyName = "quarter")]
        public string Quarter { get; set; }

        [JsonProperty(PropertyName = "score")]
        public string Score { get; set; }

        public LineScore()
        {

        }

        public LineScore(string quarter, string score)
        {
            Quarter = quarter;
            Score = score;
        }
    }


}
