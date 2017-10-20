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
    class newParticle
    {
        public Rectangle bounds,
                         sRect;
        public Texture2D texture;
        public Color color,
                     startcolor,
                     endcolor;
        public Vector2 velocity,
                       acceleration,
                       growth,
                       
                       minVel,
                       maxVel,

                       Position,
                       Size,
                       
                       origin;
        public float rotation,
                      rotationVal,
                      lifetime,
                      lifeleft,
                      percentLife,
                      colorLerp,
                      velModifyStrength;
        public Point maxSize,
                     minSize;
        public bool Collision,
                    Alive = true,
                    Active,
                    velModifyLength = false;

        public newParticle()
        {
            Active = false;
            Alive = true;
        }

        public newParticle(Rectangle bounds, Texture2D texture, Rectangle sRect, Color startcolor, Color endcolor, float colorLerp, Vector2 velocity, Vector2 minVel, Vector2 maxVel, Vector2 acceleration, Vector2 growth, Point maxSize, Point minSize, 
            float rotation, float rotationVal, float lifetime, bool Collision, Vector2 origin)
        {
            this.bounds = bounds;
            this.Position = new Vector2(bounds.X, bounds.Y);
            this.Size = new Vector2(bounds.Width, bounds.Height);
            this.texture = texture;
            this.sRect = sRect;
            this.color = startcolor;
            this.startcolor = startcolor;
            this.endcolor = endcolor;
            this.colorLerp = colorLerp;
            this.velocity = velocity;
            this.minVel = minVel;
            this.maxVel = maxVel;
            this.acceleration = acceleration;
            this.growth = growth;
            this.maxSize = maxSize;
            this.minSize = minSize;
            this.rotation = rotation;
            this.rotationVal = rotationVal;
            this.lifetime = lifetime;
            this.lifeleft = lifetime;
            this.Collision = Collision;
            this.Active = true;
            this.Alive = true;
            this.percentLife = 1f;
        }

        public void Delete()
        {
            this.bounds = Rectangle.Empty;
            this.Size = Vector2.Zero;
            this.Position = Vector2.Zero;
            this.velocity = Vector2.Zero;
            this.minVel = Vector2.Zero;
            this.maxVel = Vector2.Zero;
            this.acceleration = Vector2.Zero;
            this.growth = Vector2.Zero;
            this.maxSize = Point.Zero;
            this.minSize = Point.Zero;
            this.rotation = 0;
            this.rotationVal = 0;
            this.lifetime = 0;
            this.lifeleft = 0;
            this.Active = false;
            this.Alive = false;
            this.percentLife = 0f;
        }

        public void Update()
        {
            if (Alive == false || lifetime <= 0 || Active == false)
                return;

            if (lifetime != 0)
            {
                percentLife -= 1f / (lifetime * 60);
                if (percentLife < 0)
                    percentLife = 0;
            }

            Alive = true;
            Active = true;

            Vector2 prevPos = new Vector2(bounds.X, bounds.Y);

            Position += velocity;
            velocity += acceleration;
            bounds.X = (int)(Position.X);
            bounds.Y = (int)(Position.Y);

            if (bounds.Width > 0 && bounds.Height > 0)
            {                
                Size += growth;
                bounds.Width = (int)Size.X;
                bounds.Height = (int)Size.Y;
            }

            rotation += rotationVal;
            lifeleft -= 0.01f;

            if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < 0.00000000001f)
                velocity = Vector2.Zero;

            color = Color.Lerp(color, endcolor, colorLerp / lifetime);
            color.A = Convert.ToByte(255f * percentLife);
                //color.A = 255;

            //color.R += Convert.ToByte(Color.Lerp(startcolor, endcolor,1f/lifetime).R/3.0f);
            //color.G += Convert.ToByte(Color.Lerp(startcolor, endcolor, 1f / lifetime).G / 3.0f);
            //color.B += Convert.ToByte(Color.Lerp(startcolor, endcolor, 1f / lifetime).B / 3.0f);
            //color.R = Convert.ToByte(Difference(startcolor.R, endcolor.R) * (percentLife));
            //color.G = Convert.ToByte(Difference(startcolor.G, endcolor.G) * (percentLife));
            //color.B = Convert.ToByte(Difference(startcolor.B, endcolor.B) * (percentLife));
            //Vector4 v = Vector4.Lerp(endcolor.ToVector4(), color.ToVector4(), 1.0f / lifetime);
            //color = Color.FromNonPremultiplied((int)(v.X * 1000f), (int)(v.Y * 1000f), (int)(v.Z * 1000f), 255);
            //color = Color.Lerp(color, endcolor, 1.0f / Vector2.Distance(origin, new Vector2(bounds.X, bounds.Y)));
            //if (color.A < 50)
                //color.A = 50;

            //color.R = Convert.ToByte(endcolor.R / (lifetime - lifeleft));
            //color.G = Convert.ToByte(endcolor.G / (lifetime - lifeleft));
            //color.B = Convert.ToByte(endcolor.B / (lifetime - lifeleft));
            //color.A = ;

            //Cool effect
            /*
             * 
            //Cool effect too
             * 
            color.R += Color.Lerp(color, fade, lifetime).R;
            color.G += Color.Lerp(color, fade, lifetime).G;
            color.B += Color.Lerp(color, fade, lifetime).B;
            color.A += Color.Lerp(color, fade, lifetime).A;
             * 
            color.R += Color.Lerp(color, fade, 1 / lifetime).R;
            color.G += Color.Lerp(color, fade, 1 / lifetime).G;
            color.B += Color.Lerp(color, fade, 1 / lifetime).B;
            color.A += Color.Lerp(color, fade, 1 / lifetime).A;
             * */

            if (velModifyLength == true)
            {
                bounds.Width = (int)(velocity.Length() * velModifyStrength);
            }


            velocity.X = MathHelper.Clamp(velocity.X, -7, 7);
            velocity.Y = MathHelper.Clamp(velocity.Y, -7, 7);

            rotation = (float)Math.Atan2(bounds.Y - prevPos.Y, bounds.X - prevPos.X);

            if (lifeleft <= 0.0f || percentLife <= 0.0f)
            {
                Delete();
            }
        }

        private float Difference(float one, float two)
        {
            float diff = one - two;

            if (diff < 0)
                diff *= -1;

            return diff;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (Alive == false || Active == false)
            {
                return;
            }

            spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X, bounds.Y - (int)camera.Position.Y, bounds.Width, bounds.Height), 
                sRect, color, rotation, new Vector2(texture.Bounds.Width / 2.0f, texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alive == false || Active == false)
                return;

            spriteBatch.Draw(texture, bounds,
                sRect, color, rotation, new Vector2(sRect.Center.X, sRect.Center.Y), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, bounds,
                sRect, color, rotation, new Vector2(sRect.Center.X, sRect.Center.Y), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, bounds,
                sRect, color, rotation, new Vector2(sRect.Center.X, sRect.Center.Y), SpriteEffects.None, 0);

            //spriteBatch.Draw(texture, new Rectangle(bounds.X - 0, bounds.Y - 0, (int)(100 ), (int)(100 )),
            //    sRect, Color.FromNonPremultiplied(color.R, color.G, color.B, (int)(10*velocity.Length())), rotation, new Vector2(sRect.Center.X, sRect.Center.Y), SpriteEffects.None, 0);
        }
    }
}
