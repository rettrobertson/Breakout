using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Breakout.GamePlay
{

    public class Paddle : IObserver
    {
        public int x { get; set; }
        public int y { get; set; }
        private int originalLength { get; set; }
        private int originalX { get; set; }
        public int length { get; set; }
        public int height { get; set; }

        private Stack<Rectangle> m_lives;
        private Rectangle rect;
        private SpriteFont font;
        private string countdown = "3";
        public bool ishalved;
        public bool gameOver;

        private SoundEffect soundEffect;

        //starting for during the countdown, dying for the dying animation and living for anything else
        private enum State
        {
            starting, 
            living,
            dying
        }

        private State state;
        //this is for the cuntdown
        private double time = 0;
        

        public Paddle(int x, int y, int length, int height, SpriteFont m_font, Stack<Rectangle> m_lives, SoundEffect soundEffect)
        {
            this.x = x;
            this.y = y;
            this.length = length;
            this.originalLength = length;
            this.originalX = x;
            this.height = height;
            rect = new Rectangle(x, y, length, height);
            this.font = m_font;
            this.m_lives = m_lives;
            this.soundEffect = soundEffect;
            this.ishalved = false;
            gameOver = false;
        }
        // update function for if ball is in the range for the paddle
        public void Update(Ball ball)
        {
            if (ball.y > y && ball.y < y + height)
            {
                if (ball.x > x && ball.x < x + length)
                {
                    ball.y = 2 * (y - 16) - ball.y;
                    double paddleCenter = x + length / 2;
                    float temp = (float)(ball.x - paddleCenter) / (length / 2);
                    ball.direction = new Vector2(temp, -(float)Math.Sqrt(1 - (temp * temp)));
                    soundEffect.Play();
                }
            }
        }

        //resets to original legth and starting position
        public void resetLength()
        {
            time = 0;
            length = originalLength;
            this.x = originalX;
            rect = new Rectangle(x, y, length, height);
            state = State.starting;
            ishalved = false;
            gameOver = false;
        }
        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                // switch on if its living dying or starting
                case State.starting:
                    //display countdown
                    time += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (time > 1000)
                    {
                        if (time > 2000)
                        {
                            if (time > 3000)
                            {
                                state = State.living;
                                time = 0;
                            }
                            else
                            {
                                countdown = "1";
                            }
                        }
                        else
                        {
                            countdown = "2";
                        }
                    }
                    else
                    {
                        countdown = "3";
                    }
                    break;
                case State.dying:
                    if (length <= 0)
                    {
                        resetLength();
                        if (m_lives.Count == 0)
                        {
                            gameOver = true;
                        }
                        else
                        {
                            m_lives.Pop();
                        }
                    }
                    else
                    {
                        length = length - ((int)gameTime.ElapsedGameTime.TotalMilliseconds);
                        x += ((int)gameTime.ElapsedGameTime.TotalMilliseconds) / 2;
                        rect = new Rectangle(x, y, length, height);
                    }
                    break;
                case State.living:
                    if (ishalved)
                    {
                        // this ensures a smooth transition to the halved state
                        if (length > originalLength / 2)
                        {
                            length = length - ((int)gameTime.ElapsedGameTime.TotalMilliseconds);
                            x += ((int)gameTime.ElapsedGameTime.TotalMilliseconds) / 2;
                            rect = new Rectangle(x, y, length, height);
                        }
                    }
                    break;
            }
        }
        public void moveLeft(GameTime gameTime, float scale, double speed)
        {
            if (x > Box.x1 && state == State.living)
            {
                int moveDistance = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * speed * scale);
                x = Math.Max(x - moveDistance, Box.x1);
                rect.X = x;
            }
        }
        public void moveRight(GameTime gameTime, float scale, double speed)
        {
            if (x + length < Box.x2 && state == State.living)
            {
                int moveDistance = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * speed * scale);
                x = Math.Max(x + moveDistance, Box.x1);
                rect.X = x;
            }
        }
        public void setX(int x)
        {
            this.x = x;
            rect.X = x;
        }

        public bool isLiving()
        {
            return state == State.living;
        }
        public void isDying()
        {
            state = State.dying;
        }

        public void draw(GameTime gametime, SpriteBatch spriteBatch, Texture2D paddle)
        {
            spriteBatch.Begin();
            if (state == State.starting)
            {
                int X = x + length / 2;
                int Y = 500;
                int outline = 1;

                spriteBatch.DrawString(font, countdown, new Vector2(X + outline, Y), Color.Black) ;
                spriteBatch.DrawString(font, countdown, new Vector2(X, Y + outline), Color.Black);
                spriteBatch.DrawString(font, countdown, new Vector2(X - outline, Y), Color.Black);
                spriteBatch.DrawString(font, countdown, new Vector2(X, Y - outline), Color.Black);
                spriteBatch.DrawString(font, countdown, new Vector2(X, Y), Color.White);
            }
            spriteBatch.Draw(
                paddle,
                rect,
                Color.White);

            spriteBatch.End();
        }
    }
}
