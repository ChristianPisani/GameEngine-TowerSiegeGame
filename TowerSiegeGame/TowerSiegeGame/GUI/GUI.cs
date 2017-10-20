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
    public class GUI
    {
        public List<Button> buttons = new List<Button>();
        public List<Slider> sliders = new List<Slider>();
        public List<CheckBox> checkBoxes = new List<CheckBox>();
        public List<RadioButton> radioButtons = new List<RadioButton>();
        public List<Textbox> textBoxes = new List<Textbox>();

        public Rectangle bounds = new Rectangle(),
                         maxBounds,
                         boundsprev = new Rectangle(),
                         scrollprev,
                         left,
                         right,
                         up,
                         down,
                         drawBounds;

        public Color backDropColor = Color.FromNonPremultiplied(30, 30, 30, 255);
        public Texture2D backDropTexture;

        public Button topBar,
                      scrollbar;
        public int barDifX,
                   barDifY,

                   spaceBetweenComponents = 10,

                   drawOffSetY = 0,
                   drawOffSetX = 0,
                   lastdrawoffsetX = 0,
                   lastdrawoffsetY = 0;

        private StreamReader streamreader;

        public string name;

        public bool active;

        public SpriteFont font;
        public Texture2D texture;

        private Random rnd;

        public bool holdingSlider = false,
                    moving = false,
                    movable = false,
                    
                    Resizing = false,
                    Resizable = false,
                    scrolling = false,
                    
                    inRange = false;

        public string ResizeDir;

        MouseState ms,
                   msprev;

        public GUI(SpriteFont font, Texture2D texture, Rectangle window, Random rnd, bool active)
        {
            this.font = font;
            this.texture = texture;
            this.rnd = rnd;
            bounds = window;
            this.active = active;

            SetPositions();
        }

        public GUI(Rectangle bounds, SpriteFont font, Texture2D texture, Rectangle window, Random rnd, int SpaceBetweenComponents, bool active)
        {
            //topBar = new Button(Rectangle.Empty, Color.Gray, Color.Gray, Color.Gray, font, "", 0);
            //scrollbar = new Button(Rectangle.Empty, Color.Gray, Color.Gray, Color.Gray, font, "", 0);
            this.bounds = bounds;
            this.topBar.bounds.Y = bounds.Y;
            this.topBar.bounds.X = bounds.X;
            this.topBar.bounds.Width = 200;
            this.topBar.bounds.Height = 20;
            this.font = font;
            this.texture = texture;
            this.rnd = rnd;
            this.spaceBetweenComponents = SpaceBetweenComponents;
            this.active = active;

            SetPositions();
        }

        public void Resize()
        {
            if (Resizable == false)
                return;

            Rectangle mouseRect = new Rectangle(ms.X, ms.Y, 1, 1);

            if (ms.LeftButton == ButtonState.Pressed && moving == false && Resizing == false && holdingSlider == false && scrolling == false)
            {
                msprev = ms;
                boundsprev = bounds;

                if (mouseRect.Intersects(right))
                {
                    ResizeDir = "right";
                    Resizing = true;
                }

                if (mouseRect.Intersects(left))
                {
                    ResizeDir = "left";
                    Resizing = true;
                }

                if (mouseRect.Intersects(up))
                {
                    ResizeDir = "up";
                    Resizing = true;
                }

                if (mouseRect.Intersects(down))
                {
                    ResizeDir = "down";
                    Resizing = true;
                }
            }

            drawBounds = bounds;

            left = new Rectangle(drawBounds.X, drawBounds.Y + 1, 2, drawBounds.Height - 2);
            right = new Rectangle(drawBounds.Right - 2, drawBounds.Y + 1, 2, drawBounds.Height - 2);
            up = new Rectangle(drawBounds.X + 1, drawBounds.Y, drawBounds.Width - 2, 2);
            down = new Rectangle(drawBounds.X + 1, drawBounds.Bottom - 2, drawBounds.Width - 2, 2);

            if (Resizing == true)
            {
                switch (ResizeDir.ToLower())
                {
                    case "right":
                        bounds.Width = (ms.X + drawBounds.Width / 2) - drawBounds.Center.X;

                        if (bounds.Width < 0)
                            SwitchResizeDir();
                        break;

                    case "left":
                        bounds.Width = (ms.X - drawBounds.Width / 2) - drawBounds.Center.X;

                        bounds.X = ms.X;
                        topBar.bounds.X = ms.X;

                        bounds.Width = ms.X - boundsprev.Right;

                        if (bounds.Width > 0)
                            SwitchResizeDir();
                        break;

                    case "up":
                        bounds.Height = (ms.Y + drawBounds.Height / 2) - drawBounds.Center.Y;

                        bounds.Y = ms.Y;
                        topBar.bounds.Y = ms.Y + 10;

                        bounds.Height = boundsprev.Bottom-ms.Y;

                        if (bounds.Height < 0)
                            SwitchResizeDir();
                        break;

                    case "down":
                        bounds.Height = (drawBounds.Y - ms.Y);

                        if (bounds.Height > 0)
                            SwitchResizeDir();
                        break;
                }
            }

            bounds.Width = (int)Math.Sqrt(bounds.Width * bounds.Width);
            bounds.Height = (int)Math.Sqrt(bounds.Height * bounds.Height);

            topBar.bounds.Width = bounds.Width;
        }

        private void SwitchResizeDir()
        {
            switch (ResizeDir.ToLower())
            {
                case "right":
                    ResizeDir = "left";
                    break;

                case "left":
                    ResizeDir = "right";
                    break;

                case "up":
                    ResizeDir = "down";
                    break;

                case "down":
                    ResizeDir = "up";
                    break;
            }
        }

        public virtual void Update(SpriteFont font, Rectangle window)
        {
            if (active == false)
            {
                holdingSlider = false;
                moving = false;

                Resizing = false;
                scrolling = false;

                inRange = false;

                return;
            }

            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Released)
            {
                inRange = false;
            }
            if (new Rectangle(ms.X, ms.Y, 1, 1).Intersects(bounds) || holdingSlider == true)
            {
                inRange = true;
            }

            if (topBar != null)
            {
                topBar.Update(font, new Point(topBar.bounds.Width, topBar.bounds.Height));
                Resize();
            }

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (movable == true && topBar.Pressed == true)
                {
                    if (moving == false && scrolling == false)
                    {
                        barDifX = ms.X - topBar.bounds.X;
                        barDifY = ms.Y - topBar.bounds.Y;
                    }

                    moving = true;
                }

                if (scrollbar != null)
                {
                    if (scrollbar.Pressed == true && Resizing == false)
                    {
                        if (scrolling == false && moving == false)
                        {
                            barDifY = ms.Y - scrollbar.bounds.Y;
                        }

                        scrolling = true;
                    }
                }
            }

            if (moving == true)
            {
                Rectangle dif = topBar.bounds;

                topBar.bounds.X = ms.X - barDifX;
                topBar.bounds.Y = ms.Y - barDifY;

                scrollbar.bounds.X += topBar.bounds.X - dif.X;
                scrollbar.bounds.Y += topBar.bounds.Y - dif.Y;

                topBar.bounds.X = (int)MathHelper.Clamp(topBar.bounds.X, -(topBar.bounds.Width - topBar.bounds.Width / 6), window.Width - topBar.bounds.Width / 6);
                topBar.bounds.Y = (int)MathHelper.Clamp(topBar.bounds.Y, -(topBar.bounds.Height - topBar.bounds.Height / 6), window.Height - topBar.bounds.Height / 6);
            }

            if (moving == false && Resizing == false && inRange == true && scrolling == false)
            {
                foreach (Button button in buttons)
                {
                    button.Update(font, new Point(button.bounds.Width, button.bounds.Height));
                }

                foreach (RadioButton radio in radioButtons)
                {
                    radio.Update(font);
                }

                foreach (CheckBox checkbox in checkBoxes)
                {
                    checkbox.Update(font, new Point(checkbox.bounds.Width, checkbox.bounds.Height));
                }

                foreach (Slider slider in sliders)
                {
                    if (holdingSlider == false || slider.HoldingSlider == true || ms.MiddleButton == ButtonState.Pressed)
                    {
                        slider.Update();

                        if (slider.HoldingSlider == true)
                            holdingSlider = true;
                    }
                }
            }
            if (moving == true || Resizing == true)
            {
                SetPositions();
            }

            if (ms.LeftButton == ButtonState.Released)
            {
                holdingSlider = false;
                moving = false;
                Resizing = false;
                scrolling = false;
            }

            if (topBar != null && Resizing == false)
            {
                bounds.X = topBar.bounds.X;
                bounds.Y = topBar.bounds.Y;
            }

            if (scrollbar != null)
            {
                scrollbar.Update(font, new Point(scrollbar.bounds.Width, scrollbar.bounds.Height));

                if (scrolling == true)
                    scrollbar.bounds.Y = ms.Y - barDifY;

                scrollbar.bounds.Height = bounds.Height - (maxBounds.Height - bounds.Height);
                scrollprev = scrollbar.bounds;
                scrollbar.bounds.Height = (int)MathHelper.Clamp(scrollbar.bounds.Height, 10, bounds.Height - topBar.bounds.Height);

                scrollbar.bounds.X = bounds.X + bounds.Width - scrollbar.bounds.Width;
                scrollbar.bounds.Y = (int)MathHelper.Clamp(scrollbar.bounds.Y, topBar.bounds.Y + topBar.bounds.Height, bounds.Y + bounds.Height - scrollbar.bounds.Height);
            }

            //bounds.Height = (int)MathHelper.Clamp(bounds.Height, 350, 3000);
        }

        public void SetPositions()
        {
            int i = 1;
            foreach (Slider slider in sliders)
            {
                slider.MoveToLocation(new Point(topBar.bounds.X + 5, topBar.bounds.Y + (slider.Box.Height + spaceBetweenComponents) * i + drawOffSetY));
                i++;
            }

            foreach (CheckBox checkbox in checkBoxes)
            {
                checkbox.bounds.X = topBar.bounds.X + 5;
                checkbox.bounds.Y = topBar.bounds.Y + (checkbox.bounds.Height + spaceBetweenComponents) * i + drawOffSetY;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font, Rectangle window)
        {
            SpriteBatch batch = spriteBatch;
            spriteBatch.End();

            Rectangle scissor = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            scissor.Width = (int)MathHelper.Clamp(scissor.Width, 1, 30000);
            scissor.Height = (int)MathHelper.Clamp(scissor.Height, 1, 30000);
            scissor.X = (int)MathHelper.Clamp(scissor.X, 0, window.Width - scissor.Width);
            scissor.Y = (int)MathHelper.Clamp(scissor.Y, 0, window.Height - scissor.Height);

            Rectangle view = spriteBatch.GraphicsDevice.Viewport.Bounds;
            spriteBatch.GraphicsDevice.Viewport = new Viewport(scissor.X, scissor.Y, scissor.Width, scissor.Height);
            Rectangle oldBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            bounds.X = bounds.X - scissor.X;
            bounds.Y = bounds.Y - scissor.Y;

            lastdrawoffsetY = drawOffSetY;
            if (scrollbar != null)
            {
                drawOffSetY = topBar.bounds.Bottom-scrollbar.bounds.Y;
            }

            spriteBatch.Begin();

            if (backDropTexture != null)
            {
                spriteBatch.Draw(backDropTexture, bounds, backDropColor);
            }

            foreach (Button button in buttons)
            {
                if (drawOffSetY != lastdrawoffsetY)
                {
                    button.bounds.Y = button.bounds.Y - lastdrawoffsetY + drawOffSetY;
                }
                button.Draw(spriteBatch, texture, font, new Point(scissor.X, scissor.Y));
            }

            foreach (RadioButton radio in radioButtons)
            {
                if (drawOffSetY != lastdrawoffsetY)
                {
                    foreach (Button button in radio.Buttons)
                    {
                        button.bounds.Y = button.bounds.Y - lastdrawoffsetY + drawOffSetY;
                    }
                }

                radio.Draw(spriteBatch, new Point(scissor.X, scissor.Y));
            }

            foreach (Slider slider in sliders)
            {
                slider.Box.Y = slider.Box.Y - lastdrawoffsetY + drawOffSetY;
                slider.SliderBtn.Y = slider.SliderBtn.Y - lastdrawoffsetY + drawOffSetY;
                slider.Draw(spriteBatch, font, window, new Point(scissor.X, scissor.Y));
            }

            foreach (CheckBox checkbox in checkBoxes)
            {
                checkbox.bounds.Y = checkbox.bounds.Y - lastdrawoffsetY + drawOffSetY;
                checkbox.Draw(spriteBatch, texture, font, new Point(scissor.X, scissor.Y));
            }

            if (scrollbar != null && scrollbar.bounds.Height < bounds.Height - topBar.bounds.Height)
                scrollbar.Draw(spriteBatch, texture, font, new Point(scissor.X, scissor.Y));

            if (topBar != null)
                topBar.Draw(spriteBatch, texture, font, new Point(scissor.X, scissor.Y));

            spriteBatch.End();

            batch.GraphicsDevice.Viewport = new Viewport(view.X, view.Y, view.Width, view.Height);
            batch.Begin();

            bounds = oldBounds;
        }

        public void LoadInterface(string ComputerName, string GUIName, ContentManager Content)
        {
            buttons.Clear();

            if (File.Exists(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\GUI\" + GUIName + ".gui") == true)
            {
                streamreader = new StreamReader(@"C:\Users\" + ComputerName + @"\Documents\my games\StealthGame\GUI\" + GUIName + ".gui");

                string text = streamreader.ReadToEnd();
                text = text.Replace(System.Environment.NewLine, "");

                List<string> GUIParts = new List<string>(text.Split(new string[] { "//Buttons", "//Sliders", "//CheckBoxes", "//RadioButtons", "//TextBoxes", "//Background" }, StringSplitOptions.None));
                List<string> ButtonList = new List<string>(GUIParts[1].Split(new string[] { "," }, StringSplitOptions.None));
                List<string> SliderList = new List<string>(GUIParts[2].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> CheckBoxList = new List<string>(GUIParts[3].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> RadioButtonList = new List<string>(GUIParts[4].Split(new string[] { "|" }, StringSplitOptions.None));
                List<string> TextBoxList = new List<string>(GUIParts[5].Split(new string[] { "|" }, StringSplitOptions.None));

                if (GUIParts[6] != "")
                {
                    backDropTexture = Content.Load<Texture2D>(GUIParts[6]);
                }

                int multiplyAmount = 19;
                for (int i = 0; i < ButtonList.Count / multiplyAmount; i++)
                {
                    Button button = new Button();

                    button.bounds.X = Convert.ToInt32(ButtonList[i * multiplyAmount]);
                    button.bounds.Y = Convert.ToInt32(ButtonList[i * multiplyAmount + 1]);
                    button.bounds.Width = Convert.ToInt32(ButtonList[i * multiplyAmount + 2]);
                    button.bounds.Height = Convert.ToInt32(ButtonList[i * multiplyAmount + 3]);
                    button.Text = (ButtonList[i * multiplyAmount + 4]);
                    button.name = (ButtonList[i * multiplyAmount + 5]);
                    button.buttoncolor.R = Convert.ToByte(ButtonList[i * multiplyAmount + 6]);
                    button.buttoncolor.G = Convert.ToByte(ButtonList[i * multiplyAmount + 7]);
                    button.buttoncolor.B = Convert.ToByte(ButtonList[i * multiplyAmount + 8]);
                    button.defaultcolor.R = Convert.ToByte(ButtonList[i * multiplyAmount + 6]);
                    button.defaultcolor.G = Convert.ToByte(ButtonList[i * multiplyAmount + 7]);
                    button.defaultcolor.B = Convert.ToByte(ButtonList[i * multiplyAmount + 8]);
                    button.textcolor.R = Convert.ToByte(ButtonList[i * multiplyAmount + 9]);
                    button.textcolor.G = Convert.ToByte(ButtonList[i * multiplyAmount + 10]);
                    button.textcolor.B = Convert.ToByte(ButtonList[i * multiplyAmount + 11]);
                    button.hovercolor.R = Convert.ToByte(ButtonList[i * multiplyAmount + 12]);
                    button.hovercolor.G = Convert.ToByte(ButtonList[i * multiplyAmount + 13]);
                    button.hovercolor.B = Convert.ToByte(ButtonList[i * multiplyAmount + 14]);
                    button.pressedcolor.R = Convert.ToByte(ButtonList[i * multiplyAmount + 15]);
                    button.pressedcolor.G = Convert.ToByte(ButtonList[i * multiplyAmount + 16]);
                    button.pressedcolor.B = Convert.ToByte(ButtonList[i * multiplyAmount + 17]);

                    button.buttoncolor.A = Convert.ToByte(ButtonList[i * multiplyAmount + 18]);
                    button.defaultcolor.A = Convert.ToByte(ButtonList[i * multiplyAmount + 18]);
                    button.textcolor.A = Convert.ToByte(ButtonList[i * multiplyAmount + 18]);
                    button.hovercolor.A = Convert.ToByte(ButtonList[i * multiplyAmount + 18]);
                    button.pressedcolor.A = Convert.ToByte(ButtonList[i * multiplyAmount + 18]);

                    buttons.Add(button);
                }
            }
        }
    }
}
