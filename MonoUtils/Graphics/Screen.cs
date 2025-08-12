using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoUtils.Graphics
{
    public sealed class Screen : IDisposable
    {
        private readonly static int minDimension = 64;
        private readonly static int maxDimension = 4096;

        private bool isDisposed;
        private bool isSet;
        private Game game;
        private RenderTarget2D target;

        public int Width {
            get { return this.target.Width; }
        }
        public int Height {
            get { return this.target.Height; }
        }

        public Screen(Game game, int width, int height)
        {
            width = Math.Clamp(width, minDimension, maxDimension);
            height = Math.Clamp(height, minDimension, maxDimension);

            this.game = game ?? throw new ArgumentNullException("game");  // Error on null
            this.target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
            this.isSet = false;
        }

        public void Dispose()
        {
            if (this.isDisposed) { return; }

            this.target?.Dispose();
            this.isDisposed = true;
        }

        public void Set()
        {
            if (this.isSet) { throw new Exception("Render target is already set."); }

            this.game.GraphicsDevice.SetRenderTarget(this.target);
            this.isSet = true;
        }

        public void Unset()
        {
            if (!this.isSet) { throw new Exception("Render target is not set."); }

            this.game.GraphicsDevice.SetRenderTarget(null);
            this.isSet = false;
        }

        public void Present(Sprites sprites, bool textureFiltering = true)
        {
            if (sprites is null) { throw new ArgumentNullException("sprites"); }

            this.game.GraphicsDevice.Clear(Color.Black);

            sprites.Begin(null, textureFiltering);
            sprites.Draw(this.target, null, this.GetDestinationRectangle(), Color.White);
            sprites.End();
        }

        internal Rectangle GetDestinationRectangle()
        {
            Rectangle backbufferBounds = this.game.GraphicsDevice.PresentationParameters.Bounds;
            float backbufferAspectRatio = (float)backbufferBounds.Width / (float)backbufferBounds.Height;
            float screenAspectRatio = (float)this.Width / (float)this.Height;

            float rx = 0f;
            float ry = 0f;
            float rw = backbufferBounds.Width;
            float rh = backbufferBounds.Height;

            if (backbufferAspectRatio > screenAspectRatio)
            {
                rw = rh * screenAspectRatio;
                rx = ((float)backbufferBounds.Width - rw) / 2f;
            }
            else if (backbufferAspectRatio < screenAspectRatio)
            {
                rh = rw / screenAspectRatio;
                ry = ((float)backbufferBounds.Height - rh) / 2f;
            }

            Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }

        public void ToggleFullScreen(GraphicsDeviceManager graphics)
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();
        }
    }
}
