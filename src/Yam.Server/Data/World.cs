using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yam.Server.Data
{
    public class World
    {

        public int Id { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<WorldChange> WorldChanges { get; set; }


        public World()
        {
            WorldChanges = new List<WorldChange>();
        }
    }
}