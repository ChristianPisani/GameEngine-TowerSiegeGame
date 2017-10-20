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
    class GravityWellEditor : GUI
    {
        Slider strength,
               range,
               interval,
               percentOn;

        CheckBox inverse;

        public GravityWell editWell;

        public GravityWellEditor(SpriteFont font, Texture2D texture, Rectangle window, Texture2D sliderBoxTex, Texture2D sliderTex, Random rnd)
            : base(font, texture, window, rnd, false)
        {
            int sH = 15; //sliderHeight
            int sW = 100; //sliderWidth
            int dW = 3; //draggerWidth
            int sp = 10; //spacing

            this.spaceBetweenComponents = sp;

            strength = new Slider(new Rectangle(window.Width - 120 - sW, sp + 20, sW, sH), dW, 100000, 5000, sliderBoxTex, sliderTex, "Strength: ");
            range = new Slider(OrderAlgorithms.nextbox(strength.Box, sp, 0), dW, 1000, 0, sliderBoxTex, sliderTex, "Range: ");
            interval = new Slider(OrderAlgorithms.nextbox(range.Box, sp, 0), dW, 3, 0, sliderBoxTex, sliderTex, "UpdateInterval: ");
            percentOn = new Slider(OrderAlgorithms.nextbox(interval.Box, sp, 0), dW, 1, 0, sliderBoxTex, sliderTex, "PercentOn: ");

            inverse = new CheckBox(Color.White, Color.Gray, OrderAlgorithms.nextbox(percentOn.Box, sp, 0), Color.Gray, Color.DarkGray, Color.LightGray, font, "Inverse", 0, true);
             
            strength.SetValue(10000);
            range.SetValue(400);
            interval.SetValue(0);
            percentOn.SetValue(0.5f);

            sliders.Add(strength);
            sliders.Add(range);
            sliders.Add(interval);
            sliders.Add(percentOn);

            checkBoxes.Add(inverse);

            editWell = new GravityWell(Vector2.Zero);
            editWell.strength = strength.value;

            //movable = true;
            topBar = new Button(new Rectangle(window.Left, sliders[0].Box.Y - 30, sliders[0].Box.Width + 70, 0), Color.BurlyWood, Color.BurlyWood, Color.BurlyWood, font, "", 0);
            scrollbar = new Button(new Rectangle(bounds.Right - 5, bounds.Y, 10, 30), Color.Gray, Color.Gray, Color.Gray, font, "", 0);
            maxBounds = new Rectangle(0, 0, sliders[sliders.Count - 1].Box.Width + 70, (checkBoxes[0].bounds.Bottom) - topBar.bounds.Y + sp);

            backDropTexture = texture;
            bounds = new Rectangle(topBar.bounds.X, topBar.bounds.Y, topBar.bounds.Width, 100);
            backDropColor = Color.FromNonPremultiplied(40, 40, 40, 200);
        }

        public void Update(Rectangle window)
        {
            if (active == false)
                return;

            base.Update(font, window);

            /////////////////////////
            ///Remember to add this to Editor class too, it won't work without it!!
            /////////////////////////

            editWell.strength = sliders[0].value;
            editWell.range = sliders[1].Value;
            editWell.updateInterval = sliders[2].value;
            editWell.percentOn = sliders[3].value;

            editWell.inverse = checkBoxes[0].Checked;
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, Rectangle window)
        {
            base.Draw(spriteBatch, texture, font, window);

            //spriteBatch.DrawString(font, particleSystem.particles.Count.ToString(), new Vector2(0, 40), Color.White);

            //spriteBatch.DrawString(font, "ParticleCount: " + particleSystem.particleCount.ToString(), new Vector2(10, 200), Color.White);
            //spriteBatch.DrawString(font, "RemovalCount: " + particleSystem.removalCount.ToString(), new Vector2(10, 250), Color.White);
        }
    }
}
