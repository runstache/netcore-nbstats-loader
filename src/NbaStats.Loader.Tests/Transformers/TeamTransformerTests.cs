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
    public class TeamTransformerTests
    {

        private TeamTransformer transformer;
        private TeamEntry entry;

        [SetUp]
        public void Setup()
        {
            transformer = new TeamTransformer();

            entry = new TeamEntry()
            {
                Code = "IND",
                Type = "home",
                Name = "Indiana Pacers"
            };
            entry.AddLineScore(new LineScore(1, 20));
            entry.AddLineScore(new LineScore(2, 15));
            entry.AddLineScore(new LineScore(3, 25));
            entry.AddLineScore(new LineScore(4, 30));
            entry.AddLineScore(new LineScore(5, 14));
            entry.AddLineScore(new LineScore(6, 16));
        }

        [Test]
        public void TestTransformTeam()
        {
            var team = transformer.Transform(entry);
            Assert.AreEqual("IND", team.TeamCode);
            Assert.AreEqual("Indiana Pacers", team.TeamName);
        }

        [Test]
        public void TestTransformBoxScore()
        {
            var boxscore = transformer.TransformBoxScore(entry);
            Assert.AreEqual(20, boxscore.Quarter1);
            Assert.AreEqual(15, boxscore.Quarter2);
            Assert.AreEqual(25, boxscore.Quarter3);
            Assert.AreEqual(30, boxscore.Quarter4);
            Assert.AreEqual(30, boxscore.Ot);
            Assert.AreEqual(120, boxscore.Total);

        }

    }
}
