/*using System;
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
    class Particle : Entity
    {
        new public int lifeTime = 30;
        new public bool Existing = true;
        new public Color color;

        public int timeAlive = 0;

        public string Type = "default";

        public Vector2 particlevelocity;

        public float Rotation = 0,
                     Opacity = 100,
                     lerpAmount = 0.1f;

        public bool Lerp = true;

        public Particle(Rectangle bounds, float bounciness, bool alwaysUpdate, bool checkNPCs, bool checkPlayer, KryptonEngine krypton) : 
            base(bounds, bounciness, alwaysUpdate, checkNPCs, checkPlayer, krypton)
        {
            gravity = 0;
        }

        public override void Update(CollisionMap collisionMap, Camera camera, bool getColDir, List<NPC> NPCs, Player player, List<Entity> entities, bool collideInEntities, Rectangle window,
            List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, -200, (collisionMap.height - 3) * collisionMap.TileSize - bounds.Height - 1);

            base.Update(collisionMap, camera, getColDir, NPCs, player, entities, collideInEntities, window, electronics, krypton);

            if (Lerp == true)
            {
                particlevelocity.X = MathHelper.Lerp(particlevelocity.X, 0, lerpAmount);
                particlevelocity.Y = MathHelper.Lerp(particlevelocity.Y, 0, lerpAmount);
            }

            velocity = particlevelocity;

            timeAlive += 1;

            Random rnd = new Random();

            //Rotation += 0.1f;

            
            int c = lifeTime / timeAlive;

            c = (int)MathHelper.Clamp(c * 10, 0, 255);

            Opacity -= Opacity / lifeTime;

            color.A = (byte)((255 - ((c))));
            color.A = (byte)Opacity;
            color.A = (byte)MathHelper.Clamp(color.A, 0, 255);

            //bounds.Width -= (int)(lifeTime / (lifeTime * 0.59f));
            //bounds.Height -= (int)(lifeTime / (lifeTime * 0.59f));
            if (colXLeft == false && colXRight == false && colYDown == false && colYUp == false)
            {
                bounds.Width += 10;
                bounds.X -= 5;
                bounds.Height += 10;
                bounds.Y -= 5;
            }

            bounds.Width = (int)MathHelper.Clamp(bounds.Width, 4, 3000);
            bounds.Height = (int)MathHelper.Clamp(bounds.Height, 4, 3000);
             

            if (timeAlive == lifeTime)
                Existing = false;
        }
    }
}*/
