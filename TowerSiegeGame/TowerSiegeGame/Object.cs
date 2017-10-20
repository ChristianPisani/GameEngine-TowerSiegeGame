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
    public class Object
    {
        public Rectangle bounds,
                         edgeCheck;
        public Vector2 velocity;
        public float gravity = 0.5f,
                     friction = 0.7f,
                     weight = 1;

        public float bounciness = 0.7f;
        public float lifeTime = -1;

        public int removalStop = 1,
                   linelength = 60,
                   bouncesBeforeDeletion = 0,
                   bounces = 0;

        public List<Rectangle> ray = new List<Rectangle>(),
                               trail = new List<Rectangle>();

        public int trailLength = 0;

        public bool alwaysUpdate = false,
                    destroyOnTouch = false,
                    Existing = true,
                    DeleteNextFrame = false,
                    DoUpdate = true,
                    physicsEnabled = true,
                    collideInEntities = false;

        public bool colliding =  false,
             leftCol =    false,
             rightCol =   false,
             upCol =      false,
             downCol =    false,
             atEdge  =    false,
             collidable = false,
             castShadows = false,
             deadly = false,
             collisiondetection = true;

        public bool colXLeft = false,
                    colXRight = false,
                    colYUp = false,
                    colYDown = false;

        public int colCheckXStart = -1,
                   colCheckXEnd = 2,
                   colCheckYStart = -1,
                   colCheckYEnd = 2;

        public Color color = Color.Yellow;

        public List<Block> colObjects = new List<Block>();

        public Rectangle colDirRect = new Rectangle(),
            
                         ShatterPos = new Rectangle();

        public bool CheckPlayer = false,
                    CheckNPCs = false;

        public Texture2D texture;

        public ShadowHull hull = ShadowHull.CreateRectangle(Vector2.One);

        public Object()
        { 
        
        }

        public Object(Rectangle bounds, float bounciness, bool alwaysUpdate, bool checkNPCs, bool checkPlayer, KryptonEngine krypton, bool castShadows)
        {
            this.bounds = bounds;
            this.bounciness = bounciness;
            this.alwaysUpdate = alwaysUpdate;
            this.CheckPlayer = checkPlayer;
            this.CheckNPCs = checkNPCs;
            this.castShadows = castShadows;

            if (castShadows == true)
            {
                hull.Position.X = bounds.Center.X;
                hull.Position.Y = bounds.Center.Y;
                hull.Scale.X = bounds.Width;
                hull.Scale.Y = bounds.Height;
                krypton.Hulls.Add(hull);

                if (castShadows == true)
                {
                    hull.Visible = true;
                }
                else
                {
                    hull.Visible = false;
                }
            }
        }

        public virtual void Update(CollisionMap collisionMap, Camera camera, bool getColDir, List<NPC> NPCs, Player player, List<Object> entities, Rectangle window,
            List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            if (Existing == false)
                return;

            Rectangle raypos = Rectangle.Empty;

            if (trailLength > 0)
            {
                trail.Add(new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height));
            }

            if (trail.Count > trailLength)
            {
                trail.RemoveAt(0);
            }

            bounds.X = (int)MathHelper.Clamp(bounds.X, 0, ((collisionMap.width - 2) * collisionMap.TileSize) - bounds.Width - 1);
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height - 1);

            if (physicsEnabled == true)
            {
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

                Rectangle Intersection,
                          blockSize;

                    int x2 = (int)MathHelper.Clamp(bounds.X + velocity.X, 0, (collisionMap.width * collisionMap.TileSize) - bounds.Width);
                    int y2 = (int)MathHelper.Clamp(bounds.Y + velocity.Y, 0, (collisionMap.height * collisionMap.TileSize) - bounds.Height);

                    ray = Shapes.line(bounds.X, bounds.Y, x2, y2, 1, linelength, new Point(bounds.Width, bounds.Height), new Point(0, 0));

                    bool collision = false;
                    if (collisiondetection == true)
                    {
                        collision = CollisionDetection(collisionMap, ray, out raypos, out Intersection, out blockSize, getColDir, CheckNPCs, NPCs, CheckPlayer, player,
                            entities, window, electronics, krypton);
                    

                    if (bounds.Y + bounds.Height >= (collisionMap.height - 3) * collisionMap.TileSize)
                    {
                        colYDown = true;
                        downCol = true;
                        collision = true;

                        if (destroyOnTouch == true)
                            Existing = false;
                    }

                    bounds.Y = (int)MathHelper.Clamp(bounds.Y, -200, (collisionMap.height - 3) * collisionMap.TileSize);

                    if (collision == true || velocity.Y != 0)
                    {

                            bounds.X = (int)MathHelper.Clamp(raypos.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width);
                            bounds.Y = (int)MathHelper.Clamp(raypos.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height);

                            if ((bounds.Contains(Intersection)))
                            {
                                if (blockSize.Width > blockSize.Height)
                                {
                                    if (bounds.Center.Y >= Intersection.Center.Y)
                                    {
                                        bounds.Y += Intersection.Height;
                                    }
                                    else
                                    {
                                        bounds.Y -= Intersection.Height;
                                    }
                                }
                                else
                                {
                                    if (bounds.Center.X >= Intersection.Center.X)
                                    {
                                        bounds.X += Intersection.Width;
                                    }
                                    else
                                    {
                                        bounds.X -= Intersection.Width;
                                    }
                                }
                            }
                            else
                            {
                                if (downCol == true && Intersection.Height > 1)
                                {
                                    bounds.Y -= Intersection.Height;
                                }

                                if (upCol == true && Intersection.Height > removalStop)
                                {
                                    bounds.Y += Intersection.Height;
                                }

                                if (leftCol == true && Intersection.Width > removalStop)
                                {
                                    bounds.X += Intersection.Width;
                                }

                                if (rightCol == true && Intersection.Width > removalStop)
                                {
                                    bounds.X -= Intersection.Width;
                                }
                            }
                        }
                    }
                    else
                    {
                        bounds.X = (int)MathHelper.Clamp(bounds.X + velocity.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width);
                        bounds.Y = (int)MathHelper.Clamp(bounds.Y + velocity.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height);
                    }
                }

                if ((velocity.Y > -(4.0f + bounciness) && velocity.Y < bounciness + 4.0f && downCol == true) || colYDown == true)
                {
                    velocity.Y = 0;
                }

                if ((velocity.X > -0.00f && velocity.X < 0.00f && (downCol == true || upCol == true)))
                {
                    velocity.X = 0;
                }

            if (lifeTime > 0)
            {
                lifeTime -= 1;
            }

            if (lifeTime == 0)
                Existing = false;

            bounds.X = (int)MathHelper.Clamp(bounds.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width - 1);
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, 0, ((collisionMap.height- 1) * collisionMap.TileSize) - bounds.Height - 1);

            hull.Visible = false;
            if (castShadows == true)
            {
                hull.Visible = true;
            }

            hull.Position.X = bounds.Center.X;
            hull.Position.Y = bounds.Center.Y;
            hull.Scale.X = bounds.Width;
            hull.Scale.Y = bounds.Height;
        }

        public void ResetColDir()
        {
            colXRight = false;
            colXLeft = false;
            colYUp = false;
            colYDown = false;
        }

        public bool CollisionDetection(CollisionMap collisionMap, List<Rectangle> ray, out Rectangle raypos, out Rectangle intersection, out Rectangle blockSize, bool getColDir,
            bool CheckForNPCs, List<NPC> NPCs, bool CheckForPlayer, Player player, List<Object> entities, Rectangle window,
            List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            int ts = collisionMap.TileSize; //TileSize

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

            if (collideInEntities == true)
            {
                foreach (Object entity in entities.Concat(electronics))
                {
                    if (entity.bounds != bounds && entity.collidable == true)
                    {
                        Block newblock = new Block(entity.bounds, Color.AliceBlue);
                        colObjects.Add(newblock);
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

            atEdge = true;
            if (colObjects.Count > 0)
            {
                colDirRect.X = bounds.X - 1;
                colDirRect.Y = bounds.Y - 1;
                colDirRect.Width = bounds.Width + 2;
                colDirRect.Height = bounds.Height + 2;

                for (int l = 0; l < ray.Count; l++)
                {
                        for (int i = 0; i < colObjects.Count; i++)
                        {
                            if (colObjects[i].Shatterable == false || !(((velocity.X > 6 || velocity.X < -6) || (velocity.Y > 4 || velocity.Y < -4)) && weight > 0))
                            {
                                switch (colObjects[i].CheckCollisionDir(ray[l], velocity.X))
                                {
                                    case "left":
                                        leftCol = true;
                                        break;

                                    case "right":
                                        rightCol = true;
                                        break;

                                    case "up":
                                        upCol = true;
                                        break;

                                    case "down":
                                        downCol = true;
                                        break;

                                    default:
                                        leftCol = false;
                                        rightCol = false;
                                        upCol = false;
                                        downCol = false;
                                        //colDir = "none";
                                        break;
                                }

                                if (getColDir == true)
                                {
                                    colObjects[i].CheckCollisionDir(
                                        colDirRect,
                                        colXLeft, colXRight, colYUp, colYDown,
                                        out colXLeft, out colXRight, out colYUp, out colYDown);
                                }
                            }

                        if (ray[l].Intersects(colObjects[i].bounds) && colObjects.Count > 0)
                        {
                            if (l > 0)
                            {
                                raypos = ray[l - 1];
                            }
                            else
                            {
                                raypos = ray[0];
                            }

                            if (colObjects[i].Shatterable == true)
                            {
                                if (((velocity.X > 6 || velocity.X < -6) || (velocity.Y > 4 || velocity.Y < -4)) && weight > 0)
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
                                    intersection = Rectangle.Intersect(ray[l], colObjects[i].bounds);
                                    blockSize = colObjects[i].bounds;

                                    return true;
                                }
                            }
                            else
                            {
                                if (destroyOnTouch == true)
                                {
                                    if (bounces != bouncesBeforeDeletion)
                                    {
                                        bounces += 1;
                                    }
                                    else
                                    {
                                        DeleteNextFrame = true;
                                    }
                                }

                                intersection = Rectangle.Intersect(ray[l], colObjects[i].bounds);
                                blockSize = colObjects[i].bounds;

                                return true;
                            }
                        }

                        if (edgeCheck.Intersects(colObjects[i].bounds))
                        {
                            atEdge = false;
                        }
                    }
                }
            }

            raypos = ray[ray.Count - 1];
            intersection = Rectangle.Empty;
            blockSize = Rectangle.Empty;

            return false;
        }

        public bool BasicCollisionDetection(Rectangle colBounds, CollisionMap collisionMap)
        { 
            int ts = collisionMap.TileSize;

            for (int x = 0; x < 1 + (bounds.Right / ts) - (bounds.X / ts); x++)
            {
                for (int y = 0; y < 1 + (bounds.Bottom / ts) - (bounds.Y / ts); y++)
                {
                    int xPos = (int)MathHelper.Clamp((bounds.X / ts) + x, 0, collisionMap.width - 1);
                    int yPos = (int)MathHelper.Clamp((bounds.Y / ts) + y, 0, collisionMap.height - 1);

                    if (collisionMap.blocks[0].rows[xPos].columns[yPos].colObjects.Count > 0)
                    {
                        colObjects.AddRange(collisionMap.blocks[0].rows[
                            xPos].columns[
                            yPos].colObjects);
                    }
                }
            }

            for (int i = 0; i < colObjects.Count; i++)
            {
                if (colBounds.Intersects(colObjects[i].bounds))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
