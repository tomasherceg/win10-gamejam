using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Communication
{
    public class Change
    {
        public int Version { get; set; }

        public ChangeType Type { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int? TextureId { get; set; }

    }

    public enum ChangeType
    {
        Add,
        Remove
    }
}