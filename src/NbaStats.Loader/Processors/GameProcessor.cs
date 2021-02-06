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

        public GameProcessor(IRepository repo, AppSettings settings)
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
        }

        public void Process(GameEntry entry)
        {
            var info = entry.Game;

            // Add the Teams to the Database
            var homeEntry = info.Teams.Where(c => c.Type.ToLower() == "home").FirstOrDefault();
            var awayEntry = info.Teams.Where(c => c.Type.ToLower() == "away").FirstOrDefault();

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
                schedule = scheduleEngine.Save(schedule);

                if (!settings.scheduleOnly && schedule != null)
                {
                    // Add the Players
                    AddPlayers(home.Id, schedule.Id, entry.HomePlayers);
                    AddPlayers(away.Id, schedule.Id, entry.AwayPlayers);

                    // Add the Team Stats
                    var homeStats = entry.TeamStats.Where(c => c.Type.ToLower() == "home").FirstOrDefault();
                    var awayStats = entry.TeamStats.Where(c => c.Type.ToLower() == "away").FirstOrDefault();

                    AddTeamStats(home.Id, away.Id, schedule.Id, homeStats);
                    AddTeamStats(away.Id, home.Id, schedule.Id, awayStats);

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
                        // Add to the Roster
                        RosterEntry rosterEntry = new RosterEntry()
                        {
                            PlayerId = player.Id,
                            TeamId = teamId
                        };
                        rosterEngine.Save(rosterEntry);

                        // Add Player Stats
                        var stat = playerTransformer.TransformStat(playerEntry);
                        stat.PlayerId = player.Id;
                        stat.ScheduleId = scheduleId;
                        playerStatEngine.Save(stat);
                    }
                }
            }
        }

        private void AddTeamStats(int teamId, int oppponentId, long scheduleId, TeamStatCollection stats)
        {
            var stat = teamStatTransformer.Transform(stats);
            stat.TeamId = teamId;
            stat.OpponentId = oppponentId;
            stat.ScheduleId = scheduleId;
            teamStatEngine.Save(stat);
        }

        
    }
}
