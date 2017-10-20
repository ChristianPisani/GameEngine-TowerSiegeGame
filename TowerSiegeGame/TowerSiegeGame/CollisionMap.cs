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
    public class CollisionMap
    {
        public List<Row> blocks = new List<Row>();
        
        public int TileSize = 100;
        public int width = 200;
        public int height = 200;
        
        public CollisionMap(int width, int height, List<Block> colObjects)
        {
            this.width = width;
            this.height = height;

            blocks.Add(new Row());
            blocks.Add(new Row());
            blocks.Add(new Row());

            for (int x = 0; x < width * TileSize; x += TileSize)
            {
                blocks[0].rows.Add(new Column());
                blocks[1].rows.Add(new Column());
                blocks[2].rows.Add(new Column());

                for (int y = 0; y < height * TileSize; y += TileSize)
                { 
                    blocks[0].rows[x / TileSize].columns.Add(new ColObject());
                    blocks[1].rows[x / TileSize].columns.Add(new ColObject());
                    blocks[2].rows[x / TileSize].columns.Add(new ColObject());
                }
            }

            Update(colObjects, true);
        }

        public void Update(List<Block> colObjects, bool ClearList)
        {
            int x = 0;
            int y = 0;

            if (ClearList == true)
            {
                for (int a = 0; a < width - 1; a++)
                {
                    for (int b = 0; b < height - 1; b++)
                    {
                        blocks[0].rows[a].columns[b].colObjects.Clear();
                        blocks[1].rows[a].columns[b].colObjects.Clear();
                        blocks[2].rows[a].columns[b].colObjects.Clear();
                    }
                }
                //blocks[0].rows.Clear();
            }

            for (int i = 0; i < colObjects.Count; i++)
            {
                x = (colObjects[i].bounds.X / TileSize);
                y = (colObjects[i].bounds.Y / TileSize);

                for (int addX = 0; addX < 1 + (colObjects[i].bounds.Right / TileSize) -
                                        (x); addX++)
                {
                    for (int addY = 0; addY < 1 + (colObjects[i].bounds.Bottom / TileSize) -
                                            (y); addY++)
                    {
                        if (colObjects[i].Background == false && colObjects[i].Shatterable == false)
                        {
                            blocks[0].rows[(int)MathHelper.Clamp(x + addX, 0, width - 1)].columns[
                                (int)MathHelper.Clamp(y + addY, 0, height - 1)].colObjects.Add(colObjects[i]);
                        }
                        if (colObjects[i].Shatterable == true)
                        {
                            blocks[2].rows[(int)MathHelper.Clamp(x + addX, 0, width - 1)].columns[
                                (int)MathHelper.Clamp(y + addY, 0, height - 1)].colObjects.Add(colObjects[i]);
                        }
                        else if(colObjects[i].Background == true)
                        {
                            blocks[1].rows[(int)MathHelper.Clamp(x + addX, 0, width - 1)].columns[
                                (int)MathHelper.Clamp(y + addY, 0, height - 1)].colObjects.Add(colObjects[i]);
                        }
                    }
                }
            }
        }
    }

    public class Row
    {
        public List<Column> rows = new List<Column>();
    }

    public class Column
    {
        public List<ColObject> columns = new List<ColObject>();
    }

    public class ColObject
    {
        public List<Block> colObjects = new List<Block>();
    }
}
