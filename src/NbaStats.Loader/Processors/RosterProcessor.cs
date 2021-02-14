using Microsoft.Extensions.Logging;
using NbaStats.Data.Context;
using NbaStats.Data.DataObjects;
using NbaStats.Data.Engines;
using NbaStats.Loader.DataObject;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Loader.Transformers;
using NbaStats.Data.Repositories;

namespace NbaStats.Loader.Processors
{
    public class RosterProcessor : IProcessor<RosterItem>
    {
        private readonly IStatEngine<Player> playerEngine;
        private readonly IStatEngine<RosterEntry> rosterEngine;
        private readonly IStatEngine<Team> teamEngine;
        private readonly IStatEngine<Transaction> tranEngine;
        private readonly ILogger logger;

        public RosterProcessor(IRepository repo, ILogger logger)
        {
            this.logger = logger;
            playerEngine = new PlayerEngine(repo);
            rosterEngine = new RosterEntryEngine(repo);
            teamEngine = new TeamEngine(repo);
            tranEngine = new TransactionEngine(repo);
        }

        public void Process(RosterItem entry)
        {
            // Get the Team
            var team = teamEngine.Query(c => c.TeamCode.ToLower() == entry.TeamCode).FirstOrDefault();
            PlayerTransformer transformer = new PlayerTransformer();
            if (team != null)
            {
                // Process the Players
                foreach (string playerName in entry.Players)
                {
                    var player = playerEngine.Query(c => c.PlayerName.ToLower() == playerName.ToLower()).FirstOrDefault();
                    if (player == null)
                    {
                        logger.LogInformation($"Adding {playerName} to the database");
                        var playerEntry = new PlayerEntry()
                        {
                            FullName = playerName
                        };
                        player = transformer.Transform(playerEntry);
                        player = playerEngine.Save(player);

                        RosterEntry rosterEntry = new RosterEntry()
                        {
                            PlayerId = player.Id,
                            TeamId = team.Id
                        };
                        rosterEngine.Save(rosterEntry);
                    }

                    if (rosterEngine.Query(c => c.PlayerId == player.Id && c.TeamId == team.Id).Count() == 0)
                    {
                        var rosterEntry = rosterEngine.Query(c => c.PlayerId == player.Id).FirstOrDefault();
                        if (rosterEntry != null)
                        {
                            logger.LogInformation($"Player {playerName} is a member of another team. Adding Transaction");
                            var transaction = new Transaction()
                            {
                                PlayerId = player.Id,
                                OldTeamId = rosterEntry.TeamId,
                                NewTeamId = team.Id
                            };
                            tranEngine.Save(transaction);
                            rosterEntry.TeamId = team.Id;
                        }
                        else
                        {
                            logger.LogInformation($"Creating new Roster Entry for {playerName}");
                            rosterEntry = new RosterEntry()
                            {
                                PlayerId = player.Id,
                                TeamId = team.Id
                            };
                        }
                        logger.LogInformation($"Saving Roster Entry for {playerName}");
                        rosterEngine.Save(rosterEntry);
                    }
                }
            }
            else
            {
                logger.LogError($"Could not find Team {entry.TeamCode}");
            }
        }
    }
}
