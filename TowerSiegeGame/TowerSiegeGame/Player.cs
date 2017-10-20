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
    public class Player : Object
    {
        KeyboardState ks;
        MouseState ms;

        public Rectangle
                  drawBounds,
                  standBounds,
                  slideBounds,
                  getshotbounds;

        Keys jump = Keys.Space,
             left = Keys.A,
             right = Keys.D,
             down = Keys.S,
             up = Keys.W,
             sprint = Keys.LeftShift;

        SpriteEffects spriteEffect = new SpriteEffects();
        Animation runAnimation,
                  idleAnimation,
                  jumpAnimation,
                  fallAnimation,
                  slideAnimation,
                  
                  currentAnimation;

        bool switchAnimation = false,
             lastSwitchAnimation = false;

        int speed = 3,
            walkingspeed = 1,
            runningspeed = 3,
            sprintboost = 3,
            slideSpeed = 2,
            
            animFrame = 0;

        public Timer jumpTimer = new Timer((0.01f * 60f) * 0.3f),
              walljumpTimer = new Timer((0.01f * 60f) * 0.1f),
              slideTimer = new Timer((0.01f * 60f) * 0.5f),
              canShootTimer = new Timer((0.01f * 60f) * 1.0f);

        public bool canJump = false,
             canWallJump = true,
             canSprint = true,
             canSlide = true,
             canMove = true,

             UsingElevator = false,

             Climbing = false,
             
             ableToSlide = true;

        public bool Alive = true;

        public float angleX = 1;
        public float angleY = 0;
        public float lightLevel = 0;

        public List<Rectangle> lightlevelvis = new List<Rectangle>();

        public Light2D fovVisualization;
        public float FOV = 180.0f;

        public Player(Rectangle bounds, Rectangle slideBounds, Rectangle drawBounds,
            Texture2D mLightTexture, KryptonEngine krypton, bool CheckNPCs, bool CheckPlayer, bool castShadows, Texture2D texture)
            : base(bounds, 0.0f, true, CheckNPCs, CheckPlayer, krypton, castShadows)
        {
            friction = 0.1f;
            this.drawBounds = drawBounds;
            this.standBounds = bounds;
            this.slideBounds = slideBounds;
            getshotbounds = new Rectangle();

            this.texture = texture;

            collideInEntities = true;

            runningspeed = speed;
            walkingspeed = (int)(runningspeed * 0.0f);
            removalStop = 1;

            colCheckXStart = -1;
            colCheckXEnd = 2;
            colCheckYStart = -1;
            colCheckYEnd = 2;

            fovVisualization = new Light2D()
            {
                Texture = mLightTexture,
                Range = (float)(3000),
                Color = new Color(10, 10, 0),
                Intensity = 1.0f,
                Angle = 0.5f,
                X = (bounds.Center.X),
                Y = (bounds.Center.Y),
            };

            fovVisualization.Fov = (float)((Math.PI + 0.3f) * ((FOV/2.0f) / 100.0f));

            //krypton.mMovingLights.Add(fovVisualization);
            hull = ShadowHull.CreateRectangle(Vector2.One);
                    hull.Position.X = bounds.Center.X;
                    hull.Position.Y = bounds.Center.Y;
                    hull.Scale.X = bounds.Width;
                    hull.Scale.Y = bounds.Height;
                    //krypton.Hulls.Add(hull);

            runAnimation = new Animation(texture, 10, 0.1f, new Rectangle(0, 0, 29, 33), drawBounds, "run");
            currentAnimation = new Animation(texture, 10, 0.6f, new Rectangle(0, 0, 29, 33), drawBounds, "");
            idleAnimation = new Animation(texture, 2, 0.3f, new Rectangle(0, 33, 29, 33), drawBounds, "idle");
            jumpAnimation = new Animation(texture, 1, 0.3f, new Rectangle(0, 33 * 2, 29, 33), drawBounds, "jump");
            fallAnimation = new Animation(texture, 2, 0.2f, new Rectangle(0, 33 * 3, 29, 33), drawBounds, "fall");
            slideAnimation = new Animation(texture, 1, 0.3f, new Rectangle(0, (33 * 4), 29, 12), 
                new Rectangle(0, 0, slideBounds.Width, slideBounds.Height), "slide");
        }

        public void Update(CollisionMap collisionMap, Camera camera, KryptonEngine krypton, List<NPC> NPCs, List<Object> Entities, Rectangle mousepos, Rectangle window,
            List<ElectronicComponent> electronics)
        {
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            animFrame += 1;
            if (animFrame == 100)
                animFrame = 0;

            getshotbounds.X = bounds.X - 2;
            getshotbounds.Y = bounds.Y - 2;
            getshotbounds.Height = bounds.Height + 4;
            getshotbounds.Width = bounds.Width + 4;

            if (Alive == false)
            {
                bounds.Width = slideBounds.Width;
                bounds.Height = slideBounds.Height;
                velocity.X = MathHelper.Lerp(velocity.X, 0, friction * 0.3f);

                jumpTimer.DeActivate();
                walljumpTimer.DeActivate();
                slideTimer.DeActivate();
                canShootTimer.DeActivate();

                canJump = true;
                canWallJump = false;
                canSprint = false;
                canSlide = false;
                canMove = false;

                if (velocity.X > -0.4f && velocity.X < 0.4f)
                    velocity.X = 0;

                ResetColDir();
            }

            base.Update(collisionMap, camera, true, NPCs, this, Entities, window, electronics, krypton);
            jumpTimer.Update();
            walljumpTimer.Update();
            canShootTimer.Update();

            Climbing = false;

            if (Alive == false)
                return;

            if (ms.RightButton == ButtonState.Pressed)
            {
                if (colYDown == true)
                {
                    speed = walkingspeed;
                    canSprint = false;
                }

                if (ms.LeftButton == ButtonState.Pressed)
                {
                    if (canShootTimer.active == false)
                    {
                        Vector2 bulletdir = new Vector2(mousepos.X - bounds.Center.X, mousepos.Y - bounds.Center.Y);
                        bulletdir.Normalize();

                        Object bullet = new Object(new Rectangle(bounds.Center.X - 2, bounds.Center.Y - 2, 4, 4), 0, false, true, false, krypton, false);
                        bullet.gravity = 0;
                        bullet.velocity = bulletdir * 70f;
                        bullet.color = Color.Yellow;
                        bullet.deadly = true;
                        bullet.destroyOnTouch = true;
                        bullet.collideInEntities = true;
                        Entities.Add(bullet);

                        canShootTimer.Activate();
                    }
                }
            }
            else
            {
                speed = runningspeed;
            }

            if (colYDown == false && (colXLeft == true || colXRight == true || colYUp == true))
            {
                //Climbing = true;
            }

            if (Climbing == true && canSlide == true)
            {
                velocity.Y = 0;
                canSprint = false;

                if (ks.IsKeyDown(up) == true && colYUp == false)
                {
                    velocity.Y = -speed * 0.7f;
                }

                if (ks.IsKeyDown(down) == true)
                {
                    velocity.Y = speed * 0.8f;
                }
            }

            if ((ks.IsKeyDown(left) || ks.IsKeyDown(right)))
            {
                if (canMove == true)
                {
                    if (ks.IsKeyDown(left) && colXLeft == false && leftCol == false && walljumpTimer.stopped == true &&
                        canMove == true)
                    {
                        angleX = -1;

                        if (ks.IsKeyDown(sprint) && canSprint == true)
                        {
                            velocity.X = -(speed + sprintboost);
                        }
                        else
                        {
                            velocity.X = MathHelper.Clamp(velocity.X - 1, -speed, 0);
                        }
                    }

                    if (ks.IsKeyDown(right) && colXRight == false && rightCol == false && walljumpTimer.stopped == true &&
                        canMove == true)
                    {
                        angleX = 1;

                        if (ks.IsKeyDown(sprint) && canSprint == true)
                        {
                            velocity.X = speed + sprintboost;
                        }
                        else
                        {
                            velocity.X = MathHelper.Clamp(velocity.X + 1, 0, speed);
                        }
                    }
                }
            }
            if(!((ks.IsKeyDown(left) || ks.IsKeyDown(right))) || (colYDown == true && bounds.Width == slideBounds.Width && (velocity.X > slideSpeed || velocity.X < -slideSpeed)))
            {
                if (colYDown == true || colYUp == true)
                {
                    if (canMove == true)
                    {
                        velocity.X = MathHelper.Lerp(velocity.X, 0, friction);
                    }
                    else
                    {
                        velocity.X = MathHelper.Lerp(velocity.X, 0, friction * 0.5f);
                    }

                    if (velocity.X > -0.4f && velocity.X < 0.4f)
                        velocity.X = 0;
                }

                if (colXLeft == true || colXRight == true)
                {
                    velocity.X = 0;
                }
            }

            if (ks.IsKeyDown(jump))
            {
                if (jumpTimer.active == true)
                {
                    velocity.Y = -speed;
                }

                if (canJump == true)
                {
                    jumpTimer.Activate();
                    canJump = false;
                    canWallJump = false;
                }

                if (canJump == false && canWallJump == true)
                {
                    walljumpTimer.Activate();
                    canWallJump = false;

                    if (walljumpTimer.active == true)
                    {
                        if (colXLeft == true || leftCol == true)
                        {
                            velocity.X = MathHelper.Clamp(velocity.X + speed, 0, speed);
                            velocity.Y = -10;
                        }

                        if (colXRight == true || rightCol == true)
                        {
                            velocity.X = MathHelper.Clamp(velocity.X - speed, -speed, 0);
                            velocity.Y = -10;
                        }
                    }
                    else
                    {
                        canWallJump = true;
                    }
                }
            }
            else
            {
                jumpTimer.active = false;
                jumpTimer.stopped = true;
                walljumpTimer.active = false;
                walljumpTimer.stopped = true;

                if (colYDown == true)
                    canJump = true;
                else
                    canJump = false;

                canWallJump = true;
            }

            if (ks.IsKeyDown(down) == true && ableToSlide == true)
            {
                if (velocity.X > speed || velocity.X < -speed)
                    canMove = false;
                else
                    canMove = true;

                if (BasicCollisionDetection(new Rectangle(bounds.X, bounds.Y + (standBounds.Height - slideBounds.Height), slideBounds.Width, slideBounds.Height), collisionMap) == false)
                {
                    if (Climbing == false)
                    {
                        if (colYDown == true || jumpTimer.active == true || canSlide == false)
                        {
                            if (canSlide == true)
                            {
                                slideTimer.Activate();
                                canSlide = false;

                                if (ks.IsKeyDown(sprint))
                                {
                                    velocity.X += 2 * angleX;
                                }
                            }

                            //bounds.Y += bounds.Height - slideBounds.Height;

                            bounds.Width = slideBounds.Width;
                            bounds.Height = slideBounds.Height;

                            canJump = false;
                            canWallJump = false;
                            canSprint = false;
                            speed = slideSpeed;
                        }
                    }
                }
            }
            else
            {
                if (BasicCollisionDetection(new Rectangle(bounds.X, bounds.Y - (standBounds.Height - slideBounds.Height), standBounds.Width, standBounds.Height), collisionMap) == false)
                {
                    bounds.Width = standBounds.Width;
                    bounds.Height = standBounds.Height;

                    if (canSlide == false)
                        bounds.Y -= (standBounds.Height - slideBounds.Height);

                    speed = runningspeed;
                    canSprint = true;
                    canSlide = true;
                    canMove = true;
                }
            }

            if (slideTimer.active == true)
                slideTimer.Update();

            int ts = collisionMap.TileSize;

            HandleAnimation();

            ResetColDir();

            fovVisualization.X = bounds.Center.X;
            fovVisualization.Y = bounds.Center.Y;
            fovVisualization.Angle = ((float)((Math.Atan2((bounds.Center.Y) - bounds.Center.Y, (bounds.Center.X + angleX) - bounds.Center.X))));

            lightLevel = getLightLevel(collisionMap, bounds, krypton, electronics);

            foreach (NPC npc in NPCs)
            {
                if (npc.sightbounds.Intersects(this.bounds) && npc.fovVisualization.Color != Color.Red)
                {
                    if (CheckIfInSight(bounds.Center.X, bounds.Center.Y, npc.bounds.Center.X, npc.bounds.Center.Y, collisionMap, npc.bounds, electronics))
                    {
                        if (ms.RightButton == ButtonState.Pressed)
                        {
                            npc.Alive = false;
                        }
                    }
                }

                if (npc.Alive == false)
                {
                    ks = Keyboard.GetState();

                    if (npc.bounds.Intersects(colDirRect) && ks.IsKeyDown(Keys.F))
                    {
                        npc.velocity.X = velocity.X + angleX;
                    }
                }
            }

            foreach (Object bullet in Entities)
            {
                if (bullet.bounds.Intersects(getshotbounds) && bullet.CheckPlayer == true && bullet.deadly == true)
                {
                    //Alive = false;
                    bullet.Existing = false;
                    Climbing = false;

                    velocity += bullet.velocity * 0.1f;
                }
            }

            ableToSlide = true;

            bounds.X = (int)MathHelper.Clamp(bounds.X, 0, ((collisionMap.width - 1) * collisionMap.TileSize) - bounds.Width);
            bounds.Y = (int)MathHelper.Clamp(bounds.Y, 0, ((collisionMap.height - 1) * collisionMap.TileSize) - bounds.Height);

            hull.Position.X = bounds.Center.X;
            hull.Position.Y = bounds.Center.Y;
            hull.Scale.X = bounds.Width;
            hull.Scale.Y = bounds.Height;
        }

        public void HandleAnimation()
        {
            currentAnimation = idleAnimation;

            if (velocity.X < -2.0f)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;

                if (currentAnimation.name != runAnimation.name)
                {
                    currentAnimation = runAnimation;
                }
            }
            else if (velocity.X > 2.0f)
            {
                spriteEffect = SpriteEffects.None;
                if (currentAnimation.name != runAnimation.name)
                {
                    currentAnimation = runAnimation;
                }
            }

            if (canSlide == false)
            {
                if (currentAnimation.name != slideAnimation.name)
                {
                    currentAnimation = slideAnimation;
                }
            }

            if (colYDown == false)
            {
                if (velocity.Y < 0)
                {
                    if (currentAnimation.name != jumpAnimation.name)
                    {
                        currentAnimation = jumpAnimation;
                    }
                }
                else if (velocity.Y >= 0)
                {
                    if (currentAnimation.name != fallAnimation.name)
                    {
                        currentAnimation = fallAnimation;
                    }
                }
            }

            drawBounds.X = currentAnimation.dRect.X;
            drawBounds.Y = currentAnimation.dRect.Y;
            drawBounds.Width = currentAnimation.dRect.Width;
            drawBounds.Height = currentAnimation.dRect.Height;

            if (spriteEffect == SpriteEffects.None)
            {
                //drawBounds.X = -drawBounds.X;
            }

            currentAnimation.Animate();
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, bool Debug, Texture2D debugTexture)
        {
            if (Debug == true)
            {
                spriteBatch.Draw(debugTexture, new Rectangle(bounds.X - (int)camera.Position.X, bounds.Y - (int)camera.Position.Y,
                    bounds.Width, bounds.Height), Color.Black);
            }

            spriteBatch.Draw(currentAnimation.texture, new Rectangle(bounds.Center.X - (int)camera.Position.X - (drawBounds.Width / 2), bounds.Center.Y - (int)camera.Position.Y - (drawBounds.Height / 2),
                drawBounds.Width, drawBounds.Height), currentAnimation.sRect, 
                Color.White, 0, new Vector2(0, 0), spriteEffect, 0);
        }

        public void DrawFOV(KryptonEngine krypton)        
        {
            krypton.mMovingLights.Add(fovVisualization);
        }

        public void UpdateLineOfSight(int x0, int y0, int x1, int y1, CollisionMap collisionMap, List<ElectronicComponent> electronics)
        {
            bool insight;

            List<Rectangle> sight = Shapes.lineOfSight(x0, y0, x1,
             y1, 1, 5000, new Point(1, 1), collisionMap, angleX, angleY, FOV, out insight, bounds, electronics);
        }

        public bool CheckIfInSight(int x0, int y0, int x1, int y1, CollisionMap collisionMap, Rectangle entity, List<ElectronicComponent> electronics)
        {
            bool insight;

            List<Rectangle> sight = Shapes.lineOfSight(x0, y0, x1,
             y1, 1, (int)100, new Point(1, 1), collisionMap, angleX, angleY, FOV, out insight, entity, electronics);

            return insight;
        }

        public float getLightLevel(CollisionMap collisionMap, Rectangle entity,
            KryptonEngine krypton, List<ElectronicComponent> electronics)
        {
            bool insight = false;
            float lightlevel = krypton.AmbientColor.R * 0.01f;
            lightlevelvis.Clear();
            Vector2 lightToBounds;

            foreach (Light2D light in krypton.Lights)
            {
                lightToBounds = new Vector2(bounds.Center.Y - light.Y, bounds.Center.X - light.X);
                lightToBounds.Normalize();
                Random rnd = new Random();

                light.rayEndX = (int)MathHelper.Clamp(light.rayEndX, bounds.X + velocity.X + 1, bounds.Right + velocity.X - 1);
                light.rayEndY = (int)MathHelper.Clamp(light.rayEndY, bounds.Y + velocity.Y + 1, bounds.Bottom + velocity.Y - 1);

                List<Rectangle> sight = (Shapes.lightLevel((int)light.X, (int)light.Y, light.rayEndX,
                 light.rayEndY, 3, (int)(light.Range * light.Intensity) / 3, new Point(1, 1), collisionMap, 
                 MathHelper.ToDegrees((float)(Math.PI + (Math.Atan2(lightToBounds.X, lightToBounds.Y)))), MathHelper.ToDegrees((float)(Math.PI + light.Angle)), 
                 light.FOV, out insight, entity, electronics));
                lightlevelvis.AddRange(sight);

                if (insight == true)
                {
                    lightlevel += 1 * light.Intensity;
                }
                else
                {
                    light.rayEndX = bounds.Center.X + rnd.Next(-bounds.Width / 2 + 1, bounds.Width / 2 - 1);
                    light.rayEndY = bounds.Center.Y + rnd.Next(-bounds.Height / 2 + 1, bounds.Height / 2 - 1);
                }
            }

            return lightlevel;
        }
    }
}
