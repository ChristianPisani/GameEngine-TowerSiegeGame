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
    public class NPC : Object
    {
        int walkingspeed = 2,
            originalSpeed,
            runningspeed = 5;

        public bool Selected = false,
                    Alive = true;

        public string State = "walking";

        public int movedir = 1;

        public Vector2 angle = new Vector2(-1, 0),
                       newangle = new Vector2(0,0);

        public Light2D fovVisualization;
        public float FOV,
              awareness;
        List<Rectangle> lineOfSight = new List<Rectangle>();

        Timer coolDownTimer = new Timer((0.01f * 60f) * 15.0f),
              changeDirTimer = new Timer((0.01f * 60f) * 2.5f),
              updateLineOfSightTimer = new Timer((0.01f * 60f) * 0.05f),
              canShootTimer = new Timer((0.01f * 60f) * 1.0f);

        public Rectangle lastSpottedLocation,
                  realbounds,
                  sightbounds;

        public ElevatorStop elevatorLocation = new ElevatorStop(Rectangle.Empty, Rectangle.Empty);

        List<Point> SightPoints = new List<Point>();

        public NPC(Rectangle bounds, Rectangle npcbounds, Texture2D mLightTexture, float FOV, Color color, Random rnd, bool CheckNPCs, bool CheckPlayer, KryptonEngine krypton,
            bool castShadows)
            : base(bounds, 0.0f, true, CheckNPCs, CheckPlayer, krypton, castShadows)
        {
            this.bounds = npcbounds;
            this.sightbounds = npcbounds;
            this.FOV = FOV;
            this.color = color;

            collideInEntities = true;

            for (int i = 0; i < 3; i++)
            {
                SightPoints.Add(new Point());
            }

            realbounds = npcbounds;
            sightbounds = new Rectangle(bounds.X - 30, bounds.Y, bounds.Width + 60, bounds.Height);
            edgeCheck = new Rectangle(bounds.Center.X, bounds.Bottom, 10, 2);

            updateLineOfSightTimer.looping = true;

            changeDirTimer.Length = rnd.Next(100, 251) / 100.0f;

            friction = 1.0f;

            originalSpeed = walkingspeed;
            removalStop = 1;

            colCheckXStart = -1;
            colCheckXEnd = 2;
            colCheckYStart = -1;
            colCheckYEnd = 2;

            fovVisualization = new Light2D()
            {
                Texture = mLightTexture,
                Range = (float)(600),
                Color = new Color(200, 200, 0),
                Intensity = 0.5f,
                Angle = 0.5f,
                X = (bounds.Center.X),
                Y = (bounds.Center.Y),
                Fov = (float)((Math.PI + 0.3f) * ((FOV / 2.0f) / 100.0f)),
            };

            coolDownTimer.DeActivate();
        }

        public void Update(Player player, CollisionMap collisionMap, Camera camera, Random rnd, List<Object> entities, List<NPC> NPCs, bool DrawLineOfSight, 
            List<Floor> floors, List<ElevatorShaft> elevators, Rectangle window, List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            sightbounds.X = bounds.Center.X - sightbounds.Width / 2;
            sightbounds.Y = bounds.Y;
            edgeCheck.X = bounds.Center.X + (movedir * edgeCheck.Width) - (edgeCheck.Width / 2) + ((bounds.Width / 2) * movedir);
            edgeCheck.Y = bounds.Bottom;

            base.Update(collisionMap, camera, false, NPCs, player, entities, window, electronics, krypton);

            foreach (Object bullet in entities)
            {
                if (bullet.bounds.Intersects(new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4)) && bullet.CheckNPCs == true && bullet.deadly == true)
                {
                    Alive = false;
                    bullet.Existing = false;

                    velocity += bullet.velocity * 0.1f;
                }
            }

            if (Alive == false)
            {
                fovVisualization.IsOn = false;
                bounds.Width = realbounds.Height;
                bounds.Height = realbounds.Width;
                State = "Dead";

                return;
            }

            updateLineOfSightTimer.Update();
            canShootTimer.Update();

            awareness = 100;

            MouseState ms = Mouse.GetState();

            if(State != "lookingforplayer")
                angle = Vector2.Lerp(angle, new Vector2(movedir, 0), 0.03f);

            if (coolDownTimer.active == true && coolDownTimer.Time < 0.7f && (State.ToLower() == "chasing" || State == "lookingforplayer"))
            {
                lastSpottedLocation = player.bounds;
            }

            switch (State.ToLower())
            {
                case "walking":
                    velocity.X = walkingspeed * movedir;
                    break;

                case "running":
                    velocity.X = runningspeed * movedir;
                    break;

                case "chasing":
                    Vector2 angles = new Vector2((lastSpottedLocation.Center.X) - bounds.Center.X,
                                                 lastSpottedLocation.Center.Y - bounds.Center.Y);
                    angles.Normalize();

                    angle.X = (angles.X);
                    angle.Y = (angles.Y);

                    awareness = 200;

                    LookRandomly(rnd);

                    if (sightbounds.Intersects(lastSpottedLocation) == false)
                    {
                        velocity.X = runningspeed * angle.X;
                    }
                    else
                    {
                        velocity.X = 0;
                    }
                    break;

                case "lookingforplayer":
                    angles = new Vector2((lastSpottedLocation.Center.X) - bounds.Center.X,
                                          lastSpottedLocation.Center.Y - bounds.Center.Y);
                    angles.Normalize();

                    angle.X = (movedir);
                    LookRandomly(rnd);

                    velocity.X = walkingspeed * angle.X;
                    break;

                case "callingelevator":
                    velocity.X = 0;
                    coolDownTimer.Activate();
                    coolDownTimer.Time = 0;

                    foreach (ElevatorShaft elevator in elevators)
                    {
                        if(bounds.Intersects(elevator.elevator.bounds) && elevator.elevator.velocity.Y == 0 && elevator.elevator.inUseByNPC == false && 
                            elevator.elevator.inUseByPlayer == false)
                        {
                            State = "ridingElevator";
                            elevator.elevator.inUseByNPC = true;
                        }
                    }
                    break;

                case "goingtoelevator":
                    if (bounds.Intersects(elevatorLocation.realbounds) == false)
                    {
                        angles = new Vector2((elevatorLocation.realbounds.Center.X) - bounds.Center.X,
                                              elevatorLocation.realbounds.Center.Y - bounds.Center.Y);
                        angles.Normalize();

                        if (angles.X < 0)
                        {
                            velocity.X = -runningspeed;
                        }
                        else if (angles.X > 0)
                        {
                            velocity.X = runningspeed;
                        }
                        else
                        {
                            velocity.X = 0;
                        }
                    }
                    else
                    {
                        velocity.X = 0;

                        coolDownTimer.Activate();
                        State = "callingElevator";
                    }
                    break;

                case "searchingforelevator":
                    int proximity = 0;
                    int lastproximity = proximity;
                    coolDownTimer.Activate();
                    foreach (ElevatorShaft elevator in elevators)
                    {
                        lastproximity = proximity;

                        if (elevator.elevator.inUseByNPC == false || elevator.elevator.inUseByPlayer == false)
                        {
                            proximity = elevator.elevator.bounds.X - bounds.X;

                            if (proximity < 0)
                                proximity *= -1;

                            foreach (ElevatorStop stop in elevator.floors)
                            {
                                if (proximity < lastproximity || proximity == 0 || lastproximity == 0)
                                {
                                    if (bounds.Center.Y > stop.realbounds.Top && bounds.Center.Y < stop.realbounds.Bottom)
                                    {
                                        elevatorLocation = stop;

                                        State = "goingToElevator";
                                    }
                                }
                            }
                        }
                    }
                    break;

                case "ridingelevator":
                    velocity.X = 0;
                    velocity.Y = 0;
                    coolDownTimer.Activate();

                    foreach (ElevatorShaft shaft in elevators)
                    {
                        if (shaft.elevator.velocity.Y == 0 && shaft.elevator.bounds.Intersects(bounds) && shaft.elevator.inUseByNPC == true)
                        {
                            State = "LookingForPlayer";
                            lastSpottedLocation = bounds;
                            break;
                        }
                    }
                    break;
            }

            if (rightCol == true || leftCol == true || atEdge == true)
                movedir = -movedir;

            fovVisualization.X = bounds.Center.X;
            fovVisualization.Y = bounds.Y + 8;

            int lstartX = (int)fovVisualization.X;
            int lstartY = (int)fovVisualization.Y;

            if ((lastSpottedLocation.Top > bounds.Bottom ||
                 lastSpottedLocation.Bottom < bounds.Top) && State.ToLower() == "chasing" && player.UsingElevator == true &&
                 fovVisualization.Color == Color.Red)
            {
                State = "searchingForElevator";
            }
            else if (State.ToLower() != "searchingforelevator")
            {
                if (coolDownTimer.stopped == true)
                    State = "walking";
                else if (coolDownTimer.Time > 2)
                    State = "lookingforplayer";
            }

            if (updateLineOfSightTimer.Time == 0)
            {
                lineOfSight.Clear();
                fovVisualization.Color = Color.Green;

                fovVisualization.Angle = ((float)((Math.Atan2((angle.Y), (angle.X)))));

                for (int i = 0; i < SightPoints.Count; i++)
                {
                    SightPoints[i] = new Point(player.bounds.Center.X, player.bounds.Y + (i * (player.bounds.Height / (SightPoints.Count))));
                }

                if (fovVisualization.rectBounds.Intersects(player.bounds) && player.Alive == true)
                {
                    bool insight = false;

                    for (int i = 0; i < SightPoints.Count; i++)
                    {
                        if (CheckIfInSight(lstartX, lstartY,
                        SightPoints[i].X, SightPoints[i].Y, collisionMap, player.bounds, electronics) == true)
                        {
                            insight = true;
                            break;
                        }

                        if (DrawLineOfSight == true)
                        {
                            lineOfSight.AddRange(UpdateLineOfSight(lstartX, lstartY,
                            SightPoints[i].X, SightPoints[i].Y, collisionMap, player.bounds, electronics));
                        }
                    }

                    if (bounds.Intersects(player.bounds) || (insight == true &&

                        ((player.lightLevel >= 0.5f) ||
                         Vector2.Distance(new Vector2(bounds.Center.X, bounds.Center.Y),
                                          new Vector2(player.bounds.Center.X, player.bounds.Center.Y)) <= fovVisualization.Range * (player.lightLevel) + awareness)))
                    {
                        lastSpottedLocation = player.bounds;
                        fovVisualization.Color = Color.Red;
                        State = "chasing";

                        if (canShootTimer.active == false)
                        {
                            Vector2 bulletdir = new Vector2(player.bounds.Center.X - bounds.Center.X, player.bounds.Center.Y - bounds.Center.Y);
                            bulletdir.Normalize();

                            Object bullet = new Object(new Rectangle(bounds.Center.X + 2, bounds.Center.Y + 2, 4, 4), 0, true, false, true, krypton, false);
                            bullet.gravity = 0;
                            bullet.velocity = bulletdir * 70f;
                            bullet.color = Color.Black;
                            bullet.deadly = true;
                            bullet.destroyOnTouch = true;
                            bullet.collideInEntities = true;
                            entities.Add(bullet);

                            canShootTimer.Activate();
                        }

                        coolDownTimer.Activate();
                    }
                }
            }

            if (fovVisualization.Color == Color.Red)
                velocity.X = 0;

            coolDownTimer.Update();
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Camera camera, KryptonEngine krypton, bool DrawFOV, SpriteFont font)
        {
            if (DrawFOV == true)
            {
                foreach (Rectangle rect in lineOfSight)
                {
                    spriteBatch.Draw(texture, new Rectangle(rect.X - (int)camera.Position.X,
                        rect.Y - (int)camera.Position.Y, rect.Width, rect.Height), Color.Yellow);
                }
            }

            spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X,
                                                    bounds.Y - (int)camera.Position.Y,
                                                    bounds.Width, bounds.Height),
                                                    Color.Green);

            /*spriteBatch.Draw(texture, new Rectangle(lastSpottedLocation.X - (int)camera.Position.X,
                                                    lastSpottedLocation.Y - (int)camera.Position.Y,
                                                    lastSpottedLocation.Width, lastSpottedLocation.Height),
                                                    Color.FromNonPremultiplied(0, 200, 20, 200));*/

            if (Selected == true)
            {
                spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X,
                                                        bounds.Y - (int)camera.Position.Y,
                                                        bounds.Width, bounds.Height),
                                                        Color.Yellow);
            }

            spriteBatch.DrawString(font, State, new Vector2(bounds.X - (int)camera.Position.X,
                                                        bounds.Y - (int)camera.Position.Y - 20), Color.Blue, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
        }

        public void DrawFOV(KryptonEngine krypton)
        {
            krypton.mMovingLights.Add(fovVisualization);
        }

        private void LookRandomly(Random rnd)
        {
            changeDirTimer.Update();

            if (changeDirTimer.active == false)
            {
                rnd = new Random();
                movedir = rnd.Next(-1, 2);
                if (movedir == 0)
                    movedir = 1;

                newangle = new Vector2(movedir, rnd.Next(-2, 1));

                changeDirTimer.Length = rnd.Next(100, 251) / 100.0f;
                changeDirTimer.Activate();
            }

            angle = Vector2.Lerp(angle, new Vector2(newangle.X, newangle.Y), 0.02f);
        }

        public List<Rectangle> UpdateLineOfSight(int x0, int y0, int x1, int y1, CollisionMap collisionMap, Rectangle entity, List<ElectronicComponent> electronics)
        {
            bool insight;

            List<Rectangle> sight = Shapes.lineOfSight(x0, y0, x1,
             y1, 3, (int)fovVisualization.Range / 3, new Point(1, 1), collisionMap, angle.X, angle.Y, FOV, out insight,
             entity, electronics);

            return sight;
        }

        public bool CheckIfInSight(int x0, int y0, int x1, int y1, CollisionMap collisionMap, Rectangle entity, List<ElectronicComponent> electronics)
        {
            bool insight;

            List<Rectangle> sight = Shapes.lineOfSight(x0, y0, x1,
             y1, 1, (int)fovVisualization.Range, new Point(1, 1), collisionMap, angle.X, angle.Y, FOV, out insight, entity, electronics);

            return insight;
        }
    }
}
