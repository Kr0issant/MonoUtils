using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoUtils.Input
{
    public sealed class UtilsKeyboard
    {
        private KeyboardState previousKeyboardState;
        private KeyboardState currentKeyboardState;

        public UtilsKeyboard()
        {
            this.previousKeyboardState = Keyboard.GetState();
            this.currentKeyboardState = this.previousKeyboardState;
        }

        public void Update()
        {
            this.previousKeyboardState = this.currentKeyboardState;
            this.currentKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key)
        {
            return this.currentKeyboardState.IsKeyDown(key) && !this.previousKeyboardState.IsKeyDown(key);
        }
    }
}
