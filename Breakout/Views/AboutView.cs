using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breakout.Views
{
    public class AboutView : GameStateView
    {
        // very simple, just like the example given
        private SpriteFont m_font;
        private const string MESSAGE = "*Everything* done by Rett Robertson";

        public override void loadContent(ContentManager contentManager)
        {
            base.loadContent(contentManager);
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.About;
        }

        public override void render(GameTime gameTime)
        {
            base.render(gameTime);
            m_spriteBatch.Begin();

            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font, MESSAGE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y), Color.Yellow);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
