using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class ParticleEmitter
    {

        private Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        private Texture2D m_tex;
        private MyRandom m_random = new MyRandom();

        private int m_sourceX;
        private int m_sourceY;
        private int m_particleSize;
        private int m_speed;
        private TimeSpan m_lifetime;
        private Color m_color;

        public ParticleEmitter(Texture2D tex, Color color, int length, int sourceX, int sourceY, int size, int speed, TimeSpan lifetime)
        {
            m_sourceX = sourceX;
            m_sourceY = sourceY;
            m_particleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;
            m_color = color;
            m_tex = tex;
            //each particle is one pixel and takes up the space of the block
            for (int i = 0; i < length/2; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Particle p = new Particle(
                    m_random.Next(),
                    new Vector2(m_sourceX + j * size, m_sourceY + i * size),
                    //just a little bit of x direction to avoid hard lines on the side
                    new Vector2(m_random.nextRange((float)-0.1, (float)0.1), 1),
                    (float)m_random.nextGaussian(m_speed, 1),
                    m_lifetime);

                    if (!m_particles.ContainsKey(p.name))
                    {
                        m_particles.Add(p.name, p);
                    }
                }
            }

        }

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void update(GameTime gameTime)
        {
            //
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            List<int> removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                //
                // Apply some gravity
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, m_particleSize, m_particleSize);
            spriteBatch.Begin();
            foreach (Particle p in m_particles.Values)
            {
                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    m_tex,
                    r,
                    null,
                    m_color,
                    0,
                    new Vector2(m_tex.Width / 2, m_tex.Height / 2),
                    SpriteEffects.None,
                    0);
            }
            spriteBatch.End();
        }
    }
}
