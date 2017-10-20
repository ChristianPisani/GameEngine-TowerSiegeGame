using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TowerSiegeGame
{
    public interface ILight2D
    {
        BoundingRect Bounds { get; }

        void Draw(KryptonRenderHelper renderHelper, List<ShadowHull> hulls);
    }
}
