using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Model
{
    public class Segment
    {

        public int MinX { get; set; }

        public int MinY { get; set; }

        public int MinZ { get; set; }

        public int MaxX { get; set; }

        public int MaxY { get; set; }

        public int MaxZ { get; set; }

        public List<Tile> Tiles { get; private set; }

        public Segment()
        {
            Tiles = new List<Tile>();
        }

        public bool IsInRange(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return minX < MaxX && MinX < maxX && minY < MaxY && MinY < maxY && minZ < MaxZ && MinZ < maxZ;
        }

        public bool ContainsPoint(int x, int y, int z)
        {
            return MinX <= x && x <= MaxX && MinY <= y && y <= MaxY && MinZ <= z && z <= MaxZ;
        }


        public void RemoveTile(int x, int y, int z)
        {
            Tiles.RemoveAll(t => t.X == x && t.Y == y && t.Z == z);
        }

        public Tile GetTile(int x, int y, int z)
        {
            return Tiles.FirstOrDefault(t => t.X == x && t.Y == y && t.Z == z);
        }

        public void AddTile(Tile tile)
        {
            RemoveTile(tile.X, tile.Y, tile.Z);
            Tiles.Add(tile);
        }
    }
}