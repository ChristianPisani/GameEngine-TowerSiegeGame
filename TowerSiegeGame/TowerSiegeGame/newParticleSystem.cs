using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace TowerSiegeGame
{
    class newParticleSystem
    {
        public List<newParticle> particles = new List<newParticle>();
        public float timeToUpdate,
                     updateInterval;
        public int particleAmount,
                   maxParticles;
        public Vector2 origin,
                       minOffSet,
                       maxOffSet;

        public Rectangle sRect;
        public Texture2D texture;
        public Color color,
                     fade;
        public Vector2 minAcceleration,
                       maxAcceleration,
                       minGrowth,
                       maxGrowth,

                       minVel,
                       maxVel;
        public float pRotation,
                      minRotationVal,
                      maxRotationVal,
                      minLifetime,
                      maxLifetime,
                      colorLerp,
                      velModifierStrength;
        public Point maxSize,
                     minSize;
        public bool CollisionEnabled;

        public float gravity;

        public int range;

        Random rnd;

        public int removalCount,
            particleCount;

        public List<GravityWell> gravityWells = new List<GravityWell>();

        public bool velModifyLength = false;

        public newParticleSystem(Random rnd, float updateInterval, int maxParticles, int amount, Texture2D texture, Rectangle sRect, Color color, Color fade, float colorLerp,
            Vector2 origin, Vector2 minOffSet, Vector2 maxOffSet, Vector2 minAcl, Vector2 maxAcl, Point minSize, Point maxSize, Vector2 minGrowth, Vector2 maxGrowth, Vector2 minVel, Vector2 maxVel,
            float pRotation, float minRotation, float maxRotation, float minLifetime, float maxLifetime, bool CollisionEnabled, float gravity)
        {
            this.rnd = rnd;
            this.updateInterval = updateInterval;
            this.particleAmount = amount;
            this.texture = texture;
            this.sRect = sRect;
            this.color = color;
            this.fade = fade;
            this.origin = origin;
            this.minOffSet = minOffSet;
            this.maxOffSet = maxOffSet;
            this.minAcceleration = minAcl;
            this.maxAcceleration = maxAcl;
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.minGrowth = minGrowth;
            this.maxGrowth = maxGrowth;
            this.minVel = minVel;
            this.maxVel = maxVel;
            this.pRotation = pRotation;
            this.minRotationVal = minRotation;
            this.maxRotationVal = maxRotation;
            this.minLifetime = minLifetime;
            this.maxLifetime = maxLifetime;
            this.CollisionEnabled = CollisionEnabled;
            this.gravity = gravity;
            this.maxParticles = maxParticles;
            this.colorLerp = colorLerp;

            for (int i = 0; i < maxParticles; i++)
            {
                particles.Add(new newParticle());

                CorrectValues();
            }
        }

        private void CorrectValues()
        {
            if (minSize.X > maxSize.X)
                maxSize.X = minSize.X;

            if (minSize.Y > maxSize.Y)
                maxSize.Y = minSize.Y;

            if (minOffSet.X > maxOffSet.X)
                maxOffSet.X = minOffSet.X;

            if (minOffSet.Y > maxOffSet.Y)
                maxOffSet.Y = minOffSet.Y;

            if (minAcceleration.X > maxAcceleration.X)
                maxAcceleration.X = minAcceleration.X;

            if (minAcceleration.Y > maxAcceleration.Y)
                maxAcceleration.Y = minAcceleration.Y;

            if (minGrowth.X > maxGrowth.X)
                maxGrowth.X = minGrowth.X;

            if (minGrowth.Y > maxGrowth.Y)
                maxGrowth.Y = minGrowth.Y;

            if (minVel.X > maxVel.X)
                maxVel.X = minVel.X;

            if (minVel.Y > maxVel.Y)
                maxVel.Y = minVel.Y;

            if (minRotationVal > maxRotationVal)
                maxRotationVal = minRotationVal;

            if (minLifetime > maxLifetime)
                maxLifetime = minLifetime;
        }

        public void Update()
        {
            rnd = new Random();
            timeToUpdate -= 0.01f;

            removalCount = 0;
            //particleCount = 0;
            int count = 0;

            for (int i = 0; i < particleCount; i++)
            {
                var temp = particles[i];
                particles[i] = particles[i - removalCount];
                particles[i - removalCount] = temp;

                if (particles[i].Alive == false)
                {
                    removalCount++;
                }

                if (particles[i].Active == true)
                {
                    count++;
                }

                particles[i].velocity.Y += gravity;
                particles[i].Update();
            }

            particleCount = count;

            if (timeToUpdate <= 0)
            {
                CorrectValues();

                for (int i = 0; i < particleAmount; i++)
                {
                    particleCount++;

                    if (particleCount >= maxParticles - 1)
                    {
                        particleCount = maxParticles - 1;
                        return;
                    }

                    int size = rnd.Next(minSize.X, maxSize.X);

                    newParticle particle = new newParticle(
                    new Rectangle((int)origin.X + (int)(rnd.Next((int)(minOffSet.X * 10000.0f), (int)(maxOffSet.X * 10000.0f)) / 10000.0f),
                                  (int)origin.Y + (int)(rnd.Next((int)(minOffSet.Y * 10000.0f), (int)(maxOffSet.Y * 10000.0f)) / 10000.0f),
                                  size,
                                  size),
                    texture,
                    sRect,
                    color,
                    fade,
                    colorLerp,
                    new Vector2(rnd.Next((int)(minVel.X * 10000.0f), (int)(maxVel.X * 10000.0f)) / 10000.0f, rnd.Next((int)(minVel.Y * 10000.0f), (int)(maxVel.Y * 10000.0f)) / 10000.0f),
                    minVel,
                    maxVel,
                    new Vector2(rnd.Next((int)(minAcceleration.X * 10000.0f), (int)(maxAcceleration.X * 10000.0f)) / 10000.0f, rnd.Next((int)(minAcceleration.Y * 10000.0f), (int)(maxAcceleration.Y * 10000.0f)) / 10000.0f),
                    new Vector2(rnd.Next((int)(minGrowth.X * 10000.0f), (int)(maxGrowth.X * 10000.0f)) / 10000.0f, rnd.Next((int)(minGrowth.Y * 10000.0f), (int)(maxGrowth.Y * 10000.0f)) / 10000.0f),
                    maxSize,
                    minSize,
                    pRotation,
                    rnd.Next((int)(minRotationVal * 10000.0f), (int)(maxRotationVal * 10000.0f)) / 10000.0f,
                    rnd.Next((int)(minLifetime * 10000.0f), (int)(maxLifetime * 10000.0f)) / 10000.0f,
                    CollisionEnabled,
                    origin);

                    particles[particleCount] = particle;

                    particle.velModifyLength = velModifyLength;
                    particle.velModifyStrength = velModifierStrength;

                    particle.velocity.Y += gravity;
                    particle.Update();
                }

                timeToUpdate = updateInterval;
            }

            foreach (GravityWell well in gravityWells)
            {
                well.Suck(particles);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            for (int i = 0; i < particleCount; i++)
            {
                particles[i].Draw(spriteBatch, camera);

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //BlendState blend = spriteBatch.GraphicsDevice.BlendState;
            //spriteBatch.GraphicsDevice.BlendState = BlendState.Additive;
            for (int i = 0; i < particleCount; i++)
            {
                particles[i].Draw(spriteBatch);
            }
            //spriteBatch.GraphicsDevice.BlendState = blend;
            spriteBatch.End();

            spriteBatch.Begin();
        }
    }
}
