using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BEPUphysics.Entities;

namespace _3DTestGame
{
    /// <summary>
    /// Component that draws a model following the position and orientation of a BEPUphysics entity.
    /// </summary>
    public class ControllableModel : EntityModel
    {
       
        private static float DEFAULT_SPEED = .33f;
        private static float MAX_SPEED = 3f;
        private static float JUMP_THRESHOLD = 0.1f;
        private static float ROTATION_SPEED = 0.05f; //radians

        private float lastVelocity;
        public enum MoveMode
        {
            Free, Directional, None
        };

        public MoveMode moveMode;

        private IInput input
        {
            get
            {
                return (IInput)this.Game.Services.GetService(typeof(IInput));
            }
        }

        public ControllableModel(Game game, Model model, Entity entity)
            : this(game, model, entity, MoveMode.Directional, new Vector3(1f, 0f, 1f)) { }

        public ControllableModel(Game game, Model model, Entity entity, MoveMode style, Vector3 forward)
            : base(game, model, entity)
        {
            this.forward = forward;
            this.forward.Y = 0f;
            this.forward.Normalize();
            this.moveMode = style;
            if (this.moveMode == MoveMode.Directional)
            {
               // this.camera.fixate(this, 15.0f);
            }
        }

        // Moves the model based on the direction of the camera.
        public override void Update(GameTime gameTime)
        {
            entity.AngularVelocity = Vector3.Zero;

            if (moveMode == MoveMode.Free)
            {
                updateMoveFree(gameTime);
            }
            else if (moveMode == MoveMode.Directional)
            {
                updateMoveDirectional(gameTime);
            }

            lastVelocity = entity.LinearVelocity.Y > 0 ? 1 : -1;

            if (input.spaceKey() && Math.Abs(entity.LinearVelocity.Y) < JUMP_THRESHOLD && lastVelocity < 0)
            {
                entity.LinearMomentum = Vector3.Add(entity.LinearMomentum, new Vector3(0f, 20f, 0f));
            }
            
            base.Update(gameTime);
        }

        private void updateMoveDirectional(GameTime gameTime)
        {
            if (input.down3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(forward, -DEFAULT_SPEED));
            }
            if (input.up3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(forward, DEFAULT_SPEED));
            }
            if (input.left3())
            {
                this.forward = Vector3.Transform(this.forward, Matrix.CreateRotationY(ROTATION_SPEED));
            }
            if (input.right3())
            {
                this.forward = Vector3.Transform(this.forward, Matrix.CreateRotationY(-ROTATION_SPEED));
            }
            if (entity.LinearVelocity.Length() > MAX_SPEED)
            {
                entity.LinearVelocity.Normalize();
                Vector3.Multiply(entity.LinearVelocity, MAX_SPEED);
            }
            //change the angle to get it right
            entity.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
        }

        private void updateMoveFree(GameTime gameTime)
        {
            Vector3 camForward = camera.dir;
            camForward.Y = 0;
            camForward.Normalize();

            Vector3 right = Vector3.Cross(camForward, camera.up);
            right.Normalize();

            if (input.down3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(camForward, -DEFAULT_SPEED));
            }
            if (input.up3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(camForward, DEFAULT_SPEED));
            }
            if (input.left3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(-right, DEFAULT_SPEED));
            }
            if (input.right3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, Vector3.Multiply(right, DEFAULT_SPEED));
            }
        }
    }
}
