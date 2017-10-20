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
    public class RadioButton
    {
        public List<CheckBox> Buttons = new List<CheckBox>();

        public Vector2 textpos;

        public bool Hovered;

        public string Text;

        public Color textcolor;

        public int CheckedState = 0;

        MouseState ms;

        Texture2D texture;
        SpriteFont font1;

        public RadioButton
            (Color textcolor,
             SpriteFont Font, string Text, int numberOfButtons, int spacing, string buttonLayOut, int FirstChecked, Texture2D texture,
            Color checkedColor, Color uncheckedColor, Color baseColor, Color hoverColor, Color pressedColor,
            Rectangle button)
        {
            this.texture = texture;
            this.font1 = Font;
            this.textcolor = textcolor;
            this.Text = Text;

            for (int i = 0; i < numberOfButtons; i++)
            {
                CheckBox check = new CheckBox(checkedColor, uncheckedColor, button, baseColor,
                    hoverColor, pressedColor, font1, "", 0, false);
                check.textcolor = textcolor;
                this.Buttons.Add(check);

                if (buttonLayOut.ToLower() == "vertical")
                {
                    button = OrderAlgorithms.nextbox(button, spacing, 0);
                }
                else
                {
                    button = OrderAlgorithms.nextbox(button, spacing, 1);
                }
            }

            if (this.Buttons.Count > FirstChecked)
            {
                this.Buttons[FirstChecked].Checked = true;
                CheckedState = FirstChecked;
            }
        }

        public void Update(SpriteFont Font)
        {
            ms = Mouse.GetState();
            Hovered = false;

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].Update(font1, new Point(Buttons[i].bounds.Width, Buttons[i].bounds.Height));

                if (Buttons[i].Hovered == true)
                    Hovered = true;

                if (Buttons[i].Pressed == true && new Rectangle(ms.X, ms.Y, 1, 1).Intersects(Buttons[i].bounds))
                {
                    for (int o = 0; o < Buttons.Count; o++)
                    {
                        Buttons[o].Checked = false;
                    }

                    Buttons[i].Checked = true;
                    CheckedState = i;
                }

                Buttons[i].ExtraUpdates();
            }

            ExtraUpdates();
        }

        public void ExtraUpdates()
        {

        }

        public void OnClick()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Point origin)
        {
            foreach(CheckBox check in Buttons)
            {
                check.Draw(spriteBatch, texture, font1, origin);
            }
        }
    }
}
