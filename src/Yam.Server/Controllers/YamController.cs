using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Yam.Core.Communication;
using Yam.Core.Model;
using Yam.Server.Data;

namespace Yam.Server.Controllers
{
    [Route("api/{action}/{worldId}")]
    public class YamController : ApiController
    {

        [HttpGet]
        [ActionName("snapshot")]
        public WorldSnapshotData GetWorldSnapshot(int worldId)
        {
            using (var dc = CreateYamContext())
            {
                // TODO: performance optimization

                var world = dc.Worlds.Find(worldId);
                var changes = dc.WorldChanges.Where(c => c.WorldId == worldId).OrderBy(c => c.Id).ToList();

                var tiles = new List<Tile>();
                foreach (var change in changes)
                {
                    if (change.Type == ChangeType.Add)
                    {
                        tiles.Add(new Tile() { TextureId = change.TextureId.Value, X = change.X, Y = change.Y, Z = change.Z });
                    }
                    else if (change.Type == ChangeType.Remove)
                    {
                        tiles.RemoveAll(t => t.X == change.X && t.Y == change.Y && t.Z == change.Z);
                    }
                }
                return new WorldSnapshotData()
                {
                    Id = worldId,
                    Name = world.Name,
                    Tiles = tiles,
                    Version = changes.Any() ? changes[changes.Count - 1].Id : 0
                };
            }
        }

        [HttpGet]
        [ActionName("changes")]
        public WorldChangesData GetChanges(int worldId, int currentVersion)
        {
            using (var dc = CreateYamContext())
            {
                var changes = dc.WorldChanges.Where(c => c.WorldId == worldId && c.Id > currentVersion).OrderBy(c => c.Id).ToList();

                return new WorldChangesData()
                {
                    Changes = changes.Select(c => new Change() { Type = c.Type, TextureId = c.TextureId, X = c.X, Y = c.Y, Z = c.Z, Version = c.Id }).ToList(),
                    TargetVersion = changes.Any() ? changes[changes.Count - 1].Id : 0
                };
            }
        }

        [HttpPost]
        [ActionName("publish")]
        public void PublishChange(int worldId, Change change)
        {
            using (var dc = CreateYamContext())
            {
                var entity = new WorldChange()
                {
                    WorldId = worldId,
                    Type = change.Type,
                    X = change.X,
                    Y = change.Y,
                    Z = change.Z,
                    TextureId = change.TextureId
                };
                dc.WorldChanges.Add(entity);
                dc.SaveChanges();
            }
        }


        protected YamDbContext CreateYamContext()
        {
            return new YamDbContext();
        }

    }
} 