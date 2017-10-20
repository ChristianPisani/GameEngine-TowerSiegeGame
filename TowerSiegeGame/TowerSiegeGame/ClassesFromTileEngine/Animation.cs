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
    class Animation
    {
        public float time = 0,
                     speed = 0;

        public string name = "default";

        public int CurrentFrame = 0,
                     LastFrame = 0,
                     frames = 0;
        public Rectangle sRect,
                         dRect;
        public bool Animating = false,
                    Loop      = true,
                    Finished  = false;

        public Texture2D texture;

        public Animation(Texture2D texture, int frames, float speed, Rectangle sRect, Rectangle dRect,
            string name)
        {
            this.texture = texture;
            this.frames = frames;
            this.speed = speed;
            this.sRect = sRect;
            this.name = name;
            this.dRect = dRect;
        }

        public void Animate()
        {
            Animating = true;

            time += (1.0f/60.0f);

            sRect.X = sRect.Width * CurrentFrame;

            if (time >= speed)
            {
                CurrentFrame += 1;
                time = 0;

                if (CurrentFrame >= frames)
                {
                    Animating = false;
                    CurrentFrame = 0;
                }
            }
        }
    }
}
