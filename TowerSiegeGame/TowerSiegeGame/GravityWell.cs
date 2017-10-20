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
    class GravityWell
    {
        public Vector2 pos;
        public int range = 400;
        public float strength = 10000,
                     updateInterval = 0,
                     updateCount = 0,
                     percentOn = 0.5f;

        public bool editSelected = false,
                    deleted = false,
                    pressing = false,
                    editing = false;

        public Button editButton;

        public bool inverse = false;

        public GravityWell(Vector2 pos)
        {
            this.pos = pos;
        }

        public GravityWell(Vector2 pos, float strength)
        {
            this.pos = pos;
            this.strength = strength;
        }

        public GravityWell(Vector2 pos, float strength, int range)
        {
            this.pos = pos;
            this.strength = strength;
            this.range = range;
        }

        public GravityWell(Vector2 pos, float strength, int range, bool edit)
        {
            this.pos = pos;
            this.strength = strength;
            this.range = range;

            if (edit == true)
                editButton = new Button(new Rectangle((int)pos.X - 10, (int)pos.Y - 10, 20, 20), Color.Transparent, Color.Transparent, Color.Transparent, null, "", 0);
        }

        public void EditMode()
        {
            MouseState ms = Mouse.GetState();

            if (editButton != null)
            {
                editButton.bounds.X = (int)pos.X - 10;
                editButton.bounds.Y = (int)pos.Y - 10;
                editButton.Update(null);

                if (editButton.Pressed == true || editButton.rightPressed == true)
                {
                    editSelected = true;
                }

                if (editButton.Hovered == false)
                {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        if (editing == false)
                            editSelected = false;
                    }
                }

                if (editButton.Pressed == true)
                {
                    pos.X = ms.X;
                    pos.Y = ms.Y;
                }

                if (editSelected == true)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Delete))
                    {
                        deleted = true;
                    }

                    if (editButton.Hovered == true && ms.MiddleButton == ButtonState.Pressed)
                        deleted = true;
                }
            }
        }

        public void Suck(List<newParticle> particles)
        {
            if (inverse == false)
            {
                if (updateCount <= (updateInterval * 60.0f) * percentOn)
                {
                    Vector2 dPos;
                    float distance;

                    foreach (newParticle particle in particles)
                    {
                        dPos.X = pos.X - particle.bounds.X;
                        dPos.Y = pos.Y - particle.bounds.Y;
                        distance = dPos.Length();
                        var n = dPos / distance;
                        particle.velocity += ((strength) * n / (distance * distance + strength));

                        // add tangential acceleration for nearby particles
                        if (distance < range)
                            particle.velocity -= (45 * new Vector2(n.Y, -n.X) / (distance + strength / 100));
                    }
                }

                if (updateCount >= 0)
                    updateCount -= 1;

                if (updateCount < 0)
                {
                    updateCount = updateInterval * 60.0f;
                }
            }
            else
            {
                if (updateCount <= (updateInterval * 60.0f) * percentOn)
                {
                    Vector2 dPos;
                    float distance;

                    foreach (newParticle particle in particles)
                    {
                        dPos.X = pos.X - particle.bounds.X;
                        dPos.Y = pos.Y - particle.bounds.Y;
                        distance = dPos.Length();
                        var n = dPos / distance;
                        particle.velocity -= ((strength) * n / (distance * distance + strength));

                        // add tangential acceleration for nearby particles
                        if (distance < range)
                            particle.velocity += (45 * new Vector2(n.Y, -n.X) / (distance + strength / 100));
                    }
                }

                if (updateCount >= 0)
                    updateCount -= 1;

                if (updateCount < 0)
                {
                    updateCount = updateInterval * 60.0f;
                }
            }
        }
    }
}
