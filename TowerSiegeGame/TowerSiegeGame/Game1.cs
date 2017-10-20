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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState ms;
        KeyboardState ks;
        BlendState blend;

        SpriteFont font1;
        Texture2D background;

        KryptonEngine krypton;
        private Texture2D mLightTexture;
        ShadowHull hull;

        Texture2D pixel;
        Texture2D rect;

        List<Block> walls = new List<Block>();
        CollisionMap collisionMap;

        List<Object> entities = new List<Object>();
        List<Switch> switches = new List<Switch>();
        List<ElectronicComponent> electronics = new List<ElectronicComponent>();
        List<ElevatorShaft> elevators = new List<ElevatorShaft>();

        Player player;
        List<NPC> NPCs = new List<NPC>();
        List<Floor> Floors = new List<Floor>();

        Camera camera;

        Rectangle windowSize = new Rectangle();

        LevelEditor levelEditor;

        List<int> blockIDs = new List<int>();

        GUI pauseMenu;
        Editor editor;

        bool Paused = false,
             CanPause = true,
             Debugging = true,
             CanDebug = true;

        Random rnd = new Random();

        RenderTarget2D gamerender,
                       DrawOverAmbientLights,
                       firstRender,
                       backGroundRender;

        float gspeed = 1,
              gtime = 0;

        int ts;

        SoundEffect glassBreak;
        List<Sound> sounds = new List<Sound>();

        ParticleSystem rain;

        int canAddParticle = 0,
            particleRepeat = 60;

        Texture2D explosion;

        newParticleSystem testSystem;

        //List<Particle> particles = new List<Particle>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 700;
            Window.AllowUserResizing = true;
            // graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            windowSize = GraphicsDevice.Viewport.Bounds;

            gamerender = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            DrawOverAmbientLights = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            firstRender = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            backGroundRender = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            //Window.AllowUserResizing = true;

            this.krypton = new KryptonEngine(this, "KryptonEffect");
            this.krypton.SpriteBatchCompatablityEnabled = true;
            this.krypton.CullMode = CullMode.CullClockwiseFace;
            this.krypton.AmbientColor = Color.FromNonPremultiplied(22, 22, 30, 255);
            //this.krypton.AmbientColor = Color.DarkRed;

            this.krypton.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.mLightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 512);
            //this.mLightTexture = Content.Load<Texture2D>("Plosion_1");
            //this.mLightTexture = Content.Load<Texture2D>("pixel");
            hull = ShadowHull.CreateRectangle(Vector2.One);

            // TODO: use this.Content to load your game content here
            pixel = Content.Load<Texture2D>("Pixel");
            rect = Content.Load<Texture2D>("rect");
            background = Content.Load<Texture2D>("city");
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            glassBreak =  Content.Load<SoundEffect>("Sounds/glassbreaking");
            explosion = Content.Load<Texture2D>("plosion_1");

            levelEditor = new LevelEditor(krypton, font1, pixel, spriteBatch, mLightTexture,
                GraphicsDevice.Viewport.Bounds, graphics, this, NPCs, elevators, electronics, switches, pixel, pixel);
            collisionMap = new CollisionMap(200, 200, levelEditor.walls);

            player = new Player(new Rectangle(levelEditor.spawnPoint.X, levelEditor.spawnPoint.Y, 
                10, 35), new Rectangle(20, 20, 31, 16),
                    new Rectangle(10, 0, 29, 35), mLightTexture, krypton, false, false, false, Content.Load<Texture2D>("spritesheet"));

            blend = new BlendState();
            blend.ColorBlendFunction = BlendFunction.Add;
            blend.ColorSourceBlend = Blend.DestinationColor;
            blend.ColorDestinationBlend = Blend.One;

            List<Color> raincolors = new List<Color>();
            raincolors.Add(Color.LightBlue);
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(pixel);
            rain = new ParticleSystem(new Rectangle(0, 0, 1, 1), raincolors, true, 0, 3, 3, 20, 15, 60, 60, textures,
                0, 0, false);

            testSystem = new newParticleSystem(rnd,
                0,
                1000,
                30,
                pixel,
                pixel.Bounds,
                Color.White,
                Color.DarkOrange,
                0.1f,
                new Vector2(1400, 1000),
                new Vector2(-50, -50),
                new Vector2(50, 50),
                Vector2.Zero,
                Vector2.Zero,
                new Point(2, 2),
                new Point(4, 4),
                Vector2.Zero,
                Vector2.Zero,
                new Vector2(1.0f, 0),
                new Vector2(3.0f, 0),
                0,
                -0.1f,
                0.1f,
                1,
                1,
                false,
                0.1f);

            List<int> IDs = new List<int>();

            for (int x = 0; x < collisionMap.blocks[1].rows.Count; x++)
            {
                for (int y = 0; y < collisionMap.blocks[1].rows[x].columns.Count; y++)
                {
                    for (int i = 0; i < collisionMap.blocks[1].rows[x].columns[y].colObjects.Count; i++)
                    {
                        if (IDs.Contains(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID) == false)
                        {
                            Floors.Add(new Floor(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].bounds, pixel, rnd, electronics, krypton, switches));
                            IDs.Add(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID);
                        }
                    }
                }
            }

            camera = new Camera(windowSize, collisionMap);

            pauseMenu = new GUI(font1, pixel, windowSize, rnd, true);
            editor = new Editor(font1, pixel, windowSize, pixel, pixel, rnd, mLightTexture, mLightTexture.Bounds);

            pauseMenu.LoadInterface(levelEditor.mapHandling.ComputerName, "PauseMenu", Content);
            //editor.LoadInterface(levelEditor.mapHandling.ComputerName, "Editor", Content);

            //AddNPCs();

            //CreateHulls();
            //SetUpMap();
            //levelEditor.mapHandling.SaveMap(walls, "saaave");
        }

        private void DoParticleStuff()
        {
            if (canAddParticle >= particleRepeat)
                canAddParticle = 0;

            /*if (ms.LeftButton == ButtonState.Pressed && canAddParticle <= -1)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dir = new Vector2(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
                    dir.Normalize();
                    int size = rnd.Next(1, 3);
                    Particle newparticle = new Particle(new Rectangle(levelEditor.mousepos.X - size / 2, levelEditor.mousepos.Y - size / 2, size, size), 0, true, false, false, krypton);
                    newparticle.velocity.X = dir.X * 20.0f;
                    newparticle.velocity.Y = dir.Y * 20.0f;
                    newparticle.particlevelocity = newparticle.velocity;
                    newparticle.Lerp = true;
                    newparticle.lerpAmount = 0.35f;

                    int r = rnd.Next(50, 255);
                    newparticle.color = Color.DarkOrange;
                    newparticle.color.A = (byte)rnd.Next(1, 4);
                    newparticle.Rotation = rnd.Next(0, 3600) / 360;

                    newparticle.texture = Content.Load<Texture2D>("plosion_" + rnd.Next(1, 4).ToString());

                    particles.Add(newparticle);

                    dir = new Vector2(rnd.Next(-1000, 1000), rnd.Next(-1000, 1000));
                    dir.Normalize();
                    size = rnd.Next(1, 3);
                    newparticle = new Particle(new Rectangle(levelEditor.mousepos.X - size / 2, levelEditor.mousepos.Y - size / 2, size, size), 0, true, false, false, krypton);
                    newparticle.velocity.X = dir.X * 15.0f;
                    newparticle.velocity.Y = dir.Y * 15.0f;
                    newparticle.particlevelocity = newparticle.velocity;
                    //newparticle.lifeTime = 80;
                    newparticle.Lerp = true;

                    newparticle.color = Color.Orange;
                    newparticle.color.A = (byte)rnd.Next(1, 4);
                    newparticle.Rotation = rnd.Next(0, 3600) / 360;

                    newparticle.texture = Content.Load<Texture2D>("plosion_" + rnd.Next(1, 4).ToString());

                    particles.Add(newparticle);
                }
            }

            if (ms.LeftButton == ButtonState.Released)
            {
                canAddParticle = 0;
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(collisionMap, camera, true, NPCs, player, entities, false, windowSize, electronics, krypton);

                if (particles[i].Existing == false)
                    particles.RemoveAt(i);
            }*/

            canAddParticle += 1;
        }

        private void AddEntity(Rectangle entitybounds, int amount)
        {
            Random rnd = new Random();

            rain.Update(amount, new Rectangle(windowSize.X - 200, windowSize.Y - 200, windowSize.Width + 400, 1), rnd,
                collisionMap, camera, NPCs, player, entities, windowSize, electronics, krypton);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            windowSize = GraphicsDevice.Viewport.Bounds;

            for (int x = camera.xStart; x < camera.xStart + camera.windowTsX; x++)
            {
                for (int y = camera.yStart; y < camera.yStart + camera.windowTsY; y++)
                {
                    x = (int)MathHelper.Clamp(x, 0, collisionMap.width - 1);
                    y = (int)MathHelper.Clamp(y, 0, collisionMap.height - 1);

                    for (int i = 0; i < collisionMap.blocks[0].rows[x].columns[y].colObjects.Count; i++)
                    {
                        if (collisionMap.blocks[0].rows[x].columns[y].colObjects[i].Existing == false)
                        {
                            levelEditor.RemoveBlock(collisionMap, krypton, true);
                        }
                    }

                    for (int i = 0; i < collisionMap.blocks[1].rows[x].columns[y].colObjects.Count; i++)
                    {
                        if (collisionMap.blocks[1].rows[x].columns[y].colObjects[i].Existing == false)
                        {
                            levelEditor.RemoveBlock(collisionMap, krypton, true);
                        }
                    }

                    for (int i = 0; i < collisionMap.blocks[2].rows[x].columns[y].colObjects.Count; i++)
                    {
                        if (collisionMap.blocks[2].rows[x].columns[y].colObjects[i].Shattered == true)
                        {
                                Point blocksize = new Point(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].bounds.Width,
                                    collisionMap.blocks[2].rows[x].columns[y].colObjects[i].bounds.Height);
                                Point pos = new Point(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].bounds.Center.X,
                                    collisionMap.blocks[2].rows[x].columns[y].colObjects[i].bounds.Center.Y);
                            int shardsize = (blocksize.X / 10) + (blocksize.Y / 10);
                            shardsize = (int)MathHelper.Clamp(shardsize, 2, blocksize.X);
                            shardsize = (int)MathHelper.Clamp(shardsize, 2, blocksize.Y);

                            for (int k = 0; k < shardsize * 2; k++)
                            {
                                int size = rnd.Next(shardsize / 4, shardsize);
                                size = (int)MathHelper.Clamp(size, 2, 5);
                                Object shard = new Object(
                                    new Rectangle(pos.X + rnd.Next(-(blocksize.X / 3), (blocksize.X / 3)), 
                                        pos.Y + rnd.Next(-(blocksize.Y / 3), (blocksize.Y / 3)), size, size), 0.5f, true, false, false, krypton, false);
                                shard.weight = 0;
                                shard.color = collisionMap.blocks[2].rows[x].columns[y].colObjects[i].color;

                                Vector2 shatterdir = new Vector2(shard.bounds.X - shard.ShatterPos.X, shard.bounds.Y - shard.ShatterPos.Y);
                                shatterdir.Normalize();

                                collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.X *= (shatterdir.X);
                                collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.Y *= 1.1f;
                                collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.X =
                                    MathHelper.Clamp(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.X, -20, 20);
                                collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.Y =
                                    MathHelper.Clamp(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity.Y, -20, 20);

                                shard.velocity = shatterdir * collisionMap.blocks[2].rows[x].columns[y].colObjects[i].shardVelocity;
                                entities.Add(shard);
                            }


                            glassBreak.Play();

                            levelEditor.RemoveBlock(collisionMap, krypton, true);
                        }
                    }
                }
            }

            levelEditor.Update(collisionMap, camera, krypton, GraphicsDevice.Viewport.Bounds, GraphicsDevice, 
                graphics, NPCs, elevators, rnd, Floors, electronics, switches);

            if (levelEditor.Typing == true)
                return;

            if (levelEditor.Active == false)
                camera.CenterCamera(new Vector2(player.bounds.Center.X, player.bounds.Center.Y), windowSize);

            camera.Update(collisionMap, windowSize);

            if ((ks.IsKeyDown(Keys.P) || ks.IsKeyDown(Keys.Escape)) && editor.active == false)
            {
                if (CanPause == true)
                {
                    if (Paused == true)
                    {
                        Paused = false;
                    }
                    else
                    {
                        Paused = true;
                    }
                    CanPause = false;
                }
            }
            else {
                CanPause = true;
            }

            if (ks.IsKeyDown(Keys.B))
            {
                if (CanDebug == true)
                {
                    if (Debugging == true)
                    {
                        Debugging = false;
                    }
                    else
                    {
                        Debugging = true;
                    }
                    CanDebug = false;
                }
            }
            else
            {
                CanDebug = true;
            }

            UpdateGUI();

            if (Paused == true)
            {
                return;
            }

            if (ks.IsKeyDown(Keys.R))
            {
                entities.Clear();
                levelEditor.walls = levelEditor.mapHandling.LoadMap("buildingtest", krypton, mLightTexture, NPCs, elevators, electronics, switches, pixel);
                collisionMap.Update(levelEditor.walls, true);

                player.Alive = true;
                player.bounds.X = levelEditor.spawnPoint.X;
                player.bounds.Y = levelEditor.spawnPoint.Y;
            }

            gtime += 1;

            if (gtime == gspeed)
            {
                gtime = 0;
            }
            else {
                return;
            }

            AddEntity(new Rectangle((int)camera.Position.X, (int)camera.Position.Y - 200, 2, 2), 20);

            player.Update(collisionMap, camera, krypton, NPCs, entities, levelEditor.mousepos, windowSize, electronics);

            foreach (NPC npc in NPCs)
            {
                npc.Update(player, collisionMap, camera, rnd, entities, NPCs, Debugging, Floors, elevators, windowSize, electronics, krypton);
            }

            foreach (Floor floor in Floors)
            {
                floor.Update(ks, player, electronics, switches);
            }

            foreach(Switch clicker in switches)
            {
                clicker.Update(ks, player, electronics);
            }

            List<int> DeleteList = new List<int>();
            DeleteList.Clear();
            int o = 0;
            foreach (Object entity in entities.Concat(electronics))
            {
                entity.Update(collisionMap, camera, false, NPCs, player, entities, windowSize, electronics, krypton);

                if (entity.Existing == false)
                {
                    DeleteList.Add(o);
                }
                else {
                    o++;
                }

                if (entity.DeleteNextFrame == true)
                {
                    entity.Existing = false;
                }
            }

            if (DeleteList.Count > 0)
            {
                for (int j = 0; j < DeleteList.Count; j++)
                {
                    entities.RemoveAt(DeleteList[j]);
                }
            }

            foreach (ElevatorShaft elevator in elevators)
            {
                elevator.Update(player, NPCs, ks);
            }

            DoParticleStuff();

            base.Update(gameTime);
        }

        public void UpdateGUI()
        {
            if (pauseMenu.buttons[0].Clicked == true)
            {
                Paused = false;
                pauseMenu.buttons[0].Clicked = false;
            }

            if (pauseMenu.buttons[3].Clicked == true)
            {
                Environment.Exit(0);
            }

            if (pauseMenu.buttons[2].Clicked == true)
            {
                editor.active = true;
            }

            if (editor.buttons[0].Clicked == true)
            {
                editor.buttons[0].Clicked = false;
                editor.active = false;
            }

            if (editor.active == true)
            {
                editor.Update(font1, windowSize);
            }

            if (Paused == true && editor.active == false)
            {
                pauseMenu.Update(font1, windowSize);
                return;
            }

            testSystem.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Create a world view projection matrix to use with krypton
            Matrix world = camera.get_transformation(GraphicsDevice, windowSize);
            Matrix view = Matrix.CreateTranslation(new Vector3(camera.Position.X * camera.Zoom, camera.Position.Y * camera.Zoom, 0) * -1f);
            //Matrix view = camera.get_transformation(GraphicsDevice, windowSize);

            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            this.krypton.Matrix = world * view;
            this.krypton.Bluriness = 0;
            this.krypton.LightMapPrepare(camera);

            spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (editor.active == true)
            {
                spriteBatch.Begin();
                editor.Draw(spriteBatch, pixel, font1, windowSize);
                spriteBatch.End();
                return;
            }

            GraphicsDevice.SetRenderTarget(backGroundRender);
            {
                GraphicsDevice.Clear(Color.Transparent);

                // TODO: Add your drawing code here
                blockIDs.Clear();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,
                null, camera.get_transformation(GraphicsDevice, windowSize));

                spriteBatch.Draw(background, new Rectangle((int)(-camera.Position.X * 0.3f), (int)(-camera.Position.Y * 0.3f + 100), background.Width * 7, background.Height * 7), Color.FromNonPremultiplied(60, 60, 60, 255));
                spriteBatch.Draw(background, new Rectangle((int)(-camera.Position.X * 0.5f), (int)(-camera.Position.Y * 0.5f), background.Width * 8, background.Height * 8), background.Bounds, Color.FromNonPremultiplied(160, 160, 160, 255), 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(background, new Rectangle((int)(-camera.Position.X * 0.7f), (int)(-camera.Position.Y * 0.7f), background.Width * 9, background.Height * 9 - 100), background.Bounds, Color.FromNonPremultiplied(255, 255, 255, 255), 0, Vector2.Zero, SpriteEffects.None, 0);

            }

            GraphicsDevice.SetRenderTarget(firstRender);
            {
                GraphicsDevice.Clear(Color.Transparent);

                for (int x = camera.xStart; x < camera.xStart + camera.windowTsX; x++)
                {
                    for (int y = camera.yStart; y < camera.yStart + camera.windowTsY; y++)
                    {
                        x = (int)MathHelper.Clamp(x, 0, collisionMap.width - 1);
                        y = (int)MathHelper.Clamp(y, 0, collisionMap.height - 1);

                        for (int i = 0; i < collisionMap.blocks[1].rows[x].columns[y].colObjects.Count; i++)
                        {
                            if (blockIDs.Contains(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID) == false)
                            {
                                collisionMap.blocks[1].rows[x].columns[y].colObjects[i].Draw(spriteBatch, pixel, camera, Debugging);
                                blockIDs.Add(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID);
                            }
                        }

                        if (Debugging == true)
                        {
                            if (collisionMap.blocks[0].rows[x].columns[y].colObjects.Count == 0)
                            {
                                spriteBatch.Draw(rect, new Rectangle(x * collisionMap.TileSize - (int)camera.Position.X,
                                    y * collisionMap.TileSize - (int)camera.Position.Y, collisionMap.TileSize, collisionMap.TileSize), Color.Green);
                            }
                            else
                            {
                                spriteBatch.Draw(rect, new Rectangle(x * collisionMap.TileSize - (int)camera.Position.X,
                                    y * collisionMap.TileSize - (int)camera.Position.Y, collisionMap.TileSize, collisionMap.TileSize), Color.Red);
                            }
                        }
                    }
                }
            }

            GraphicsDevice.SetRenderTarget(DrawOverAmbientLights);
            {
                GraphicsDevice.Clear(Color.Transparent);

                foreach (Floor floor in Floors)
                {
                    //floor.lightSwitch.Draw(spriteBatch, camera, font1, player);
                }
                foreach (Switch clicker in switches)
                {
                    clicker.Draw(spriteBatch, camera, font1, player);
                }

                foreach (Object entity in entities.Concat(electronics))
                {
                    if (entity.bounds.Intersects(camera.Size))
                    {
                        if (entity.trail.Count > 0)
                        {
                            Rectangle rect = entity.trail[0];

                            Vector2 direction = new Vector2(rect.Center.X, rect.Center.Y) - new Vector2(entity.bounds.Center.X, entity.bounds.Center.Y);
                            direction.Normalize();

                            float rotation = (float)Math.Atan2(
                                          (double)direction.Y,
                                          (double)direction.X);

                            int length = (int)new Vector2(entity.bounds.Center.X - rect.Center.X, entity.bounds.Center.Y - rect.Center.Y).Length();

                            if (entity.velocity.Y > entity.velocity.X)
                            {
                                spriteBatch.Draw(entity.texture,
                                            new Rectangle(((int)entity.bounds.Center.X - (int)(entity.bounds.Center.X - rect.Center.X) / 2) - (int)camera.Position.X,
                                           ((int)entity.bounds.Center.Y - ((int)entity.bounds.Center.Y - rect.Center.Y) / 2) - (int)camera.Position.Y,
                                           length, rect.Height), null,
                                           entity.color, rotation, new Vector2(entity.texture.Bounds.Width / 2.0f, entity.texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
                            }
                            else
                            {
                                spriteBatch.Draw(entity.texture,
                                            new Rectangle(((int)entity.bounds.Center.X - (int)(entity.bounds.Center.X - rect.Center.X) / 2) - (int)camera.Position.X,
                                           ((int)entity.bounds.Center.Y - ((int)entity.bounds.Center.Y - rect.Center.Y) / 2) - (int)camera.Position.Y,
                                           rect.Width, length), null,
                                           entity.color, rotation, new Vector2(entity.texture.Bounds.Width / 2.0f, entity.texture.Bounds.Height / 2.0f), SpriteEffects.None, 0);
                            }
                        }

                        spriteBatch.Draw(pixel, new Rectangle(entity.bounds.X - (int)camera.Position.X,
                            entity.bounds.Y - (int)camera.Position.Y, entity.bounds.Width,
                            entity.bounds.Height), entity.color);
                    }
                }

                bool drawnpcs = true;
                foreach (ElevatorShaft elevator in elevators)
                {
                    elevator.Draw(spriteBatch, camera, font1);
                    elevator.elevator.Draw(spriteBatch, camera, font1);

                    if (elevator.elevator.inUseByPlayer == true && elevator.elevator.velocity.Y != 0)
                    {
                        //drawnpcs = false;
                    }
                }

                if (drawnpcs == true)
                {
                    foreach (NPC npc in NPCs)
                    {
                        if (camera.Size.Intersects(npc.bounds))
                        {
                            npc.Draw(spriteBatch, pixel, camera, krypton, Debugging, font1);
                        }
                    }
                }

                player.Draw(spriteBatch, camera, Debugging, this.rect);

                rain.Draw(spriteBatch, camera);

                testSystem.Draw(spriteBatch, camera);

                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(gamerender);
            {
                GraphicsDevice.Clear(Color.White);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                spriteBatch.Draw((Texture2D)backGroundRender, backGroundRender.Bounds, Color.White);
                spriteBatch.Draw((Texture2D)firstRender, firstRender.Bounds, Color.White);
                spriteBatch.Draw((Texture2D)DrawOverAmbientLights, DrawOverAmbientLights.Bounds, Color.White);

                spriteBatch.End();

                this.krypton.Draw(gameTime);

                spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp, null, null,
                null, camera.get_transformation(GraphicsDevice, windowSize));
                {
                    foreach (Floor floor in Floors)
                    {
                        if (floor.lightsOn == true)
                        {
                            for (int i = 0; i < floor.Intensity; i++)
                            {
                                spriteBatch.Draw(pixel, new Rectangle(floor.bounds.X - (int)camera.Position.X,
                                    floor.bounds.Y - (int)camera.Position.Y, floor.bounds.Width, floor.bounds.Height),
                                    floor.lightColor);
                            }
                        }
                    }
                }
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,
                null, camera.get_transformation(GraphicsDevice, windowSize));

                blockIDs.Clear();
                for (int x = camera.xStart; x < camera.xStart + camera.windowTsX; x++)
                {
                    for (int y = camera.yStart; y < camera.yStart + camera.windowTsY; y++)
                    {
                        x = (int)MathHelper.Clamp(x, 0, collisionMap.width - 1);
                        y = (int)MathHelper.Clamp(y, 0, collisionMap.height - 1);

                        for (int i = 0; i < collisionMap.blocks[0].rows[x].columns[y].colObjects.Count; i++)
                        {
                            if (blockIDs.Contains(collisionMap.blocks[0].rows[x].columns[y].colObjects[i].blockID) == false)
                            {
                                collisionMap.blocks[0].rows[x].columns[y].colObjects[i].Draw(spriteBatch, pixel, camera, Debugging);
                                blockIDs.Add(collisionMap.blocks[0].rows[x].columns[y].colObjects[i].blockID);
                            }
                        }

                        for (int i = 0; i < collisionMap.blocks[2].rows[x].columns[y].colObjects.Count; i++)
                        {
                            if (blockIDs.Contains(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].blockID) == false)
                            {
                                collisionMap.blocks[2].rows[x].columns[y].colObjects[i].Draw(spriteBatch, pixel, camera, Debugging);
                                blockIDs.Add(collisionMap.blocks[2].rows[x].columns[y].colObjects[i].blockID);
                            }
                        }

                        for (int i = 0; i < collisionMap.blocks[1].rows[x].columns[y].colObjects.Count; i++)
                        {
                            if (blockIDs.Contains(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID) == false &&
                                collisionMap.blocks[1].rows[x].columns[y].colObjects[i].bounds.Intersects(player.bounds) == false)
                            {
                                //collisionMap.blocks[1].rows[x].columns[y].colObjects[i].Draw(spriteBatch, pixel, camera);
                                //blockIDs.Add(collisionMap.blocks[1].rows[x].columns[y].colObjects[i].blockID);
                            }
                        }
                    }
                }

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, null, null,
                null, camera.get_transformation(GraphicsDevice, windowSize));
                /*foreach (Particle particle in particles)
                {
                    spriteBatch.Draw(particle.texture, new Rectangle(
                        particle.bounds.Center.X - (int)camera.Position.X - 0,
                        particle.bounds.Center.Y - (int)camera.Position.Y - 0,
                        particle.bounds.Width + 20, particle.bounds.Height + 20), particle.texture.Bounds, particle.color, particle.Rotation,
                        new Vector2(particle.texture.Bounds.Center.X, particle.texture.Bounds.Center.Y), SpriteEffects.None, 0);

                    spriteBatch.Draw(mLightTexture, new Rectangle((int)(particle.bounds.X - camera.Position.X),
    (int)(particle.bounds.Y - camera.Position.Y),
    (int)(particle.bounds.Width * 1.3f), (int)(particle.bounds.Height * 1.3f)), particle.color * 0.6f);
                }*/

                foreach (Light2D light in krypton.Lights)
                {
                    if (light.rectBounds.Intersects(camera.Size))
                    {
                        if (light.Fov >= 6.19f)
                        {
                            spriteBatch.Draw(mLightTexture, new Rectangle((int)(light.X - camera.Position.X - light.Range / 2),
                                (int)(light.Y - camera.Position.Y - light.Range / 2),
                                (int)light.Range, (int)light.Range), light.Color * 0.6f);

                            /*spriteBatch.Draw(mLightTexture, new Rectangle((int)(light.X - camera.Position.X - light.Range / 4),
                                (int)(light.Y - camera.Position.Y - light.Range / 4),
                                (int)light.Range / 2, (int)light.Range / 2), light.Color * 0.2f);*/
                        }
                    }
                }
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,
                null, camera.get_transformation(GraphicsDevice, windowSize));

                levelEditor.Draw(spriteBatch, camera, krypton, NPCs, electronics, switches);

                if (Debugging == true)
                {
                    foreach (Rectangle rect in player.lightlevelvis)
                    {
                        spriteBatch.Draw(pixel, new Rectangle(rect.X - (int)camera.Position.X, rect.Y - (int)camera.Position.Y,
                            rect.Width, rect.Height), Color.White);
                    }
                }

                spriteBatch.End();
            }
            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            spriteBatch.Draw((Texture2D)gamerender, new Rectangle(
                (int)(windowSize.X),
                (int)(windowSize.Y),
                (int)(windowSize.Width), 
                (int)(windowSize.Height)), Color.White);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                              RasterizerState.CullCounterClockwise);
            levelEditor.DrawInterFace(spriteBatch, camera, windowSize,
                gameTime, GraphicsDevice);

            spriteBatch.DrawString(font1, (camera.Position.X / ts).ToString(), new Vector2(0, 0), Color.Wheat);

            if (Paused == true)
            {
                pauseMenu.Draw(spriteBatch, pixel, font1, windowSize);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
