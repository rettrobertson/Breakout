using CS5410.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Breakout.GamePlay
{
    public class BlockHolder : IObserver
    {
        // this class holds the individual blocks
        private readonly Block[,] blocks;
        // breaks is for scoring 25 per row
        private readonly int[] breaks;
        private readonly int blockLength;
        // distance from the top of the screen
        private const int y1 = 200;
        private const int blocks_per_row = 20;
        private const int numberOfRows = 8;
        private readonly int y2;

        public BlockHolder(int length, Texture2D tex, Paddle paddle, ScoreKeeper scoreKeeper, SoundEffect soundEffect, List<ParticleEmitter> emitters)
        {
            blockLength = length / blocks_per_row;
            y2 = y1 + blockLength / 2 * numberOfRows;
            blocks = new Block[blocks_per_row,numberOfRows];
            breaks = new int[numberOfRows];
            for (int i = 0; i < numberOfRows; i++)
            {
                breaks[i] = 0;
            }
            //double for loop to instantiate all the blocks
            for (int i = 0; i < blocks_per_row; i++)
            {
                for (int j = 0; j < numberOfRows; j++)
                { 
                    int temp = j / 2;
                    switch (temp) {
                        case 0:
                            blocks[i, j] = new Block(i * blockLength, y1 + j * (blockLength / 2), blockLength, tex, Color.Green, 5, paddle, scoreKeeper, soundEffect, emitters, j, breaks, blocks_per_row);
                            break;
                        case 1:
                            blocks[i, j] = new Block(i * blockLength, y1 + j * (blockLength / 2), blockLength, tex, Color.Blue, 3, paddle, scoreKeeper, soundEffect, emitters, j, breaks, blocks_per_row);
                            break;
                        case 2:
                            blocks[i, j] = new Block(i * blockLength, y1 + j * (blockLength / 2), blockLength, tex, Color.Orange, 2, paddle, scoreKeeper, soundEffect, emitters, j, breaks, blocks_per_row);
                            break;
                        case 3:
                            blocks[i, j] = new Block(i * blockLength, y1 + j * (blockLength / 2), blockLength, tex, Color.Yellow, 1, paddle, scoreKeeper, soundEffect, emitters, j, breaks, blocks_per_row);
                            break;
                    }
                   
                }
            }
        }
        // lots of if statements to check which block the ball is hitting, uses math to only check up to four blocks instead of checking against all blocks in the double array
        public void Update(Ball ball)
        {
            if (ball.y - ball.radius < y2 && ball.y + ball.radius > y1)
            {
                if (ball.x + ball.radius < Box.x2 && ball.y + ball.radius < y2 && !blocks[(ball.x + ball.radius)/blockLength, (ball.y - y1 + ball.radius)/(blockLength/2)].isBroken)
                {
                    blocks[(ball.x + ball.radius) / blockLength, (ball.y - y1 + ball.radius) / (blockLength / 2)].Update(ball);
                }
                else if (ball.x - ball.radius >= 0 && ball.y + ball.radius < y2 && !blocks[(ball.x - ball.radius) / blockLength, (ball.y - y1 + ball.radius) / (blockLength / 2)].isBroken)
                {
                    blocks[(ball.x - ball.radius) / blockLength, (ball.y - y1 + ball.radius) / (blockLength / 2)].Update(ball);
                }
                else if (ball.x + ball.radius < Box.x2 && ball.y - ball.radius >= y1 && !blocks[(ball.x + ball.radius) / blockLength, (ball.y - y1 - ball.radius) / (blockLength / 2)].isBroken)
                {
                    blocks[(ball.x + ball.radius) / blockLength, (ball.y - y1 - ball.radius) / (blockLength / 2)].Update(ball);
                }
                else if (ball.x - ball.radius >= 0 && ball.y - ball.radius >= y1 && !blocks[(ball.x - ball.radius) / blockLength, (ball.y - y1 - ball.radius) / (blockLength / 2)].isBroken)
                {
                    blocks[(ball.x - ball.radius) / blockLength, (ball.y - y1 - ball.radius) / (blockLength / 2)].Update(ball);
                }

                return;
            }
        }

        public GameStateEnum Update()
        {
            foreach (Block block in blocks)
            {
                if (!block.isBroken)
                {
                    return GameStateEnum.GamePlay;
                }
            }
            // if all blocks are broken game is over
            return GameStateEnum.MainMenu;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Block block in blocks)
            {
                block.Draw(gameTime, spriteBatch);
            }
        }
        public void Reset()
        {
            foreach (Block block in blocks)
            {
                block.reset();
            }
            for (int i = 0; i < numberOfRows; i++)
            {
                breaks[i] = 0;
            }
        }
    }
}
