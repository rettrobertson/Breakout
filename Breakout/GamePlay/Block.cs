using CS5410.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Breakout.GamePlay
{
    public class Block
    {
        private int x;
        private int y;
        private int length;
        public bool isBroken;
        private Color color;
        private SoundEffect soundEffect;
        private int level;
        private int[] breaks;

        private Rectangle rect;
        private Texture2D tex;
        private Paddle paddle;
        private int score;
        private int blocks_per_row;

        private List<ParticleEmitter> emitters;
        private ScoreKeeper scoreKeeper { get; set; }

        public Block(int x, int y, int length, Texture2D tex, Color color, int score, Paddle paddle, ScoreKeeper scoreKeeper, SoundEffect soundEffect, List<ParticleEmitter> emitters, int level, int[] breaks, int blocks_per_row)
        {
            this.soundEffect = soundEffect;
            this.x = x;
            this.y = y;
            this.length = length;
            isBroken = false;
            rect = new Rectangle(x, y, length - 3, length / 2 - 3);
            this.tex = tex;
            this.color = color;
            this.paddle = paddle;
            this.score = score;
            this.scoreKeeper = scoreKeeper;
            this.emitters = emitters;
            this.level = level;
            this.breaks = breaks;
            this.blocks_per_row= blocks_per_row;
        }

        public void Update(Ball ball)
        {
            if (!isBroken)
            {
                // this is for the 25 points per row
                breaks[level]++;
                if (breaks[level] >= blocks_per_row)
                {
                    scoreKeeper.Update(25);
                    //shouldn't be necessary but just in case
                    breaks[level] = 0;
                }
                // if its on the top row halve the paddle
                if (y <= 200)
                {
                    paddle.ishalved = true;
                }

                // add an emmitter
                emitters.Add(new ParticleEmitter(
                                    tex, color, length,
                                    x, y,
                                    1,
                                    2,
                                    new TimeSpan(0, 0, 0, 0, 500)));

                //get closest point and make a line segment from the closest point in the reverse direction of the ball's direction
                // then get line segments for each of the block's sides
                Point closestPoint = getClosestPoint(ball.x, ball.y).point;
                (Point, Point) line2 = (new Point(closestPoint.x, closestPoint.y), new Point((int)(closestPoint.x + (-ball.direction.X * length)), (int)(closestPoint.y + (-ball.direction.Y * length))));
                (Point, Point) line1 = (new Point(x, y), new Point(x, y + length / 2));
                (Point, Point) line3 = (new Point(x, y), new Point(x + length, y));
                (Point, Point) line4 = (new Point(x + length, y + length/2), new Point(x, y + length / 2));
                (Point, Point) line5 = (new Point(x + length, y + length/2), new Point(x + length, y));
                // depending on which side of the block the line segment intersects, it will tell us which axis to rebound on
                if (intersect(line2.Item1, line2.Item2, line1.Item1, line1.Item2))
                {
                    ball.ReboundOnXAxis();
                }
                else if (intersect(line2.Item1, line2.Item2, line3.Item1, line3.Item2))
                {
                    ball.ReboundOnYAxis();
                }
                else if (intersect(line2.Item1, line2.Item2, line4.Item1, line4.Item2))
                {
                    ball.ReboundOnYAxis();
                }
                else if (intersect(line2.Item1, line2.Item2, line5.Item1, line5.Item2))
                {
                    ball.ReboundOnXAxis();
                }
                scoreKeeper.Update(this.score);
                ball.bricks_removed++;
                soundEffect.Play();
                isBroken = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!isBroken)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(
                    tex,
                    rect,
                    color);

                spriteBatch.End();
            }
        }

        public void reset()
        {
            this.isBroken = false;
        }

        // use some math to get closest point
        private Length getClosestPoint(int x, int y)
        {
            Point middle = new Point(this.x + length/2, this.y + length/4);
            Point point1 = new Point(x - 16, y + 16);
            Point point2 = new Point(x + 16, y + 16);
            Point point3 = new Point(x + 16, y - 16);
            Point point4 = new Point(x - 16, y - 16);
            Length length1 = new Length((middle.x - point1.x) * (middle.x - point1.x) + (middle.y - point1.y) * (middle.y - point1.y), point1);
            Length length2 = new Length((middle.x - point2.x) * (middle.x - point2.x) + (middle.y - point2.y) * (middle.y - point2.y), point2);
            Length length3 = new Length((middle.x - point3.x) * (middle.x - point3.x) + (middle.y - point3.y) * (middle.y - point3.y), point3);
            Length length4 = new Length((middle.x - point4.x) * (middle.x - point4.x) + (middle.y - point4.y) * (middle.y - point4.y), point4);
            return Min(Min(length1, length2), Min(length3, length4));
        }
        //this was so I can return a Point but use a min function for the lengt
        private class Length
        {
            public double length;
            public Point point;
            public Length(double length, Point point)
            {
                this.length = length;
                this.point = point;
            }
        }
        //simple min function
        private Length Min(Length length1, Length length2)
        {
            if (length2.length < length1.length)
            {
                return length2;
            }
            else
            {
                return length1;
            }
        }
        //there might be a built in class for this already
        private class Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        };

        // copied off stack overflow, forgot the link
        private bool intersect(Point p1, Point q1, Point p2, Point q2)
        {
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }
        private int orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            int val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0; // collinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }
        private bool onSegment(Point p, Point q, Point r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }
    }
}
