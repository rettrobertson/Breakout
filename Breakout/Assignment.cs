using Breakout.GamePlay;
using Breakout.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Breakout
{
    public class Assignment : Game
    {
        private IGameState m_currentState;
        private GameStateEnum m_nextStateEnum = GameStateEnum.MainMenu;
        private Dictionary<GameStateEnum, IGameState> m_states;

        private const int height = 1080;
        private const int width = 1920;
        private GraphicsDeviceManager m_graphics;

        public Assignment()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            m_graphics.PreferredBackBufferWidth = width;
            m_graphics.PreferredBackBufferHeight = height;

            m_graphics.ApplyChanges();

            m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new MainMenuView() },
                { GameStateEnum.GamePlay, new GamePlayView() },
                { GameStateEnum.HighScores, new HighScoresView() },
                { GameStateEnum.Help, new HelpView() },
                { GameStateEnum.About, new AboutView() },
                { GameStateEnum.Pause, new PauseView() }
            };

            foreach (var item in m_states)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
            }

            m_currentState = m_states[m_nextStateEnum];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // box is an static class to tell where the balls' bounds are
            Box.y1 = 0;
            Box.x1 = 0;
            Box.x2 = width;
            Box.y2 = height;

            foreach (var item in m_states)
            {
                item.Value.loadContent(this.Content);
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            GameStateEnum temp = m_nextStateEnum;
            m_nextStateEnum = m_currentState.processInput(gameTime);
            if (m_nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }
            if (m_nextStateEnum == GameStateEnum.GamePlay && temp == GameStateEnum.MainMenu)
            {
                m_states[GameStateEnum.GamePlay].reset();
            }
            m_currentState.update(gameTime);
            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_currentState.render(gameTime);
            m_currentState = m_states[m_nextStateEnum];

            // TODO: Add your drawing code here
            

            base.Draw(gameTime);
        }
    }
}