using Breakout.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breakout.Views
{
    public class MainMenuView : GameStateView
    {
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;

        private KeyboardInput m_inputKeyboard;
        private GameStateEnum returnEnum = GameStateEnum.MainMenu;

        private SoundEffect soundEffect;

        //enum for the different menus, borrowed from startercode
        private enum MenuState
        {
            NewGame,
            HighScores,
            Help,
            About,
            Quit
        }

        private MenuState m_currentSelection = MenuState.NewGame;

        public override void loadContent(ContentManager contentManager)
        {
            base.loadContent(contentManager);
            //Keyboard input for up down and enter
            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Down, true, new InputDeviceHelper.CommandDelegate(onDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new InputDeviceHelper.CommandDelegate(onUp));
            m_inputKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate(onEnter));

            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");

            soundEffect = contentManager.Load<SoundEffect>("Audio/sound-4");
        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputKeyboard.Update(gameTime);
            //if return enum changed we'll go to the new view
            GameStateEnum temp = returnEnum;
            returnEnum = GameStateEnum.MainMenu;
            return temp;
        }
        public override void update(GameTime gameTime)
        {
        }
        public override void render(GameTime gameTime)
        {
            base.render(gameTime);
            m_spriteBatch.Begin();

            // I split the first one's parameters on separate lines to help you see them better
            float bottom = drawMenuItem(
                m_currentSelection == MenuState.NewGame ? m_fontMenuSelect : m_fontMenu,
                "New Game",
                200,
                m_currentSelection == MenuState.NewGame ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.HighScores ? m_fontMenuSelect : m_fontMenu, "High Scores", bottom, m_currentSelection == MenuState.HighScores ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.Help ? m_fontMenuSelect : m_fontMenu, "Help", bottom, m_currentSelection == MenuState.Help ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == MenuState.About ? m_fontMenuSelect : m_fontMenu, "About", bottom, m_currentSelection == MenuState.About ? Color.Yellow : Color.Blue);
            drawMenuItem(m_currentSelection == MenuState.Quit ? m_fontMenuSelect : m_fontMenu, "Quit", bottom, m_currentSelection == MenuState.Quit ? Color.Yellow : Color.Blue);

            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }

        #region input Handlers
        // if statements on down and up to ensure wraparound
        private void onDown(GameTime gametime, float scale)
        {
            m_currentSelection = m_currentSelection + 1;
            soundEffect.Play();
            if (m_currentSelection > MenuState.Quit)
            {
                m_currentSelection = MenuState.NewGame;
            }
        }

        private void onUp(GameTime gametime, float scale)
        {
            soundEffect.Play();
            m_currentSelection = m_currentSelection - 1;
            if (m_currentSelection  < MenuState.NewGame) 
            {
                m_currentSelection = MenuState.Quit;
            }
        }

        private void onEnter(GameTime gametime, float scale)
        {
            switch (m_currentSelection)
            {
                //changes the return enum
                case MenuState.NewGame:
                    returnEnum = GameStateEnum.GamePlay;
                    break;
                case MenuState.HighScores:
                    returnEnum = GameStateEnum.HighScores;
                    break;
                case MenuState.Help:
                    returnEnum = GameStateEnum.Help;
                    break;
                case MenuState.About:
                    returnEnum = GameStateEnum.About;
                    break;
                case MenuState.Quit:
                    returnEnum = GameStateEnum.Exit;
                    break;
            }
        }
        #endregion
    }
}