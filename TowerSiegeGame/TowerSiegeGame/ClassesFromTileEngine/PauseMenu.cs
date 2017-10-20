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
    public class PauseMenu
    {
        public Button options,
                      resume,
                      quit;

        public SpriteFont font1;

        public bool CanPressEsc = true;

        public PauseMenu(Rectangle screensize, Color buttoncolor, Color hovercolor, Color pressedcolor, SpriteFont Font)
        {
            resume = new Button(new Rectangle(0, screensize.Height / 3, screensize.Width, 100), buttoncolor, hovercolor, pressedcolor, Font, "Resume", 1);
            options = new Button(OrderAlgorithms.nextbox(resume.bounds, 50, 0), buttoncolor, hovercolor, pressedcolor, Font, "Options", 1);
            quit = new Button(OrderAlgorithms.nextbox(options.bounds, 50, 0), buttoncolor, hovercolor, pressedcolor, Font, "Exit", 0);

            this.font1 = Font;
        }

        public void Update(Rectangle screensize, bool realShowPauseMenu, out bool showPauseMenu)
        {
            showPauseMenu = realShowPauseMenu;

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
            {
                if (CanPressEsc == true)
                {
                    if (showPauseMenu == false)
                    {
                        showPauseMenu = true;
                    }
                    else
                    {
                        showPauseMenu = false;
                    }

                    CanPressEsc = false;
                }
            }
            else
            {
                CanPressEsc = true;
            }

            if (showPauseMenu == true)
            {
                resume.bounds.Y = screensize.Height / 2 - (resume.bounds.Height + (resume.bounds.Height / 2));
                options.bounds.Y = OrderAlgorithms.nextbox(resume.bounds, 50, 0).Y;
                quit.bounds.Y = OrderAlgorithms.nextbox(options.bounds, 50, 0).Y;

                options.Update(font1, new Point(screensize.Width, 100));
                resume.Update(font1, new Point(screensize.Width, 100));
                quit.Update(font1, new Point(screensize.Width, 100));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, options.bounds, options.buttoncolor);
            spriteBatch.Draw(texture, resume.bounds, resume.buttoncolor);
            spriteBatch.Draw(texture, quit.bounds, quit.buttoncolor);

            spriteBatch.DrawString(font1, resume.Text, resume.textpos, Color.Wheat);
            spriteBatch.DrawString(font1, options.Text, options.textpos, Color.Wheat);
            spriteBatch.DrawString(font1, quit.Text, quit.textpos, Color.Wheat);
        }
    }
}
