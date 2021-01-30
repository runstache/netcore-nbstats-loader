using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class LineScore
    {
        [JsonProperty(PropertyName = "quarter")]
        public int Quarter { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }
    }
}
