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
    public class SaveHandling
    {
        StreamWriter streamwriter;
        StreamReader streamreader;

        public Textbox MapName;

        bool LoadTyping = false,
             SaveTyping = false;

        public string ComputerName = System.Environment.UserName;

        public SaveHandling(SpriteFont font1, SpriteBatch spriteBatch, Texture2D texture, Rectangle textBoxBounds)
        {
            MapName = new Textbox(font1, spriteBatch, texture, textBoxBounds);
            MapName.Size = new Point(300, 40);
            MapName.textinput.OneLine = true;
        }

        public void Update(Rectangle windowSize, KeyboardState ks, List<Block> realwalls, 
            out List<Block> walls, out bool Typing, KryptonEngine krypton,
            Texture2D mLightTexture, List<NPC> NPCs, List<ElevatorShaft> elevators, Texture2D texture, List<ElectronicComponent> electronics, List<Switch> switches)
        {
            walls = realwalls;
            Typing = false;

            MapName.actualpos = new Point(windowSize.Width / 2, windowSize.Height / 2);

            if ((ks.IsKeyDown(Keys.LeftControl) == true && ks.IsKeyDown(Keys.S) == true) && LoadTyping == false)
            {
                SaveTyping = true;
                MapName.Text = "";
                MapName.posinstring = 0;
            }

            if ((ks.IsKeyDown(Keys.LeftControl) == true && ks.IsKeyDown(Keys.L) == true) && SaveTyping == false)
            {
                LoadTyping = true;
                MapName.Text = "";
                MapName.posinstring = 0;
            }

            if (ks.IsKeyDown(Keys.Escape) == true)
            {
                SaveTyping = false;
                LoadTyping = false;
                Typing = false;
            }

            if (SaveTyping == true)
            {
                MapName.Title = "Saving";
                MapName.active = true;
                MapName.Update(true);
                Typing = true;

                if (ks.IsKeyDown(Keys.Enter) == true)
                {
                    SaveMap(walls, krypton, NPCs, elevators, electronics, switches, MapName.Text);
                    SaveTyping = false;
                }
            }
            else if (LoadTyping == true)
            {
                MapName.Title = "Loading";
                MapName.active = true;
                MapName.Update(true);
                Typing = true;

                if (ks.IsKeyDown(Keys.Enter) == true)
                {
                    walls = LoadMap(MapName.Text, krypton, mLightTexture, NPCs, elevators, electronics, switches, texture);
                    LoadTyping = false;
                }
            }
            else
            {
                MapName.Update(false);
                MapName.active = false;
            }
        }

        public void SaveMap(List<Block> walls, KryptonEngine krypton,
            List<NPC> NPCs, List<ElevatorShaft> elevators, List<ElectronicComponent> electronics, List<Switch> switches, string name)
        {
            if (File.Exists(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Maps\" + name + ".map") == false)
            {
                Directory.CreateDirectory(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Maps");
            }

            streamwriter = new StreamWriter(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Maps\" + name + ".map");

            foreach (Block wall in walls)
            {
                streamwriter.Write(wall.bounds.X.ToString() + ",");
                streamwriter.Write(wall.bounds.Y.ToString() + ",");
                streamwriter.Write(wall.bounds.Width.ToString() + ",");
                streamwriter.Write(wall.bounds.Height.ToString() + ",");
                streamwriter.Write(wall.color.R.ToString() + ",");
                streamwriter.Write(wall.color.G.ToString() + ",");
                streamwriter.Write(wall.color.B.ToString() + ",");
                streamwriter.Write(wall.color.A.ToString() + ",");
                streamwriter.Write(wall.blockID.ToString() + ",");
                streamwriter.Write(wall.Background.ToString() + ",");
                streamwriter.Write(wall.Shatterable.ToString() + ",");
                streamwriter.Write(wall.CastShadows.ToString() + ",");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Write("//lights" + System.Environment.NewLine);

            foreach (Light2D light in krypton.Lights)
            {
                streamwriter.Write(light.X.ToString() + "|");
                streamwriter.Write(light.Y.ToString() + "|");
                streamwriter.Write(light.Range.ToString() + "|");
                streamwriter.Write(light.Intensity.ToString() + "|");
                streamwriter.Write(light.Color.R.ToString() + "|");
                streamwriter.Write(light.Color.G.ToString() + "|");
                streamwriter.Write(light.Color.B.ToString() + "|");
                streamwriter.Write(light.Fov.ToString() + "|");
                streamwriter.Write(light.FOV.ToString() + "|");
                streamwriter.Write(light.Angle.ToString() + "|");
                streamwriter.Write(light.IsOn.ToString() + "|");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Write("//NPCs" + System.Environment.NewLine);

            foreach (NPC npc in NPCs)
            {
                streamwriter.Write(npc.bounds.X.ToString() + "|");
                streamwriter.Write(npc.bounds.Y.ToString() + "|");
                streamwriter.Write(npc.bounds.Width.ToString() + "|");
                streamwriter.Write(npc.bounds.Height.ToString() + "|");
                streamwriter.Write(npc.FOV.ToString() + "|");
                streamwriter.Write(npc.color.R.ToString() + "|");
                streamwriter.Write(npc.color.G.ToString() + "|");
                streamwriter.Write(npc.color.B.ToString() + "|");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Write("//Elevators" + System.Environment.NewLine);

            foreach (ElevatorShaft elevator in elevators)
            {
                streamwriter.Write(elevator.elevator.bounds.X.ToString() + "|");
                streamwriter.Write(elevator.elevator.bounds.Y.ToString() + "|");
                streamwriter.Write(elevator.elevator.bounds.Width.ToString() + "|");
                streamwriter.Write(elevator.elevator.bounds.Height.ToString() + "|");
                streamwriter.Write(elevator.elevator.currentfloor.ToString() + "|");
                streamwriter.Write(elevator.elevator.speed.ToString() + "|");
                streamwriter.Write(elevator.elevator.color.R.ToString() + "|");
                streamwriter.Write(elevator.elevator.color.G.ToString() + "|");
                streamwriter.Write(elevator.elevator.color.B.ToString() + "|");
                streamwriter.Write(System.Environment.NewLine);
                streamwriter.Write("floors(");

                foreach (ElevatorStop stop in elevator.floors)
                {
                    streamwriter.Write(stop.bounds.X.ToString() + "/");
                    streamwriter.Write(stop.bounds.Y.ToString() + "/");
                }

                streamwriter.Write(")|");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Write("//Electronics" + System.Environment.NewLine);

            foreach (ElectronicComponent component in electronics)
            {
                streamwriter.Write(component.bounciness.ToString() + "|");
                streamwriter.Write(component.bounds.X.ToString() + "|");
                streamwriter.Write(component.bounds.Y.ToString() + "|");
                streamwriter.Write(component.bounds.Width.ToString() + "|");
                streamwriter.Write(component.bounds.Height.ToString() + "|");
                streamwriter.Write(component.velocity.X.ToString() + "|");
                streamwriter.Write(component.velocity.Y.ToString() + "|");
                streamwriter.Write(component.CheckNPCs.ToString() + "|");
                streamwriter.Write(component.CheckPlayer.ToString() + "|");
                streamwriter.Write(component.color.R.ToString() + "|");
                streamwriter.Write(component.color.G.ToString() + "|");
                streamwriter.Write(component.color.B.ToString() + "|");
                streamwriter.Write(component.color.A.ToString() + "|");
                streamwriter.Write(component.destroyOnTouch.ToString() + "|");
                streamwriter.Write(component.friction.ToString() + "|");
                streamwriter.Write(component.gravity.ToString() + "|");
                streamwriter.Write(component.lifeTime.ToString() + "|");
                streamwriter.Write(component.physicsEnabled.ToString() + "|");
                streamwriter.Write(component.trailLength.ToString() + "|");
                streamwriter.Write(component.weight.ToString() + "|");
                streamwriter.Write(component.collidable.ToString() + "|");
                streamwriter.Write(component.castShadows.ToString() + "|");
                streamwriter.Write(component.type + "|");
                streamwriter.Write(component.UpdatePosition.ToString() + "|");
                streamwriter.Write(component.IsOn.ToString() + "|");
                streamwriter.Write(component.ID.ToString() + "|");
                streamwriter.Write(component.Delay.ToString() + "|");
                streamwriter.Write(System.Environment.NewLine);

                streamwriter.Write("ConnectedComponents(");

                foreach (float comp in component.connectedComponents)
                {
                    streamwriter.Write(comp.ToString() + "/");
                }

                streamwriter.Write(")|");
                streamwriter.Write(System.Environment.NewLine);

                streamwriter.Write("OnState(");
                streamwriter.Write(component.OnState.bounciness.ToString() + "|");
                streamwriter.Write(component.OnState.bounds.X.ToString() + "|");
                streamwriter.Write(component.OnState.bounds.Y.ToString() + "|");
                streamwriter.Write(component.OnState.bounds.Width.ToString() + "|");
                streamwriter.Write(component.OnState.bounds.Height.ToString() + "|");
                streamwriter.Write(component.OnState.velocity.X.ToString() + "|");
                streamwriter.Write(component.OnState.velocity.Y.ToString() + "|");
                streamwriter.Write(component.OnState.CheckNPCs.ToString() + "|");
                streamwriter.Write(component.OnState.CheckPlayer.ToString() + "|");
                streamwriter.Write(component.OnState.color.R.ToString() + "|");
                streamwriter.Write(component.OnState.color.G.ToString() + "|");
                streamwriter.Write(component.OnState.color.B.ToString() + "|");
                streamwriter.Write(component.OnState.color.A.ToString() + "|");
                streamwriter.Write(component.OnState.destroyOnTouch.ToString() + "|");
                streamwriter.Write(component.OnState.friction.ToString() + "|");
                streamwriter.Write(component.OnState.gravity.ToString() + "|");
                streamwriter.Write(component.OnState.lifeTime.ToString() + "|");
                streamwriter.Write(component.OnState.physicsEnabled.ToString() + "|");
                streamwriter.Write(component.OnState.trailLength.ToString() + "|");
                streamwriter.Write(component.OnState.weight.ToString() + "|");
                streamwriter.Write(component.OnState.collidable.ToString() + "|");
                streamwriter.Write(component.OnState.castShadows.ToString());
                streamwriter.Write(")|");
                streamwriter.Write(System.Environment.NewLine);

                streamwriter.Write("OffState(");
                streamwriter.Write(component.OffState.bounciness.ToString() + "|");
                streamwriter.Write(component.OffState.bounds.X.ToString() + "|");
                streamwriter.Write(component.OffState.bounds.Y.ToString() + "|");
                streamwriter.Write(component.OffState.bounds.Width.ToString() + "|");
                streamwriter.Write(component.OffState.bounds.Height.ToString() + "|");
                streamwriter.Write(component.OffState.velocity.X.ToString() + "|");
                streamwriter.Write(component.OffState.velocity.Y.ToString() + "|");
                streamwriter.Write(component.OffState.CheckNPCs.ToString() + "|");
                streamwriter.Write(component.OffState.CheckPlayer.ToString() + "|");
                streamwriter.Write(component.OffState.color.R.ToString() + "|");
                streamwriter.Write(component.OffState.color.G.ToString() + "|");
                streamwriter.Write(component.OffState.color.B.ToString() + "|");
                streamwriter.Write(component.OffState.color.A.ToString() + "|");
                streamwriter.Write(component.OffState.destroyOnTouch.ToString() + "|");
                streamwriter.Write(component.OffState.friction.ToString() + "|");
                streamwriter.Write(component.OffState.gravity.ToString() + "|");
                streamwriter.Write(component.OffState.lifeTime.ToString() + "|");
                streamwriter.Write(component.OffState.physicsEnabled.ToString() + "|");
                streamwriter.Write(component.OffState.trailLength.ToString() + "|");
                streamwriter.Write(component.OffState.weight.ToString() + "|");
                streamwriter.Write(component.OffState.collidable.ToString() + "|");
                streamwriter.Write(component.OffState.castShadows.ToString());
                streamwriter.Write(")|");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Write("//Switches" + System.Environment.NewLine);

            foreach (Switch component in switches)
            {
                streamwriter.Write(component.bounciness.ToString() + "|");
                streamwriter.Write(component.bounds.X.ToString() + "|");
                streamwriter.Write(component.bounds.Y.ToString() + "|");
                streamwriter.Write(component.bounds.Width.ToString() + "|");
                streamwriter.Write(component.bounds.Height.ToString() + "|");
                streamwriter.Write(component.velocity.X.ToString() + "|");
                streamwriter.Write(component.velocity.Y.ToString() + "|");
                streamwriter.Write(component.CheckNPCs.ToString() + "|");
                streamwriter.Write(component.CheckPlayer.ToString() + "|");
                streamwriter.Write(component.color.R.ToString() + "|");
                streamwriter.Write(component.color.G.ToString() + "|");
                streamwriter.Write(component.color.B.ToString() + "|");
                streamwriter.Write(component.color.A.ToString() + "|");
                streamwriter.Write(component.destroyOnTouch.ToString() + "|");
                streamwriter.Write(component.friction.ToString() + "|");
                streamwriter.Write(component.gravity.ToString() + "|");
                streamwriter.Write(component.lifeTime.ToString() + "|");
                streamwriter.Write(component.physicsEnabled.ToString() + "|");
                streamwriter.Write(component.trailLength.ToString() + "|");
                streamwriter.Write(component.weight.ToString() + "|");
                streamwriter.Write(component.collidable.ToString() + "|");
                streamwriter.Write(component.castShadows.ToString() + "|");
                streamwriter.Write(component.type + "|");
                streamwriter.Write(component.UpdatePosition.ToString() + "|");
                streamwriter.Write(component.IsOn.ToString() + "|");
                streamwriter.Write(component.ID.ToString() + "|");
                streamwriter.Write(component.Delay.ToString() + "|");
                streamwriter.Write(component.Cyclable.ToString() + "|");
                streamwriter.Write(component.ClickDelay.ToString() + "|");
                streamwriter.Write(component.UseDelay.ToString() + "|");

                streamwriter.Write("ConnectedComponents(");

                foreach (float comp in component.connectedComponents)
                {
                    streamwriter.Write(comp.ToString() + "/");
                }

                streamwriter.Write(")|");
                streamwriter.Write(System.Environment.NewLine);
            }

            streamwriter.Dispose();
        }

        public List<Block> LoadMap(string mapname, KryptonEngine krypton,
            Texture2D mLightTexture, List<NPC> NPCs, List<ElevatorShaft> elevators, List<ElectronicComponent> electronics, List<Switch> switches, Texture2D texture)
        {
            List<Block> walls = new List<Block>();

            if (File.Exists(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Maps\" + mapname + ".map") == true)
            {
                streamreader = new StreamReader(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\Maps\" + mapname + ".map");

                string text = streamreader.ReadToEnd();
                text = text.Replace(System.Environment.NewLine, "");

                List<string> levelParts = new List<string>(text.Split(new string[] { "//lights", "//NPCs", "//Elevators", "//Electronics", "//Switches" }, StringSplitOptions.None));
                List<string> LevelTileList = new List<string>(levelParts[0].Split(new string[] { "," }, StringSplitOptions.None));
                List<string> LevelLightList = new List<string>(levelParts[1].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> LevelNPCList = new List<string>(levelParts[2].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> LevelElevatorList = new List<string>(levelParts[3].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> LevelElectronicsList = new List<string>(levelParts[4].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> LevelSwitchesList = new List<string>(levelParts[5].Split(new string[] { "|" }, StringSplitOptions.None));

                for (int i = 0; i < LevelTileList.Count / 12; i++ )
                {
                    Block block = new Block(Rectangle.Empty, Color.Black);

                    block.bounds.X = Convert.ToInt32(LevelTileList[i * 12]);
                    block.bounds.Y = Convert.ToInt32(LevelTileList[i * 12 + 1]);
                    block.bounds.Width = Convert.ToInt32(LevelTileList[i * 12 + 2]);
                    block.bounds.Height = Convert.ToInt32(LevelTileList[i * 12 + 3]);
                    block.color.R = Convert.ToByte(LevelTileList[i * 12 + 4]);
                    block.color.G = Convert.ToByte(LevelTileList[i * 12 + 5]);
                    block.color.B = Convert.ToByte(LevelTileList[i * 12 + 6]);
                    block.color.A = Convert.ToByte(LevelTileList[i * 12 + 7]);
                    block.blockID = Convert.ToInt32(i - 8);
                    block.Background = Convert.ToBoolean(LevelTileList[i * 12 + 9]);
                    block.Shatterable = Convert.ToBoolean(LevelTileList[i * 12 + 10]);
                    block.CastShadows = Convert.ToBoolean(LevelTileList[i * 12 + 11]);

                    block.UpdateBlock(block.bounds.X, block.bounds.Y, block.bounds.Width,
                        block.bounds.Height);
                        

                    walls.Add(block);
                }

                krypton.Lights.Clear();
                krypton.mMovingLights.Clear();
                for (int i = 0; i < LevelLightList.Count / 11; i++)
                {
                    Color newColor = new Color(Convert.ToByte(LevelLightList[i * 11 + 4]),
                                               Convert.ToByte(LevelLightList[i * 11 + 5]),
                                               Convert.ToByte(LevelLightList[i * 11 + 6]));

                    Light2D light = new Light2D();

                    light.Texture = mLightTexture;
                    light.X = (float)Convert.ToDecimal(LevelLightList[i * 11]);
                    light.Y = (float)Convert.ToDecimal(LevelLightList[i * 11 + 1]);
                    light.Range = (float)Convert.ToDecimal(LevelLightList[i * 11 + 2]);
                    light.Intensity = (float)Convert.ToDecimal(LevelLightList[i * 11 + 3]);
                    light.Color = newColor;
                    light.Fov = (float)Convert.ToDecimal(LevelLightList[i * 11 + 7]);
                    light.FOV = (float)Convert.ToDecimal(LevelLightList[i * 11 + 8]);
                    light.Angle = (float)Convert.ToDecimal(LevelLightList[i * 11 + 9]);
                    light.IsOn = Convert.ToBoolean(LevelLightList[i * 11 + 10]);

                    krypton.Lights.Add(light);
                }
                krypton.Initialize();

                NPCs.Clear();
                int multiplyAmount = 8;
                Random rnd = new Random();
                for (int i = 0; i < LevelNPCList.Count / multiplyAmount; i++)
                {
                    Rectangle npcbounds = new Rectangle(
                        Convert.ToInt32(LevelNPCList[i * multiplyAmount]),
                        Convert.ToInt32(LevelNPCList[i * multiplyAmount + 1]),
                        Convert.ToInt32(LevelNPCList[i * multiplyAmount + 2]),
                        Convert.ToInt32(LevelNPCList[i * multiplyAmount + 3]));

                    Color color = new Color(Convert.ToByte(LevelNPCList[i * multiplyAmount + 5]), 
                        Convert.ToByte(LevelNPCList[i * multiplyAmount + 6]),
                        Convert.ToByte(LevelNPCList[i * multiplyAmount + 7]));

                    NPC npc = new NPC(npcbounds, npcbounds, mLightTexture, 
                        (float)Convert.ToDecimal(LevelNPCList[i * multiplyAmount + 4]), color, rnd, false, false, krypton, false);
                    npc.DrawFOV(krypton);

                    NPCs.Add(npc);
                }

                multiplyAmount = 10;
                elevators.Clear();
                for (int i = 0; i < LevelElevatorList.Count / multiplyAmount; i++)
                {
                    Rectangle bounds = new Rectangle(
                        Convert.ToInt32(LevelElevatorList[i * multiplyAmount]),
                        Convert.ToInt32(LevelElevatorList[i * multiplyAmount + 1]),
                        Convert.ToInt32(LevelElevatorList[i * multiplyAmount + 2]),
                        Convert.ToInt32(LevelElevatorList[i * multiplyAmount + 3]));

                    int speed = Convert.ToInt32(LevelElevatorList[i * multiplyAmount + 5]);

                    Color color = new Color(Convert.ToByte(LevelElevatorList[i * multiplyAmount + 6]),
                        Convert.ToByte(LevelElevatorList[i * multiplyAmount + 7]),
                        Convert.ToByte(LevelElevatorList[i * multiplyAmount + 8]));

                    Elevator elevator = new Elevator(bounds, speed, texture, color);

                    elevator.currentfloor = Convert.ToInt32(LevelElevatorList[i * multiplyAmount + 4]);

                    ElevatorShaft shaft = new ElevatorShaft(elevator, elevator.currentfloor);
                    shaft.floors.Clear();

                    LevelElevatorList[i * multiplyAmount + 9] = LevelElevatorList[i * multiplyAmount + 9].Replace("floors(", "");
                    LevelElevatorList[i * multiplyAmount + 9] = LevelElevatorList[i * multiplyAmount + 9].Replace(")", "");
                    List<string> ElevatorFloors = new List<string>(LevelElevatorList[i * multiplyAmount + 9].Split(new string[] { "/" }, StringSplitOptions.None));

                    for (int j = 0; j < ElevatorFloors.Count / 2; j++)
                    {
                        Rectangle stopbounds = new Rectangle(
                            Convert.ToInt32(ElevatorFloors[j * 2 + 0]),
                            Convert.ToInt32(ElevatorFloors[j * 2 + 1]),
                            elevator.bounds.Width, 1);

                        Rectangle stoprealbounds = elevator.bounds;

                        ElevatorStop stop = new ElevatorStop(stopbounds, stoprealbounds);

                        shaft.floors.Add(stop);
                    }

                    elevators.Add(shaft);
                }

                multiplyAmount = 72;
                electronics.Clear();
                for (int i = 0; i < LevelElectronicsList.Count / multiplyAmount; i++)
                {
                    Object OnState = new Object();
                    Object OffState = new Object();
                    ElectronicComponent component = new ElectronicComponent();

                    component.bounciness = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount]);
                    component.bounds.X = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 1]);
                    component.bounds.Y = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 2]);
                    component.bounds.Width = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 3]);
                    component.bounds.Height = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 4]);
                    component.velocity.X = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 5]);
                    component.velocity.Y = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 6]);
                    component.CheckNPCs = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 7]);
                    component.CheckPlayer = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 8]);
                    component.color = new Color(Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 9]),
                                                Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 10]),
                                                Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 11]),
                                                Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 12]));
                    component.destroyOnTouch = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 13]);
                    component.friction = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 14]);
                    component.gravity = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 15]);
                    component.lifeTime = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 16]);
                    component.physicsEnabled = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 17]);
                    component.trailLength = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 18]);
                    component.weight = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 19]);
                    component.collidable = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 20]);
                    component.castShadows = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 21]);
                    component.type = (LevelElectronicsList[i * multiplyAmount + 22].ToString());
                    component.UpdatePosition = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 23]);
                    component.IsOn = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 24]);
                    component.ID = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 25]);
                    component.Delay = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 26]);

                    LevelElectronicsList[i * multiplyAmount + 27] = LevelElectronicsList[i * multiplyAmount + 27].Replace("ConnectedComponents()", "");
                    LevelElectronicsList[i * multiplyAmount + 27] = LevelElectronicsList[i * multiplyAmount + 27].Replace("ConnectedComponents(", "");
                    LevelElectronicsList[i * multiplyAmount + 27] = LevelElectronicsList[i * multiplyAmount + 27].Replace(")", "");
                    List<string> Connections = new List<string>(LevelElectronicsList[i * multiplyAmount + 27].Split(new string[] { "/" }, StringSplitOptions.None));

                    if (Connections.Count > 0)
                    {
                        for (int o = 0; o < Connections.Count - 1; o++)
                        {
                            float connection = Convert.ToSingle(Connections[o]);
                            component.connectedComponents.Add(connection);
                        }
                    }

                    LevelElectronicsList[i * multiplyAmount + 28] = LevelElectronicsList[i * multiplyAmount + 28].Replace("OnState(", "");

                    OnState.bounciness = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 28]);
                    OnState.bounds.X = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 29]);
                    OnState.bounds.Y = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 30]);
                    OnState.bounds.Width = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 31]);
                    OnState.bounds.Height = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 32]);
                    OnState.velocity.X = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 33]);
                    OnState.velocity.Y = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 34]);
                    OnState.CheckNPCs = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 35]);
                    OnState.CheckPlayer = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 36]);
                    OnState.color = new Color(Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 37]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 38]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 39]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 40]));
                    OnState.destroyOnTouch = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 41]);
                    OnState.friction = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 42]);
                    OnState.gravity = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 43]);
                    OnState.lifeTime = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 44]);
                    OnState.physicsEnabled = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 45]);
                    OnState.trailLength = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 46]);
                    OnState.weight = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 47]);
                    OnState.collidable = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 48]);

                    LevelElectronicsList[i * multiplyAmount + 49] = LevelElectronicsList[i * multiplyAmount + 49].Replace(")", "");

                    OnState.castShadows = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 49]);

                    LevelElectronicsList[i * multiplyAmount + 50] = LevelElectronicsList[i * multiplyAmount + 50].Replace("OffState(", "");

                    OffState.bounciness = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 50]);
                    OffState.bounds.X = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 51]);
                    OffState.bounds.Y = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 52]);
                    OffState.bounds.Width = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 53]);
                    OffState.bounds.Height = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 54]);
                    OffState.velocity.X = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 55]);
                    OffState.velocity.Y = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 56]);
                    OffState.CheckNPCs = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 57]);
                    OffState.CheckPlayer = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 58]);
                    OffState.color = new Color(Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 59]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 60]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 61]),
                                              Convert.ToByte(LevelElectronicsList[i * multiplyAmount + 62]));
                    OffState.destroyOnTouch = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 63]);
                    OffState.friction = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 64]);
                    OffState.gravity = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 65]);
                    OffState.lifeTime = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 66]);
                    OffState.physicsEnabled = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 67]);
                    OffState.trailLength = Convert.ToInt32(LevelElectronicsList[i * multiplyAmount + 68]);
                    OffState.weight = Convert.ToSingle(LevelElectronicsList[i * multiplyAmount + 69]);
                    OffState.collidable = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 70]);

                    LevelElectronicsList[i * multiplyAmount + 71] = LevelElectronicsList[i * multiplyAmount + 71].Replace(")", "");

                    OffState.castShadows = Convert.ToBoolean(LevelElectronicsList[i * multiplyAmount + 71]);

                    component.OnState = OnState;
                    component.OffState = OffState;

                    electronics.Add(component);
                }

                multiplyAmount = 31;
                for (int i = 0; i < LevelSwitchesList.Count / multiplyAmount; i++)
                {
                    Object OnState = new Object();
                    Object OffState = new Object();
                    Switch component = new Switch(texture, Color.White, false, 0, 0, rnd, electronics, OnState, OffState, false, krypton, false);

                    component.bounciness = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount]);
                    component.bounds.X = Convert.ToInt32(LevelSwitchesList[i * multiplyAmount + 1]);
                    component.bounds.Y = Convert.ToInt32(LevelSwitchesList[i * multiplyAmount + 2]);
                    component.bounds.Width = Convert.ToInt32(LevelSwitchesList[i * multiplyAmount + 3]);
                    component.bounds.Height = Convert.ToInt32(LevelSwitchesList[i * multiplyAmount + 4]);
                    component.velocity.X = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 5]);
                    component.velocity.Y = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 6]);
                    component.CheckNPCs = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 7]);
                    component.CheckPlayer = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 8]);
                    component.color = new Color(Convert.ToByte(LevelSwitchesList[i * multiplyAmount + 9]),
                                                Convert.ToByte(LevelSwitchesList[i * multiplyAmount + 10]),
                                                Convert.ToByte(LevelSwitchesList[i * multiplyAmount + 11]),
                                                Convert.ToByte(LevelSwitchesList[i * multiplyAmount + 12]));
                    component.destroyOnTouch = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 13]);
                    component.friction = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 14]);
                    component.gravity = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 15]);
                    component.lifeTime = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 16]);
                    component.physicsEnabled = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 17]);
                    component.trailLength = Convert.ToInt32(LevelSwitchesList[i * multiplyAmount + 18]);
                    component.weight = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 19]);
                    component.collidable = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 20]);
                    component.castShadows = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 21]);
                    component.type = (LevelSwitchesList[i * multiplyAmount + 22].ToString());
                    component.UpdatePosition = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 23]);
                    component.IsOn = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 24]);
                    component.ID = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 25]);
                    component.Delay = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 26]);
                    component.Cyclable = Convert.ToBoolean(LevelSwitchesList[i * multiplyAmount + 27]);
                    component.ClickDelay = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 28]);
                    component.UseDelay = Convert.ToSingle(LevelSwitchesList[i * multiplyAmount + 29]);

                    LevelSwitchesList[i * multiplyAmount + 30] = LevelSwitchesList[i * multiplyAmount + 30].Replace("ConnectedComponents()", "");
                    LevelSwitchesList[i * multiplyAmount + 30] = LevelSwitchesList[i * multiplyAmount + 30].Replace("ConnectedComponents(", "");
                    LevelSwitchesList[i * multiplyAmount + 30] = LevelSwitchesList[i * multiplyAmount + 30].Replace(")", "");
                    List<string> Connections = new List<string>(LevelSwitchesList[i * multiplyAmount + 30].Split(new string[] { "/" }, StringSplitOptions.None));

                    if (Connections.Count > 0)
                    {
                        for (int o = 0; o < Connections.Count - 1; o++)
                        {
                            float connection = Convert.ToSingle(Connections[o]);
                            component.connectedComponents.Add(connection);
                        }
                    }

                    OnState.bounciness = component.bounciness;
                    OnState.bounds.X = component.bounds.X;
                    OnState.bounds.Y = component.bounds.Y;
                    OnState.bounds.Width = component.bounds.Width;
                    OnState.bounds.Height = component.bounds.Height;
                    OnState.velocity.X = component.velocity.X;
                    OnState.velocity.Y = component.velocity.Y;
                    OnState.CheckNPCs = component.CheckNPCs;
                    OnState.CheckPlayer = component.CheckPlayer;
                    OnState.color = component.color;
                    OnState.destroyOnTouch = component.destroyOnTouch;
                    OnState.friction = component.friction;
                    OnState.gravity = component.gravity;
                    OnState.lifeTime = component.lifeTime;
                    OnState.physicsEnabled = component.physicsEnabled;
                    OnState.trailLength = component.trailLength;
                    OnState.weight = component.weight;
                    OnState.collidable = component.collidable;

                    component.OnState = OnState;
                    component.OffState = OnState;
                    switches.Add(component);
                }

                streamreader.Dispose();
            }

            krypton.Hulls.Clear();
            ShadowHull hull;
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i].Background == false && walls[i].CastShadows == true)
                {
                    hull = ShadowHull.CreateRectangle(Vector2.One);
                    hull.Position.X = walls[i].bounds.Center.X;
                    hull.Position.Y = walls[i].bounds.Center.Y;
                    hull.Scale.X = walls[i].bounds.Width;
                    hull.Scale.Y = walls[i].bounds.Height;

                    krypton.Hulls.Add(hull);
                }
            }

            return walls;
        }

        public void LoadGUI()
        { 
        
        }

        public void Draw()
        {
            MapName.Draw();
        }
    }
}
