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
    class Editor : GUI
    {
        public RadioButton editors;
        public Button exit;
        public ParticleEditor particleEditor;
        public GravityWellEditor gravEditor;
        public CheckBox drawGravityWells;

        Vector2 msEditPos = new Vector2(0,0);

        public Button gravityWellAdder;

        public Editor(SpriteFont font, Texture2D texture, Rectangle window, Texture2D sliderBoxTex, Texture2D sliderTex, Random rnd, Texture2D pTex, Rectangle pSRect)
            : base(font, texture, window, rnd, false)
        {
            backDropTexture = texture;
            bounds = window;

            editors = new RadioButton(Color.Wheat, font, "", 5, 0, "horizontal", 0, texture, Color.FromNonPremultiplied(40, 100, 20, 200), Color.DarkRed, Color.DarkRed, Color.Red, Color.LightGray, new Rectangle(0, 0, 130, 15));
            editors.textcolor = Color.White;
            editors.Buttons[0].Text = "Particles";
            editors.Buttons[1].Text = "Entity";
            editors.Buttons[2].Text = "Block";
            editors.Buttons[3].Text = "Lights";
            editors.Buttons[4].Text = "Ballsack";

            exit = new Button(new Rectangle(window.Width - 10 - 30, 10, 30, 30), Color.DarkRed, Color.Red, Color.LightGray, font, "X", 0);
            exit.textcolor = Color.White;

            buttons.Add(exit);
            radioButtons.Add(editors);

            particleEditor = new ParticleEditor(font, texture, window, sliderBoxTex, sliderTex, rnd, pTex, pSRect);
            particleEditor.SetPositions();

            gravEditor = new GravityWellEditor(font, texture, window, sliderBoxTex, sliderTex, rnd);
            gravEditor.SetPositions();
            gravEditor.active = false;
            gravEditor.movable = false;
            gravEditor.bounds.Height = gravEditor.maxBounds.Height;

            gravityWellAdder = new Button(new Rectangle(0, 16, 120, 20), Color.DarkGray, Color.Gray, Color.White, font, "Add gravitywell", 0);
            gravityWellAdder.textScale = 0.6f;
            //gravityWellAdder.textpos = new Vector2(5, gravityWellAdder.bounds.Center.Y + ((font.MeasureString(gravityWellAdder.Text).Y * gravityWellAdder.textScale) / 2));
            gravityWellAdder.textcolor = Color.Black;

            drawGravityWells = new CheckBox(Color.White, Color.Gray, OrderAlgorithms.nextbox(gravityWellAdder.bounds, 0, 1), Color.Gray, Color.DarkGray, Color.LightGray, font, "Draw gravitywells", 0, true);
            drawGravityWells.bounds.Width = drawGravityWells.bounds.Height;
        }

        public override void Update(SpriteFont font, Rectangle window)
        {
            MouseState ms = Mouse.GetState();

            base.Update(font, window);

            if (editors.Buttons[0].Checked == true)
            {
                if (ms.RightButton == ButtonState.Pressed)
                {
                    if (msEditPos == new Vector2(ms.X, ms.Y))
                    {
                        msEditPos = new Vector2(ms.X, ms.Y);
                        gravEditor.topBar.bounds.X = ms.X;
                        gravEditor.topBar.bounds.Y = ms.Y;
                        gravEditor.SetPositions();
                        gravEditor.active = true;
                    }
                }
                else
                {
                    msEditPos = new Vector2(ms.X, ms.Y);
                }

                if (gravEditor.inRange == false)
                {
                    particleEditor.Update(font, window);

                    if (ms.LeftButton == ButtonState.Pressed)
                        gravEditor.active = false;
                }
                else
                {
                    particleEditor.particleSystem.Update();
                }

                if (drawGravityWells.Checked == true)
                {
                    for (int i = 0; i < particleEditor.particleSystem.gravityWells.Count; i++)
                    {
                        particleEditor.particleSystem.gravityWells[i].EditMode();
                        particleEditor.particleSystem.gravityWells[i].editing = false;

                        if (particleEditor.particleSystem.gravityWells[i].editSelected == true && gravEditor.active == true)
                        {
                            if (ms.RightButton == ButtonState.Pressed)
                            {
                                particleEditor.particleSystem.gravityWells[i].editing = true;
                                gravEditor.sliders[0].value = particleEditor.particleSystem.gravityWells[i].strength;
                                gravEditor.sliders[1].value = particleEditor.particleSystem.gravityWells[i].range;
                                gravEditor.sliders[2].value = particleEditor.particleSystem.gravityWells[i].updateInterval;
                                gravEditor.sliders[3].value = particleEditor.particleSystem.gravityWells[i].percentOn;
                                gravEditor.checkBoxes[0].Checked = particleEditor.particleSystem.gravityWells[i].inverse;
                            }
                            else
                            {
                                particleEditor.particleSystem.gravityWells[i].editing = true;
                                particleEditor.particleSystem.gravityWells[i].strength = gravEditor.sliders[0].value;
                                particleEditor.particleSystem.gravityWells[i].range = gravEditor.sliders[1].Value;
                                particleEditor.particleSystem.gravityWells[i].updateInterval = gravEditor.sliders[2].value;
                                particleEditor.particleSystem.gravityWells[i].percentOn = gravEditor.sliders[3].value;
                                particleEditor.particleSystem.gravityWells[i].inverse = gravEditor.editWell.inverse;
                            }
                        }

                        if (particleEditor.particleSystem.gravityWells[i].deleted == true)
                            particleEditor.particleSystem.gravityWells.RemoveAt(i);
                    }
                }

                gravityWellAdder.Update(font, new Point(gravityWellAdder.bounds.Width, gravityWellAdder.bounds.Height));
                drawGravityWells.Update(font);

                gravEditor.Update(window);

                GravityWell newWell = new GravityWell(new Vector2(20, 80), gravEditor.editWell.strength, gravEditor.editWell.range, true);
                newWell.updateInterval = gravEditor.editWell.updateInterval;
                newWell.percentOn = gravEditor.editWell.percentOn;

                if(gravityWellAdder.Clicked == true)
                    particleEditor.particleSystem.gravityWells.Add(newWell);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, Rectangle window)
        {
            base.Draw(spriteBatch, texture, font, window);

            if (editors.Buttons[0].Checked == true)
            {
                drawGravityWells.Draw(spriteBatch, texture, font, Point.Zero);
                gravityWellAdder.Draw(spriteBatch, texture, font, new Point(0, 0));
                particleEditor.Draw(spriteBatch, texture, font, window);

                if (gravEditor.active == true)
                    gravEditor.Draw(spriteBatch, texture, font, window);

                if (drawGravityWells.Checked == true)
                {
                    foreach (GravityWell well in particleEditor.particleSystem.gravityWells)
                    {
                        if (well.editSelected == false)
                            spriteBatch.Draw(texture, new Rectangle((int)well.pos.X - 5, (int)well.pos.Y - 5, 10, 10), Color.White);
                        else
                            spriteBatch.Draw(texture, new Rectangle((int)well.pos.X - 5, (int)well.pos.Y - 5, 10, 10), Color.Yellow);
                    }
                }
            }
            if (editors.Buttons[4].Checked == true)
            {
                spriteBatch.Draw(texture, window, Color.HotPink);
                spriteBatch.DrawString(font, "Ballsack", new Vector2(window.Center.X, window.Center.Y) - font.MeasureString("Ballsack") / 2, Color.Wheat);
            }
        }
    }
}
