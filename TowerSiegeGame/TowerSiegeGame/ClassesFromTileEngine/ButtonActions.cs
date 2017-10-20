using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowerSiegeGame
{
    class ButtonActions
    {
        public static void ExecuteAction(int i)
        {
            switch (i)
            { 
                case 0:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
