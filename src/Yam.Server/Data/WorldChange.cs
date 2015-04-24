using System;
using System.Collections.Generic;
using System.Linq;
using Yam.Core.Communication;

namespace Yam.Server.Data
{
    public class WorldChange
    {

        public int Id { get; set; }

        public int WorldId { get; set; }

        public virtual World World { get; set; }

        public ChangeType Type { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int? TextureId { get; set; }
    }
}