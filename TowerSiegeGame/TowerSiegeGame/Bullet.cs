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
    class Bullet : Entity
    {
        public Bullet(Rectangle bounds, float bounciness, bool alwaysUpdate, bool checkNPCs, bool checkPlayer, KryptonEngine krypton)
            : base(bounds, bounciness, alwaysUpdate, checkNPCs, checkPlayer, krypton)
        {
            destroyOnTouch = true;
        }

        public override void Update(CollisionMap collisionMap, Camera camera, bool getColDir, List<NPC> NPCs, Player player, List<Entity> entities, bool collideInEntities, Rectangle window,
            List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            bounds.X = (int)MathHelper.Clamp(bounds.X, 0, (collisionMap.width * collisionMap.TileSize) - bounds.Width);
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, 0, (collisionMap.height * collisionMap.TileSize) - bounds.Height);

            Rectangle raypos = Rectangle.Empty;

            if (downCol == false)
            {
                velocity.Y += gravity;
            }

            if (downCol == true || upCol == true)
            {
                if (velocity.X > 0.0f)
                {
                    velocity.X = MathHelper.Clamp(velocity.X - friction, 0, 2000);
                }
                else
                {
                    velocity.X = MathHelper.Clamp(velocity.X + friction, -2000, 0);
                }


                velocity.Y = -velocity.Y * bounciness;
            }

            if (leftCol == true || rightCol == true)
            {
                velocity.X = -velocity.X * bounciness;
            }

            int x2 = (int)MathHelper.Clamp(bounds.X + velocity.X, 0, (collisionMap.width * collisionMap.TileSize) - bounds.Width);
            int y2 = (int)MathHelper.Clamp(bounds.Y + velocity.Y, 0, (collisionMap.height * collisionMap.TileSize) - bounds.Height);

            ray = Shapes.line(bounds.X, bounds.Y, x2, y2, 1, linelength, new Point(bounds.Width, bounds.Height), new Point(0, 0));

            bool collision = false;
            collision = CollisionDetection(collisionMap, ray, out raypos, NPCs, player, CheckPlayer, CheckNPCs);

            if (collision == true || velocity.Y != 0)
            {
                bounds.X = (int)MathHelper.Clamp(raypos.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width);
                bounds.Y = (int)MathHelper.Clamp(raypos.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height);
            }
            else
            {
                bounds.X = (int)MathHelper.Clamp(bounds.X + velocity.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width);
                bounds.Y = (int)MathHelper.Clamp(bounds.Y + velocity.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height);
            }

            if ((velocity.Y > -(4.0f + bounciness) && velocity.Y < bounciness + 4.0f && downCol == true) || colYDown == true)
            {
                velocity.Y = 0;
            }

            if ((velocity.X > -0.00f && velocity.X < 0.00f && (downCol == true || upCol == true)))
            {
                velocity.X = 0;
            }
        }

        public bool CollisionDetection(CollisionMap collisionMap, List<Rectangle> ray, out Rectangle raypos,
                                                List<NPC> NPCs, Player player, bool CheckForPlayer, bool CheckForNPCs)
        {
            int ts = collisionMap.TileSize; //TileSize
            raypos = new Rectangle();

            colObjects.Clear();

            for (int x = colCheckXStart; x < colCheckXEnd + (bounds.Right / ts) - (bounds.X / ts); x++)
            {
                for (int y = colCheckYStart; y < colCheckYEnd + (bounds.Bottom / ts) - (bounds.Y / ts); y++)
                {
                    int xPos = (int)MathHelper.Clamp((bounds.X / ts) + x, 0, (collisionMap.width - 1));
                    int yPos = (int)MathHelper.Clamp((bounds.Y / ts) + y, 0, (collisionMap.height - 1));

                    if (collisionMap.blocks[0].rows[xPos].columns[yPos].colObjects.Count > 0)
                    {
                        colObjects.AddRange(collisionMap.blocks[0].rows[
                            xPos].columns[
                            yPos].colObjects);
                    }

                    if (collisionMap.blocks[2].rows[xPos].columns[yPos].colObjects.Count > 0)
                    {
                        colObjects.AddRange(collisionMap.blocks[2].rows[
                            xPos].columns[
                            yPos].colObjects);
                    }
                }
            }

            if (CheckForNPCs == true)
            {
                foreach (NPC npc in NPCs)
                {
                    Block newblock = new Block(npc.bounds, Color.AliceBlue);
                    colObjects.Add(newblock);
                }
            }

            if (CheckForPlayer == true)
            {
                Block newblock = new Block(player.bounds, Color.AliceBlue);
                colObjects.Add(newblock);
            }

            if (colObjects.Count > 0)
            {
                for (int l = 0; l < ray.Count; l++)
                {
                    for (int i = 0; i < colObjects.Count; i++)
                    {
                        if (ray[l].Intersects(colObjects[i].bounds) && colObjects.Count > 0)
                        {
                            raypos.X = colObjects[i].bounds.X;
                            raypos.Y = colObjects[i].bounds.Y;

                            if (colObjects[i].Shatterable == true)
                            {
                                colObjects[i].Existing = false;
                                colObjects[i].Shattered = true;
                                colObjects[i].shardVelocity.X = velocity.X * 0.5f;
                                colObjects[i].shardVelocity.Y = velocity.Y * 0.5f;

                                leftCol = false;
                                rightCol = false;
                                upCol = false;
                                downCol = false;

                                ResetColDir();

                                ShatterPos = bounds;
                            }
                            else
                            {
                                if (destroyOnTouch == true)
                                {
                                    DeleteNextFrame = true;
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            raypos = ray[ray.Count - 1];

            return false;
        }
    }
}*/
