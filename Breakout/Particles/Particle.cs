using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410.Particles
{
    public class Particle
    {
        public Particle(int name, Vector2 position, Vector2 direction, float speed, TimeSpan lifetime)
        {
            this.name = name;
            this.position = position;
            this.direction = direction;
            //just to be sure no particles "fall" upwards
            if (speed <= 0)
            {
                this.speed = speed * -1;
            }
            else
            {
                this.speed = speed;
            }
            this.lifetime = lifetime;

        }

        public int name;
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public TimeSpan lifetime;
    }
}
