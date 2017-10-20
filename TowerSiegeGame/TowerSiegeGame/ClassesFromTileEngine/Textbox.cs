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
    public class Textbox
    {
        public string Text = "",
                      Title = "Textbox",
                      substring = "";

        public bool Writable = true,
                    CanLeft  = true,
                    CanRight = true,
                    UpdateKeys = true,
                    active = true;

        public Point position,
                     titleposition,
                     actualpos,
                     textposition = new Point(0, 0),
                     MinimumSize = new Point(300, 40),
                     Size = new Point(300, 40);

        Timer deletetimer = new Timer((0.01f * 60f) * 0.5f),
              updateleft = new Timer((0.01f * 60f) * 0.5f),
              updateright = new Timer((0.01f * 60f) * 0.5f);

        SpriteFont font1;

        public Vector2 writepos = new Vector2(0, 0);

        public int posinstring = 0,
                   linenumber = 0;

        public Color bordercolor = Color.Yellow,
                     bodycolor = Color.DarkGreen;

        public TextInput textinput = new TextInput();

        SpriteBatch spriteBatch;
        public Texture2D texture;

        public Textbox(SpriteFont font1, SpriteBatch spriteBatch, Texture2D texture, Rectangle bounds)
        {
            this.font1 = font1;
            this.spriteBatch = spriteBatch;
            this.texture = texture;

            this.actualpos.X = bounds.X;
            this.actualpos.Y = bounds.Y;
            this.Size.X = bounds.Width;
            this.Size.Y = bounds.Height;
        }

        public void Update(bool typingMode)
        {
            deletetimer.Update();
            updateleft.Update();
            updateright.Update();

            if (deletetimer.active == false)
                textinput.CanBackSpace = true;

            if (updateleft.active == false)
                CanLeft = true;

            if (updateright.active == false)
                CanRight = true;

            if (typingMode == true)
            {
                textinput.Update();

                textinput.keystate = Keyboard.GetState();
                if (textinput.keystate.IsKeyDown(Keys.Back))
                {
                    if (textinput.CanBackSpace == true)
                    {
                        if (posinstring > 0)
                        {
                            Text = Text.Remove(posinstring - 1, 1);
                            posinstring -= 1;
                        }
                        textinput.CanBackSpace = false;
                    }
                }
                else
                {
                    deletetimer.Activate();
                    textinput.CanBackSpace = true;
                }
            }

            position = new Point(actualpos.X - (Size.X / 2), actualpos.Y - (Size.Y / 2));

            titleposition = new Point(position.X, position.Y - (int)font1.MeasureString(Title).Y);

            Size = new Point((int)font1.MeasureString(Text).X, (int)font1.MeasureString(Text).Y);
            if (Size.X < MinimumSize.X)
            {
                Size.X = MinimumSize.X;
            }

            if (Size.Y < MinimumSize.Y)
            {
                Size.Y = MinimumSize.Y;
            }

            textposition.X = position.X + (Size.X / 2) - (int)font1.MeasureString(Text).X / 2;
            textposition.Y = position.Y + (Size.Y / 2) - (int)font1.MeasureString(Text).Y / 2;

            if(textinput.TextString.Length > 0)
                Text = Text.Insert(posinstring, textinput.TextString);

            posinstring += textinput.TextString.Length;

            textinput.keystate = Keyboard.GetState();
            if (textinput.keystate.IsKeyDown(Keys.Left) == true)
            {
                if (posinstring > 0 && CanLeft == true)
                {
                    posinstring -= 1;
                    CanLeft = false;
                }
            }
            else {
                updateleft.Activate();
                CanLeft = true;
            }

            if (textinput.keystate.IsKeyDown(Keys.Right) == true)
            {
                if (posinstring < Text.Length && CanRight == true)
                {
                    posinstring += 1;
                    CanRight = false;
                }
            }
            else {
                updateright.Activate();
                CanRight = true;
            }

            substring = Text.Substring(0, posinstring);

            List<string> Line = new List<string>(substring.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None));

            substring = Line[Line.Count - 1];

            int t = (int)font1.MeasureString(Text.Substring(0, posinstring)).Y;
            int b = (int)font1.MeasureString("|").Y;

            if (posinstring == 0)
            {
                b = 0;
            }

            if ((int)font1.MeasureString(Line[Line.Count - 1]).Y == 0 && posinstring > 0)
            {
                b = (int)(font1.MeasureString("|").Y * 0.9f);
            }

            //Title = (font1.MeasureString(Line[Line.Count - 1]).Y).ToString();

            writepos = new Vector2(font1.MeasureString(substring).X + textposition.X - font1.MeasureString("|").X / 2,
                textposition.Y - ((font1.MeasureString("|").Y * 0.1f)) + t - b);
        }

        public void Draw()
        {
            if (active == false)
                return;

            spriteBatch.Draw(texture, new Rectangle(position.X - 4, position.Y, Size.X + 8, Size.Y), bodycolor);
            spriteBatch.Draw(texture, new Rectangle(position.X - 4, position.Y, 2, Size.Y), bordercolor);
            spriteBatch.Draw(texture, new Rectangle(position.X + Size.X + 4, position.Y, 2, Size.Y + 1), bordercolor);
            spriteBatch.Draw(texture, new Rectangle(position.X - 4, position.Y, Size.X + 8, 1), bordercolor);
            spriteBatch.Draw(texture, new Rectangle(position.X - 4, position.Y + Size.Y, Size.X + 8, 1), bordercolor);

            spriteBatch.DrawString(font1, Title, new Vector2(titleposition.X, titleposition.Y), Color.Wheat);
            spriteBatch.DrawString(font1, Text, new Vector2(textposition.X, textposition.Y), Color.Wheat);

            if (Text.Length > 0)
                spriteBatch.DrawString(font1, "|", new Vector2(writepos.X, writepos.Y), Color.White);
        }
    }
}
