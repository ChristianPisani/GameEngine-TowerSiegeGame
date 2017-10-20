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
    public class ElectronicComponent : Object
    {
        public bool IsOn = true,
                    PreviousOnState = true,
                    StateChanged = false,
                    
                    UpdatePosition = true;

        public float Delay = 0;

        public float ID;

        public string type = "receiver";

        public List<float> connectedComponents = new List<float>();

        public Object OnState,
               OffState;

        public ElectronicComponent()
            : base()
        {

        }

        public ElectronicComponent(Random rnd, List<ElectronicComponent> electronics,
            Object OnState, Object OffState, bool IsOn, KryptonEngine krypton, bool castShadows)
            : base(OnState.bounds, OnState.bounciness, OnState.alwaysUpdate, OnState.CheckNPCs, OnState.CheckPlayer, krypton, castShadows)
        {
            ID = rnd.Next(-999999, 999999);

            this.castShadows = false;

            this.OnState = OnState;
            this.OffState = OffState;

            if (IsOn == true)
                SetBaseVariables(OnState);
            else
                SetBaseVariables(OffState);

            for (int i = 0; i < electronics.Count; i++ )
            {
                if (ID == electronics[i].ID)
                {
                    ID = rnd.Next(-999999, 999999);
                    i = 0;
                }
            }
        }

        public override void Update(CollisionMap collisionMap, Camera camera, bool getColDir, List<NPC> NPCs, Player player, List<Object> entities, Rectangle window,
            List<ElectronicComponent> electronics, KryptonEngine krypton)
        {
            if (IsOn != PreviousOnState)
            {
                StateChanged = true;
            }

            if (StateChanged == true)
            {
                if (IsOn == true)
                {
                    SetBaseVariables(OnState);
                }
                else
                {
                    SetBaseVariables(OffState);
                }
            }

            base.Update(collisionMap, camera, getColDir, NPCs, player, entities, window, electronics, krypton);

            PreviousOnState = IsOn;
            StateChanged = false;

            OnState.hull.Position.X = bounds.Center.X;
            OnState.hull.Position.Y = bounds.Center.Y;
            OnState.hull.Scale.X = bounds.Width;
            OnState.hull.Scale.Y = bounds.Height;
            OffState.hull.Position.X = bounds.Center.X;
            OffState.hull.Position.Y = bounds.Center.Y;
            OffState.hull.Scale.X = bounds.Width;
            OffState.hull.Scale.Y = bounds.Height;
        }

        private void SetBaseVariables(Object newBase)
        {
            this.alwaysUpdate = newBase.alwaysUpdate;
            this.atEdge = newBase.atEdge;
            this.bounciness = newBase.bounciness;

            if (UpdatePosition == true)
            {
                this.bounds.X = newBase.bounds.X;
                this.bounds.Y = newBase.bounds.Y;
            }
            this.bounds.Width = newBase.bounds.Width;
            this.bounds.Height = newBase.bounds.Height;

            this.velocity = newBase.velocity;
            this.CheckNPCs = newBase.CheckNPCs;
            this.CheckPlayer = newBase.CheckPlayer;
            this.colCheckXEnd = newBase.colCheckXEnd;
            this.colCheckXStart = newBase.colCheckXStart;
            this.colCheckYEnd = newBase.colCheckYEnd;
            this.colCheckYStart = newBase.colCheckYStart;
            this.colDirRect = newBase.colDirRect;
            this.colliding = newBase.colliding;
            this.colObjects = newBase.colObjects;
            this.color = newBase.color;
            this.colXLeft = newBase.colXLeft;
            this.colXRight = newBase.colXRight;
            this.colYDown = newBase.colYDown;
            this.colYUp = newBase.colYUp;
            this.DeleteNextFrame = newBase.DeleteNextFrame;
            this.destroyOnTouch = newBase.destroyOnTouch;
            this.DoUpdate = newBase.DoUpdate;
            this.downCol = newBase.downCol;
            this.edgeCheck = newBase.edgeCheck;
            this.Existing = newBase.Existing;
            this.friction = newBase.friction;
            this.gravity = newBase.gravity;
            this.leftCol = newBase.leftCol;
            this.lifeTime = newBase.lifeTime;
            this.linelength = newBase.linelength;
            this.physicsEnabled = newBase.physicsEnabled;
            this.ray = newBase.ray;
            this.removalStop = newBase.removalStop;
            this.rightCol = newBase.rightCol;
            this.ShatterPos = newBase.ShatterPos;
            this.texture = newBase.texture;
            this.trail = newBase.trail;
            this.trailLength = newBase.trailLength;
            this.upCol = newBase.upCol;
            this.weight = newBase.weight;
            this.collidable = newBase.collidable;

            if (newBase.castShadows == false)
            {
                OnState.hull.Visible = false;
                OffState.hull.Visible = false;
                this.hull.Visible = false;
            }
            else
            {
                OnState.hull.Visible = true;
                OffState.hull.Visible = true;
                //this.hull.Visible = true;
            }
        }
    }
}
