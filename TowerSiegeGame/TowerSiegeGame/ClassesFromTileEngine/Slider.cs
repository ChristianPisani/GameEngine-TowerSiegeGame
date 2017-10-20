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
    public class Slider
    {
        public Rectangle Box;
        public Rectangle SliderBtn;
        float MaxValue;
        float MinValue;
        public float value;
        public int Value;
        public bool HoldingSlider;

        Texture2D boxTex,
                  sliderTex;

        string Text;

        public Slider(Rectangle Box, int draggerWidth, float MaxValue, float MinValue,
            Texture2D boxTex, Texture2D sliderTex, string Text)
        {
            this.Box = Box;
            this.SliderBtn = new Rectangle(Box.Left, Box.Y, draggerWidth, Box.Height);
            this.MaxValue = MaxValue;
            this.MinValue = MinValue;
            this.boxTex = boxTex;
            this.sliderTex = sliderTex;
            this.Text = Text;
        }

        public void SetValue(float value)
        {
            this.Value = (int)value;
            this.value = value;

            SliderBtn.X = (int)(Box.X + (((Box.Width - (SliderBtn.Width / 2.0f)) / (MaxValue - MinValue)) * (value - MinValue)));
            SliderBtn.Y = Box.Y;
        }

        public void MoveToLocation(Point location)
        {
            Box.X = location.X;
            Box.Y = location.Y;
            SetValue(value);
        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();

            if (new Rectangle(ms.X, ms.Y, 1, 1).Intersects(Box) && ms.LeftButton == ButtonState.Pressed)
            {
                HoldingSlider = true;
            }

            if (HoldingSlider == true)
                SliderBtn.X = ms.X - SliderBtn.Width / 2;

            if (SliderBtn.X < Box.Left)
                SliderBtn.X = Box.Left;

            if (SliderBtn.X + SliderBtn.Width > Box.Right)
                SliderBtn.X = Box.Right - SliderBtn.Width;

            if (ms.LeftButton == ButtonState.Released)
                HoldingSlider = false;

            Value = (int)(MinValue + ((SliderBtn.X - Box.X) * ((float)(MaxValue - MinValue) / ((float)Box.Width - SliderBtn.Width))));
            value = (MinValue + ((SliderBtn.X - Box.X) * ((float)(MaxValue - MinValue) / ((float)Box.Width - SliderBtn.Width))));
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Rectangle window, Point origin)
        {
            spriteBatch.Draw(boxTex, new Rectangle(Box.X - origin.X, Box.Y - origin.Y, Box.Width, Box.Height), Color.Black);
            spriteBatch.Draw(sliderTex, new Rectangle(SliderBtn.X - origin.X, SliderBtn.Y - origin.Y, SliderBtn.Width, SliderBtn.Height), Color.White);

            float scale = 0.6f;
            spriteBatch.DrawString(font, Text, new Vector2((Box.X) - origin.X, ((Box.Y + 2 - (font.MeasureString(Text).Y) * scale)) - origin.Y), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            if (value <= 2 && value >= -2)
            {
                spriteBatch.DrawString(font, value.ToString(), new Vector2((Box.Right + 2) - origin.X, (Box.Y + Box.Height / 2 - font.MeasureString(value.ToString()).Y / 2) - origin.Y), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font, Value.ToString(), new Vector2((Box.Right + 2) - origin.X, (Box.Y + Box.Height / 2 - font.MeasureString(value.ToString()).Y / 2) - origin.Y), Color.White);
            }
        }
    }
}
