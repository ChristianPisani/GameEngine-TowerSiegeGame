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
    public class LightEditor
    {
        public Slider r,
                      g,
                      b,
                      FOV,
                      radius,
                      intensity,
                      radiusMax,
                      radiusMin;

        public bool active = false,
                    canpresskey = true;

        SpriteFont font1;
        Texture2D texture;

        public Light2D editlight;

        public CheckBox checkbox;

        KryptonEngine krypton;

        public LightEditor(Rectangle firstbox, GraphicsDeviceManager graphics, SpriteFont font1, 
            Game game, Texture2D texture, Texture2D mLightTexture, Rectangle windowSize, int offset,
            Texture2D sliderBoxTex, Texture2D sliderTex)
        {
            int dW = 4;
            r = new Slider(firstbox, dW, 255, 0, sliderBoxTex, sliderTex, "R: ");
            g = new Slider(OrderAlgorithms.nextbox(r.Box, 10, 0), dW, 255, 0, sliderBoxTex, sliderTex, "G: ");
            b = new Slider(OrderAlgorithms.nextbox(g.Box, 10, 0), dW, 255, 0, sliderBoxTex, sliderTex, "B: ");
            FOV = new Slider(OrderAlgorithms.nextbox(b.Box, 10, 0), dW, (int)(370), 0, sliderBoxTex, sliderTex, "FOV: ");
            radius = new Slider(OrderAlgorithms.nextbox(FOV.Box, 10, 0), dW, 6000, 10, sliderBoxTex, sliderTex, "Radius: ");
            intensity = new Slider(OrderAlgorithms.nextbox(radius.Box, 20, 0), dW, 0, 2000, sliderBoxTex, sliderTex, "Intensity: ");
            radiusMin = new Slider(OrderAlgorithms.nextbox(intensity.Box, 20, 0), dW, 0, 2000, sliderBoxTex, sliderTex, "minRadius: ");
            radiusMax = new Slider(OrderAlgorithms.nextbox(radiusMin.Box, 20, 0), dW, 0, 2000, sliderBoxTex, sliderTex, "maxRadius: ");
            
            editlight = new Light2D();
            editlight.Texture = mLightTexture;
            this.font1 = font1;
            this.texture = texture;

            krypton = new KryptonEngine(game, "KryptonEffect");
            this.krypton.SpriteBatchCompatablityEnabled = true;
            this.krypton.CullMode = CullMode.CullClockwiseFace;
            this.krypton.AmbientColor = Color.FromNonPremultiplied(75, 75, 75, 255);
            this.krypton.Initialize();
            krypton.Lights.Add(editlight);

            r.Box = new Rectangle(windowSize.Right - offset, r.Box.Y, r.Box.Width, r.Box.Height);
            g.Box = OrderAlgorithms.nextbox(r.Box, 10, 0);
            b.Box = OrderAlgorithms.nextbox(g.Box, 10, 0);
            FOV.Box = OrderAlgorithms.nextbox(b.Box, 10, 0);
            radius.Box = OrderAlgorithms.nextbox(FOV.Box, 10, 0);
            intensity.Box = OrderAlgorithms.nextbox(radius.Box, 20, 0);

            r.SliderBtn.X = r.Box.Center.X;
            g.SliderBtn.X = g.Box.Center.X;
            b.SliderBtn.X = b.Box.Center.X;
            FOV.SliderBtn.X = FOV.Box.Right;
            radius.SliderBtn.X = radius.Box.Left + radius.Box.Width / 3;
            intensity.SliderBtn.X = intensity.Box.Left + intensity.Box.Width / 3;

            r.Update();
            g.Update();
            b.Update();
            FOV.Update();
            radius.Update();
            intensity.Update();
            radiusMin.Update();
            radiusMax.Update();
        }

        public void Update(GraphicsDeviceManager graphics, Rectangle Windowsize, int offset)
        {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.L) && canpresskey == true)
            {
                canpresskey = false;

                if (active == false)
                {
                    active = true;
                }
                else
                {
                    active = false;
                }
            }
            else if(ks.IsKeyUp(Keys.L)) {
                canpresskey = true;
            }

            if (active == true)
            {
                Color color = new Color(r.Value, g.Value, b.Value);
                editlight.Color = color;
                //editlight.Range = radius.Value;
                editlight.Intensity = intensity.Value / 1000.0f;
                editlight.FOV = FOV.Value;
                editlight.Fov = (float)((Math.PI + 0.3f) * ((FOV.Value / 2.0f) / 100.0f));
                editlight.Angle = 0;
                editlight.Position = new Vector2(Windowsize.Center.X, Windowsize.Center.Y);
                editlight.RangeMin = radiusMin.Value;
                editlight.RangeMax = radiusMax.Value;
                editlight.RangeChange = 1;

                r.Box = new Rectangle(Windowsize.Right - offset, r.Box.Y, r.Box.Width, r.Box.Height);
                g.Box = OrderAlgorithms.nextbox(r.Box, 10, 0);
                b.Box = OrderAlgorithms.nextbox(g.Box, 10, 0);
                FOV.Box = OrderAlgorithms.nextbox(b.Box, 10, 0);
                radius.Box = OrderAlgorithms.nextbox(FOV.Box, 10, 0);
                intensity.Box = OrderAlgorithms.nextbox(radius.Box, 20, 0);

                r.Update();
                g.Update();
                b.Update();
                FOV.Update();
                radius.Update();
                intensity.Update();
                radiusMin.Update();
                radiusMax.Update();

                krypton.Lights[0] = editlight;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, Rectangle viewport, GameTime gameTime,
            GraphicsDevice device)
        {
            if (active == false)
                return;

            // Create FOV world view projection matrix to use with krypton
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateTranslation(new Vector3(0, 0, 0) * -1f);

            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            this.krypton.Matrix = Matrix.Identity;
            this.krypton.Bluriness = 0;
            this.krypton.LightMapPrepare(camera);

            spriteBatch.End();
            spriteBatch.Begin();
            //spriteBatch.Draw(texture, viewport, Color.White);
            device.Clear(Color.White);
            //spriteBatch.Draw(texture, viewport, Color.Gray);
            spriteBatch.Draw(texture, new Rectangle((int)editlight.X, (int)editlight.Y, 10, 10), editlight.Color);

            this.krypton.Draw(gameTime);

            r.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            g.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            b.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            FOV.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            radius.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            intensity.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            radiusMin.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
            radiusMax.Draw(spriteBatch, font1, viewport, new Point(viewport.X, viewport.Y));
        }
    }
}
