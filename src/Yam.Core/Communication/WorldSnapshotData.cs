using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yam.Core.Model;

namespace Yam.Core.Communication
{
    public class WorldSnapshotData
    {

        public int Id { get; set; }

        public string Name { get; set; }


        public int Version { get; set; }

        public List<Tile> Tiles { get; set; }


    }
}
