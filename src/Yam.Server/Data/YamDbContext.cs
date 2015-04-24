using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Yam.Server.Data
{
    public class YamDbContext : DbContext
    {

        public DbSet<World> Worlds { get; set; }

        public DbSet<WorldChange> WorldChanges { get; set; }

        public YamDbContext() : base("DB")
        {
        }
    }
}