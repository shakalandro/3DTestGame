using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace _3DTestGame
{
    class UserInput : Microsoft.Xna.Framework.GameComponent
    {
        public readonly double MOUSE_SENSITIVITY = 10;

        public KeyboardState keyState {
            get;
            protected set;
        }

        public MouseState mouseState {
            get;
            protected set;
        }

        private MouseState prevMouseState;

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            keyState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            keyState = Keyboard.GetState();
            base.Update(gameTime);
        } 

        public Boolean up()
        {
            return keyState.IsKeyDown(Keys.W);
        }

        public Boolean down()
        {
            return keyState.IsKeyDown(Keys.S);
        }

        public Boolean left()
        {
            return keyState.IsKeyDown(Keys.A);
        }

        public Boolean right()
        {
            return keyState.IsKeyDown(Keys.D);
        }

        public Boolean up2()
        {
            return (mouseState.X - prevMouseState.X) > MOUSE_SENSITIVITY; 
        }

        public Boolean down2()
        {
            return (prevMouseState.X - mouseState.X) > MOUSE_SENSITIVITY;
        }

        public Boolean left2()
        {
            return (prevMouseState.Y - mouseState.Y) > MOUSE_SENSITIVITY;
        }

        public Boolean right2()
        {
            return (mouseState.Y - prevMouseState.Y) > MOUSE_SENSITIVITY;
        }
    }
}
