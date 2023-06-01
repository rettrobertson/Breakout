using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Breakout.GamePlay
{
    public class BallList
    {
        // pretty simple class for the list of balls
        private Paddle paddle;
        private SoundEffect soundEffect;
        public List<Ball> balls { get; set; }
        public List<Ball> deletes { get; set; }

        public BallList(SoundEffect soundEffect)
        {
            this.soundEffect = soundEffect;
            balls = new List<Ball>();
            deletes = new List<Ball>();
        }

        public void Subscribe (Paddle paddle)
        {
            this.paddle = paddle;
        }

        public void Update(Ball ball)
        {
            if (ball != null && balls.Contains(ball))
            {
                if (balls.Count <= 1)
                {
                    paddle.isDying();
                    soundEffect.Play();
                }
                deletes.Add(ball);
            }
        }
        public void Update(GameTime gametime)
        {
            foreach (Ball ball in deletes)
            {
                balls.Remove(ball);
            }
            deletes.Clear();
        }
    }
}
