using Breakout.GamePlay;
using Breakout.Input;
using CS5410;
using CS5410.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Breakout.Views
{
    public class GamePlayView : GameStateView
    {
        private KeyboardInput m_inputKeyboard;
        private GameStateEnum returnEnum = GameStateEnum.GamePlay;

        private Texture2D m_texPaddle;
        private Texture2D m_texCircle;
        private Texture2D m_texLives;
        private Texture2D m_texBlock;

        private Vector2 scoreLocation;

        private SpriteFont m_font;

        private SoundEffect m_audio_break;
        private SoundEffect m_audio_die;
        private SoundEffect m_audio_paddle;
        private Song m_audio_music;

        private const double SPRITE_MOVE_PIXELS_PER_MS = 1.5;
        private Stack<Rectangle> m_lives;
        private const int startLives = 3;

        private BallList balls;
        private Paddle paddle;
        private MyRandom rnd= new();
        private BlockHolder blocks;
        private ScoreKeeper scoreKeeper;
        private List<ParticleEmitter> m_emitters;
        private List<Vector2> m_vectors;

        public override void loadContent(ContentManager contentManager)
        {
            base.loadContent(contentManager);
            m_inputKeyboard = new KeyboardInput();

            m_inputKeyboard.registerCommand(Keys.Left, false, new InputDeviceHelper.CommandDelegate(onMoveLeft));
            m_inputKeyboard.registerCommand(Keys.Right, false, new InputDeviceHelper.CommandDelegate(onMoveRight));
            m_inputKeyboard.registerCommand(Keys.Escape, true, new InputDeviceHelper.CommandDelegate(onEscape));

            m_texPaddle = contentManager.Load<Texture2D>("Images/paddle");
            m_texCircle = contentManager.Load<Texture2D>("Images/ball-white");
            m_texLives = contentManager.Load<Texture2D>("Images/lives");
            m_texBlock = contentManager.Load<Texture2D>("Images/block");

            /*Sound effects by
            https://pixabay.com/sound-effects/search/block/?manual_search=1&order=None */
            m_audio_die = contentManager.Load<SoundEffect>("Audio/sound-1");
            m_audio_break = contentManager.Load<SoundEffect>("Audio/sound-2");
            m_audio_paddle = contentManager.Load<SoundEffect>("Audio/sound-3");

            /*Space Jazz by Kevin MacLeod | https://incompetech.com/
            Music promoted by https://www.chosic.com/free-music/all/
            Creative Commons CC BY 3.0
            https://creativecommons.org/licenses/by/3.0/ */
            m_audio_music = contentManager.Load<Song>("Audio/music");

            m_font = contentManager.Load<SpriteFont>("Fonts/menu-select");

            m_lives = new Stack<Rectangle>();

            scoreKeeper = new ScoreKeeper();
            balls = new BallList(m_audio_die);
            paddle = new Paddle(Box.x2/2 - 200, Box.y2 - 100, 400, 50, m_font, m_lives, m_audio_paddle);
            balls.Subscribe(paddle);

            scoreLocation = new Vector2(Box.x2 - 350, Box.y2 - 200);
            m_vectors = new List<Vector2>();
            m_vectors.Add(new Vector2(Box.x2 - 351, Box.y2 - 201));
            m_vectors.Add(new Vector2(Box.x2 - 351, Box.y2 - 199));
            m_vectors.Add(new Vector2(Box.x2 - 349, Box.y2 - 201));
            m_vectors.Add(new Vector2(Box.x2 - 349, Box.y2 - 199));

            m_emitters = new List<ParticleEmitter>();
            blocks = new BlockHolder(Box.x2 - Box.x1, m_texBlock, paddle, scoreKeeper, m_audio_break, m_emitters);
        }

        public override void reset()
        {
            populateLives();
            balls.balls.Clear();
            paddle.resetLength();
            MediaPlayer.Play(m_audio_music);
            blocks.Reset();
            scoreKeeper.reset();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            // if you died game over
            if (paddle.gameOver)
            {
                MediaPlayer.Stop();
                scoreKeeper.saveSomething();
                return GameStateEnum.MainMenu;
            }
            returnEnum = blocks.Update();
            // if you won game over
            if (returnEnum == GameStateEnum.MainMenu)
            {
                MediaPlayer.Stop();
                scoreKeeper.saveSomething();
            }
            m_inputKeyboard.Update(gameTime);
            GameStateEnum temp = returnEnum;
            returnEnum = GameStateEnum.GamePlay;
            return temp;
        }

        public override void update(GameTime gameTime)
        {
            // adds a ball when the countdown finishes
            if (balls.balls.Count == 0 && paddle.isLiving())
            {
                addBall();
            }
            foreach (Ball ball in balls.balls)
            {
                ball.update(gameTime);
            }
            balls.Update(gameTime);
            paddle.Update(gameTime);
            foreach (ParticleEmitter emitter in m_emitters)
            {
                emitter.update(gameTime);
            }
            // this is for every 100 points
            if (scoreKeeper.addball)
            {
                addBall();
                scoreKeeper.addball = false;
            }
            
        }

        public override void render(GameTime gameTime)
        {
            base.render(gameTime);
            foreach (Ball ball in balls.balls)
            {
                ball.draw(gameTime, m_spriteBatch, m_texCircle);
            }
            blocks.Draw(gameTime, m_spriteBatch);
            paddle.draw(gameTime, m_spriteBatch, m_texPaddle);
            foreach (ParticleEmitter emitter in m_emitters)
            {
                emitter.draw(m_spriteBatch);
            }
            m_spriteBatch.Begin();
            foreach (Rectangle rect in m_lives)
            {
                m_spriteBatch.Draw(
                m_texLives,
                rect,
                Color.White);
            }
            foreach (Vector2 vector in m_vectors)
            {
                m_spriteBatch.DrawString(m_font, "Score: " + scoreKeeper.score.ToString(), vector, Color.Black);
            }
            m_spriteBatch.DrawString(m_font, "Score: " + scoreKeeper.score.ToString(), scoreLocation, Color.White);
            m_spriteBatch.End();

                   }

        #region Input Handlers
        /// <summary>
        /// The various moveX methods subtract half of the height/width because the rendering is performed
        /// from the center of the rectangle because it can rotate
        /// </summary>

        private void onMoveLeft(GameTime gameTime, float scale)
        {
            paddle.moveLeft(gameTime, scale, SPRITE_MOVE_PIXELS_PER_MS);
            returnEnum = GameStateEnum.GamePlay;
        }

        private void onMoveRight(GameTime gameTime, float scale)
        {
            paddle.moveRight(gameTime, scale, SPRITE_MOVE_PIXELS_PER_MS);
            returnEnum = GameStateEnum.GamePlay;

        }

        private void onEscape(GameTime gameTime, float scale)
        {
            returnEnum = GameStateEnum.Pause;
            scoreKeeper.saveSomething();
        }
        #endregion

        #region addBall

        private void addBall()
        {
            Ball ball = new Ball(paddle.x + paddle.length / 2, paddle.y - 17, rnd.nextRange((float)-0.5, (float)0.5), m_audio_paddle);
            ball.Subscribe(paddle);
            ball.Subscribe(balls);
            ball.Subscribe(blocks);
            balls.balls.Add(ball);
        }
        #endregion
        private void populateLives()
        {
            m_lives.Clear();
            for (int i = 0; i < startLives; i++)
            {
                m_lives.Push(new Rectangle(i * 75 + 50, Box.y2 - 50, 50, 50));
            }
        }
    }
}
