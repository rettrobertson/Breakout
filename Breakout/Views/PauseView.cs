using Breakout.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Views
{
    public  class PauseView : GameStateView
    {
        // simple pause screen, similar to main menu, because there's only two options I use a bool insead of enum
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;

        private KeyboardInput m_inputKeyboard;
        GameStateEnum returnEnum = GameStateEnum.Pause;

        bool resume = true;

        private SoundEffect soundEffect;

        public override void loadContent(ContentManager contentManager)
        {
            base.loadContent(contentManager);
            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Down, true, new InputDeviceHelper.CommandDelegate(onDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new InputDeviceHelper.CommandDelegate(onDown));
            m_inputKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate(onEnter));

            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");

            soundEffect = contentManager.Load<SoundEffect>("Audio/sound-4");
        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputKeyboard.Update(gameTime);
            GameStateEnum temp = returnEnum;
            returnEnum = GameStateEnum.Pause;
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
                resume ? m_fontMenuSelect : m_fontMenu,
                "Resume",
                200,
                resume ? Color.Yellow : Color.Blue);
            drawMenuItem(!resume ? m_fontMenuSelect : m_fontMenu, "Main Menu", bottom, !resume ? Color.Yellow : Color.Blue);

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
        private void onDown(GameTime gametime, float scale)
        {
            soundEffect.Play();
            resume = !resume;
        }

        private void onEnter(GameTime gametime, float scale)
        {
            if (resume)
            {
                returnEnum = GameStateEnum.GamePlay;
            }
            else
            {
                MediaPlayer.Stop();
                returnEnum = GameStateEnum.MainMenu;
            }
        }
        #endregion

    }
}
