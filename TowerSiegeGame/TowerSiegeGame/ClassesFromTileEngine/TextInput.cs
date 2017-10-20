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
    public class TextInput
    {
        public KeyboardState keystate = Keyboard.GetState();
        public string TextString = "",
                      OldTextString = "";
        public Keys key = Keys.S;

        public bool CanPressA = true,
                           CanPressB = true,
                           CanPressC = true,
                           CanPressD = true,
                           CanPressE = true,
                           CanPressF = true,
                           CanPressG = true,
                           CanPressH = true,
                           CanPressI = true,
                           CanPressJ = true,
                           CanPressK = true,
                           CanPressL = true,
                           CanPressM = true,
                           CanPressN = true,
                           CanPressO = true,
                           CanPressP = true,
                           CanPressQ = true,
                           CanPressR = true,
                           CanPressS = true,
                           CanPressT = true,
                           CanPressU = true,
                           CanPressV = true,
                           CanPressW = true,
                           CanPressX = true,
                           CanPressY = true,
                           CanPressZ = true,
                           CanPressÆ = true,
                           CanPressØ = true,
                           CanPressÅ = true,
                           CanPunctuate = true,
                           CanDecimal = true,
                           CanQuestionMark = true,
                           CanExclamation = true,
                           CanBackSpace = true,
                           CanSpace = true,
                           CanEnter = true,
                           CanPlus = true,
                           CanOne = true,
                           CanTwo = true,
                           CanThree = true,
                           CanFour = true,
                           CanFive = true,
                           CanSix = true,
                           CanSeven = true,
                           CanEight = true,
                           CanNine = true,
                           CanZero = true,
                           CanApostrophe = true;

        public bool OneLine = false,
                    Finished = false;

        public void Update()
        {
            keystate = Keyboard.GetState();

            OldTextString = TextString;
            TextString = "";

            CheckInput();
            CheckModifiers();
        }

        private void CheckModifiers()
        {
            keystate = Keyboard.GetState();

            if (keystate.IsKeyDown(Keys.Space))
            {
                if (CanSpace == true)
                {
                    TextString += " ";
                    CanSpace = false;
                }
            }
            else {
                CanSpace = true;
            }

            if (keystate.IsKeyDown(Keys.Enter))
            {
                if (CanEnter == true)
                {
                    if (OneLine == false)
                    {
                        TextString += System.Environment.NewLine;
                    }
                    else {
                        Finished = true;
                    }
                    CanEnter = false;
                }
            }
            else
            {
                CanEnter = true;
            }
        }

        private void CheckInput()
        {
            string Text = "";

            if (keystate.IsKeyDown(Keys.A))
            {
                if (CanPressA)
                {
                    Text += "a";
                    CanPressA = false;
                }
            }
            else
            {
                CanPressA = true;
            }

            if (keystate.IsKeyDown(Keys.B))
            {
                if (CanPressB)
                {
                    Text += "b";
                    CanPressB = false;
                }
            }
            else
            {
                CanPressB = true;
            }

            if (keystate.IsKeyDown(Keys.C))
            {
                if (CanPressC)
                {
                    Text += "c";
                    CanPressC = false;
                }
            }
            else
            {
                CanPressC = true;
            }

            if (keystate.IsKeyDown(Keys.D))
            {
                if (CanPressD)
                {
                    Text += "d";
                    CanPressD = false;
                }
            }
            else
            {
                CanPressD = true;
            }

            if (keystate.IsKeyDown(Keys.E))
            {
                if (CanPressE)
                {
                    Text += "e";
                    CanPressE = false;
                }
            }
            else
            {
                CanPressE = true;
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                if (CanPressF)
                {
                    Text += "f";
                    CanPressF = false;
                }
            }
            else
            {
                CanPressF = true;
            }

            if (keystate.IsKeyDown(Keys.G))
            {
                if (CanPressG)
                {
                    Text += "g";
                    CanPressG = false;
                }
            }
            else
            {
                CanPressG = true;
            }

            if (keystate.IsKeyDown(Keys.H))
            {
                if (CanPressH)
                {
                    Text += "h";
                    CanPressH = false;
                }
            }
            else
            {
                CanPressH = true;
            }

            if (keystate.IsKeyDown(Keys.I))
            {
                if (CanPressI)
                {
                    Text += "i";
                    CanPressI = false;
                }
            }
            else
            {
                CanPressI = true;
            }

            if (keystate.IsKeyDown(Keys.J))
            {
                if (CanPressJ)
                {
                    Text += "j";
                    CanPressJ = false;
                }
            }
            else
            {
                CanPressJ = true;
            }

            if (keystate.IsKeyDown(Keys.K))
            {
                if (CanPressK)
                {
                    Text += "k";
                    CanPressK = false;
                }
            }
            else
            {
                CanPressK = true;
            }

            if (keystate.IsKeyDown(Keys.L))
            {
                if (CanPressL)
                {
                    Text += "l";
                    CanPressL = false;
                }
            }
            else
            {
                CanPressL = true;
            }

            if (keystate.IsKeyDown(Keys.M))
            {
                if (CanPressM)
                {
                    Text += "m";
                    CanPressM = false;
                }
            }
            else
            {
                CanPressM = true;
            }

            if (keystate.IsKeyDown(Keys.N))
            {
                if (CanPressN)
                {
                    Text += "n";
                    CanPressN = false;
                }
            }
            else
            {
                CanPressN = true;
            }

            if (keystate.IsKeyDown(Keys.O))
            {
                if (CanPressO)
                {
                    Text += "o";
                    CanPressO = false;
                }
            }
            else
            {
                CanPressO = true;
            }

            if (keystate.IsKeyDown(Keys.P))
            {
                if (CanPressP)
                {
                    Text += "p";
                    CanPressP = false;
                }
            }
            else
            {
                CanPressP = true;
            }

            if (keystate.IsKeyDown(Keys.Q))
            {
                if (CanPressQ)
                {
                    Text += "q";
                    CanPressQ = false;
                }
            }
            else
            {
                CanPressQ = true;
            }

            if (keystate.IsKeyDown(Keys.R))
            {
                if (CanPressR)
                {
                    Text += "r";
                    CanPressR = false;
                }
            }
            else
            {
                CanPressR = true;
            }

            if (keystate.IsKeyDown(Keys.S))
            {
                if (CanPressS)
                {
                    Text += "s";
                    CanPressS = false;
                }
            }
            else
            {
                CanPressS = true;
            }

            if (keystate.IsKeyDown(Keys.T))
            {
                if (CanPressT)
                {
                    Text += "t";
                    CanPressT = false;
                }
            }
            else
            {
                CanPressT = true;
            }

            if (keystate.IsKeyDown(Keys.U))
            {
                if (CanPressU)
                {
                    Text += "u";
                    CanPressU = false;
                }
            }
            else
            {
                CanPressU = true;
            }

            if (keystate.IsKeyDown(Keys.V))
            {
                if (CanPressV)
                {
                    Text += "v";
                    CanPressV = false;
                }
            }
            else
            {
                CanPressV = true;
            }

            if (keystate.IsKeyDown(Keys.W))
            {
                if (CanPressW)
                {
                    Text += "w";
                    CanPressW = false;
                }
            }
            else
            {
                CanPressW = true;
            }

            if (keystate.IsKeyDown(Keys.X))
            {
                if (CanPressX)
                {
                    Text += "x";
                    CanPressX = false;
                }
            }
            else
            {
                CanPressX = true;
            }

            if (keystate.IsKeyDown(Keys.Y))
            {
                if (CanPressY)
                {
                    Text += "y";
                    CanPressY = false;
                }
            }
            else
            {
                CanPressY = true;
            }

            if (keystate.IsKeyDown(Keys.Z))
            {
                if (CanPressZ)
                {
                    Text += "z";
                    CanPressZ = false;
                }
            }
            else
            {
                CanPressZ = true;
            }

            if (keystate.IsKeyDown(Keys.OemPeriod))
            {
                if (CanPunctuate)
                {
                    Text += ".";
                    CanPunctuate = false;
                }
            }
            else
            {
                CanPunctuate = true;
            }

            if (keystate.IsKeyDown(Keys.Multiply))
            {
                if (CanApostrophe == true)
                {
                    Text += "'";
                    CanApostrophe = false;
                }
            }
            else
            {
                CanApostrophe = true;
            }

            if (keystate.IsKeyDown(Keys.OemComma))
            {
                if (CanDecimal == true)
                {
                    Text += ",";
                    CanDecimal = false;
                }
            }
            else
            {
                CanDecimal = true;
            }

            if (keystate.IsKeyDown(Keys.OemPlus))
            {
                if ((keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift)))
                {
                    if (CanQuestionMark == true)
                    {
                        Text += "?";
                        CanQuestionMark = false;
                        CanPlus = false;
                    }
                }
                else {
                    if (CanPlus == true)
                    {
                        Text += "+";
                        CanPlus = false;
                        CanQuestionMark = false;
                    }
                }
            }
            else {
                CanQuestionMark = true;
                CanPlus = true;
            }

            if (keystate.IsKeyDown(Keys.D1))
            {
                if ((keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift)))
                {
                    if (CanExclamation == true)
                    {
                        Text += "!";
                        CanExclamation = false;
                        CanOne = false;
                    }
                }
                else
                {
                    if (CanOne == true)
                    {
                        Text += "1";
                        CanOne = false;
                        CanExclamation = false;
                    }
                }
            }
            else
            {
                CanExclamation = true;
                CanOne = true;
            }

            if (keystate.IsKeyDown(Keys.D2))
            {
                if (CanTwo)
                {
                    Text += "2";
                    CanTwo = false;
                }
            }
            else
            {
                CanTwo = true;
            }

            if (keystate.IsKeyDown(Keys.D3))
            {
                if (CanThree)
                {
                    Text += "3";
                    CanThree = false;
                }
            }
            else
            {
                CanThree = true;
            }

            if (keystate.IsKeyDown(Keys.D4))
            {
                if (CanFour)
                {
                    Text += "4";
                    CanFour = false;
                }
            }
            else
            {
                CanFour = true;
            }

            if (keystate.IsKeyDown(Keys.D5))
            {
                if (CanFive)
                {
                    Text += "5";
                    CanFive = false;
                }
            }
            else
            {
                CanFive = true;
            }

            if (keystate.IsKeyDown(Keys.D6))
            {
                if (CanSix)
                {
                    Text += "6";
                    CanSix = false;
                }
            }
            else
            {
                CanSix = true;
            }

            if (keystate.IsKeyDown(Keys.D7))
            {
                if (CanSeven)
                {
                    Text += "7";
                    CanSeven = false;
                }
            }
            else
            {
                CanSeven = true;
            }

            if (keystate.IsKeyDown(Keys.D8))
            {
                if (CanEight)
                {
                    Text += "8";
                    CanEight = false;
                }
            }
            else
            {
                CanEight = true;
            }

            if (keystate.IsKeyDown(Keys.D9))
            {
                if (CanNine)
                {
                    Text += "9";
                    CanNine = false;
                }
            }
            else
            {
                CanNine = true;
            }

            if (keystate.IsKeyDown(Keys.D0))
            {
                if (CanZero)
                {
                    Text += "0";
                    CanZero = false;
                }
            }
            else
            {
                CanZero = true;
            }

            if (keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift))
            {
                Text = Text.ToUpper();
            }            

            TextString += Text;
        }

        public bool CheckIfAnyKeyIsPressed()
        {
            if (CanPressA == false ||
                CanPressB == false ||
                CanPressC == false ||
                CanPressD == false ||
                CanPressE == false ||
                CanPressF == false ||
                CanPressG == false ||
                CanPressH == false ||
                CanPressI == false ||
                CanPressJ == false ||
                CanPressK == false ||
                CanPressL == false ||
                CanPressM == false ||
                CanPressN == false ||
                CanPressO == false ||
                CanPressP == false ||
                CanPressQ == false ||
                CanPressR == false ||
                CanPressS == false ||
                CanPressT == false ||
                CanPressU == false ||
                CanPressV == false ||
                CanPressW == false ||
                CanPressX == false ||
                CanPressY == false ||
                CanPressZ == false ||
                CanPressÆ == false ||
                CanPressØ == false ||
                CanPressÅ == false ||
                CanPunctuate == false ||
                CanDecimal == false ||
                CanQuestionMark == false ||
                CanExclamation == false ||
                CanBackSpace == false ||
                CanSpace == false ||
                CanEnter == false ||
                CanPlus == false ||
                CanOne == false ||
                CanTwo == false ||
                CanThree == false ||
                CanFour == false ||
                CanFive == false ||
                CanSix == false ||
                CanSeven == false ||
                CanEight == false ||
                CanNine == false ||
                CanZero == false)
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
