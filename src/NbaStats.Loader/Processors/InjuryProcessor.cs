using NbaStats.Data.DataObjects;
using NbaStats.Data.Engines;
using NbaStats.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Loader.DataObject;
using System.Linq;
using NbaStats.Loader.Helpers;
using Microsoft.Extensions.Logging;

namespace NbaStats.Loader.Processors
{
    public class InjuryProcessor : IProcessor<InjuryEntry>
    {
        private readonly IStatEngine<Injury> injuryEngine;
        private readonly IStatEngine<Player> playerEngine;
        private readonly ILogger logger;

        public InjuryProcessor(IRepository repo, ILogger logger)
        {
            injuryEngine = new InjuryEngine(repo);
            playerEngine = new PlayerEngine(repo);
            this.logger = logger;

        }

        public void Process(InjuryEntry entry)
        {
            foreach (InjuredPlayer player in entry.Players)
            {
                Player p = playerEngine.Query(c => c.PlayerName == player.PlayerName.ToLower()).FirstOrDefault();
                if (p != null)
                {
                    Injury injury = new Injury()
                    {
                        PlayerId = p.Id,
                        ScratchDate = DataTypeHelper.ConvertDateTime(player.Date),
                        InjuryStatus = player.Status
                    };
                    logger.LogInformation($"Adding Injured Player {p.PlayerName}");
                    injuryEngine.Save(injury);
                }
                else
                {
                    logger.LogError($"Player {player.PlayerName} does not exist");
                }
            }            
        }     
    }
}
