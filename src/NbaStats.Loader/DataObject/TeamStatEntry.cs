using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class TeamStatEntry
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        public TeamStatEntry()
        {

        }

        public TeamStatEntry(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
