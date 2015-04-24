using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yam.Core.Model;

namespace Yam
{
    public class WorldRenderingComponent : DrawableGameComponent
    {

        public World GameWorld { get; set; }

        public Matrix ViewMatrix { get; set; }

        public Matrix ProjectionMatrix { get; set; }

        public Vector3 CameraPosition { get; set; }


        private bool lookAtPositionDirty = true;
        private Vector3 lookAtPosition;
        public Vector3 LookAtPosition
        {
            get { return lookAtPosition; }
            set
            {
                lookAtPosition = value;
                lookAtPositionDirty = true;
            }
        }


        private BasicEffect basicEffect;
        private DynamicVertexBuffer vertexBuffer;
        private DynamicVertexBuffer wireFrameVertexBuffer;
        private DynamicIndexBuffer indexBuffer;
        private DynamicIndexBuffer wireFrameIndexBuffer;

        private List<Tile> tilesToDraw;
        
        public Tile HighlightedTile { get; private set; }
        
        public int? NewTilePlaceX { get; set; }
        public int? NewTilePlaceY { get; set; }
        public int? NewTilePlaceZ { get; set; }

        public WorldRenderingComponent(Game game) : base(game)
        {
        }


        protected override void LoadContent()
        {
            PrepareCubeBuffers();

            base.LoadContent();
        }


        private void PrepareCubeBuffers()
        {
            var i = 0;
            var cube = new VertexPositionNormalTexture[24];

            // bottom
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, -1, 0), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(0f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, -1, 0), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, -1, 0), new Vector2(1f, 0f));

            // front
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0f, 1f));

            // right
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0f, 1f));

            // back
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0f, 1f));

            // left
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0f, 1f));

            // top
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(0f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, -0.5f), new Vector3(0, 1, 0), new Vector2(1f, 0f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 1, 0), new Vector2(1f, 1f));
            cube[i++] = new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0, 1, 0), new Vector2(0f, 1f));

            var cubeIndices = new List<short>();
            for (int j = 0; j < 6; j++)
            {
                cubeIndices.Add((short)(j * 4 + 0));
                cubeIndices.Add((short)(j * 4 + 1));
                cubeIndices.Add((short)(j * 4 + 2));
                cubeIndices.Add((short)(j * 4 + 0));
                cubeIndices.Add((short)(j * 4 + 2));
                cubeIndices.Add((short)(j * 4 + 3));
            }

            var wireFrameCube = new[]
            {
                new VertexPositionColor(new Vector3(-0.51f, -0.51f, -0.51f), Color.Black),
                new VertexPositionColor(new Vector3(0.51f, -0.51f, -0.51f), Color.Black),
                new VertexPositionColor(new Vector3(0.51f, -0.51f, 0.51f), Color.Black),
                new VertexPositionColor(new Vector3(-0.51f, -0.51f, 0.51f), Color.Black),
                new VertexPositionColor(new Vector3(-0.51f, 0.51f, -0.51f), Color.Black),
                new VertexPositionColor(new Vector3(0.51f, 0.51f, -0.51f), Color.Black),
                new VertexPositionColor(new Vector3(0.51f, 0.51f, 0.51f), Color.Black),
                new VertexPositionColor(new Vector3(-0.51f, 0.51f, 0.51f), Color.Black)
            };
            var wireFrameCubeIndices = new short[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 };
            
            vertexBuffer = new DynamicVertexBuffer(Game.GraphicsDevice, typeof(VertexPositionNormalTexture), cube.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(cube, 0, cube.Length, SetDataOptions.Discard);
            indexBuffer = new DynamicIndexBuffer(Game.GraphicsDevice, IndexElementSize.SixteenBits, cubeIndices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(cubeIndices.ToArray(), 0, cubeIndices.Count, SetDataOptions.Discard);

            wireFrameVertexBuffer = new DynamicVertexBuffer(Game.GraphicsDevice, typeof(VertexPositionColor), wireFrameCube.Length, BufferUsage.WriteOnly);
            wireFrameVertexBuffer.SetData(wireFrameCube, 0, wireFrameCube.Length, SetDataOptions.Discard);
            wireFrameIndexBuffer = new DynamicIndexBuffer(Game.GraphicsDevice, IndexElementSize.SixteenBits, wireFrameCubeIndices.Length, BufferUsage.WriteOnly);
            wireFrameIndexBuffer.SetData(wireFrameCubeIndices, 0, wireFrameCubeIndices.Length, SetDataOptions.Discard);

            basicEffect = new BasicEffect(Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            // get tiles that are in the view
            if (lookAtPositionDirty)
            {
                lock (GameWorld)
                {
                    tilesToDraw = GameWorld.GetTiles((int)LookAtPosition.X - 30, (int)LookAtPosition.Y - 30, (int)LookAtPosition.Z - 30,
                        (int)LookAtPosition.X + 30, (int)LookAtPosition.Y + 30, (int)LookAtPosition.Z + 30).ToList();

                    // highlight the tile the mouse is on and detect the place where the new tile will be added
                    int? placementX, placementY, placementZ;
                    HighlightedTile = FindHighlightedTile(out placementX, out placementY, out placementZ);

                    if (placementX != null)
                    {
                        NewTilePlaceX = placementX;
                        NewTilePlaceY = placementY;
                        NewTilePlaceZ = placementZ;
                    }
                    else
                    {
                        NewTilePlaceX = null;
                        NewTilePlaceY = null;
                        NewTilePlaceZ = null;
                    }
                }

                lookAtPositionDirty = false;
            }

            base.Update(gameTime);
        }

        private Tile FindHighlightedTile(out int? placementX, out int? placementY, out int? placementZ)
        {
            placementX = null;
            placementY = null;
            placementZ = null;

            var prevX = 0;
            var prevY = 0;
            var prevZ = 0;

            var pointer1 = GraphicsDevice.Viewport.Unproject(new Vector3(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f, 0f), ProjectionMatrix, ViewMatrix, Matrix.Identity);
            var pointer2 = GraphicsDevice.Viewport.Unproject(new Vector3(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f, 1f), ProjectionMatrix, ViewMatrix, Matrix.Identity);
            var rayDirection = Vector3.Normalize(pointer2 - pointer1);

            for (float i = 1.5f; i < 5; i += 0.1f)
            {
                var point = CameraPosition + rayDirection * i;

                var x = (int)Math.Round(point.X);
                var y = (int)Math.Round(point.Y);
                var z = (int)Math.Round(point.Z);

                var tile = GameWorld.GetTile(x, y, z);
                if (i > 1.5f && tile != null)
                {
                    if (prevX != x || prevY != y || prevZ != z)
                    {
                        placementX = prevX;
                        placementY = prevY;
                        placementZ = prevZ;
                        return tile;
                    }
                    else
                    {
                        return tile;
                    }
                }

                prevX = x;
                prevY = y;
                prevZ = z;
            }
            return null;
        }

        public override void Draw(GameTime gameTime)
        {
            // draw the tiles
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = false;
            basicEffect.TextureEnabled = true;
            basicEffect.View = ViewMatrix;
            basicEffect.Projection = ProjectionMatrix;
            foreach (var tileGroup in tilesToDraw.GroupBy(t => t.TextureId))
            {
                basicEffect.Texture = ((Game1)Game).Textures[tileGroup.Key];
                foreach (var tile in tileGroup)
                {
                    RenderCube(tile.X, tile.Y, tile.Z);
                }
            }

            // draw the highlighted tile
            if (HighlightedTile != null)
            {
                basicEffect.LightingEnabled = false;
                basicEffect.VertexColorEnabled = true;
                basicEffect.TextureEnabled = false;
                GraphicsDevice.SetVertexBuffer(wireFrameVertexBuffer);
                GraphicsDevice.Indices = wireFrameIndexBuffer;
                RenderCubeWireframe(HighlightedTile.X, HighlightedTile.Y, HighlightedTile.Z);
            }

            base.Draw(gameTime);
        }

        private void RenderCube(int x, int y, int z)
        {
            basicEffect.World = Matrix.CreateTranslation(x, y, z);
            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
            }
        }

        private void RenderCubeWireframe(int x, int y, int z)
        {
            basicEffect.World = Matrix.CreateScale(1.02f) * Matrix.CreateTranslation(x, y, z);
            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 24, 0, 24);
            }
        }

    }
}
