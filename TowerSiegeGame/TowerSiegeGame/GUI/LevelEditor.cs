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
    public class LevelEditor
    {
        KeyboardState ks;
        MouseState ms;

        Vector2 selectionStart,
                lastMS,
                mouse = Vector2.Zero;

        public Rectangle mousepos;
        Rectangle tiledMousePos,
            lastTiledMousePos;


        Rectangle selection = new Rectangle();
        public Rectangle spawnPoint = new Rectangle();

        public List<Block> walls = new List<Block>(),
                           blocks = new List<Block>();
        Block selectedBlock = new Block(Rectangle.Empty, Color.MediumVioletRed);
        int selectedComponent;

        public bool canAddBlock = false,
                    Active = false,
                    Typing = false,
                    CanLeftClick = true,
                    CanRightClick = true;

        private bool canActivate = true,
                     movingBlocks = false,
                     movingSpawn = false,
                     dragging = false;

        public SaveHandling mapHandling;
        public BlockHandling blockHandling;

        SpriteFont font1;

        RadioButton editMode,
                    blockSelector;

        Texture2D texture;
        Texture2D mLightTexture;
        LightEditor lightEditor;

        List<Rectangle> circuitLine = new List<Rectangle>(),
                        wiring = new List<Rectangle>();

        public LevelEditor(KryptonEngine krypton,
            SpriteFont font1, Texture2D texture, SpriteBatch spriteBatch, Texture2D mLightTexture,
            Rectangle viewPort, GraphicsDeviceManager graphics, Game game, List<NPC> NPCs, List<ElevatorShaft> elevators, List<ElectronicComponent> electronics, List<Switch> switches,
            Texture2D sliderBoxTex, Texture2D sliderTex)
        {
            //krypton = realkrypton;
            this.font1 = font1;
            this.mLightTexture = mLightTexture;
            this.lightEditor = new LightEditor(
                new Rectangle(viewPort.Center.X, viewPort.Center.Y - 150, 200, 50), graphics, font1,
                game, texture, mLightTexture, viewPort, 300, sliderBoxTex, sliderTex);

            this.texture = texture;

            spawnPoint = new Rectangle(2000, 1000, 10, 10);

            blockHandling = new BlockHandling();
            mapHandling = new SaveHandling(font1, spriteBatch, texture, Rectangle.Empty);
            //blockHandling.SaveBlock(new Block(new Rectangle(0, 0, 30, 50), Color.Bisque), "testBlock");
            this.selectedBlock = blockHandling.LoadBlockFromFile("testBlock");

            this.walls = mapHandling.LoadMap("buildingtest", krypton, mLightTexture, NPCs, elevators, electronics, switches, texture);
            //mapHandling.SaveMap(walls, krypton, "saaave");

            editMode = new RadioButton(Color.White, font1, "EditMode", 6, 10, "vertical", 0, texture,
                Color.Green, Color.Gray, Color.Gray, Color.LightGray, Color.White, 
                new Rectangle(20, 20, 20, 20));

            string[] directoryContents = Directory.GetFiles(
@"C:\Users\" + blockHandling.ComputerName + @"\Documents\my games\StealthGame\Blocks");

            foreach(string loadBlock in directoryContents)
            {
                string editloadBlock = Path.GetFileName(loadBlock);
                editloadBlock = editloadBlock.Replace(".block", "");
                blocks.Add(blockHandling.LoadBlockFromFile(editloadBlock));
            }

            blockSelector = new RadioButton(Color.White, font1, "LoadedBlocks", blocks.Count, 10, "vertical",
                0, texture, Color.Green, Color.Gray, Color.Gray, Color.LightGray, Color.White,
                new Rectangle(viewPort.Right - 100, viewPort.Top + 50, 40, 40));
        }

        public void Update(CollisionMap collisionMap, Camera camera, KryptonEngine krypton, 
            Rectangle windowSize, GraphicsDevice device, GraphicsDeviceManager graphics,
            List<NPC> NPCs, List<ElevatorShaft> elevators, Random rnd, List<Floor> floors, List<ElectronicComponent> electronics, List<Switch> switches)
        {
            lastMS = mouse;

            int tileSizeX = 10,
                tileSizeY = 10;
            lastTiledMousePos = tiledMousePos;

            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            editMode.Update(font1);
            blockSelector.Update(font1);
            selectedBlock = blocks[blockSelector.CheckedState];

            mouse = new Vector2(ms.X + windowSize.Width / 2 * camera.Zoom, ms.Y + windowSize.Height / 2 * camera.Zoom);
            Matrix transform = Matrix.Invert(camera.get_transformation(device, windowSize));

            Vector2.Transform(ref mouse, ref transform, out mouse);

            mouse.X += -windowSize.Width / 2 + camera.Position.X + 0.5f;
            mouse.Y += -windowSize.Height / 2 + camera.Position.Y + 0.5f;

            mousepos = new Rectangle((int)mouse.X, (int)mouse.Y, 1, 1);

            tiledMousePos = new Rectangle((int)Math.Floor((decimal)(mousepos.X / tileSizeX) * tileSizeX),
                (int)Math.Floor((decimal)(mousepos.Y / tileSizeY) * tileSizeY), 1, 1);
            selectedBlock.bounds.X = tiledMousePos.X;
            selectedBlock.bounds.Y = tiledMousePos.Y;

            //krypton = realkrypton;

            int ts = collisionMap.TileSize;

            if (ms.LeftButton == ButtonState.Pressed || (ms.RightButton == ButtonState.Pressed && 
                (editMode.CheckedState == 0 || editMode.CheckedState == 2 || editMode.CheckedState == 4)))
            {
                selection.X = (int)selectionStart.X - (int)camera.Position.X;
                selection.Y = (int)selectionStart.Y - (int)camera.Position.Y;
                selection.Width = (int)(mouse.X - selectionStart.X);
                selection.Height = (int)(mouse.Y - selectionStart.Y);

                if (selection.Height < 0)
                {
                    selection.Height = -selection.Height;
                    selection.Y = selection.Y - selection.Height;
                }

                if (selection.Width < 0)
                {
                    selection.Width = -selection.Width;
                    selection.X = selection.X - selection.Width;
                }

                selection.X = (int)MathHelper.Clamp(selection.X + camera.Position.X, 0, (collisionMap.width - 1) * ts - selection.Width);
                selection.Y = (int)MathHelper.Clamp(selection.Y + camera.Position.Y, 0, (collisionMap.height - 1) * ts - selection.Height);

                if ((selection.Width > 1 || selection.Height > 1) && movingBlocks == false)
                {
                    dragging = true;
                }
            }
            else
            {
                if (canAddBlock == false)
                {
                    selection = Rectangle.Empty;
                    selectionStart = new Vector2(mouse.X,
                        mouse.Y);
                }

                movingBlocks = false;
                movingSpawn = false;
                dragging = false;
            }

            if (ks.IsKeyDown(Keys.F1))
            {
                if (canActivate == true)
                {
                    if (Active == false)
                    {
                        Active = true;
                    }
                    else
                    {
                        Active = false;

                        foreach(Block wall in walls)
                        {
                            wall.Selected = false;
                        }

                        foreach (Light2D light in krypton.Lights)
                        {
                            light.Selected = false;
                        }

                        foreach (NPC npc in NPCs)
                        {
                            npc.Selected = false;
                        }

                        foreach (ElevatorShaft elevator in elevators)
                        {
                            elevator.selected = false;

                            foreach (ElevatorStop rect in elevator.floors)
                            {
                                rect.selected = false;
                            }
                        }
                    }
                }

                canActivate = false;
            }
            else
            {
                canActivate = true;
            }

            if (Active == true)
            {
                if (lightEditor.active == false)
                {
                    mapHandling.Update(windowSize, ks, walls, out walls, out Typing, krypton,
                        mLightTexture, NPCs, elevators, texture, electronics, switches);
                }

                if (Typing == true)
                    return;

                lightEditor.Update(graphics, windowSize, 300);

                if (lightEditor.active == true)
                    return;

                if (ks.IsKeyDown(Keys.Delete))
                {
                    RemoveBlock(collisionMap, krypton, false);
                    RemoveLight(krypton);
                    RemoveElevator(elevators);
                    RemoveNPCs(NPCs);
                }

                if (ms.LeftButton == ButtonState.Released)
                {
                    if (circuitLine.Count > 10)
                    {
                        foreach (ElectronicComponent component in electronics)
                        {
                            if (mousepos.Intersects(component.bounds) && component.type.ToLower() == "receiver")
                            {
                                switches[selectedComponent].connectedComponents.Add(component.ID);
                            }
                        }
                    }
                }

                if ((ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed))
                {
                    if (ms.RightButton == ButtonState.Pressed)
                    {
                        canAddBlock = true;

                        foreach (Light2D light in krypton.Lights)
                        {
                            if (light.Selected == true && editMode.CheckedState == 2)
                            {
                                movingBlocks = true;
                                canAddBlock = false;
                                light.Angle = ((float)((Math.Atan2((mouse.Y - light.Y), (mouse.X - light.X)))));
                            }
                        }
                    }

                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        if (editMode.CheckedState == 5)
                        {
                            bool drawLine = false;

                            for (int o = 0; o < electronics.Count; o++ )
                            {
                                if (electronics[o].bounds.Intersects(new Rectangle((int)selectionStart.X, (int)selectionStart.Y, 1, 1)) && electronics[o].type.ToLower() == "transmitter")
                                {
                                    drawLine = true;
                                    selectedComponent = o;
                                }
                            }

                            for (int o = 0; o < switches.Count; o++)
                            {
                                if (switches[o].bounds.Intersects(new Rectangle((int)selectionStart.X, (int)selectionStart.Y, 1, 1)) && switches[o].type.ToLower() == "transmitter")
                                {
                                    drawLine = true;
                                    selectedComponent = o;
                                }
                            }

                            if (drawLine == true)
                            {
                                circuitLine = Shapes.line((int)mouse.X, (int)mouse.Y, (int)selectionStart.X, (int)selectionStart.Y,
                                    1, 3000, new Point(1, 1), Point.Zero);
                            }
                            else
                            {
                                if (CanLeftClick == true)
                                {
                                    Object compOn = new Object(new Rectangle((int)mouse.X - 10, (int)mouse.Y - 5, 10, 60), 0, false, false, false, krypton, true);
                                    compOn.color = Color.Black;
                                    compOn.physicsEnabled = true;
                                    compOn.velocity.X = 0;
                                    compOn.velocity.Y = 0;
                                    compOn.gravity = 0;
                                    compOn.collidable = true;
                                    compOn.texture = texture;

                                    Object compOff = new Object(new Rectangle(compOn.bounds.X, compOn.bounds.Y, 10, 60), 0, false, false, false, krypton, true);
                                    compOff.color = Color.Black;
                                    compOff.physicsEnabled = true;
                                    compOff.velocity.X = 5;
                                    compOff.velocity.Y = 0;
                                    compOff.gravity = 0.0f;
                                    compOff.collidable = true;
                                    compOff.texture = texture;
                                    compOff.deadly = true;

                                    ElectronicComponent comp = new ElectronicComponent(rnd, electronics, compOn, compOff, true, krypton, false);
                                    comp.UpdatePosition = false;

                                    electronics.Add(comp);
                                }
                            }
                        }

                        if ((mousepos.Intersects(spawnPoint) || movingSpawn == true) && dragging == false)
                        {
                            if (movingBlocks == false)
                            {
                                movingSpawn = true;
                            }

                            movingBlocks = true;
                            //dragging = true;

                            spawnPoint.X += (int)((mouse.X + (int)camera.Position.X) - (lastMS.X + (int)camera.Position.X));
                            spawnPoint.Y += (int)((mouse.Y + (int)camera.Position.Y) - (lastMS.Y + (int)camera.Position.Y));
                        }

                        foreach (Block wall in walls)
                        {
                            if (wall.Selected == true)
                            {
                                if ((mousepos.Intersects(wall.bounds) || movingBlocks == true) && dragging == false)
                                {
                                    movingBlocks = true;

                                    wall.UpdateBlock(wall.bounds.X + (((int)tiledMousePos.X) - ((int)lastTiledMousePos.X)),
                                                         wall.bounds.Y + (((int)tiledMousePos.Y) - ((int)lastTiledMousePos.Y)),
                                                          wall.bounds.Width,
                                                          wall.bounds.Height, collisionMap);

                                    if (wall.stoppedX == true || wall.stoppedY == true)
                                    {
                                        dragging = true;
                                    }
                                }
                            }
                        }

                        foreach (Light2D light in krypton.Lights)
                        {
                            if (light.Selected == true)
                            {
                                if ((mousepos.Intersects(light.selectionBounds()) || movingBlocks == true) && dragging == false)
                                {
                                    movingBlocks = true;

                                    light.X += ((mouse.X + (int)camera.Position.X) - (lastMS.X + (int)camera.Position.X));
                                    light.Y += ((mouse.Y + (int)camera.Position.Y) - (lastMS.Y + (int)camera.Position.Y));
                                }
                            }
                        }

                        foreach (NPC npc in NPCs)
                        {
                            if (npc.Selected == true)
                            {
                                if ((mousepos.Intersects(npc.bounds) || movingBlocks == true) && dragging == false)
                                {
                                    movingBlocks = true;

                                    npc.velocity = Vector2.Zero;
                                    npc.bounds.X += (int)((mouse.X + (int)camera.Position.X) - (lastMS.X + (int)camera.Position.X));
                                    npc.bounds.Y += (int)((mouse.Y + (int)camera.Position.Y) - (lastMS.Y + (int)camera.Position.Y));
                                }
                            }
                        }

                        foreach (ElevatorShaft elevator in elevators)
                        {
                            if (mousepos.Intersects(elevator.elevator.bounds) && movingBlocks == false && dragging == false)
                            {
                                elevator.selected = true;
                            }
                            else
                            {
                                foreach (ElevatorStop rect in elevator.floors)
                                {
                                    if (rect.selected == true)
                                    {
                                        if ((mousepos.Intersects(rect.realbounds) || movingBlocks == true) && dragging == false)
                                        {
                                            movingBlocks = true;

                                            rect.bounds.Y += (((int)mousepos.Y + (int)camera.Position.Y) - ((int)lastMS.Y + (int)camera.Position.Y));

                                            rect.realbounds.X = rect.bounds.X;
                                            rect.realbounds.Y = rect.bounds.Y - rect.realbounds.Height;
                                        }
                                    }
                                }
                            }

                            if (elevator.selected == true)
                            {
                                if ((mousepos.Intersects(elevator.elevator.bounds) || movingBlocks == true) && dragging == false)
                                {
                                    movingBlocks = true;

                                    elevator.elevator.bounds.X += (((int)mousepos.X + (int)camera.Position.X) - ((int)lastMS.X + (int)camera.Position.X));
                                    elevator.elevator.bounds.Y += (((int)mousepos.Y + (int)camera.Position.Y) - ((int)lastMS.Y + (int)camera.Position.Y));
                                }
                            }
                        }

                        if (movingBlocks == false)
                        {
                            foreach (Block wall in walls)
                            {
                                if (wall.Selected == true)
                                {
                                    if (movingBlocks == false)
                                    {
                                        wall.Selected = false;
                                    }
                                }

                                if (editMode.CheckedState == 0 || editMode.CheckedState == 1)
                                {
                                    if (selection.Intersects(wall.bounds))
                                    {
                                        switch (editMode.CheckedState)
                                        {
                                            case 0:
                                                if (wall.Background == false)
                                                {
                                                    wall.Selected = true;
                                                }
                                                break;

                                            case 1:
                                                if (wall.Background == true)
                                                {
                                                    wall.Selected = true;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }

                            foreach (Light2D light in krypton.Lights)
                            {
                                if (light.Selected == true)
                                {
                                    if (movingBlocks == false)
                                    {
                                        light.Selected = false;
                                    }
                                }

                                if (editMode.CheckedState == 2)
                                {
                                    if (selection.Intersects(light.selectionBounds()))
                                    {
                                        light.Selected = true;
                                    }
                                }
                            }

                            foreach (NPC npc in NPCs)
                            {
                                if (npc.Selected == true)
                                {
                                    if (movingBlocks == false)
                                    {
                                        npc.Selected = false;
                                    }
                                }

                                if (editMode.CheckedState == 3)
                                {
                                    if (selection.Intersects(npc.bounds))
                                    {
                                        npc.Selected = true;
                                    }
                                }
                            }

                            bool checkElevator = true;
                            foreach (ElevatorShaft elevator in elevators)
                            {
                                foreach (ElevatorStop rect in elevator.floors)
                                {
                                    if (selection.Intersects(rect.realbounds) && editMode.CheckedState == 4)
                                    {
                                        rect.selected = true;
                                        checkElevator = false;
                                    }
                                    else
                                    {
                                        rect.selected = false;
                                    }
                                }

                                if (checkElevator == true)
                                {
                                    if (selection.Intersects(elevator.elevator.bounds) && editMode.CheckedState == 4)
                                    {
                                        elevator.selected = true;
                                    }
                                    else
                                    {
                                        elevator.selected = false;
                                    }
                                }
                            }
                        }

                        CanLeftClick = false;
                    }
                    else
                    {
                        circuitLine.Clear();
                    }
                }
                else
                {
                    CanLeftClick = true;
                    circuitLine.Clear();
                }

                if (ms.RightButton == ButtonState.Released)
                {
                    if (canAddBlock == true)
                    {
                        switch (editMode.CheckedState)
                        {
                            case 0:
                                AddBlock(collisionMap, krypton, selection, false, rnd);
                                break;

                            case 1:
                                AddBlock(collisionMap, krypton, selectedBlock.bounds, false, rnd);
                                break;

                            case 2:
                                Vector2 lightAngle = new Vector2(mousepos.X - selectionStart.X, mousepos.Y - selectionStart.Y);
                                float angle = ((float)((Math.Atan2((lightAngle.Y), (lightAngle.X)))));
                                AddLight(selectionStart.X, selectionStart.Y, lightEditor.editlight.Range,
                                    lightEditor.editlight.FOV,
                                    angle,
                                    lightEditor.editlight.Color,
                                    lightEditor.editlight.Intensity,
                                    krypton);
                                break;

                            case 3:
                                Rectangle npcpos = new Rectangle(mousepos.X, mousepos.Y, 20, 30);
                                NPC newnpc = new NPC(npcpos, npcpos, mLightTexture, 60, Color.Green,
                                    rnd, false, false, krypton, false);
                                newnpc.DrawFOV(krypton);
                                NPCs.Add(newnpc);
                                break;

                            case 4:
                                AddElevator(selection, elevators);
                                break;
                        }
                        canAddBlock = false;
                    }
                }

                int i = 0;
                int j = 0;
                foreach (Block block in walls)
                {
                    if (block.Background == false && block.CastShadows == true)
                    {
                        krypton.Hulls[i].Position.X = walls[i + j].bounds.Center.X;
                        krypton.Hulls[i].Position.Y = walls[i + j].bounds.Center.Y;
                        i++;
                    }
                    else {
                        j++;
                    }
                }

                collisionMap.Update(walls, true);
            }
        }

        public void AddBlock(CollisionMap collisionMap, KryptonEngine krypton, Rectangle block, bool addAsSelected, Random rnd)
        {
            int ts = collisionMap.TileSize;

            Block newBlock = new Block(block, selectedBlock.color);
            newBlock.blockID = rnd.Next(-999999999, 999999999);
            newBlock.Background = selectedBlock.Background;
            newBlock.Shatterable = selectedBlock.Shatterable;
            newBlock.CastShadows = selectedBlock.CastShadows;

            if (addAsSelected == true)
                newBlock.Selected = true;

            walls.Add(newBlock);

            if (newBlock.Background == false && newBlock.CastShadows == true)
            {
                AddHull(newBlock.bounds, krypton, out krypton);
            }
        }

        public void AddElevator(Rectangle block, List<ElevatorShaft> elevators)
        {
            Elevator elevator = new Elevator(block, 2, texture, Color.White);
            ElevatorShaft shaft = new ElevatorShaft(elevator, 1);

            bool addShaft = true;
            for (int i = 0; i < elevators.Count; i++)
            {
                if (elevators[i].selected == true)
                {
                    ElevatorStop elevatorStop = new ElevatorStop(new Rectangle(elevators[i].elevator.bounds.X, block.Y, elevators[i].elevator.bounds.Width, 1),
                        new Rectangle(elevators[i].elevator.bounds.X, block.Y - elevators[i].elevator.bounds.Height,
                                      elevators[i].elevator.bounds.Width, elevators[i].elevator.bounds.Height));
                    elevators[i].floors.Add(elevatorStop);

                    addShaft = false;
                    break;
                }
            }

            if (addShaft == true)
            {
                elevators.Add(shaft);
            }
        }

        public void RemoveElevator(List<ElevatorShaft> elevators)
        {
                for (int i = 0; i < elevators.Count; i++)
                {
                    if (elevators[i].selected == true)
                    {
                        elevators.RemoveAt(i);
                    }
                    else
                    {
                        for (int j = 0; j < elevators[i].floors.Count; j++)
                        {
                            if (elevators[i].floors[j].selected == true)
                            {
                                elevators[i].floors.RemoveAt(j);
                            }
                        }
                    }
                }
        }

        public void RemoveBlock(CollisionMap collisionMap, KryptonEngine krypton, bool UpdateColMap)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                if (walls[i].Selected == true || walls[i].Existing == false)
                {
                    //if (walls[i].Background == false && walls[i].CastShadows == false)
                    {
                        RemoveHull(walls[i].bounds, krypton, out krypton);
                    }
                    walls.RemoveAt(i);
                }
            }

            if (UpdateColMap == true)
            {
                collisionMap.Update(walls, true);
            }
        }

        public void RemoveLight(KryptonEngine krypton)
        {
            List<int> lightsToRemove = new List<int>();
            int i = 0;
            foreach(Light2D light in krypton.Lights)
            {
                if (light.Selected == true)
                {
                    lightsToRemove.Add(i);
                }
                i++;
            }

            int lightsremoved = 0;
            for (int j = 0; j < lightsToRemove.Count; j++)
            {
                krypton.Lights.RemoveAt(lightsToRemove[j - lightsremoved]);
                lightsremoved++;
            }
        }

        public void AddLight(float posX, float posY, float range, float mFOV, float angle, Color color,
            float intensity, KryptonEngine krypton)
        {
            Light2D newLight = new Light2D()
            {
                X = posX,
                Y = posY,
                Texture = mLightTexture,
                Range = range,
                Color = color,
                Intensity = intensity,
                Angle = angle,
                FOV = MathHelper.Clamp(mFOV, -360, 360),
                Fov = (float)((Math.PI + 0.3f) * ((mFOV / 2.0f) / 100.0f)),
                RangeChange = 1,
                RangeMax = 400,
                RangeMin = 350,
            };

            krypton.Lights.Add(newLight);
        }

        public void RemoveHull(Rectangle beGoneHull, KryptonEngine realkrypton, out KryptonEngine krypton)
        {
            krypton = realkrypton;

            for (int i = 0; i < krypton.Hulls.Count; i++ )
            {
                if (krypton.Hulls[i].Position.X == beGoneHull.Center.X &&
                    krypton.Hulls[i].Position.Y == beGoneHull.Center.Y &&
                    krypton.Hulls[i].Scale.X == beGoneHull.Width &&
                    krypton.Hulls[i].Scale.Y == beGoneHull.Height)
                {
                    krypton.Hulls.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveNPCs(List<NPC> NPCs)
        {
            for (int i = 0; i < NPCs.Count; i++)
            {
                if (NPCs[i].Selected == true)
                {
                    NPCs[i].fovVisualization.IsOn = false;
                    NPCs.RemoveAt(i);
                }
            }
        }

        public void AddHull(Rectangle newHull, KryptonEngine realkrypton, out KryptonEngine krypton)
        {
            ShadowHull hull;
            krypton = realkrypton;

            hull = ShadowHull.CreateRectangle(Vector2.One);
            hull.Position.X = newHull.Center.X;
            hull.Position.Y = newHull.Center.Y;
            hull.Scale.X = newHull.Width;
            hull.Scale.Y = newHull.Height;

            krypton.Hulls.Add(hull);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, 
            KryptonEngine krypton, List<NPC> NPCs, List<ElectronicComponent> electronics, List<Switch> switches)
        {
            if (Active == false)
                return;

            if (editMode.CheckedState == 1)
            {
                selectedBlock.Draw(spriteBatch, texture, camera, false);
            }
            mapHandling.Draw();

            foreach (Light2D light in krypton.Lights)
            {
                if (light.IsOn == true)
                {
                    if (light.Selected == true)
                        spriteBatch.Draw(texture, new Rectangle(light.selectionBounds().X - (int)camera.Position.X,
                            light.selectionBounds().Y - (int)camera.Position.Y, light.selectionBounds().Width,
                            light.selectionBounds().Height), Color.Yellow);
                    else
                        spriteBatch.Draw(texture, new Rectangle(light.selectionBounds().X - (int)camera.Position.X,
                            light.selectionBounds().Y - (int)camera.Position.Y, light.selectionBounds().Width,
                            light.selectionBounds().Height), Color.Gray);
                }
            }

            spriteBatch.Draw(texture, new Rectangle(spawnPoint.X - (int)camera.Position.X,
                            spawnPoint.Y - (int)camera.Position.Y, spawnPoint.Width,
                            spawnPoint.Height), Color.FromNonPremultiplied(0, 255, 0, 100));

            if (movingBlocks == false && ((ms.LeftButton == ButtonState.Pressed || (ms.RightButton == ButtonState.Pressed &&
                (editMode.CheckedState != 2)))) && editMode.CheckedState != 5)
            {
                spriteBatch.Draw(texture, new Rectangle(selection.X - (int)camera.Position.X,
                                                        selection.Y - (int)camera.Position.Y,
                                                        selection.Width, selection.Height),
                                                        Color.FromNonPremultiplied(0, 200, 10, 150));
            }

            if (editMode.CheckedState == 5)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    foreach (Rectangle rect in circuitLine)
                    {
                        spriteBatch.Draw(texture, new Rectangle(rect.X - (int)camera.Position.X,
                                                                rect.Y - (int)camera.Position.Y,
                                                                rect.Width, rect.Height),
                                                                Color.FromNonPremultiplied(0, 200, 10, 150));
                    }
                }

                foreach (ElectronicComponent component in electronics)
                {
                    if (component.connectedComponents.Count > 0)
                    {
                        foreach (float ID in component.connectedComponents)
                        {
                            foreach (ElectronicComponent comp in electronics)
                            {
                                if (comp.ID == ID)
                                {
                                    Vector2 direction = new Vector2(comp.bounds.Center.X, comp.bounds.Center.Y) - new Vector2(component.bounds.Center.X, component.bounds.Center.Y);
                                    direction.Normalize();

                                    float rotation = (float)Math.Atan2(
                                                  (double)direction.Y,
                                                  (double)direction.X);

                                    int length = (int)new Vector2(component.bounds.Center.X - comp.bounds.Center.X, component.bounds.Center.Y - comp.bounds.Center.Y).Length();

                                    spriteBatch.Draw(texture, 
                                        new Rectangle(((int)component.bounds.Center.X - (int)(component.bounds.Center.X - comp.bounds.Center.X) / 2) - (int)camera.Position.X,
                                        ((int)component.bounds.Center.Y - ((int)component.bounds.Center.Y - comp.bounds.Center.Y) / 2) - (int)camera.Position.Y, 
                                        length, 3), null,
                                        Color.Green, rotation, new Vector2(texture.Bounds.Width / 2.0f, texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
                                }
                            }
                        }
                    }
                }

                foreach (Switch component in switches)
                {
                    if (component.connectedComponents.Count > 0)
                    {
                        foreach (float ID in component.connectedComponents)
                        {
                            foreach (ElectronicComponent comp in electronics)
                            {
                                if (comp.ID == ID)
                                {
                                    Vector2 direction = new Vector2(comp.bounds.Center.X, comp.bounds.Center.Y) - new Vector2(component.bounds.Center.X, component.bounds.Center.Y);
                                    direction.Normalize();

                                    float rotation = (float)Math.Atan2(
                                                  (double)direction.Y,
                                                  (double)direction.X);

                                    int length = (int)new Vector2(component.bounds.Center.X - comp.bounds.Center.X, component.bounds.Center.Y - comp.bounds.Center.Y).Length();

                                    spriteBatch.Draw(texture,
                                        new Rectangle(((int)component.bounds.Center.X - (int)(component.bounds.Center.X - comp.bounds.Center.X) / 2) - (int)camera.Position.X,
                                        ((int)component.bounds.Center.Y - ((int)component.bounds.Center.Y - comp.bounds.Center.Y) / 2) - (int)camera.Position.Y,
                                        length, 3), null,
                                        Color.Green, rotation, new Vector2(texture.Bounds.Width / 2.0f, texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawInterFace(SpriteBatch spriteBatch, Camera camera, Rectangle viewPort, 
            GameTime gameTime, GraphicsDevice device)
        {
            if (Active == false)
                return;

            editMode.Draw(spriteBatch, new Point(viewPort.X, viewPort.Y));
            blockSelector.Draw(spriteBatch, new Point(viewPort.X, viewPort.Y));
            lightEditor.Draw(spriteBatch, camera, viewPort, gameTime, device);
        }
    }
}
