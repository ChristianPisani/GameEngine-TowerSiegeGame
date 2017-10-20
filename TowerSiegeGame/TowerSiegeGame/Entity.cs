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

/*namespace Entity
{
    class Entity
    {
        public Rectangle Base;
        public List<Joint> Joints = new List<Joint>();
        public List<childObject> children = new List<childObject>();

        public Rectangle left,
                         right,
                         up,
                         down;

        public float Rotation;

        public Entity(Rectangle Base)
        {
            this.Base = Base;
        }

        public void Update(MouseState ms)
        {
            for (int i = 0; i < Joints.Count; i++)
            {
                Vector2 newPos = GetPositionOfSatellite(
                    new Vector2(Base.X, Base.Y), 
                    new Vector2(children[i].distanceToParent.X, 
                                children[i].distanceToParent.Y).Length(), 
                                Vector2ToRadian(new Vector2(children[i].distanceToParent.X, 
                                                            children[i].distanceToParent.Y)) + Rotation); 

                Joints[i].Position = new Vector2(newPos.X, newPos.Y);
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].bounds = new Rectangle((int)Joints[i].Position.X, (int)Joints[i].Position.Y, children[i].bounds.Width, children[i].bounds.Height);
                children[i].Update(ms, Base);
            }

            //Rotation += 0.01f;
        }

        public float Vector2ToRadian(Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public Vector2 GetPositionOfSatellite(Vector2 center, float distance, float directionInRadians)
        {

            float yDifference = (float)Math.Sin(directionInRadians);

            float xDifference = (float)Math.Cos(directionInRadians);

            Vector2 direction = new Vector2(xDifference, yDifference);

            Vector2 precisePositionOfSatellite = center + direction * distance;

            return precisePositionOfSatellite;

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Base, texture.Bounds, Color.Black, Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);

            foreach (childObject kid in children)
            {
                kid.Draw(spriteBatch, Rotation);
                //spriteBatch.Draw(texture, kid.bounds, texture.Bounds, Color.Black, Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            }

            foreach (Joint joint in Joints)
            {
                spriteBatch.Draw(texture, new Rectangle((int)joint.Position.X, (int)joint.Position.Y, 3, 3), texture.Bounds, Color.Red, Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
            }
        }
    }
}
*/