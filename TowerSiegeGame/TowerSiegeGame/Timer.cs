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
    public class Timer
    {
        public float Time,
              Length;

        public bool active = false,
                    stopped = false,
                    looping = false;

        public Timer(float Length)
        {
            this.Length = Length;

            Activate();
        }

        public void Update()
        {
            if(active == true && stopped == false)
                Time += 0.01f;

            if (Time >= Length)
            {
                active = false;
                stopped = true;

                if (looping == true)
                    Activate();
            }
        }

        public void Activate()
        {
            stopped = false;
            active = true;
            Time = 0;
        }

        public void DeActivate()
        {
            stopped = true;
            active = false;
            Time = Length;
        }
    }
}
