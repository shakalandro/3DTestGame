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
                return 1f;
            }
        }
        public float rotateSpeed { 
            get {
                return 0.02f;
            }
        }

        public bool normalLight{ get; protected set; }

        public UserInput input;
        private EntityModel fixation;
        private float fixationDistance;

        public Camera(Game game, Vector3 pos, Vector3 dir, Vector3 up) : base(game)
        {
            this.pos = pos;
            this.dir = dir;
            this.dir.Normalize();
            this.up = up;
            float aspectRatio = Game.Window.ClientBounds.Width / Game.Window.ClientBounds.Height;
            this.projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, aspectRatio, 1, 700);
            CreateLookAt();
            this.input = ((ISTestGame)game).input;
            this.normalLight = true;
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
            if (this.fixation != null)
            {
                updateFixation(gameTime);
            } 
            else
            {
                updateFreeRoam(gameTime);
            }
            CreateLookAt();
            if (input.oneKey())
            {
                normalLight = true;
            }
            if (input.twoKey())
            {
                normalLight = false;
            }
            base.Update(gameTime);
        }

        public void updateFreeRoam(GameTime gameTime)
        {
            // Move the camera using WASD and QE for forward and backward
            if (input.up())
            {
                pos += up * moveSpeed;
            }
            else if (input.down())
            {
                pos -= up * moveSpeed;
            }
            if (input.left())
            {
                pos += Vector3.Cross(up, dir) * moveSpeed;
            }
            else if (input.right())
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
            updateMouseDrag(gameTime);
        }

        public void updateMouseDrag(GameTime gameTime)
        {
            // Rotate the camera using the mouse
            if (input.leftClick())
            {
                if (input.up2())
                {
                    dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(Vector3.Cross(up, dir), -rotateSpeed));
                }
                else if (input.down2())
                {
                    dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(Vector3.Cross(up, dir), rotateSpeed));
                }
                if (input.left2())
                {
                    dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(up, rotateSpeed));
                }
                else if (input.right2())
                {
                    dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(up, -rotateSpeed));
                }
            }
        }

        public void updateFixation(GameTime gameTime)
        {
            if (!input.leftClick())
            {
                Vector3 newDir = new Vector3(fixation.forward.X, 0f, fixation.forward.Z);
                newDir.Normalize();
                newDir.Y = -0.5f;
                this.dir = newDir;

                this.pos = new Vector3(
                    fixation.entity.Position.X - (this.dir.X * fixationDistance),
                    fixation.entity.Position.Y + fixationDistance,
                    fixation.entity.Position.Z - (this.dir.Z * fixationDistance)
                );
            }
            else
            {
                updateMouseDrag(gameTime);
            }
        }

        public void fixate(EntityModel m, float followDistance)
        {
            this.fixation = m;
            this.fixationDistance = followDistance;
        }
    }
}
