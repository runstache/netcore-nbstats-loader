using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NbaStats.Loader.Constants;
using NbaStats.Loader.DataObject;
using NbaStats.Loader.Transformers;

namespace NbaStats.Loader.Tests.Transformers
{
    [TestFixture]
    public class TeamStatTransformerTests
    {
        private TeamStatTransformer transformer;
        private TeamStatCollection stat;

        [SetUp]
        public void Setup()
        {
            transformer = new TeamStatTransformer();
            stat = new TeamStatCollection()
            {
                Type = "home"
            };

            stat.AddStat(new TeamStatEntry("FG", "35-87"));
            stat.AddStat(new TeamStatEntry("FieldGoal%", "40.2"));
            stat.AddStat(new TeamStatEntry("3PT", "13-39"));
            stat.AddStat(new TeamStatEntry("ThreePoint%", "33.3"));
            stat.AddStat(new TeamStatEntry("FT", "19-22"));
            stat.AddStat(new TeamStatEntry("FreeThrow%", "86.4"));
            stat.AddStat(new TeamStatEntry("Rebounds", "57"));
            stat.AddStat(new TeamStatEntry("OffensiveRebounds", "12"));
            stat.AddStat(new TeamStatEntry("DefensiveRebounds", "35"));
            stat.AddStat(new TeamStatEntry("Assists", "23"));
            stat.AddStat(new TeamStatEntry("Steals", "10"));
            stat.AddStat(new TeamStatEntry("Blocks", "8"));
            stat.AddStat(new TeamStatEntry("TotalTurnovers", "19"));
            stat.AddStat(new TeamStatEntry("PointsOffTurnovers", "26"));
            stat.AddStat(new TeamStatEntry("FastBreakPoints", "12"));
            stat.AddStat(new TeamStatEntry("PointsinPaint", "40"));
            stat.AddStat(new TeamStatEntry("Fouls", "23"));
            stat.AddStat(new TeamStatEntry("TechnicalFouls", "1"));
            stat.AddStat(new TeamStatEntry("FlagrantFouls", "0"));
            stat.AddStat(new TeamStatEntry("LargestLead", "1"));            
        }

        [Test]
        public void TestConverTeamStat()
        {
            var result = transformer.Transform(stat);

            Assert.AreEqual(23, result.Assits);
            Assert.AreEqual(8, result.Blocks);
            Assert.AreEqual(35, result.DefensiveRebound);
            Assert.AreEqual(35, result.FGCompleted);
            Assert.AreEqual(0.402, result.FGPercentage);
            Assert.AreEqual(87, result.FGTaken);
            Assert.AreEqual(23, result.Fouls);
            Assert.AreEqual(0.864, result.FreeThrowPercentage);
            Assert.AreEqual(19, result.FreeThrowsCompleted);
            Assert.AreEqual(22, result.FreeThrowsTaken);
            Assert.AreEqual(12, result.OffensiveRebound);
            Assert.AreEqual(10, result.Steals);
            Assert.AreEqual(13, result.ThreeCompleted);
            Assert.AreEqual(0.333, result.ThreePercentage);
            Assert.AreEqual(39, result.ThreeTaken);
            Assert.AreEqual(19, result.TurnOvers);            
        }

        [Test]
        public void TestTransformNull()
        {
            var result = transformer.Transform(null);
            Assert.IsNull(result);

        }
    }
}
