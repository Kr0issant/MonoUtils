using System;
using MonoUtils.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoUtils.Input
{
    public sealed class UtilsMouse
    {
        private MouseState previousMouseState;
        private MouseState currentMouseState;

        private float deltaX;
        private float deltaY;
        public Point WindowPosition
        {
            get { return currentMouseState.Position; }  // TODO: Make it consistent with coordinate system (y 0 is bottom)
        }

        public float DeltaX { get { return deltaX; } }
        public float DeltaY { get { return deltaY; } }

        public UtilsMouse()
        {
            this.previousMouseState = Mouse.GetState();
            this.currentMouseState = this.previousMouseState;
        }

        public void Update()
        {
            this.previousMouseState = this.currentMouseState;
            this.currentMouseState = Mouse.GetState();

            deltaX = currentMouseState.X - previousMouseState.X;
            deltaY = currentMouseState.Y - previousMouseState.Y;
        }

        public Vector2 GetScreenPosition(Screen screen)
        {
            Rectangle screenDestinationRectangle = screen.GetDestinationRectangle();

            Point windowPosition = this.WindowPosition;

            float relX = windowPosition.X - screenDestinationRectangle.X;
            float relY = windowPosition.Y - screenDestinationRectangle.Y;

            relX /= (float)screenDestinationRectangle.Width;
            relY /= (float)screenDestinationRectangle.Height;

            relX *= (float)screen.Width;
            relY *= (float)screen.Height;

            relY = screen.Height - relY;  // Use cartesian coordinates (0, 0 is bottom left)

            return new Vector2(relX, relY);
        }

        public bool IsLeftButtonDown()
        {
            return this.currentMouseState.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightButtonDown()
        {
            return this.currentMouseState.RightButton == ButtonState.Pressed;
        }
        public bool IsMiddleButtonDown()
        {
            return this.currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonClicked()
        {
            return this.currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }
        public bool IsRightButtonClicked()
        {
            return this.currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released;
        }
        public bool IsMiddleButtonClicked()
        {
            return this.currentMouseState.MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Released;
        }
        public bool IsScrollingUp()
        {
            return this.currentMouseState.ScrollWheelValue < this.previousMouseState.ScrollWheelValue;
        }
        public bool IsScrollingDown()
        {
            return this.currentMouseState.ScrollWheelValue > this.previousMouseState.ScrollWheelValue;
        }
    }
}
