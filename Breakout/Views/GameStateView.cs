using Breakout.GamePlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Breakout.Views
{
    public abstract class GameStateView : IGameState
    {
        protected GraphicsDeviceManager m_graphics;
        protected SpriteBatch m_spriteBatch;

        protected Texture2D m_texBackground;
        protected Rectangle m_recBackground;

        public void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            m_graphics = graphics;
            m_spriteBatch = new SpriteBatch(graphicsDevice);
        }
        public virtual void loadContent(ContentManager contentManager)
        {
            m_texBackground = contentManager.Load<Texture2D>("Images/background");
            m_recBackground = new Rectangle(0, 0, Box.x2, Box.y2);
        }
        public abstract GameStateEnum processInput(GameTime gameTime);
        public virtual void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texBackground, m_recBackground, Color.White);
            m_spriteBatch.End();
        }
        public abstract void update(GameTime gameTime);
        public virtual void reset() { }
    }
}
