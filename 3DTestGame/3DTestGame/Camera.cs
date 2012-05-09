using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3DTestGame
{
    public class Camera : Microsoft.Xna.Framework.GameComponent, ICamera
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 pos { get; protected set; }
        public Vector3 dir { get; protected set; }
        public Vector3 up { get; protected set; }
        public float moveSpeed { 
            get {
                return 0.05f;
            }
        }
        public float rotateSpeed { 
            get {
                return 0.01f;
            }
        }

        public UserInput input;

        public Camera(Game game, Vector3 pos, Vector3 dir, Vector3 up) : base(game)
        {
            this.pos = pos;
            this.dir = dir;
            this.dir.Normalize();
            this.up = up;
            float aspectRatio = Game.Window.ClientBounds.Width / Game.Window.ClientBounds.Height;
            this.projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, aspectRatio, 1, 100);
            CreateLookAt();
            this.input = ((ISTestGame)game).input;
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(pos, pos + dir, up);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Move the camera using WASD and QE for forward and backward
            if (input.up())
            {
                pos += up * moveSpeed;
            } else if (input.down())
            {
                pos -= up * moveSpeed;
            }
            if (input.left())
            {
                pos += Vector3.Cross(up, dir) * moveSpeed;
            } else if (input.right()) 
            {
                pos -= Vector3.Cross(up, dir) * moveSpeed;
            }
            if (input.forward())
            {
                pos += dir * moveSpeed;
            }
            else if (input.backward())
            {
                pos -= dir * moveSpeed;
            }
            // Rotate the camera using the mouse
            if (input.up3())
            {
                dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(Vector3.Cross(up, dir), -rotateSpeed));
            }
            else if (input.down3())
            {
                dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(Vector3.Cross(up, dir), rotateSpeed));
            }
            if (input.left3())
            {
                dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(up, rotateSpeed));
            }
            else if (input.right3())
            {
                dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(up, -rotateSpeed));
            }
            CreateLookAt();
            base.Update(gameTime);
        }
    }
}
