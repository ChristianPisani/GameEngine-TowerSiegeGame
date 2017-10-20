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
    public class Camera
    {
        public Vector2 Position,
                       LastPosition,
                       DrawPosition,
                       Velocity;
        public Rectangle Size;
        public BoundingRect boundingSize;
        KeyboardState ks;

        float speed = 40,
              originalSpeed = 40;
        public float Zoom = 1,
                     Rotation = 0.00f;

        Keys up = Keys.Up,
            down = Keys.Down,
            left = Keys.Left,
            right = Keys.Right;

        public int ts,

            windowTsX,
            windowTsY,
            xStart,
            yStart;

        public Camera(Rectangle windowSize, CollisionMap collisionMap)
        {
            ts = collisionMap.TileSize;

            windowTsX = (int)(((windowSize.Width) / ts));
            windowTsY = (int)(((windowSize.Height) / ts));
            xStart = (int)((((Position.X) + ((windowSize.Width / 2))) / ts));
            yStart = (int)((((Position.Y) + ((windowSize.Height / 2))) / ts));
        }

        public void Update(CollisionMap collisionMap, Rectangle window)
        {
            LastPosition = Position;

            ks = Keyboard.GetState();

            speed = originalSpeed / Zoom;

            if (ks.IsKeyDown(Keys.U))
            {
                Zoom += (0.03f * Zoom);
            }

            if (ks.IsKeyDown(Keys.I))
            {
                Zoom -= (0.03f * Zoom);
            }

            if (ks.IsKeyDown(Keys.K))
            {
                //Rotation += (0.03f);
            }

            if (ks.IsKeyDown(Keys.J))
            {
                //Rotation -= (0.03f);
            }

            Zoom = MathHelper.Clamp(Zoom, 0.2f, 20);

            if(ks.IsKeyDown(up))
            {
                Position.Y -= speed;
            }

            if (ks.IsKeyDown(down))
            {
                Position.Y += speed;
            }

            if (ks.IsKeyDown(left))
            {
                Position.X -= speed;
            }

            if (ks.IsKeyDown(right))
            {
                Position.X += speed;
            }

            Size.Width = (int)(window.Width / Zoom) + ts;
            Size.Height = (int)(window.Height / Zoom) + ts;
            Size.X = (int)((((Position.X) - (Size.Width) / 2 + (window.Width))));
            Size.Y = (int)((((Position.Y) - (Size.Height) / 2 + (window.Height))));
            boundingSize = new BoundingRect(Size.X, Size.Y, Size.Width, Size.Height);

            Position.X = (int)MathHelper.Clamp(Position.X, -((window.Width) / 2 / -Zoom) - window.Width, (-((window.Width) / 2 / -Zoom) - window.Width) + ((((collisionMap.width - 1) - (window.Width / (ts * Zoom))) * ts)));
            Position.Y = (int)MathHelper.Clamp(Position.Y, -((window.Height) / 2 / -Zoom) - window.Height, (-((window.Height) / 2 / -Zoom) - window.Height) + ((((collisionMap.height - 1) - (window.Height / (ts * Zoom))) * ts)));

            Velocity = Position - LastPosition;

            ts = collisionMap.TileSize;

            windowTsX = (int)((((window.Width) / (ts * Zoom)) + 4));
            windowTsY = (int)((((window.Height) / (ts * Zoom)) + 4));
            xStart = (int)((((Position.X) - (windowTsX * (ts)) / 2 + (window.Width)) / (ts)));
            yStart = (int)((((Position.Y) - (windowTsY * (ts)) / 2 + (window.Height)) / (ts)));

            windowTsX = (int)MathHelper.Clamp(windowTsX, 0, collisionMap.width);
            windowTsY = (int)MathHelper.Clamp(windowTsY, 0, collisionMap.height);
            xStart = (int)MathHelper.Clamp(xStart, 0, collisionMap.width - (window.Width / (ts * Zoom)) - 4);
            yStart = (int)MathHelper.Clamp(yStart, 0, collisionMap.height - (window.Height / (ts * Zoom)) - 4);
        }

        public void CenterCamera(Vector2 pos, Rectangle window)
        {
            Position = Vector2.Lerp(Position, pos - new Vector2(window.Width, window.Height), 0.2f);
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice, Rectangle ViewPort)
        {
            Matrix _transform = 
              Matrix.CreateTranslation(new Vector3(-ViewPort.Width, -ViewPort.Height, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ViewPort.Width * 0.5f, ViewPort.Height * 0.5f, 0));
            return _transform;
        }
    }
}
