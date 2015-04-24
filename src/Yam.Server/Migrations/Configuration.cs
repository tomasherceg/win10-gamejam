using Yam.Core.Communication;
using Yam.Server.Data;

namespace Yam.Server.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Yam.Server.Data.YamDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Yam.Server.Data.YamDbContext context)
        {
            var world = context.Worlds.FirstOrDefault();
            if (world == null)
            {
                world = new World() { Name = "Default" };

                for (int x = -25; x < 25; x++)
                {
                    for (int y = -25; y < 25; y++)
                    {
                        world.WorldChanges.Add(new WorldChange() { Type = ChangeType.Add, X = x, Z = y, TextureId = 1 });
                    }
                }
                context.Worlds.Add(world);
            }
        }
    }
}
