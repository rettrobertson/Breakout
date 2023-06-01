using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Breakout.GamePlay;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System;

namespace Breakout.Views
{
    public class HighScoresView : GameStateView
    {
        //very simple loading, just like the example. highscores is an object that holds the array of high scores
        private SpriteFont m_font;
        private const string MESSAGE = "These are the high scores";

        private HighScores highscores;
        private bool saving = false;
        private bool loading = false;
        private bool keypress = false;

        public override void loadContent(ContentManager contentManager)
        {
            base.loadContent(contentManager);
            highscores = new HighScores(new (int, DateTime)[] { (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now) });
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            loadSomething();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R)) 
            {
                highscores = new HighScores(new (int, DateTime) [] { (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now) , (0, DateTime.Now) });
                saveSomething();
                keypress = true;
            }
            else
            {
                loadSomething();
            }
            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            base.render(gameTime);
            m_spriteBatch.Begin();

            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font, MESSAGE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y * 5), Color.Yellow);
            int i = 0;
            foreach ((int, DateTime) score in highscores.scores)
            {
                m_spriteBatch.DrawString(m_font, score.Item1.ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight / 2 - (stringSize.Y *i)), Color.White);
                i++;
            }
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
        private void saveSomething()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    //
                    // Create something to save
                    finalizeSaveAsync(highscores);
                }
            }
        }

        private async void finalizeSaveAsync(HighScores state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("YouShouldNotHaveThisFileOnYourComputerAlready.xml", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(HighScores));
                                mySerializer.Serialize(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.saving = false;
            });
        }

        private void loadSomething()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    finalizeLoadAsync();
                }
            }
        }

        private async void finalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("YouShouldNotHaveThisFileOnYourComputerAlready.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(HighScores));
                                    highscores = (HighScores)mySerializer.Deserialize(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this.loading = false;
            });
        }
    }
}
