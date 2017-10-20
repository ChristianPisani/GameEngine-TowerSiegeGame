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
    class Shapes
    {
        public static List<Rectangle> line(int x0, int y0, int x1, int y1, int TileSize, int Length, Point size, Point offset)
        {
            List<Rectangle> pointlist = new List<Rectangle>();

            int x0b = x0;
            int y0b = y0;
            int x1b = x1;
            int y1b = y1;

            int dx = Math.Abs((x1 / TileSize) - (x0 / TileSize));
            int dy = Math.Abs((y1 / TileSize) - (y0 / TileSize));

            int sx;
            int sy;

            if (x0 < x1)
                sx = TileSize;
            else
                sx = -TileSize;


            if (y0 < y1)
                sy = TileSize;
            else
                sy = -TileSize;

            int err = dx - dy;

            int linelength = 0;

            do
            {
                linelength += 1;

                pointlist.Add(new Rectangle(x0 + offset.X, y0 + offset.Y, size.X, size.Y));

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            } while (((x0 / TileSize) != (x1 / TileSize) || (y0 / TileSize) != (y1 / TileSize)) && (linelength != Length));

            return pointlist;
        }


        public static List<Rectangle> lineOfSight(int x0, int y0, int x1, int y1, int TileSize, int Length, Point size, CollisionMap collisionMap,
            float angleX, float angleY, float FOV, out bool InSight, Rectangle entity, List<ElectronicComponent> electronics)
        {
            List<Rectangle> pointlist = new List<Rectangle>();

            InSight = false;

            int x0b = x0;
            int y0b = y0;
            int x1b = x1;
            int y1b = y1;

            int dx = Math.Abs((x1 / TileSize) - (x0 / TileSize));
            int dy = Math.Abs((y1 / TileSize) - (y0 / TileSize));

            int sx;
            int sy;

            if (x0 < x1)
                sx = TileSize;
            else
                sx = -TileSize;


            if (y0 < y1)
                sy = TileSize;
            else
                sy = -TileSize;

            int err = dx - dy;

            int linelength = 0;

            do
            {
                linelength += 1;

                pointlist.Add(new Rectangle(x0, y0, size.X, size.Y));

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }

                int rowpos = (int)MathHelper.Clamp(pointlist[linelength - 1].X / collisionMap.TileSize, 0, collisionMap.width - 1);
                int columnpos = (int)MathHelper.Clamp(pointlist[linelength - 1].Y / collisionMap.TileSize, 0, collisionMap.height - 1);

                Vector2 angle0 = new Vector2(x0, y0);
                Vector2 angle1 = new Vector2(x0 + angleX, y0 + angleY);
                Vector2 angle2 = new Vector2(x1, y1);

                float angle = MathHelper.ToDegrees((float)(Math.PI - (Math.Atan2(angle2.Y - angle0.Y, angle2.X - angle0.X))));
                float angle4 = MathHelper.ToDegrees((float)(Math.PI - (Math.Atan2(angle1.Y - angle0.Y, angle1.X - angle0.X))));

                float fovtop = (angle4 + (FOV/2.0f)),
                      fovbottom = (angle4 - (FOV/2.0f));

                bool checkingmethod = false;

                if (fovbottom < 0)
                {
                    fovbottom = 360 + fovbottom;
                    checkingmethod = true;
                }

                if (fovtop > 360)
                {
                    fovtop = fovtop - 360;
                    checkingmethod = true;
                }

                if ((angle < fovtop || angle > fovbottom) == false && checkingmethod == true)
                {
                    return pointlist;
                } 

                if ((angle > fovtop || angle < fovbottom) && checkingmethod == false)
                {
                    return pointlist;
                }

                foreach (ElectronicComponent comp in electronics)
                {
                    if (pointlist[linelength - 1].Intersects(comp.bounds) && comp.collidable == true)
                    {
                        return pointlist;
                    }
                }

                foreach (Block block in collisionMap.blocks[0].rows[rowpos].columns[columnpos].colObjects)
                {
                    if(pointlist[linelength - 1].Intersects(block.bounds))
                    {
                        return pointlist;
                    }
                }

                if(entity.Intersects(pointlist[linelength - 1]) == true)
                {
                    InSight = true;
                }
            } while (((x0 / TileSize) != (x1 / TileSize) || (y0 / TileSize) != (y1 / TileSize)) && (linelength != Length));

            return pointlist;
        }

        public static List<Rectangle> lightLevel(int x0, int y0, int x1, int y1, int TileSize, int Length, Point size, CollisionMap collisionMap,
                 float angle, float lightangle, float FOV, out bool InSight, Rectangle entity, List<ElectronicComponent> electronics)
        {
            List<Rectangle> pointlist = new List<Rectangle>();

            InSight = false;

            int x0b = x0;
            int y0b = y0;
            int x1b = x1;
            int y1b = y1;

            int dx = Math.Abs((x1 / TileSize) - (x0 / TileSize));
            int dy = Math.Abs((y1 / TileSize) - (y0 / TileSize));

            int sx;
            int sy;

            if (x0 < x1)
                sx = TileSize;
            else
                sx = -TileSize;


            if (y0 < y1)
                sy = TileSize;
            else
                sy = -TileSize;

            int err = dx - dy;

            int linelength = 0;

            do
            {
                linelength += 1;

                pointlist.Add(new Rectangle(x0, y0, size.X, size.Y));

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }

                int rowpos = (int)MathHelper.Clamp(pointlist[linelength - 1].X / collisionMap.TileSize, 0, collisionMap.width - 1);
                int columnpos = (int)MathHelper.Clamp(pointlist[linelength - 1].Y / collisionMap.TileSize, 0, collisionMap.height - 1);

                float fovtop = (lightangle + (FOV / 2.0f)),
                      fovbottom = (lightangle - (FOV / 2.0f));

                bool checkingmethod = false;

                if (fovbottom < 0)
                {
                    fovbottom = 360 + fovbottom;
                    checkingmethod = true;
                }

                if (fovtop > 360)
                {
                    fovtop = fovtop - 360;
                    checkingmethod = true;
                }

                if ((angle < fovtop || angle > fovbottom) == false && checkingmethod == true)
                {
                    return pointlist;
                }

                if ((angle > fovtop || angle < fovbottom) && checkingmethod == false)
                {
                    return pointlist;
                }

                foreach (ElectronicComponent comp in electronics)
                {
                    if (pointlist[linelength - 1].Intersects(comp.bounds) && comp.collidable == true)
                    {
                        return pointlist;
                    }
                }

                for (int i = 0; i < collisionMap.blocks[0].rows[rowpos].columns[columnpos].colObjects.Count; i++)
                {
                    if (pointlist[linelength - 1].Intersects(collisionMap.blocks[0].rows[rowpos].columns[columnpos].colObjects[i].bounds))
                    {
                        return pointlist;
                    }
                }

                if (entity.Intersects(pointlist[linelength - 1]) == true)
                {
                    InSight = true;
                }
            } while (((x0 / TileSize) != (x1 / TileSize) || (y0 / TileSize) != (y1 / TileSize)) && (linelength != Length));

            return pointlist;
        }
    }
}
