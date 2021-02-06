using NbaStats.Loader.DataObject;
using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Data.DataObjects;
using NbaStats.Data.Engines;
using NbaStats.Data.Repositories;
using NbaStats.Loader.Transformers;
using System.Linq;
using NbaStats.Loader.Helpers;
using NbaStats.Loader.Configuration;
using Microsoft.Extensions.Logging;

namespace NbaStats.Loader.Processors
{
    public class GameProcessor : IProcessor<GameEntry>
    {
        private readonly IStatEngine<Team> teamEngine;
        private readonly TeamTransformer teamTransformer;
        private readonly TeamStatTransformer teamStatTransformer;
        private readonly IStatEngine<ScheduleEntry> scheduleEngine;
        private readonly PlayerTransformer playerTransformer;
        private readonly IStatEngine<Player> playerEngine;
        private readonly IStatEngine<PlayerStat> playerStatEngine;
        private readonly AppSettings settings;
        private readonly IStatEngine<RosterEntry> rosterEngine;
        private readonly IStatEngine<Transaction> transactionEngine;
        private readonly IStatEngine<TeamStat> teamStatEngine;
        private readonly IStatEngine<BoxScoreEntry> boxscoreEngine;
        private readonly ILogger logger;

        public GameProcessor(IRepository repo, AppSettings settings, ILogger logger)
        {
            teamEngine = new TeamEngine(repo);
            scheduleEngine = new ScheduleEntryEngine(repo);
            teamTransformer = new TeamTransformer();
            teamStatTransformer = new TeamStatTransformer();
            playerTransformer = new PlayerTransformer();
            playerEngine = new PlayerEngine(repo);
            rosterEngine = new RosterEntryEngine(repo);
            transactionEngine = new TransactionEngine(repo);
            playerStatEngine = new PlayerStatEngine(repo);
            teamStatEngine = new TeamStatEngine(repo);
            boxscoreEngine = new BoxScoreEngine(repo);

            this.settings = settings;
            this.logger = logger;
        }

        public void Process(GameEntry entry)
        {
            logger.LogInformation($"Processing Game {entry.Id}");
            var info = entry.Game;

            // Add the Teams to the Database
            
            var homeEntry = info.Teams.Where(c => c.Type.ToLower() == "home").FirstOrDefault();
            var awayEntry = info.Teams.Where(c => c.Type.ToLower() == "away").FirstOrDefault();
            logger.LogInformation($"Adding Teams {homeEntry.Name} and {awayEntry.Name} to the database");

            var home = teamTransformer.Transform(homeEntry);
            var away = teamTransformer.Transform(awayEntry);

            if (home != null && away != null)
            {

                home = teamEngine.Save(home);
                away = teamEngine.Save(away);

                ScheduleEntry schedule = new ScheduleEntry()
                {
                    AwayTeamId = away.Id,
                    HomeTeamId = home.Id,
                    GameDate = DataTypeHelper.ConvertDateTime(info.Date)
                };
                logger.LogInformation($"Adding Schedule for {info.Date}");
                schedule = scheduleEngine.Save(schedule);

                if (!settings.ScheduleOnly && schedule != null)
                {
                    logger.LogInformation("Adding Players to Database");
                    // Add the Players
                    AddPlayers(home.Id, schedule.Id, entry.HomePlayers);
                    AddPlayers(away.Id, schedule.Id, entry.AwayPlayers);

                    logger.LogInformation("Adding Team Stats to Database");
                    // Add the Team Stats
                    var homeStats = entry.TeamStats.Where(c => c.Type.ToLower() == "home").FirstOrDefault();
                    var awayStats = entry.TeamStats.Where(c => c.Type.ToLower() == "away").FirstOrDefault();

                    AddTeamStats(home.Id, away.Id, schedule.Id, homeStats);
                    AddTeamStats(away.Id, home.Id, schedule.Id, awayStats);

                    logger.LogInformation("Adding Boxscore to Database");
                    // Add the Boxscore
                    var homeBoxscore = teamTransformer.TransformBoxScore(homeEntry);
                    homeBoxscore.ScheduleId = schedule.Id;
                    homeBoxscore.TeamId = home.Id;
                    boxscoreEngine.Save(homeBoxscore);

                    var awayBoxscore = teamTransformer.TransformBoxScore(awayEntry);
                    awayBoxscore.TeamId = away.Id;
                    awayBoxscore.ScheduleId = schedule.Id;
                    boxscoreEngine.Save(awayBoxscore);                   
                }            
            } 
            else
            {
                throw new Exception("Home and Away Team failed to convert.");
            }
        }

        private void AddPlayers(int teamId, long scheduleId, List<PlayerEntry> players)
        {
            foreach (PlayerEntry playerEntry in players)
            {
                if (!string.IsNullOrEmpty(playerEntry.FullName))
                {
                    var player = playerTransformer.Transform(playerEntry);
                    if (player != null)
                    {
                        // Add the Player
                        player = playerEngine.Save(player);
                        if (player != null)
                        {
                            // Check for other team roster
                            if (rosterEngine.Query(c => c.PlayerId == player.Id && c.TeamId != teamId).Count() > 0)
                            {
                                logger.LogInformation($"Player {player.PlayerName} has moved teams.  Adding Transaction.");
                                List<RosterEntry> entries = rosterEngine.Query(c => c.PlayerId == player.Id && c.TeamId != teamId).ToList();
                                foreach (RosterEntry entry in entries)
                                {
                                    // Add a Transaction
                                    Transaction transaction = new Transaction()
                                    {
                                        NewTeamId = teamId,
                                        PlayerId = player.Id,
                                        OldTeamId = entry.TeamId
                                    };
                                    transactionEngine.Save(transaction);

                                    // Remove the Entry
                                    rosterEngine.Delete(entry);
                                }
                            }
                            logger.LogInformation($"Adding player {player.PlayerName} to Team Roster");
                            // Add to the Roster
                            RosterEntry rosterEntry = new RosterEntry()
                            {
                                PlayerId = player.Id,
                                TeamId = teamId
                            };
                            rosterEngine.Save(rosterEntry);

                            logger.LogInformation($"Adding Player Stats for {player.PlayerName} to Database");
                            // Add Player Stats
                            var stat = playerTransformer.TransformStat(playerEntry);
                            if (stat != null)
                            {
                                stat.PlayerId = player.Id;
                                stat.ScheduleId = scheduleId;
                                playerStatEngine.Save(stat);
                            }
                            else
                            {
                                logger.LogInformation($"{player.PlayerName} did not play");
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning($"Failed to Convert Player: {playerEntry.Name}");
                    }
                }
            }
        }

        private void AddTeamStats(int teamId, int oppponentId, long scheduleId, TeamStatCollection stats)
        {
            var stat = teamStatTransformer.Transform(stats);
            if (stats != null)
            {
                stat.TeamId = teamId;
                stat.OpponentId = oppponentId;
                stat.ScheduleId = scheduleId;
                teamStatEngine.Save(stat);
            }
        }

        
    }
}
