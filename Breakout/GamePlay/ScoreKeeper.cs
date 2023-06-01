using System.IO.IsolatedStorage;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System;

namespace Breakout.GamePlay
{
    public class ScoreKeeper
    {
        // keeps track of current score and stores high scores for updating, addball is for if we need to add a ball
        public int score { get; set; }
        public bool addball;
        private HighScores scores; 
        private bool saving = false;
        private bool loading = false;
        private DateTime TimeStamp { get; set; }

        public ScoreKeeper()
        {
            score = 0;
            addball = false;
            scores =new HighScores(new (int, DateTime)[] { (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now) });
            this.TimeStamp= DateTime.Now;
            loadSomething();
        }

        public void Update(int score)
        {
            // we add points one at a time to see if we cross a point for adding a ball
            for (int i = this.score + 1; i <= this.score + score; i++)
            {
                if (i % 100 == 0)
                {
                    addball = true;
                }
            }
            this.score += score;
            // check if new scores is higher than any high scores
            scores.check(this.score, this.TimeStamp);
        }

        //for new game
        public void reset()
        {
            score = 0;
            addball = false;
            scores = new HighScores(new (int, DateTime)[] { (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now), (0, DateTime.Now) });
            this.TimeStamp = DateTime.Now;
            loadSomething();
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
                                    scores = (HighScores)mySerializer.Deserialize(fs);
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

        public void saveSomething()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    finalizeSaveAsync(scores);
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

    }
}
