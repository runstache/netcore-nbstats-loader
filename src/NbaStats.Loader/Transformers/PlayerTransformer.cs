using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Data.DataObjects;
using NbaStats.Loader.DataObject;
using System.Security.Cryptography;
using System.Linq;
using NbaStats.Loader.Helpers;
using NbaStats.Loader.Constants;

namespace NbaStats.Loader.Transformers
{
    public class PlayerTransformer
    {
        public Player Transform(PlayerEntry entry)
        {
            var player = new Player()
            {
                PlayerName = entry.FullName
            };

            using (MD5 md5 = MD5.Create())
            {
                byte[] content = Encoding.ASCII.GetBytes(entry.FullName.ToLower().Replace(" ", ""));
                byte[] hash = md5.ComputeHash(content);
                StringBuilder code = new StringBuilder();
                for (int i =0; i < hash.Length; i++)
                {
                    code.Append(hash[i].ToString("X2"));
                }
                player.PlayerCode = code.ToString();
            }
            return player;

        }

        /// <summary>
        /// Transforms a Player Entry to the given Players Statistics Entry.
        /// </summary>
        /// <param name="entry">Player Entry</param>
        /// <returns>Player Stat</returns>
        public PlayerStat TransformStat(PlayerEntry entry)
        {
            var stat = new PlayerStat()
            {
                Assists = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Assists)),
                Blocks = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Blocks)),
                DefensiveRebound = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.DefensiveRebounds)),
                Fouls = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Fouls)),
                GameMinutes = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Minutes)),
                OffensiveRebound = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.OffensiveRebounds)),
                Points = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Points)),
                Steals = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.Steals)),
                Turnovers = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, PlayerStatConstants.TurnOvers))
            };

            // Three Point Statistics
            string threePoint = GetStatValue(entry.Stats, PlayerStatConstants.ThreePoint);
            if (!string.IsNullOrEmpty(threePoint))
            {
                string[] parts = threePoint.Split("-");
                if (parts.Length > 1)
                {
                    stat.ThreeCompleted = DataTypeHelper.ConvertToInteger(parts[0]);
                    stat.ThreeTaken = DataTypeHelper.ConvertToInteger(parts[1]);
                    if (stat.ThreeCompleted > 0 && stat.ThreeTaken > 0)
                    {
                        double pct = Convert.ToDouble(stat.ThreeCompleted) / Convert.ToDouble(stat.ThreeTaken);
                        stat.ThreePercentage = Math.Round(pct, 2);
                    }
                    else
                    {
                        stat.ThreePercentage = 0;
                    }
                }
            }

            // Point Differential
            string pointDiff = GetStatValue(entry.Stats, PlayerStatConstants.PointDifferential);
            if (!string.IsNullOrEmpty(pointDiff))
            {
                stat.PointDifferential = DataTypeHelper.ConvertToInteger(pointDiff.Replace("+", ""));
            }

            // Free Throws Percentage
            string foulShout = GetStatValue(entry.Stats, PlayerStatConstants.FreeThrows);
            if (!string.IsNullOrEmpty(foulShout))
            {
                string[] parts = foulShout.Split("-");
                if (parts.Length > 1)
                {
                    stat.FTTaken = DataTypeHelper.ConvertToInteger(parts[0]);
                    stat.FTCompleted = DataTypeHelper.ConvertToInteger(parts[1]);

                    if (stat.FTCompleted > 0 && stat.FTTaken > 0)
                    {
                        double pct = Convert.ToDouble(stat.FTCompleted) / Convert.ToDouble(stat.FTTaken);
                        stat.FTPercentage = Math.Round(pct, 2);
                    }
                    else
                    {
                        stat.FTPercentage = 0;
                    }
                }
            }

            // Field Goal Percentage
            string fieldGoals = GetStatValue(entry.Stats, PlayerStatConstants.FieldGoals);
            if (!string.IsNullOrEmpty(fieldGoals))
            {
                string[] parts = fieldGoals.Split("-");
                if (parts.Length > 1)
                {
                    stat.FGTaken = DataTypeHelper.ConvertToInteger(parts[1]);
                    stat.FGCompleted = DataTypeHelper.ConvertToInteger(parts[0]);

                    if (stat.FGCompleted > 0 && stat.FGTaken > 0)
                    {
                        double pct = Convert.ToDouble(stat.FGCompleted) / Convert.ToDouble(stat.FGTaken);
                        stat.FGPercentage = Math.Round(pct, 2);
                    }
                    else
                    {
                        stat.FGPercentage = 0;
                    }
                }
            }                      
            return stat;
        }

        private string GetStatValue(List<StatEntry> stats, string key)
        {
            var stat = stats.Where(c => c.Name == key).FirstOrDefault();
            if (stat != null)
            {
                return stat.Value;
            }
            return "";
        }
    }
}
