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
    public class Switch : ElectronicComponent
    {
        public float ClickDelay = 0,
                     UseDelay = 0,
                     TimeToUse = 0,
                     TimeToClick = 0;

        Keys ActivateButton = Keys.W;

        public new Texture2D texture;

        public bool Cyclable = false,
                    Clicked = false,
                    CountDown = false;

        bool CanClick = true;

        public Switch(Texture2D texture, Color color, bool Cyclable, float Delay, float UseDelay, Random rnd, List<ElectronicComponent> electronics,
            Object OnState, Object OffState, bool StartState, KryptonEngine krypton, bool castShadows) :
            base(rnd, electronics, OnState, OffState, StartState, krypton, castShadows)
        {
            this.bounds = OnState.bounds;
            this.texture = texture;
            this.color = color;
            this.Cyclable = Cyclable;
            this.Delay = Delay;
            this.UseDelay = UseDelay;
        }

        public void Update(KeyboardState ks, Player player, List<ElectronicComponent> electronics)
        {
            if (TimeToUse == 0)
            {
                if (player.bounds.Intersects(bounds))
                {
                    if (ks.IsKeyDown(ActivateButton) == true)
                    {
                        if (CanClick == true)
                        {
                            TimeToUse = UseDelay;
                            CountDown = true;
                            CanClick = false;
                        }
                    }
                    else
                    {
                        CanClick = true;
                    }
                }
            }
            else
            {
                if(TimeToUse > 0)
                    TimeToUse -= 1;

                if (TimeToUse == 0 && Clicked == true)
                {
                    Clicked = false;
                }
            }

            if (CountDown == true)
            {
                if (TimeToClick == 0)
                {
                    OnUse(electronics);

                    if (Cyclable == true)
                    {
                        if (Clicked == false)
                            Clicked = true;
                        else
                            Clicked = false;
                    }
                    else
                    {
                        Clicked = true;
                    }

                    CountDown = false;
                    TimeToClick = ClickDelay;
                }
                else
                {
                    if(TimeToClick > 0)
                        TimeToClick -= 1;
                }
            }
        }

        public void OnUse(List<ElectronicComponent> electronics)
        {
            if (connectedComponents.Count > 0)
            {
                foreach (float connection in connectedComponents)
                {
                    foreach (ElectronicComponent comp in electronics)
                    {
                        if (comp.ID == connection)
                        {
                            if (comp.IsOn == true)
                            {
                                comp.IsOn = false;
                            }
                            else
                            {
                                comp.IsOn = true;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, SpriteFont font, Player player)
        {
            spriteBatch.Draw(texture, new Rectangle(bounds.X - (int)camera.Position.X, bounds.Y - (int)camera.Position.Y,
                bounds.Width, bounds.Height), color);

            if (player.bounds.Intersects(bounds))
            {
                spriteBatch.DrawString(font, "W: Activate", new Vector2(bounds.Right + 4 - camera.Position.X, bounds.Center.Y - 10 - camera.Position.Y), Color.White, 
                    0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
        }
    }
}
