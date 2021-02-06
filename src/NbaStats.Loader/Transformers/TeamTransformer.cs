using System;
using System.Collections.Generic;
using System.Text;
using NbaStats.Data.DataObjects;
using NbaStats.Loader.DataObject;
using NbaStats.Loader.Constants;
using NbaStats.Loader.Helpers;
using System.Linq;

namespace NbaStats.Loader.Transformers
{
    public class TeamTransformer
    {
        public Team Transform(TeamEntry entry)
        {
            if (entry != null)
            {
                return new Team()
                {
                    TeamCode = entry.Code,
                    TeamName = entry.Name
                };
            }
            return null;
        }

        public BoxScoreEntry TransformBoxScore(TeamEntry entry)
        {
            if (entry != null)
            {
                var boxscore = new BoxScoreEntry()
                {
                    Quarter1 = GetLineScoreValue(entry.LineScores, 1),
                    Quarter2 = GetLineScoreValue(entry.LineScores, 2),
                    Quarter3 = GetLineScoreValue(entry.LineScores, 3),
                    Quarter4 = GetLineScoreValue(entry.LineScores, 4)
                };

                boxscore.Ot = entry.LineScores.Where(c => c.Quarter > 4).Sum(c => c.Score);
                boxscore.Total = boxscore.Quarter1 + boxscore.Quarter2 + boxscore.Quarter3 + boxscore.Quarter4 + boxscore.Ot;
                return boxscore;
            }
            return null;
        }

        private int GetLineScoreValue(List<LineScore> scores, int quarter)
        {
            var lineScore = scores.Where(c => c.Quarter == quarter).FirstOrDefault();
            if (lineScore != null)
            {
                return lineScore.Score;
            }
            return 0;

        }

    }
}
