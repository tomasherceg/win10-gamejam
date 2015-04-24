using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Communication
{
    public class WorldChangesData
    {

        public int TargetVersion { get; set; }

        public List<Change> Changes { get; set; }
    }
}