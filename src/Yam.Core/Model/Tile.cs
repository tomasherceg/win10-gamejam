using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Model
{
    public class Tile
    {

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int TextureId { get; set; }


        public bool IsInRange(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return minX <= X && X <= maxX && minY <= Y && Y <= maxY && minZ <= Z && Z <= maxZ;
        }
    }
}