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
    class ParticleEditor : GUI
    {
        public Slider R,
                      G,
                      B,
                      A,
                      amount,
                      interval,
                      velMaxX,
                      velMinX,
                      velMaxY,
                      velMinY,
                      aclMax,
                      aclMin,
                      growthMax,
                      growthMin,
                      rotMax,
                      rotMin,
                      sizeMax,
                      sizeMin,
                      range,
                      fadeR,
                      fadeG,
                      fadeB,
                      fadeA,
                      maxLifetime,
                      minLifetime,
                      gravity,
                      colorLerp,
                      velModifierStrength;

        public CheckBox velModifier;

        Rectangle grab;
        bool grabbing = false;

        public newParticleSystem particleSystem;
        MouseState ms;

        public ParticleEditor(SpriteFont font, Texture2D texture, Rectangle window, Texture2D sliderBoxTex, Texture2D sliderTex, Random rnd, Texture2D pTexture, Rectangle pSRect) 
            : base(font, texture, window, rnd, true)
        {
            int sH = 15; //sliderHeight
            int sW = 255; //sliderWidth
            int dW = 3; //draggerWidth
            int sp = 10; //spacing

            this.spaceBetweenComponents = sp;
            
            R = new Slider(new Rectangle(window.Width - 120 - sW, sp + 20, sW, sH), dW, 255, 0, sliderBoxTex, sliderTex, "R: ");
            G = new Slider(OrderAlgorithms.nextbox(R.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "G: ");
            B = new Slider(OrderAlgorithms.nextbox(G.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "B: ");
            A = new Slider(OrderAlgorithms.nextbox(B.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "A: ");
            velMinX = new Slider(OrderAlgorithms.nextbox(A.Box, sp, 0), dW, 20, -20, sliderBoxTex, sliderTex, "minVelX: ");
            velMaxX = new Slider(OrderAlgorithms.nextbox(velMinX.Box, sp, 0), dW, 20, -20, sliderBoxTex, sliderTex, "maxVelX: ");
            velMinY = new Slider(OrderAlgorithms.nextbox(velMaxX.Box, sp, 0), dW, 20, -20, sliderBoxTex, sliderTex, "minVelY: ");
            velMaxY = new Slider(OrderAlgorithms.nextbox(velMinY.Box, sp, 0), dW, 20, -20, sliderBoxTex, sliderTex, "maxVelY: ");
            sizeMin = new Slider(OrderAlgorithms.nextbox(velMaxY.Box, sp, 0), dW, 50, 1, sliderBoxTex, sliderTex, "sizeMin: ");
            sizeMax = new Slider(OrderAlgorithms.nextbox(sizeMin.Box, sp, 0), dW, 50, 1, sliderBoxTex, sliderTex, "sizeMax: ");
            range = new Slider(OrderAlgorithms.nextbox(sizeMax.Box, sp, 0), dW, 300, 0, sliderBoxTex, sliderTex, "Range: ");
            fadeR = new Slider(OrderAlgorithms.nextbox(range.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "fadeR: ");
            fadeG = new Slider(OrderAlgorithms.nextbox(fadeR.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "fadeG: ");
            fadeB = new Slider(OrderAlgorithms.nextbox(fadeG.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "fadeB: ");
            fadeA = new Slider(OrderAlgorithms.nextbox(fadeB.Box, sp, 0), dW, 255, 0, sliderBoxTex, sliderTex, "fadeA: ");
            minLifetime = new Slider(OrderAlgorithms.nextbox(fadeA.Box, sp, 0), dW, 3, 0, sliderBoxTex, sliderTex, "minLifetime: ");
            maxLifetime = new Slider(OrderAlgorithms.nextbox(minLifetime.Box, sp, 0), dW, 3, 0, sliderBoxTex, sliderTex, "maxLifetime: ");
            amount = new Slider(OrderAlgorithms.nextbox(maxLifetime.Box, sp, 0), dW, 1000, 0, sliderBoxTex, sliderTex, "Amount: ");
            interval = new Slider(OrderAlgorithms.nextbox(amount.Box, sp, 0), dW, 3, 0, sliderBoxTex, sliderTex, "Interval: ");
            growthMin = new Slider(OrderAlgorithms.nextbox(interval.Box, sp, 0), dW, 2, -2, sliderBoxTex, sliderTex, "minGrowth: ");
            growthMax = new Slider(OrderAlgorithms.nextbox(growthMin.Box, sp, 0), dW, 2, -2, sliderBoxTex, sliderTex, "maxGrowth: ");
            rotMin = new Slider(OrderAlgorithms.nextbox(growthMax.Box, sp, 0), dW, 2, -2, sliderBoxTex, sliderTex, "minRotation: ");
            rotMax = new Slider(OrderAlgorithms.nextbox(rotMin.Box, sp, 0), dW, 2, -2, sliderBoxTex, sliderTex, "maxRotation: ");
            gravity = new Slider(OrderAlgorithms.nextbox(rotMax.Box, sp, 0), dW, 1, 0, sliderBoxTex, sliderTex, "Gravity: ");
            aclMax = new Slider(OrderAlgorithms.nextbox(gravity.Box, sp, 0), dW, 1, 0, sliderBoxTex, sliderTex, "Wind: ");
            colorLerp = new Slider(OrderAlgorithms.nextbox(aclMax.Box, sp, 0), dW, 0.4f, 0, sliderBoxTex, sliderTex, "Lerp: ");

            velModifierStrength = new Slider(OrderAlgorithms.nextbox(colorLerp.Box, sp, 0), dW, 15, 0, sliderBoxTex, sliderTex, "velModifierStrength: ");
            velModifier = new CheckBox(Color.White, Color.Gray, OrderAlgorithms.nextbox(velModifierStrength.Box, sp, 0), Color.Gray, Color.DarkGray, Color.LightGray, font, "velLengthModifier", 0, true);
            velModifier.bounds.Width = velModifier.bounds.Height;
            velModifier.textcolor = Color.LightGray;
            velModifier.textOffSet = velModifier.bounds.Width + 10;

            R.SetValue(130);
            G.SetValue(82);
            B.SetValue(23);
            A.SetValue(100);
            velMinX.SetValue(-3);
            velMaxX.SetValue(3);
            velMinY.SetValue(-3);
            velMaxY.SetValue(3);
            sizeMin.SetValue(3);
            sizeMax.SetValue(10);
            range.SetValue(20);
            fadeR.SetValue(100);
            fadeG.SetValue(255);
            fadeB.SetValue(100);
            fadeA.SetValue(100);
            minLifetime.SetValue(1);
            maxLifetime.SetValue(1);
            amount.SetValue(50);
            growthMin.SetValue(0);
            growthMax.SetValue(0);
            rotMin.SetValue(-0.2f);
            rotMax.SetValue(0.2f);
            gravity.SetValue(0.4f);
            aclMax.SetValue(0);
            colorLerp.SetValue(0.1f);
            velModifierStrength.SetValue(6);

            sliders.Add(R);
            sliders.Add(G);
            sliders.Add(B);
            sliders.Add(A);
            sliders.Add(velMinX);
            sliders.Add(velMaxX);
            sliders.Add(velMinY);
            sliders.Add(velMaxY);
            sliders.Add(sizeMin);
            sliders.Add(sizeMax);
            sliders.Add(range);
            sliders.Add(fadeR);
            sliders.Add(fadeG);
            sliders.Add(fadeB);
            sliders.Add(fadeA);
            sliders.Add(minLifetime);
            sliders.Add(maxLifetime);
            sliders.Add(amount);
            sliders.Add(interval);
            sliders.Add(growthMin);
            sliders.Add(growthMax);
            sliders.Add(rotMin);
            sliders.Add(rotMax);
            sliders.Add(gravity);
            sliders.Add(aclMax);
            sliders.Add(colorLerp);
            sliders.Add(velModifierStrength);

            checkBoxes.Add(velModifier);

            particleSystem = new newParticleSystem(
                rnd,
                0,
                30000,
                10,
                pTexture,
                pSRect,
                Color.FromNonPremultiplied(R.Value, G.Value, B.Value, 255),
                Color.FromNonPremultiplied(fadeR.Value, fadeG.Value, fadeB.Value, 255),
                colorLerp.value,
                new Vector2(window.Width / 2 + 300, window.Height / 2),
                new Vector2(-(range.value / 2), -(range.value / 2)),
                new Vector2((range.value / 2), (range.value / 2)),
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Point(sizeMin.Value, sizeMin.Value),
                new Point(sizeMax.Value, sizeMax.Value),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(velMinX.value, velMinY.value),
                new Vector2(velMaxX.value, velMaxY.value),
                0,
                0,
                0,
                5,
                6,
                false,
                0.1f);

            grab = new Rectangle((int)particleSystem.origin.X - 25, (int)particleSystem.origin.Y - 25, 50, 50);

            Resizable = true;
            movable = true;
            topBar = new Button(new Rectangle(R.Box.X, R.Box.Y - 30, R.Box.Width + 70, 10), Color.BurlyWood, Color.BurlyWood, Color.BurlyWood, font, "", 0);
            scrollbar = new Button(new Rectangle(bounds.Right - 5, bounds.Y, 10, 30), Color.Gray, Color.Gray, Color.Gray, font, "", 0);
            maxBounds = new Rectangle(0, 0, R.Box.Width + 70, (velModifier.bounds.Bottom) - topBar.bounds.Y + sp);

            backDropTexture = texture;
            bounds = new Rectangle(topBar.bounds.X, topBar.bounds.Y, topBar.bounds.Width, 600);
            backDropColor = Color.FromNonPremultiplied(40, 40, 40, 200);
        }

        public override void Update(SpriteFont font, Rectangle window)
        {
            if (ms.LeftButton == ButtonState.Released)
            {
                inRange = false;
            }
            if (new Rectangle(ms.X, ms.Y, 1, 1).Intersects(bounds))
            {
                inRange = true;
            }

            if (grabbing == false)
            {
                base.Update(font, window);
            }

            ms = Mouse.GetState();

            if (new Rectangle(ms.X, ms.Y, 1, 1).Intersects(grab) && ms.LeftButton == ButtonState.Pressed && moving == false && Resizing == false)
                grabbing = true;

            particleSystem.gravity = gravity.value;

            if (grabbing == false && base.holdingSlider == false && moving == false && Resizing == false && inRange == true)
            {
                R.Update();
                G.Update();
                B.Update();
                velMinX.Update();
                velMaxX.Update();
                velMinY.Update();
                velMaxY.Update();
                sizeMin.Update();
                sizeMax.Update();
                range.Update();
                fadeR.Update();
                fadeG.Update();
                fadeB.Update();
                minLifetime.Update();
                maxLifetime.Update();
                colorLerp.Update();
                velModifierStrength.Update();

                velModifier.Update(font, new Point(velModifier.bounds.Width, velModifier.bounds.Height));
            }

            particleSystem.origin = new Vector2(window.Width / 2 - 200, window.Height / 2);
            particleSystem.color = Color.FromNonPremultiplied(R.Value, G.Value, B.Value, A.Value);
            particleSystem.minVel = new Vector2(velMinX.value, velMinY.value);
            particleSystem.maxVel = new Vector2(velMaxX.value, velMaxY.value);
            particleSystem.minSize = new Point(sizeMin.Value, sizeMin.Value);
            particleSystem.maxSize = new Point(sizeMax.Value, sizeMax.Value);
            particleSystem.minOffSet = new Vector2(-(range.value / 2), -(range.value / 2));
            particleSystem.maxOffSet = new Vector2((range.value / 2), (range.value / 2));
            particleSystem.fade = Color.FromNonPremultiplied(fadeR.Value, fadeG.Value, fadeB.Value, 10);
            particleSystem.maxLifetime = maxLifetime.value;
            particleSystem.minLifetime = minLifetime.value;
            particleSystem.particleAmount = amount.Value;
            particleSystem.updateInterval = interval.value;
            particleSystem.minRotationVal = rotMin.value;
            particleSystem.maxRotationVal = rotMax.value;
            particleSystem.minGrowth = new Vector2(growthMin.value, growthMin.value);
            particleSystem.maxGrowth = new Vector2(growthMax.value, growthMax.value);
            particleSystem.minAcceleration = new Vector2(aclMax.value, 0);
            particleSystem.maxAcceleration = new Vector2(aclMax.value, 0);
            particleSystem.range = range.Value / 2;
            particleSystem.colorLerp = colorLerp.value;
            particleSystem.velModifierStrength = velModifierStrength.value;

            particleSystem.velModifyLength = velModifier.Checked;

            if (ms.LeftButton == ButtonState.Released)
                grabbing = false;

            if (grabbing == true)
            {
                grab.X = ms.X;
                grab.Y = ms.Y;
                particleSystem.origin.X = grab.X;
                particleSystem.origin.Y = grab.Y;
            }
            else
            {
                grab.X = window.Width / 2 - 200; 
                grab.Y = window.Height / 2;
            }

            particleSystem.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, Rectangle window)
        {
            particleSystem.Draw(spriteBatch);

            base.Draw(spriteBatch, texture, font, window);
            //spriteBatch.DrawString(font, particleSystem.particles.Count.ToString(), new Vector2(0, 40), Color.White);

            //spriteBatch.DrawString(font, "ParticleCount: " + particleSystem.particleCount.ToString(), new Vector2(10, 200), Color.White);
            //spriteBatch.DrawString(font, "RemovalCount: " + particleSystem.removalCount.ToString(), new Vector2(10, 250), Color.White);
        }
    }
}
