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
    public class Floor
    {
        public Rectangle bounds;

        public bool lightsOn = true;

        public float Intensity = 3.0f;

        public Color lightColor = Color.FromNonPremultiplied(255, 255, 255, 255);

        public Texture2D lightTexture;

        public Switch lightSwitch;

        public Floor(Rectangle bounds, Texture2D lightTexture, Random rnd, List<ElectronicComponent> electronics, KryptonEngine krypton, List<Switch> switches)
        {
            this.bounds = bounds;
            this.lightTexture = lightTexture;

            Object compOn = new Object(new Rectangle(bounds.X + bounds.Width / 3, bounds.Bottom - 30, 10, 7), 0, false, false, false, krypton, false);
            compOn.color = Color.Green;
            compOn.physicsEnabled = false;

            //lightSwitch = new Switch(lightTexture, Color.Gray, true, 0, 0, rnd, electronics, compOn, compOn, true, krypton, false);
            //lightSwitch.physicsEnabled = false;
            //lightSwitch.gravity = 0;
            //lightSwitch.type = "transmitter";
            //switches.Add(lightSwitch);
        }

        public void Update(KeyboardState ks, Player player, List<ElectronicComponent> electronics, List<Switch> switches)
        {
            //lightSwitch.Update(ks, player, electronics);

            //lightsOn = lightSwitch.Clicked;
        }

        public void Draw(SpriteBatch spritebatch, bool DebugDraw, Camera camera)
        {
            if (lightsOn == true)
            {
                spritebatch.Draw(lightTexture, new Rectangle(bounds.X - (int)camera.Position.X,
                    bounds.Y - (int)camera.Position.Y, bounds.Width, bounds.Height),
                    lightColor);
            }
        }
    }
}
