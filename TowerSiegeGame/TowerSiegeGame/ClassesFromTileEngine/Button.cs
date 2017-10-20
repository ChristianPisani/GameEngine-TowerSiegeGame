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
using System.IO;

namespace TowerSiegeGame
{
    public class Button
    {
        public Rectangle bounds;

        public Vector2 textpos;

        public bool Pressed,
                    rightPressed,
                    Hovered,
                    mousedown,
                    rightMousedown,
                    Clicked,
                    rightClicked;

        public string Text;

        int btnaction = 1;

        public float textOffSet = 10,
                     textScale = 1.0f;

        public Color textcolor,
                     buttoncolor,
                     defaultcolor,
                     hovercolor,
                     pressedcolor;

        public MouseState ms = Mouse.GetState();

        private StreamReader streamreader;
        public string name;

        public Button()
        { 
        
        }

        public Button(Rectangle button, Color buttoncolor, Color hovercolor, 
            Color pressedcolor, SpriteFont Font, string Text, int ButtonAction)
        {
            bounds = button;
            this.defaultcolor = buttoncolor;
            this.hovercolor = hovercolor;
            this.pressedcolor = pressedcolor;
            this.Text = Text;

            if (Font != null)
            {
                textpos = new Vector2(bounds.Width / 2 + Font.MeasureString(Text).X / 2, bounds.Height / 2 + Font.MeasureString(Text).Y / 2);
            }
            btnaction = ButtonAction;
        }

        public void Update(SpriteFont Font)
        {
            Update(Font, new Point(bounds.Width, bounds.Height));
        }

        public void Update(SpriteFont Font, Point Size)
        {
            ms = Mouse.GetState();

            bounds.Width = Size.X;
            bounds.Height = Size.Y;

            buttoncolor = defaultcolor;

            Clicked = false;

            if (new Rectangle(ms.X, ms.Y, 1, 1).Intersects(bounds))
            {
                buttoncolor = hovercolor;
                Hovered = true;

                if (ms.RightButton == ButtonState.Pressed && rightMousedown == false)
                {
                    rightPressed = true;
                }

                if (ms.LeftButton == ButtonState.Pressed && mousedown == false)
                {
                    Pressed = true;
                }
            }
            else {
                Hovered = false;
            }

            if (Pressed == true)
                buttoncolor = pressedcolor;

            if (ms.LeftButton == ButtonState.Released)
            {
                if (Pressed == true && Hovered == true)
                {
                    OnClick(ms.LeftButton);
                }

                Pressed = false;
            }

            if (ms.RightButton == ButtonState.Released)
            {
                if (rightPressed == true && Hovered == true)
                {
                    OnClick(ms.RightButton);
                }

                rightPressed = false;
            }

            if (ms.LeftButton == ButtonState.Pressed)
                mousedown = true;
            else
                mousedown = false;

            if (ms.RightButton == ButtonState.Pressed)
                rightMousedown = true;
            else
                rightMousedown = false;

            if (Font != null)
            {
                textpos = new Vector2(bounds.X + (bounds.Width / 2 - Font.MeasureString(Text).X / 2), bounds.Y + (bounds.Height / 2 - Font.MeasureString(Text).Y / 2));
            }

            ExtraUpdates();
        }

        public virtual void ExtraUpdates()
        { 
        
        }

        public virtual void OnClick(ButtonState state)
        {
            if (state == ms.LeftButton)
                Clicked = true;

            if (state == ms.RightButton)
                rightClicked = true;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, Point origin)
        {
            spriteBatch.Draw(texture, new Rectangle(bounds.X - origin.X, bounds.Y - origin.Y, bounds.Width, bounds.Height), buttoncolor);
            spriteBatch.DrawString(font, Text, new Vector2((bounds.X + textOffSet) - origin.X, (bounds.Center.Y - ((font.MeasureString(Text).Y * textScale) / 2)) - origin.Y), textcolor, 
                0, Vector2.Zero, textScale, SpriteEffects.None, 0);
        }
    }
}
