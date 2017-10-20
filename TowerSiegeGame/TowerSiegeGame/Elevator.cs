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
    public class Elevator
    {
        public Rectangle bounds;

        public int currentfloor,
            floors,
            speed,
            floorToMoveTo;

        public Vector2 velocity = new Vector2();

        public Keys up = Keys.W,
            down = Keys.S;

        public bool inUseByPlayer = false,
                    inUseByNPC = false,
                    Moving = false,
                    movingToFloor = false;

        public Texture2D texture;
        public Color color;

        public Elevator(Rectangle bounds, int speed, Texture2D texture, Color color)
        {
            this.bounds = bounds;
            this.texture = texture;
            this.color = color;
            this.speed = speed;

        }

        public void Update(Player player, List<NPC> NPCs, KeyboardState ks)
        {
            bounds.Y += (int)velocity.Y;

            if (velocity.Y == 0)
            {
                inUseByNPC = false;
                inUseByPlayer = false;
            }

            if (movingToFloor == false)
            {
                if (bounds.Intersects(player.bounds) && player.canJump == true && player.canSlide == true)
                {
                    player.ableToSlide = false;

                    if (ks.IsKeyDown(up) && currentfloor < floors && velocity.Y == 0)
                    {
                        velocity.Y = -speed;
                        inUseByPlayer = true;
                        player.UsingElevator = true;
                        Moving = true;
                        floorToMoveTo = currentfloor + 1;
                    }

                    if (ks.IsKeyDown(down) && currentfloor != 0 && velocity.Y == 0)
                    {
                        velocity.Y = speed;
                        inUseByPlayer = true;
                        player.UsingElevator = true;
                        Moving = true;
                        floorToMoveTo = currentfloor - 1;
                    }
                }
                else
                {
                    inUseByPlayer = false;
                    player.UsingElevator = false;
                }

                if (inUseByPlayer == true)
                {
                    player.colYDown = true;
                    player.bounds.Y = bounds.Bottom - player.bounds.Height;
                    player.bounds.Width = player.standBounds.Width;
                    player.bounds.Height = player.standBounds.Height;
                    player.bounds.Y = (int)MathHelper.Clamp(player.bounds.Y, bounds.Y, bounds.Bottom - player.bounds.Height);

                    if (velocity.Y != 0)
                    {
                        player.bounds.X = (int)MathHelper.Clamp(bounds.Center.X - player.bounds.Width / 2, bounds.X, bounds.Right - player.bounds.Width);
                    }
                }


                foreach (NPC npc in NPCs)
                {
                    if (bounds.Intersects(npc.bounds) && npc.State.ToLower() == "ridingelevator")
                    {
                        movingToFloor = false;
                        Moving = false;

                        if (Moving == false)
                        {
                            if (inUseByNPC == false)
                            {
                                if (npc.lastSpottedLocation.Top < npc.bounds.Center.Y)
                                {
                                    velocity.Y = -speed;
                                    floorToMoveTo = currentfloor + 1;
                                }
                                else if (npc.lastSpottedLocation.Bottom > npc.bounds.Center.Y)
                                {
                                    velocity.Y = speed;
                                    floorToMoveTo = currentfloor - 1;
                                }
                            }
                            inUseByNPC = true;

                            npc.bounds.Y = bounds.Bottom - npc.bounds.Height;
                            npc.bounds.Y = (int)MathHelper.Clamp(npc.bounds.Y, bounds.Y, bounds.Bottom - npc.bounds.Height);

                            if (velocity.Y != 0)
                            {
                                npc.bounds.X = (int)MathHelper.Clamp(bounds.Center.X - npc.bounds.Width / 2, bounds.X, bounds.Right - npc.bounds.Width);
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font)
        {
            if (inUseByPlayer == true && velocity.Y != 0)
            {
                //spriteBatch.Draw(texture, new Rectangle(0, 0, 2000, 2000), Color.Black);
            }

            spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X,
                                                    bounds.Y - (int)camera.Position.Y,
                                                    bounds.Width, bounds.Height),
                                                    color);

            spriteBatch.DrawString(font, currentfloor.ToString() + System.Environment.NewLine + floorToMoveTo.ToString(),
                new Vector2(bounds.X - 20 - camera.Position.X, bounds.Y - camera.Position.Y), Color.Wheat);
        }
    }

    public class ElevatorStop
    {
        public bool selected = false,
                    deleted = false;

        public Rectangle bounds,
                         realbounds;

        public int floor = 0;

        public ElevatorStop(Rectangle bounds, Rectangle realbounds)
        {
            this.bounds = bounds;
            this.realbounds.X = bounds.X;
            this.realbounds.Y = bounds.Y - realbounds.Height;
            this.realbounds.Width = realbounds.Width;
            this.realbounds.Height = realbounds.Height;
        }
    }

    public class ElevatorShaft
    {
        public Elevator elevator;

        public bool selected = false;

        public List<ElevatorStop> floors = new List<ElevatorStop>();

        public ElevatorShaft(Elevator elevator, int startfloor)
        {
            this.elevator = elevator;

            for (int i = 0; i < 5; i++ )
            {
                floors.Add(new ElevatorStop(new Rectangle(elevator.bounds.X, elevator.bounds.Bottom - i * 100, elevator.bounds.Width, 1), elevator.bounds));
            }

            elevator.bounds.Y = floors[startfloor].bounds.Center.Y - elevator.bounds.Height;
            elevator.floors = floors.Count - 1;
        }

        public void Update(Player player, List<NPC> NPCs, KeyboardState ks)
        {
            floors = floors.OrderByDescending(o => o.bounds.Y).ToList();

            elevator.floors = floors.Count - 1;

            elevator.Update(player, NPCs, ks);

            elevator.floorToMoveTo = (int)MathHelper.Clamp(elevator.floorToMoveTo, 0, floors.Count - 1);

            for (int i = 0; i < floors.Count; i++ )
            {
                floors[i].bounds.X = elevator.bounds.X;
                floors[i].realbounds.X = elevator.bounds.X;
                floors[i].floor = i;

                if (floors[i].realbounds.Intersects(player.bounds))
                {
                    if ((ks.IsKeyDown(elevator.up) || ks.IsKeyDown(elevator.down)) &&
                    elevator.currentfloor != i && elevator.velocity.Y == 0 && elevator.Moving == false &&
                        player.canJump == true && player.canSlide == true)
                    {
                        elevator.movingToFloor = true;

                        elevator.floorToMoveTo = i;
                    }
                }

                if (elevator.bounds.Bottom - floors[i].bounds.Bottom <= elevator.speed && elevator.bounds.Bottom - floors[i].bounds.Bottom > -elevator.speed &&
                    elevator.currentfloor != i)
                {
                    elevator.currentfloor = i;
                }
            }

            foreach (NPC npc in NPCs)
            {
                if (npc.State.ToLower() == "callingelevator" && elevator.velocity.Y == 0 && elevator.Moving == false &&
                    elevator.bounds.X == npc.elevatorLocation.bounds.X)
                {
                    elevator.movingToFloor = true;
                    elevator.Moving = true;

                    elevator.floorToMoveTo = npc.elevatorLocation.floor;
                }
            }

            if (elevator.movingToFloor == true)
            {
                if (floors[elevator.floorToMoveTo].bounds.Y > floors[elevator.currentfloor].bounds.Y)
                {
                    elevator.velocity.Y = elevator.speed;
                }
                if (floors[elevator.floorToMoveTo].bounds.Y < floors[elevator.currentfloor].bounds.Y)
                {
                    elevator.velocity.Y = -elevator.speed;
                }
            }

            if (elevator.currentfloor == elevator.floorToMoveTo)
            {
                elevator.movingToFloor = false;

                elevator.bounds.Y = floors[elevator.floorToMoveTo].bounds.Y - elevator.bounds.Height;
                elevator.velocity.Y = 0;
            }

            if (elevator.movingToFloor == false)
            {
                if(elevator.bounds.Intersects(player.bounds) && elevator.velocity.Y != 0 && elevator.inUseByNPC == false)
                    player.bounds.Y = elevator.bounds.Bottom - player.bounds.Height;

                elevator.Moving = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font)
        {
            if (selected == true)
            {
                if (floors.Count > 0)
                {
                    int Height = -(floors[floors.Count - 1].realbounds.Y - floors[0].realbounds.Y);

                    spriteBatch.Draw(elevator.texture, new Rectangle(elevator.bounds.X - (int)camera.Position.X,
                                      floors[floors.Count - 1].realbounds.Y - (int)camera.Position.Y,
                                      elevator.bounds.Width, Height),
                                      Color.FromNonPremultiplied(0, 255, 0, 100));
                }
            }

            foreach (ElevatorStop rect in floors)
            {
                if (rect.selected == true)
                {
                    spriteBatch.Draw(elevator.texture, new Rectangle(rect.bounds.X - (int)camera.Position.X,
                                                          rect.realbounds.Y - (int)camera.Position.Y,
                                                          elevator.bounds.Width, elevator.bounds.Height),
                                                          Color.Yellow);
                }
                else {
                    spriteBatch.Draw(elevator.texture, new Rectangle(rect.bounds.X - (int)camera.Position.X,
                                                            rect.realbounds.Y - (int)camera.Position.Y,
                                                            elevator.bounds.Width, elevator.bounds.Height),
                                                            Color.Wheat);
                }
            }
        }
    }
}
