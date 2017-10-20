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
    public class CheckBox : Button
    {
        public bool Checked = false,
                    CanBeChecked = true;

        public Color uncheckedcolor,
                     checkedcolor;

        public CheckBox(Color checkedcolor, Color uncheckedcolor, 
            Rectangle button, Color buttoncolor, Color hovercolor, Color pressedcolor, 
            SpriteFont Font, string Text, int ButtonAction, bool CanBeChecked) 
            : base (button, buttoncolor, hovercolor, pressedcolor, Font, Text, ButtonAction)
        {
            this.checkedcolor = checkedcolor;
            this.uncheckedcolor = uncheckedcolor;
            this.CanBeChecked = CanBeChecked;
        }

        public override void ExtraUpdates()
        {
            if (Checked == true)
                defaultcolor = checkedcolor;
            else
                defaultcolor = uncheckedcolor;

            buttoncolor = defaultcolor;

            if (Hovered == true && Pressed == false && Checked == false)
                buttoncolor = hovercolor;
        }

        public override void OnClick(ButtonState state)
        {
            if (CanBeChecked == true)
            {
                if (Checked == true)
                    Checked = false;
                else
                    Checked = true;
            }
        }
    }
}
