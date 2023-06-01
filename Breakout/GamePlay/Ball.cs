using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Breakout.GamePlay
{
    public class Ball : ISubject
    {
        #region properties
        public List<IObserver> Subscribers { get; } = new List<IObserver>();
        private BallList balls { get; set; }

        private Rectangle rect = new Rectangle(0, 0, 32, 32);
        public int x { get; set; }
        public int y { get; set; }
        public int radius { get; set; }
        public Vector2 direction { get; set; }
        public double speed { get; set; }
        public int bricks_removed { get; set; }

        private SoundEffect soundEffect;
        #endregion
        public Ball(int x, int y, float X, SoundEffect soundEffect)
        {
            this.x = x;
            this.y = y;
            speed = 0.7;
            radius = 16;
            bricks_removed = 0;
            direction = new Vector2(X, -(float)Math.Sqrt(1 - (X * X)));
            rect.X = x - 16;
            rect.Y = y - 16;
            this.soundEffect= soundEffect;
        }

        #region subject
        //observers can observe the ball, such as the paddle and blockholder
        public void Subscribe(IObserver observer)
        {
            if (observer != null && !Subscribers.Contains(observer))
                Subscribers.Add(observer);
        }

        public void Subscribe(BallList balls)
        {
            this.balls = balls;
        }

        public void Unsubscribe(IObserver observer)
        {
            if (!Subscribers.Contains(observer)) return;
            Subscribers.Remove(observer);
        }

        private void UnsubscribeAll()
        {
            List<IObserver> deletes = new List<IObserver>();
            foreach (IObserver observer in Subscribers)
            {
                deletes.Add(observer);
            }
            foreach (IObserver observer in deletes)
            {
                Unsubscribe(observer);
            }
        }

        public void Notify()
        {
            foreach (var observer in Subscribers)
                observer.Update(this);
        }
        #endregion

        public void update(GameTime gametime)
        {
            var newX = x + speed * gametime.ElapsedGameTime.TotalMilliseconds * direction.X;
            var newY = y + speed * gametime.ElapsedGameTime.TotalMilliseconds * direction.Y;
            // will rebound if out of bounds
            if (newX < Box.x1 + 16)
            {
                newX = 2 * (Box.x1 + 16) - newX;
                ReboundOnXAxis();
                soundEffect.Play();
            }
            else if (newX > Box.x2 - 16)
            {
                newX = 2 * (Box.x2 - 16) - newX;
                ReboundOnXAxis();
                soundEffect.Play();
            }
            if (newY < Box.y1 + 16)
            {
                newY = 2 * (Box.y1 + 16) - newY;
                ReboundOnYAxis();
                soundEffect.Play();
            }
            // unless it fell off below
            else if (newY > Box.y2 - 16)
            {
                balls.Update(this);
                UnsubscribeAll();
                soundEffect.Play();
            }
            x = (int)(newX);
            y = (int)(newY);
            Notify();

            rect.X = x - 16;
            rect.Y = y - 16;
            //sets the speed based on bricks removed
            if (bricks_removed > 3)
            {
                if (bricks_removed > 11)
                {
                    if (bricks_removed > 35)
                    {
                        if (bricks_removed > 61)
                        {
                            speed = 1.6;
                        }
                        else
                        {
                            speed = 1.3;
                        }
                    }
                    else
                    {
                        speed = 1;
                    }
                }
                else
                {
                    speed = 0.8;
                }
            }
        }

        public void draw(GameTime gametime, SpriteBatch spriteBatch, Texture2D circle)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(
                circle,
                rect,
                Color.White);

            spriteBatch.End();
        }


        #region math
        public void ReboundOnXAxis()
        {
            this.direction = new Vector2(-direction.X, direction.Y);
        }

        public void ReboundOnYAxis()
        {
            this.direction = new Vector2(direction.X, -direction.Y);
        }
        #endregion
    }
}
