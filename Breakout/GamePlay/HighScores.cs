using Microsoft.VisualBasic;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GamePlay
{
    public class HighScores
    {
        public HighScores() { }

        public HighScores((int, DateTime)[] scores)
        {
            this.scores = scores;
        }
        // uses datetime to differentiate between different games since scores could be the same
        public (int, DateTime)[] scores { get; set;}
        // checks if a new scores should be on the leaderboard and in what place
        public void check(int score, DateTime dateTime)
        {
            (int, DateTime) temp = (-1, DateTime.Now);
            List<(int, DateTime)> stuff = scores.ToList();
            foreach ((int, DateTime) thing in stuff)
            {
                if (thing.Item2 == dateTime)
                {
                    temp = thing;
                }
            }
            if (temp.Item1 >= 0)
            {
                stuff.Add((score, dateTime));
                if (temp.Item1 < score)
                {
                    stuff.Remove(temp);
                }
                else
                {
                    stuff.Remove((score, dateTime));
                }
                stuff.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            }
            else
            {
                stuff.Add((score, dateTime));
                stuff.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                stuff.RemoveAt(0);
            }
            scores = stuff.ToArray();
        }
    }
}
