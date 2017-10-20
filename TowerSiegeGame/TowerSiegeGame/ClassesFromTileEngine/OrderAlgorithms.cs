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
    class OrderAlgorithms
    {
        public static Rectangle nextbox(Rectangle firstbox, int Spacing, int layout)
        {
            Rectangle box = new Rectangle();

            if (layout == 0)
            {
                box = new Rectangle(firstbox.X, firstbox.Y + firstbox.Height + Spacing, firstbox.Width, firstbox.Height);
            }
            else
            {
                box = new Rectangle(firstbox.X + firstbox.Width + Spacing, firstbox.Y, firstbox.Width, firstbox.Height);
            }

            return box;
        }
    }
}
