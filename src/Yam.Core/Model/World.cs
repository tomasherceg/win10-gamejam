using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Model
{
    public class World
    {

        private List<Segment> segments = new List<Segment>();

        private const int SegmentSize = 50;


        public IEnumerable<Tile> GetTiles(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return segments
                .Where(s => s.IsInRange(minX, minY, minZ, maxX, maxY, maxZ))
                .SelectMany(s => s.Tiles.Where(t => t.IsInRange(minX, minY, minZ, maxX, maxY, maxZ)));
        }

        public Tile GetTile(int x, int y, int z)
        {
            var segment = GetSegment(x, y, z);
            return segment != null ? segment.GetTile(x, y, z) : null;
        }

        public void AddTile(Tile tile)
        {
            var segment = GetSegment(tile.X, tile.Y, tile.Z);
            if (segment == null)
            {
                segment = new Segment()
                {
                    MinX = (int)Math.Floor((float)tile.X / SegmentSize) * SegmentSize,
                    MinY = (int)Math.Floor((float)tile.Y / SegmentSize) * SegmentSize,
                    MinZ = (int)Math.Floor((float)tile.Z / SegmentSize) * SegmentSize
                };
                segment.MaxX = segment.MinX + SegmentSize - 1;
                segment.MaxY = segment.MinY + SegmentSize - 1;
                segment.MaxZ = segment.MinZ + SegmentSize - 1;
                segments.Add(segment);
            }
            segment.AddTile(tile);
        }

        public void RemoveTile(int x, int y, int z)
        {
            var segment = GetSegment(x, y, z);
            if (segment != null)
            {
                segment.RemoveTile(x, y, z);
            }
        }

        private Segment GetSegment(int x, int y, int z)
        {
            return segments.FirstOrDefault(s => s.ContainsPoint(x, y, z));
        }

    }
}
