using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NbaStats.Data.DataObjects;
using NbaStats.Loader.DataObject;
using NbaStats.Loader.Transformers;

namespace NbaStats.Loader.Tests.Transformers
{
    [TestFixture]
    public class PlayerTransformerTests
    {
        private PlayerTransformer transformer;
        private PlayerEntry entry;

        [SetUp]
        public void Setup()
        {
            transformer = new PlayerTransformer();

            entry = new PlayerEntry()
            {
                FullName = "Stanley Johnson",
                Name = "S. Johnson",
                Position = "SF",
                FullUrl = "https://www.espn.com/nba/player/_/id/3134881/stanley-johnson"
            };

            entry.AddStat(new StatEntry("min", "31"));
            entry.AddStat(new StatEntry("fg", "1-6"));
            entry.AddStat(new StatEntry("3pt", "0-2"));
            entry.AddStat(new StatEntry("ft", "1-2"));
            entry.AddStat(new StatEntry("oreb", "0"));
            entry.AddStat(new StatEntry("dreb", "6"));
            entry.AddStat(new StatEntry("reb", "6"));
            entry.AddStat(new StatEntry("ast", "4"));
            entry.AddStat(new StatEntry("stl", "1"));
            entry.AddStat(new StatEntry("blk", "1"));
            entry.AddStat(new StatEntry("to", "3"));
            entry.AddStat(new StatEntry("pf", "4"));
            entry.AddStat(new StatEntry("plusminus", "+3"));
            entry.AddStat(new StatEntry("pts", "3"));
            
        }

        [Test]
        public void TestTransformPlayerEntry()
        {
            var player = transformer.Transform(entry);
            Assert.AreEqual("Stanley Johnson", player.PlayerName);
            Assert.AreEqual("D3070BA900FDA0552039B9966132D960", player.PlayerCode);
        }

        [Test]
        public void TestTransformPlayerStat()
        {
            var playerStat = transformer.TransformStat(entry);
            Assert.AreEqual(4, playerStat.Assists);
            Assert.AreEqual(1, playerStat.Blocks);
            Assert.AreEqual(6, playerStat.DefensiveRebound);
            Assert.AreEqual(1, playerStat.FGCompleted);
            Assert.AreEqual(0.17d, playerStat.FGPercentage);
            Assert.AreEqual(6, playerStat.FGTaken);
            Assert.AreEqual(4, playerStat.Fouls);
            Assert.AreEqual(31, playerStat.GameMinutes);
            Assert.AreEqual(0, playerStat.OffensiveRebound);
            Assert.AreEqual(3, playerStat.PointDifferential);
            Assert.AreEqual(3, playerStat.Points);
            Assert.AreEqual(1, playerStat.Steals);
            Assert.AreEqual(0, playerStat.ThreeCompleted);
            Assert.AreEqual(0, playerStat.ThreePercentage);
            Assert.AreEqual(2, playerStat.ThreeTaken);
            Assert.AreEqual(3, playerStat.Turnovers);            
        }

        [Test]
        public void TestNullPlayer()
        {
            var player = transformer.Transform(null);
            Assert.IsNull(player);
        }

        [Test]
        public void TestConvertStatDnp()
        {
            entry.Stats.Clear();
            entry.AddStat(new StatEntry("dnp", "Did not play"));

            var playerStat = transformer.TransformStat(entry);
            Assert.IsNull(playerStat);
        }
    }
}
