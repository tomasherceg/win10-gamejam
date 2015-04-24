using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Yam.Core;
using Yam.Core.Communication;
using Yam.Core.Model;

namespace Yam
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public const int WorldId = 2;
        public const string ServerUrl = "http://yam.herceg.cz/";


        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D cursorTexture;

        private Vector3 cameraPosition;
        private Vector3 lookAtPosition;
        private float cameraRotationH = 0;
        private float cameraRotationV = MathHelper.Pi * 0.3f;
        private float cameraDistance = 10;

        private float fieldOfView;
        private float zNear;
        private float zFar;

        private WorldRenderingComponent worldRenderingComponent;
        private TextureSelectorComponent textureSelectorComponent;
        private WorldUpdateService worldUpdateService;

        private Vector2 lastMouseMovement;
        private int lastScrollWheelValue;
        private bool isBuilding = false;
        private World world;

        public Dictionary<int, Texture2D> Textures { get; private set; }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Textures = new Dictionary<int, Texture2D>();

            // handle relative mouse movements
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = null;
            Windows.Devices.Input.MouseDevice.GetForCurrentView().MouseMoved += (sender, args) =>
            {
                lastMouseMovement = new Vector2(args.MouseDelta.X, args.MouseDelta.Y);
            };
        }
        
        protected override void Initialize()
        {
            // init the world
            world = new World();
            worldUpdateService = new WorldUpdateService(WorldId, world, new Uri(ServerUrl));
            worldUpdateService.Start();

            // set up components
            worldRenderingComponent = new WorldRenderingComponent(this) { GameWorld = world };
            worldRenderingComponent.Visible = true;
            Components.Add(worldRenderingComponent);
            textureSelectorComponent = new TextureSelectorComponent(this);
            textureSelectorComponent.Visible = true;
            Components.Add(textureSelectorComponent);

            // prepare the camera
            zNear = 0.1f;
            zFar = 100.0f;
            fieldOfView = MathHelper.Pi * 0.4f;
            cameraPosition = new Vector3(0f, -2f, 0f);
            worldRenderingComponent.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, GraphicsDevice.Viewport.AspectRatio, zNear, zFar);
            
            UpdateCamera();

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cursorTexture = Content.Load<Texture2D>("cross");

            LoadTextures();

            base.LoadContent();
        }

        private void LoadTextures()
        {
            for (int i = 1; i <= 15; i++)
            {
                Textures[i] = Content.Load<Texture2D>(i.ToString());
            }
        }


        protected override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            // handle camera
            HandleCameraControls(gameTime, mouse);

            // adding or removing tiles
            HandleBuilding(mouse);

            base.Update(gameTime);
        }

        private void HandleCameraControls(GameTime gameTime, MouseState mouse)
        {
            var updateCamera = false;

            // determine mouse picking position
            var deltaX = (int)lastMouseMovement.X;
            var deltaY = (int)lastMouseMovement.Y;
            if (deltaX != 0 || deltaY != 0 || mouse.ScrollWheelValue != lastScrollWheelValue)
            {
                // move the view
                cameraRotationH += deltaX * 0.0002f * gameTime.ElapsedGameTime.Milliseconds;
                cameraRotationV += deltaY * 0.0002f * gameTime.ElapsedGameTime.Milliseconds;
                cameraRotationV = MathHelper.Clamp(cameraRotationV, -MathHelper.Pi * 0.4f, MathHelper.Pi * 0.4f);

                // zoom the view
                if (mouse.ScrollWheelValue != lastScrollWheelValue)
                {
                    var cameraVector = Vector3.Normalize(cameraPosition - lookAtPosition);
                    cameraDistance = 10f + MathHelper.Clamp(mouse.ScrollWheelValue, -10000, 1000) * 0.001f * -5f;
                    cameraPosition = lookAtPosition + cameraVector * cameraDistance;
                    lastScrollWheelValue = mouse.ScrollWheelValue;
                }
                updateCamera = true;
                lastMouseMovement = Vector2.Zero;
            }

            // use keys
            var keyboard = Keyboard.GetState();
            var moveDelta = Vector3.Zero;
            if (keyboard.IsKeyDown(Keys.W))
            {
                moveDelta.Z -= (float)Math.Cos(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                moveDelta.X -= (float)Math.Sin(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                moveDelta.Z += (float)Math.Cos(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                moveDelta.X += (float)Math.Sin(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                moveDelta.Z -= (float)Math.Sin(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                moveDelta.X += (float)Math.Cos(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                moveDelta.Z += (float)Math.Sin(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                moveDelta.X -= (float)Math.Cos(cameraRotationH) * 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            if (keyboard.IsKeyDown(Keys.E))
            {
                moveDelta.Y -= 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            if (keyboard.IsKeyDown(Keys.C))
            {
                moveDelta.Y += 0.003f * gameTime.ElapsedGameTime.Milliseconds;
                updateCamera = true;
            }
            cameraPosition += moveDelta;

            if (updateCamera)
            {
                UpdateCamera();
            }
        }

        private void HandleBuilding(MouseState mouse)
        {
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (!isBuilding && worldRenderingComponent.NewTilePlaceX != null)
                {
                    isBuilding = true;
                    var tile = new Tile()
                    {
                        X = worldRenderingComponent.NewTilePlaceX.Value,
                        Y = worldRenderingComponent.NewTilePlaceY.Value,
                        Z = worldRenderingComponent.NewTilePlaceZ.Value,
                        TextureId = textureSelectorComponent.SelectedTexture
                    };
                    worldUpdateService.PushChange(new Change() { TextureId = tile.TextureId, X = tile.X, Y = tile.Y, Z = tile.Z, Type = ChangeType.Add });
                    world.AddTile(tile);
                }
            }
            else if (mouse.RightButton == ButtonState.Pressed)
            {
                if (!isBuilding && worldRenderingComponent.HighlightedTile != null)
                {
                    isBuilding = true;
                    var tile = worldRenderingComponent.HighlightedTile;
                    worldUpdateService.PushChange(new Change() { X = tile.X, Y = tile.Y, Z = tile.Z, Type = ChangeType.Remove });
                    world.RemoveTile(tile.X, tile.Y, tile.Z);
                }
            }
            else
            {
                isBuilding = false;
            }
        }

        private void UpdateCamera()
        {
            var cameraDirection = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(cameraRotationV) * Matrix.CreateRotationY(cameraRotationH));
            lookAtPosition = cameraPosition + cameraDistance * cameraDirection;
            var lookDirection = Vector3.Normalize(lookAtPosition - cameraPosition);
            var leftVector = Vector3.Cross(lookDirection, Vector3.Up);
            var upVector = Vector3.Cross(lookDirection, leftVector);

            var view = Matrix.CreateLookAt(cameraPosition, lookAtPosition, upVector);
            worldRenderingComponent.ViewMatrix = view;
            worldRenderingComponent.CameraPosition = cameraPosition;
            worldRenderingComponent.LookAtPosition = lookAtPosition;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            // draw cursor
            spriteBatch.Begin();
            spriteBatch.Draw(cursorTexture, new Vector2(GraphicsDevice.Viewport.Width * 0.5f - cursorTexture.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.5f - cursorTexture.Height * 0.5f), Color.White);
            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
