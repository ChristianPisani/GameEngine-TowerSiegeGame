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
    class ParticleSystem
    {
        List<Object> Entitys = new List<Object>();

        Rectangle EntityBounds;
        List<Color> EntityColors = new List<Color>();
        List<Texture2D> EntityTextures = new List<Texture2D>();

        bool destroyEntitysOnTouch,
             CollisionDetection;
        float gravity,
              maxVelX,
              minVelX,
              maxVelY,
              minVelY,
              maxLifeTime,
              minLifeTime,
              weight,
              trailLength;

        public ParticleSystem(Rectangle EntityBounds, List<Color> EntityColors, bool destroyOnTouch, float gravity,
            float maxVelX, float minVelX, float maxVelY, float minVelY, float maxLifeTime, float minLifeTime, List<Texture2D> textures,
            int trailLength, float weight, bool CollisionDetection)
        {
            this.EntityBounds = EntityBounds;
            this.EntityColors = EntityColors;
            this.destroyEntitysOnTouch = destroyOnTouch;
            this.gravity = gravity;
            this.maxVelX = maxVelX;
            this.minVelX = minVelX;
            this.maxVelY = maxVelY;
            this.minVelY = minVelY;
            this.maxLifeTime = maxLifeTime;
            this.minLifeTime = minLifeTime;
            this.EntityTextures = textures;
            this.weight = weight;
            this.trailLength = trailLength;
            this.CollisionDetection = CollisionDetection;
        }

        public void Update(int amount,  Rectangle systemBounds, Random rnd,
            CollisionMap collisionMap, Camera camera, List<NPC> NPCs, Player player, List<Object> entities, Rectangle windowSize, List<ElectronicComponent> electronics,
            KryptonEngine krypton)
        {
            List<int> DeleteList = new List<int>();
            DeleteList.Clear();
            int o = 0;

            for (int i = 0; i < amount; i++)
            {
                Object Entity = new Object(new Rectangle(rnd.Next(0, systemBounds.Width) + (int)camera.Position.X,
                    rnd.Next(systemBounds.Y, systemBounds.Y + systemBounds.Height) + (int)camera.Position.Y, 
                    EntityBounds.Width, EntityBounds.Height), 0.0f, false, false, false, krypton, false);
                Entity.collisiondetection = CollisionDetection;
                Entity.destroyOnTouch = destroyEntitysOnTouch;
                Entity.collideInEntities = false;
                Entity.color = EntityColors[rnd.Next(0, EntityColors.Count)];
                Entity.texture = EntityTextures[rnd.Next(0, EntityTextures.Count - 1)];
                Entity.velocity.X = rnd.Next((int)(minVelX * 1000), (int)(maxVelX * 1000)) / 1000.0f;
                Entity.gravity = gravity;
                Entity.velocity.Y = rnd.Next((int)(minVelY * 1000), (int)(maxVelY * 1000)) / 1000.0f;
                Entity.lifeTime = (int)(rnd.Next((int)(minLifeTime * 1000), (int)(maxLifeTime * 1000)) / 1000.0f);
                Entity.trailLength = 2;
                Entity.weight = 0;
                Entity.bouncesBeforeDeletion = 1;
                Entity.friction = 1000;
                Entitys.Add(Entity);
            }

            foreach (Object Entity in Entitys)
            {
                Entity.Update(collisionMap, camera, true, NPCs, player, entities, windowSize, electronics, krypton);

                if (Entity.Existing == false)
                {
                    DeleteList.Add(o);
                }
                else
                {
                    o++;
                }

                if (Entity.DeleteNextFrame == true)
                {
                    Entity.Existing = false;
                }
            }

            if (DeleteList.Count > 0)
            {
                for (int j = 0; j < DeleteList.Count; j++)
                {
                    Entitys.RemoveAt(DeleteList[j]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (Object Entity in Entitys)
            {
                if (Entity.trail.Count > 0)
                {
                    Rectangle rect = Entity.trail[0];

                    Vector2 direction = new Vector2(rect.Center.X, rect.Center.Y) - new Vector2(Entity.bounds.Center.X, Entity.bounds.Center.Y);
                    direction.Normalize();

                    float rotation = (float)Math.Atan2(
                                  (double)direction.Y,
                                  (double)direction.X);

                    int length = (int)new Vector2(Entity.bounds.Center.X - rect.Center.X, Entity.bounds.Center.Y - rect.Center.Y).Length();

                    spriteBatch.Draw(Entity.texture,
                                new Rectangle(((int)Entity.bounds.Center.X - (int)(Entity.bounds.Center.X - rect.Center.X) / 2) - (int)camera.Position.X,
                               ((int)Entity.bounds.Center.Y - ((int)Entity.bounds.Center.Y - rect.Center.Y) / 2) - (int)camera.Position.Y,
                               length, EntityBounds.Width), null,
                               Entity.color, rotation, new Vector2(Entity.texture.Bounds.Width / 2.0f, Entity.texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
                }

                spriteBatch.Draw(Entity.texture, new Rectangle(Entity.bounds.X - (int)camera.Position.X,
                                            Entity.bounds.Y - (int)camera.Position.Y,
                                            Entity.bounds.Width, Entity.bounds.Height),
                                            Entity.color);
            }
        }
    }
}
