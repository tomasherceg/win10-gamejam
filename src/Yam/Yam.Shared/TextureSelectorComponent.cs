using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yam
{
    public class TextureSelectorComponent : DrawableGameComponent
    {
        private const int texturesCount = 15;
        private const int fieldSize = 48;
        private const int textureSize = 32;
        private const int margin = 10;
        private const int largeTextureSize = 52;


        private int selectedTexture = 0;
        private bool isKeyDown = false;

        private SpriteBatch spriteBatch;

        public int SelectedTexture
        {
            get {  return selectedTexture + 1; }
        }

        public TextureSelectorComponent(Game game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left))
            {
                if (!isKeyDown)
                {
                    selectedTexture--;
                    if (selectedTexture < 0)
                    {
                        selectedTexture = texturesCount - 1;
                    }
                    isKeyDown = true;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                if (!isKeyDown)
                {
                    selectedTexture++;
                    if (selectedTexture >= texturesCount)
                    {
                        selectedTexture = 0;
                    }
                    isKeyDown = true;
                }
            }
            else
            {
                isKeyDown = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            var left = GraphicsDevice.Viewport.Width / 2 - fieldSize / 2 - selectedTexture * fieldSize;
            var top = GraphicsDevice.Viewport.Height - margin - textureSize;
            for (int i = 0; i < texturesCount; i++)
            {
                if (i != selectedTexture)
                {
                    spriteBatch.Draw(((Game1)Game).Textures[i + 1], new Rectangle(left + i * fieldSize + 4, top, textureSize, textureSize), new Color(255, 255, 255, 180));
                }
            }
            spriteBatch.Draw(((Game1)Game).Textures[selectedTexture + 1], new Rectangle(left + selectedTexture * fieldSize + (fieldSize - textureSize) / 2 - margin, top - margin, largeTextureSize, largeTextureSize), Color.White);

            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}
