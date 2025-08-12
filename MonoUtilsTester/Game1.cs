using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoUtils.Graphics;
using MonoUtils.Input;

namespace MonoUtilsTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Sprites sprites;
        private Shapes shapes;
        private Camera camera;
        private Texture2D texture;
        private Screen screen;
        private UtilsKeyboard keyboard = new UtilsKeyboard();
        private UtilsMouse mouse = new UtilsMouse();

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.graphics.ApplyChanges();

            this.sprites = new Sprites(this);
            this.shapes = new Shapes(this);
            this.screen = new Screen(this, 640, 480);
            this.camera = new Camera(this.screen);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.texture = this.Content.Load<Texture2D>("smiley");
        }

        protected override void Update(GameTime gameTime)
        {
            keyboard.Update();
            mouse.Update();

            if (keyboard.IsKeyClicked(Keys.Escape)) { this.Exit(); }
            if (keyboard.IsKeyClicked(Keys.F)) { this.screen.ToggleFullScreen(this.graphics); }

            if (mouse.IsScrollingUp())
            {
                this.camera.MoveZ(10f);
            }
            else if (mouse.IsScrollingDown())
            {
                this.camera.MoveZ(-10f);
            }
            
            if (keyboard.IsKeyClicked(Keys.R) && keyboard.IsKeyDown(Keys.LeftControl))
            {
                this.camera.ResetZ();
            }
            
            if (keyboard.IsKeyDown(Keys.Left)) { this.camera.MoveCam(new Vector2(-5f, 0f) * camera.ZoomFactor); }
            if (keyboard.IsKeyDown(Keys.Right)) { this.camera.MoveCam(new Vector2(5f, 0f) * camera.ZoomFactor); }
            if (keyboard.IsKeyDown(Keys.Up)) { this.camera.MoveCam(new Vector2(0f, 5f) * camera.ZoomFactor); }
            if (keyboard.IsKeyDown(Keys.Down)) { this.camera.MoveCam(new Vector2(0f, -5f) * camera.ZoomFactor); }

            if (mouse.IsMiddleButtonDown()) { this.camera.MoveCam(new Vector2(-mouse.DeltaX, mouse.DeltaY) * camera.ZoomFactor); }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            Viewport vp = this.GraphicsDevice.Viewport;

            this.sprites.Begin(this.camera, false);
            //this.sprites.Draw(texture, null, new Rectangle(vp.Width/2 - 64, vp.Height/2 - 64, 128, 128), Color.White);
            //this.sprites.Draw(texture, null, new Vector2(8, 8), new Vector2(0,0), MathHelper.PiOver4, new Vector2(2f, 2f), Color.White);
            this.sprites.End();

            this.shapes.Begin(this.camera);
            this.shapes.DrawCircle(new Vector2(0, 0), 100f, Color.White, Shapes.FillMode.Border);
            this.shapes.DrawRectangle(16, 16, 128, 64, Color.Red, Shapes.FillMode.Filled);
            this.shapes.DrawRectangle(-64, 0, 164, 64, Color.Black, Shapes.FillMode.Border);
            this.shapes.DrawLine(new Vector2(-100f, -60f), new Vector2(40f, 45f), 2f, Color.Black);

            this.shapes.End();

            this.screen.Unset();
            this.screen.Present(this.sprites);

            base.Draw(gameTime);
        }
    }
}
