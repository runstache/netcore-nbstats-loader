using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NbaStats.Loader.DataObject
{
    public class GameEntry
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "game")]
        public GameInfo Game { get; set; }

        [JsonProperty(PropertyName = "awayPlayers")]
        public List<PlayerEntry> AwayPlayers { get; set; }

        [JsonProperty(PropertyName = "homePlayers")]
        public List<PlayerEntry> HomePlayers { get; set; }

        [JsonProperty(PropertyName = "teams")]
        public List<TeamStat> TeamStats { get; set; }
    }
}
