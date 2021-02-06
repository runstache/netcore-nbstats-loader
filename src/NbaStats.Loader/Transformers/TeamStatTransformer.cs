using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Data.DataObjects;
using NbaStats.Loader.Constants;
using NbaStats.Loader.DataObject;
using NbaStats.Loader.Helpers;
using System.Linq;

namespace NbaStats.Loader.Transformers
{
    public class TeamStatTransformer
    {
        public Data.DataObjects.TeamStat Transform(TeamStatCollection entry)
        {
            if (entry != null)
            {
                var stat = new TeamStat()
                {
                    Assits = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.Assists)),
                    Blocks = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.Blocks)),
                    DefensiveRebound = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.DefensiveRebounds)),
                    Fouls = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.Fouls)),
                    OffensiveRebound = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.OffensiveRebounds)),
                    Steals = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.Steals)),
                    TurnOvers = DataTypeHelper.ConvertToInteger(GetStatValue(entry.Stats, TeamStatConstants.Turnovers))
                };

                // Free Throws
                string freeThrows = GetStatValue(entry.Stats, TeamStatConstants.FreeThrows);
                if (!string.IsNullOrEmpty(freeThrows))
                {
                    string[] parts = freeThrows.Split("-");
                    if (parts.Length > 1)
                    {
                        stat.FreeThrowsCompleted = DataTypeHelper.ConvertToInteger(parts[0]);
                        stat.FreeThrowsTaken = DataTypeHelper.ConvertToInteger(parts[1]);

                        double pct = DataTypeHelper.ConverToDouble(GetStatValue(entry.Stats, TeamStatConstants.FreeThrowPercentage)) / 100;
                        stat.FreeThrowPercentage = Math.Round(pct, 3);

                    }
                }

                // Field Goals
                string fieldGoals = GetStatValue(entry.Stats, TeamStatConstants.FieldGoals);
                if (!string.IsNullOrEmpty(fieldGoals))
                {
                    string[] parts = fieldGoals.Split("-");
                    if (parts.Length > 1)
                    {
                        stat.FGCompleted = DataTypeHelper.ConvertToInteger(parts[0]);
                        stat.FGTaken = DataTypeHelper.ConvertToInteger(parts[1]);

                        double pct = DataTypeHelper.ConverToDouble(GetStatValue(entry.Stats, TeamStatConstants.FieldGoalPercentage)) / 100;
                        stat.FGPercentage = Math.Round(pct, 3);
                    }
                }
                // Three Point
                string threePoint = GetStatValue(entry.Stats, TeamStatConstants.ThreePoint);
                if (!string.IsNullOrEmpty(threePoint))
                {
                    string[] parts = threePoint.Split("-");
                    if (parts.Length > 1)
                    {
                        stat.ThreeCompleted = DataTypeHelper.ConvertToInteger(parts[0]);
                        stat.ThreeTaken = DataTypeHelper.ConvertToInteger(parts[1]);

                        double pct = DataTypeHelper.ConverToDouble(GetStatValue(entry.Stats, TeamStatConstants.ThreePointPercentage)) / 100;
                        stat.ThreePercentage = Math.Round(pct, 3);
                    }
                }
                return stat;
            }
            return null;
        }

        private string GetStatValue(List<TeamStatEntry> stats, string key)
        {            
            TeamStatEntry entry = stats.Where(c => c.Type == key).FirstOrDefault();
            if (entry != null)
            {
                return entry.Value;
            }
            return "";
        }
    }
}
