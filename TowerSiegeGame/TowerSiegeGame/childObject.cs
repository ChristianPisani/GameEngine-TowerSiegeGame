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

namespace Entity
{
    class childObject
    {
        public Rectangle bounds, drawBounds, mouseRect;
        public float rotation;
        public Texture2D texture;
        public Color color = Color.Black;
        public Vector2 distanceToParent;

        public Rectangle left,
                         right,
                         up,
                         down;

        bool rotate = false, Rotating = false;
        float rotationSpeed = 0;

        bool Moving = false,
             Resizing = false;

        string ResizeDir = "";

        public childObject(Rectangle bounds, float rotation, Vector2 distanceToParent, Texture2D texture)
        {
            this.bounds = bounds;
            this.rotation = rotation;
            this.distanceToParent = distanceToParent;
            this.texture = texture;
        }

        public void Update(MouseState ms, Rectangle Base)
        {
            color = Color.Black;
            mouseRect = new Rectangle(ms.X, ms.Y, 1, 1);

            if (ms.LeftButton == ButtonState.Pressed && Moving == false && Resizing == false)
            {
                if (mouseRect.Intersects(right))
                {
                    ResizeDir = "right";
                    Resizing = true;
                }

                if (mouseRect.Intersects(left))
                {
                    ResizeDir = "left";
                    Resizing = true;
                }

                if (mouseRect.Intersects(up))
                {
                    ResizeDir = "up";
                    Resizing = true;
                }

                if (mouseRect.Intersects(down))
                {
                    ResizeDir = "down";
                    Resizing = true;
                }
            }

            if (Resizing == false && (mouseRect.Intersects(new Rectangle(drawBounds.X, drawBounds.Y, bounds.Width, bounds.Height)) || Moving == true))
            {
                color = Color.Green;
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    bounds.X = ms.X;
                    bounds.Y = ms.Y;
                    distanceToParent = new Vector2((ms.X - Base.X), (ms.Y - Base.Y));
                    Moving = true;
                }
            }

            if (ms.LeftButton == ButtonState.Released)
            {
                Moving = false;
                Resizing = false;
            }

            if (ms.MiddleButton == ButtonState.Released)
            {
                Rotating = false;
            }

            drawBounds.X = bounds.X - bounds.Width / 2;
            drawBounds.Y = bounds.Y - bounds.Height / 2;
            drawBounds.Height = bounds.Height;
            drawBounds.Width = bounds.Width;

            left = new Rectangle(drawBounds.X, drawBounds.Y + 1, 2, drawBounds.Height - 2);
            right = new Rectangle(drawBounds.Right - 2, drawBounds.Y + 1, 2, drawBounds.Height - 2);
            up = new Rectangle(drawBounds.X + 1, drawBounds.Y, drawBounds.Width - 2, 2);
            down = new Rectangle(drawBounds.X + 1, drawBounds.Bottom - 2, drawBounds.Width - 2, 2);

            if (Resizing == true)
            {
                switch (ResizeDir.ToLower())
                { 
                    case "right":
                        bounds.Width = (ms.X + drawBounds.Width / 2) - drawBounds.Center.X;

                        if (bounds.Width < 0)
                            SwitchResizeDir();
                        break;

                    case "left":
                        bounds.Width = (ms.X - drawBounds.Width / 2) - drawBounds.Center.X;

                        if (bounds.Width > 0)
                            SwitchResizeDir();
                        break;

                    case "up":
                        bounds.Height = (ms.Y + drawBounds.Height / 2) - drawBounds.Center.Y;

                        if (bounds.Height < 0)
                            SwitchResizeDir();
                        break;

                    case "down":
                        bounds.Height = (ms.Y - drawBounds.Height / 2) - drawBounds.Center.Y;

                        if (bounds.Height > 0)
                            SwitchResizeDir();
                        break;
                }
            }

            bounds.Width = (int)Math.Sqrt(bounds.Width * bounds.Width);
            bounds.Height = (int)Math.Sqrt(bounds.Height * bounds.Height);

            if (ms.MiddleButton == ButtonState.Pressed && mouseRect.Intersects(drawBounds))
            {
                Rotating = true;
            }

            if (Rotating == true)
            {
                rotation = (float)Math.Atan2(ms.Y - bounds.Center.Y, ms.X - bounds.Center.X);
            }
        }

        private void SwitchResizeDir()
        {
            switch (ResizeDir.ToLower())
            {
                case "right":
                    ResizeDir = "left";
                    break;

                case "left":
                    ResizeDir = "right";
                    break;

                case "up":
                    ResizeDir = "down";
                    break;

                case "down":
                    ResizeDir = "up";
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, float baseRotation)
        {
            if (rotate == true)
            {
                rotation += rotationSpeed;
            }

            spriteBatch.Draw(texture, bounds, texture.Bounds, color, rotation + baseRotation, new Vector2(texture.Bounds.Width / 2.0f, texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);

            spriteBatch.Draw(texture, new Rectangle(left.X, left.Y,
                    left.Width, left.Height), Color.Green);
            spriteBatch.Draw(texture, new Rectangle(right.X, right.Y,
                right.Width, right.Height), Color.Green);
            spriteBatch.Draw(texture, new Rectangle(up.X, up.Y,
                up.Width, up.Height), Color.Green);
            spriteBatch.Draw(texture, new Rectangle(down.X, down.Y,
                down.Width, down.Height), Color.Green);

            if (mouseRect.Intersects(left) || mouseRect.Intersects(right) || mouseRect.Intersects(up) || mouseRect.Intersects(down) || Resizing == true)
            {
                spriteBatch.Draw(texture, new Rectangle(mouseRect.X - 8, mouseRect.Y - 4, 16, 8), Color.Red);
            }
        }
    }
}
