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
using System.IO;

namespace TowerSiegeGame
{
    public class BlockHandling
    {
        StreamWriter streamwriter;
        StreamReader streamreader;

        public string ComputerName = System.Environment.UserName;

        public void SaveBlock(Block block, string name)
        {
            if (File.Exists(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Blocks\" + name + ".block") == false)
            {
                Directory.CreateDirectory(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Blocks");
            }

            streamwriter = new StreamWriter(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Blocks\" + name + ".block");

            streamwriter.Write(name + "," + System.Environment.NewLine + System.Environment.NewLine);
            streamwriter.Write(block.bounds.Width.ToString() + "," + " //Width " + System.Environment.NewLine);
            streamwriter.Write(block.bounds.Height.ToString() + "," + " //Height " + System.Environment.NewLine);
            streamwriter.Write(block.color.R.ToString() + "," + " //Color.R " + System.Environment.NewLine);
            streamwriter.Write(block.color.G.ToString() + "," + " //Color.G " + System.Environment.NewLine);
            streamwriter.Write(block.color.B.ToString() + "," + " //Color.B " + System.Environment.NewLine);
            streamwriter.Write(block.color.A.ToString() + "," + " //Color.A " + System.Environment.NewLine);
            streamwriter.Write(block.blockID.ToString() + "," + " //BlockID (changing this might lead to unwanted results) " + System.Environment.NewLine);
            streamwriter.Write(block.Background.ToString() + "," + " //ShouldItBeBackground ");
            streamwriter.Write(block.Shatterable.ToString() + "," + " //CanItBreak ");
            streamwriter.Write(block.CastShadows.ToString() + "," + " //CastShadows ");

            streamwriter.Dispose();
        }

        public Block LoadBlockFromFile(string blockname)
        {
            Block block = new Block(Rectangle.Empty, Color.Black);

            if (File.Exists(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Blocks\" + blockname + ".block") == true)
            {
                streamreader = new StreamReader(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Blocks\" + blockname + ".block");

                string text = streamreader.ReadToEnd();
                text = text.Replace(System.Environment.NewLine, "");
                text = text.Replace("//Width", "");
                text = text.Replace("//Height", "");
                text = text.Replace("//Color.R", "");
                text = text.Replace("//Color.G", "");
                text = text.Replace("//Color.B", "");
                text = text.Replace("//Color.A", "");
                text = text.Replace("//BlockID (changing this might lead to unwanted results)", "");
                text = text.Replace("//ShouldItBeBackGround", "");
                text = text.Replace("//CanItBreak", "");
                text = text.Replace("//CastShadows", "");
                text = text.Replace(" ", "");

                List<string> LevelTileList = new List<string>(text.Split(new string[] { "," }, StringSplitOptions.None));

                for (int i = 0; i < LevelTileList.Count / 10; i++)
                {
                    block = new Block(Rectangle.Empty, Color.Black);

                    string s = LevelTileList[i * 10 + 1];
                    string b = s = LevelTileList[i * 10 + 2];
                    string g = LevelTileList[i * 10 + 3];
                    string h = LevelTileList[i * 10 + 4];
                    string c = LevelTileList[i * 10 + 5];
                    string gg = LevelTileList[i * 10 + 6];
                    string v = LevelTileList[i * 10 + 7];
                    string a = LevelTileList[i * 10 + 8];
                    string e = LevelTileList[i * 10 + 9];
                    string l = LevelTileList[i * 10 + 10];

                    block.bounds.Width = Convert.ToInt32(LevelTileList[i * 10 + 1]);
                    block.bounds.Height = Convert.ToInt32(LevelTileList[i * 10 + 2]);
                    block.color.R = Convert.ToByte(LevelTileList[i * 10 + 3]);
                    block.color.G = Convert.ToByte(LevelTileList[i * 10 + 4]);
                    block.color.B = Convert.ToByte(LevelTileList[i * 10 + 5]);
                    block.color.A = Convert.ToByte(LevelTileList[i * 10 + 6]);
                    block.blockID = Convert.ToInt32(LevelTileList[i * 10 + 7]);
                    block.Background = Convert.ToBoolean(LevelTileList[i * 10 + 8]);
                    block.Shatterable = Convert.ToBoolean(LevelTileList[i * 10 + 9]);
                    block.CastShadows = Convert.ToBoolean(LevelTileList[i * 10 + 10]);

                    block.UpdateBlock(block.bounds.X, block.bounds.Y, block.bounds.Width,
                        block.bounds.Height);
                }

                streamreader.Dispose();
            }

            return block;
        }
    }
}
