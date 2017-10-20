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
    public class Block
    {
        public Rectangle bounds;
        public Color color;

        public Rectangle left,
                         right,
                         up,
                         down;

        public float Fragility = 0;

        public bool Selected = false,
                    Existing = true,
                    Background = false,
                    
                    stoppedX,
                    stoppedY,
                    
                    Shatterable = false,
                    CastShadows = true,
                    Shattered = false;

        Random rnd = new Random();

        public Vector2 shardVelocity = new Vector2(0, 0);

        public SoundEffectInstance shatterSound;

        public int blockID;

        void SetUp(Rectangle bounds, Color color)
        {
            this.bounds = bounds;
            this.color = color;

            left = new Rectangle(bounds.X, bounds.Y + 1, 2, bounds.Height - 2);
            right = new Rectangle(bounds.Right - 2, bounds.Y + 1, 2, bounds.Height - 2);
            up = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - 2, 2);
            down = new Rectangle(bounds.X + 1, bounds.Bottom - 2, bounds.Width - 2, 2);

            AssignBlockID(rnd.Next(-99999, 99999));
        }

        public Block(Rectangle bounds, Color color)
        {
            SetUp(bounds, color);
        }

        public Block(Rectangle bounds, Color color, SoundEffectInstance shatterSound)
        {
            SetUp(bounds, color);

            this.shatterSound = shatterSound;
        }

        public void AssignBlockID(int blockID)
        {
            this.blockID = blockID;
        }

        public void UpdateBlock(int newposX, int newposY, int newwidth, int newheight)
        {
            bounds.X = newposX;
            bounds.Y = newposY;
            bounds.Width = newwidth;
            bounds.Height = newheight;

            left = new Rectangle(bounds.X, bounds.Y + 1, 2, bounds.Height - 2);
            right = new Rectangle(bounds.Right - 2, bounds.Y + 1, 2, bounds.Height - 2);
            up = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - 2, 2);
            down = new Rectangle(bounds.X + 1, bounds.Bottom - 2, bounds.Width - 2, 2);
        }

        public void UpdateBlock(int newposX, int newposY, int newwidth, int newheight, CollisionMap collisionMap)
        {
            bounds.X = newposX;
            bounds.Y = newposY;
            bounds.Width = newwidth;
            bounds.Height = newheight;

            if (bounds.X < 0 || bounds.X > (collisionMap.width - 1) * collisionMap.TileSize - bounds.Width)
            {
                stoppedX = true;
            }
            else
            {
                stoppedX = false;
            }

            if (bounds.Y < 0 || bounds.Y > (collisionMap.height - 1) * collisionMap.TileSize - bounds.Height)
            {
                stoppedY = true;
            }
            else
            {
                stoppedY = false;
            }

            if (Shattered == true)
            {
                shatterSound.Play();
            }

            bounds.X = (int)MathHelper.Clamp(bounds.X, 0, (collisionMap.width - 1) * collisionMap.TileSize - bounds.Width);
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, 0, (collisionMap.height - 1) * collisionMap.TileSize - bounds.Height);

            left = new Rectangle(bounds.X, bounds.Y + 1, 2, bounds.Height - 2);
            right = new Rectangle(bounds.Right - 2, bounds.Y + 1, 2, bounds.Height - 2);
            up = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - 2, 2);
            down = new Rectangle(bounds.X + 1, bounds.Bottom - 2, bounds.Width - 2, 2);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Camera camera, bool Debug)
        {
            spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X, bounds.Y - (int)camera.Position.Y, 
                bounds.Width, bounds.Height), color);

            if (Selected == true)
            {
                spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X, bounds.Y - (int)camera.Position.Y,
                    bounds.Width, bounds.Height), Color.FromNonPremultiplied(255, 255, 0, 255));
            }

            if (Debug == true)
            {
                spriteBatch.Draw(texture, new Rectangle(left.X - (int)camera.Position.X, left.Y - (int)camera.Position.Y,
                    left.Width, left.Height), Color.Green);
                spriteBatch.Draw(texture, new Rectangle(right.X - (int)camera.Position.X, right.Y - (int)camera.Position.Y,
                    right.Width, right.Height), Color.Green);
                spriteBatch.Draw(texture, new Rectangle(up.X - (int)camera.Position.X, up.Y - (int)camera.Position.Y,
                    up.Width, up.Height), Color.Green);
                spriteBatch.Draw(texture, new Rectangle(down.X - (int)camera.Position.X, down.Y - (int)camera.Position.Y,
                    down.Width, down.Height), Color.Green);
            }
        }

        public string CheckCollisionDir(Rectangle collider, float velocityX)
        {
            string colDir = "";

            if (collider.Right > left.Left || collider.Left < right.Right)
            {
                if (collider.Intersects(left))
                    colDir = "right";
                else if (collider.Intersects(right))
                    colDir = "left";
            }


            if (collider.Bottom > up.Top || collider.Top < down.Bottom)
            {
                if (collider.Intersects(up))
                    colDir = "down";
                else if (collider.Intersects(down))
                    colDir = "up";
            }

            return colDir;
        }

        public void CheckCollisionDir(Rectangle collider, 
            bool leftColX, bool rightColX, bool upColY, bool downColY,
            out bool leftCol, out bool rightCol, out bool upCol, out bool downCol)
        {
            leftCol = leftColX;
            rightCol = rightColX;
            upCol = upColY;
            downCol = downColY;

            if (collider.Right > left.Left || collider.Left < right.Right)
            {
                if (collider.Intersects(left))
                {
                    rightCol = true;
                }
                if (collider.Intersects(right))
                {
                    leftCol = true;
                }
            }


            if (collider.Bottom > up.Top || collider.Top < down.Bottom)
            {
                if (collider.Intersects(up))
                {
                    downCol = true;
                }
                if (collider.Intersects(down))
                {
                    upCol = true;
                }
            }
        }
    }
}
