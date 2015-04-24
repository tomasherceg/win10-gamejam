using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Yam.Core.Communication;
using Yam.Core.Model;

namespace Yam.Core
{
    public class WorldUpdateService
    {
        private int currentVersion = 0;
        private Connection connection;

        public int WorldId { get; set; }
        
        public World World { get; private set; }

        public Uri BaseUri { get; private set; }

        public bool IsLoaded { get; private set; }

        
        public WorldUpdateService(int worldId, World world, Uri baseUri)
        {
            WorldId = worldId;
            World = world;
            BaseUri = baseUri;
        }

        
        public void Start()
        {
            Task.Factory.StartNew(async () =>
            {
                using (var client = CreateHttpClient())
                {
                    // download the data
                    var result = await client.GetAsync(new Uri(BaseUri, "api/snapshot/" + WorldId));
                    result.EnsureSuccessStatusCode();
                    var snapshot = await ParseJson<WorldSnapshotData>(result);

                    // load the tiles into the world
                    lock (World)
                    {
                        foreach (var tile in snapshot.Tiles)
                        {
                            World.AddTile(tile);
                        }
                        currentVersion = snapshot.Version;
                        IsLoaded = true;
                    }
                }

                // connect to the SignalR
                connection = new Connection(new Uri(BaseUri, "api/notifications").AbsoluteUri);
                connection.Received += OnReceived;
                await connection.Start();
                await connection.Send("set-world:" + WorldId);
            });
        }

        private void OnReceived(string obj)
        {
            if (obj == "notify")
            {
                CheckForUpdates();
            }
        }

        private void CheckForUpdates()
        {
            Task.Factory.StartNew(async () =>
            {
                using (var client = CreateHttpClient())
                {
                    // download the new version
                    var json = await client.GetAsync(new Uri(BaseUri, "api/changes/" + WorldId + "?currentVersion=" + currentVersion));
                    var changes = await ParseJson<WorldChangesData>(json);

                    // apply the changes
                    lock (World)
                    {
                        foreach (var change in changes.Changes.Where(c => c.Version > currentVersion))
                        {
                            if (change.Type == ChangeType.Add)
                            {
                                World.AddTile(new Tile() { TextureId = change.TextureId.Value, X = change.X, Y = change.Y, Z = change.Z });
                            }
                            else
                            {
                                World.RemoveTile(change.X, change.Y, change.Z);
                            }
                        }
                        currentVersion = changes.TargetVersion;
                    }
                }
            });
        }

        public void PushChange(Change change)
        {
            Task.Factory.StartNew(async () =>
            {
                using (var client = CreateHttpClient())
                {
                    var result = await client.PostAsync(new Uri(BaseUri, "api/publish/" + WorldId), CreateJson(change));
                    result.EnsureSuccessStatusCode();
                }
            });
        }

        private static StringContent CreateJson(object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }

        private static async Task<T> ParseJson<T>(HttpResponseMessage result)
        {
            var json = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
