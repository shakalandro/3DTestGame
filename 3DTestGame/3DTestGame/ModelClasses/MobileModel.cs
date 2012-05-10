using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DTestGame
{
    public class MobileModel : PhysicalModel
    {
        private IInput input
        {
            get
            {
                return (IInput)this.Game.Services.GetService(typeof(IInput));
            }
        }

        public float speed;

        private static float DEFAULT_SPEED = 0.01f;

        public MobileModel(Game game, Model m, Boolean t, Vector3 pos, float scale)
            : this(game, m, t, pos, scale, false, DEFAULT_SPEED) { }

        public MobileModel(Game game, Model m, Boolean t, Vector3 pos, float scale, Boolean solid, float speed)
            :base(game, m, t, pos, scale, solid)
        {
            this.speed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            if (input.down3())
            {
                this.Velocity = Vector3.Add(this.Velocity, new Vector3(0f, 0f, DEFAULT_SPEED));
            }
            if (input.up3())
            {
                this.Velocity = Vector3.Add(this.Velocity, new Vector3(0f, 0f, -DEFAULT_SPEED));
            }
            if (input.left3())
            {
                this.Velocity = Vector3.Add(this.Velocity, new Vector3(-DEFAULT_SPEED, 0f, 0f));
            }
            if (input.right3())
            {
                this.Velocity = Vector3.Add(this.Velocity, new Vector3(DEFAULT_SPEED, 0f, 0f));
            }
            CheckMouseClick();
            base.Update(gameTime);
        }

        public override void ChangeEffect(BasicEffect e)
        {
            if (BasicModel.selected == this)
            {
                e.DiffuseColor = new Vector3(0.9f, 0f, 0f);
            }
            else
            {
                e.DiffuseColor = new Vector3(1f, 1f, 1f);
            }
            base.ChangeEffect(e);
        }

        private void CheckMouseClick()
        {
            if (this.input.leftClick())
            {
                // Create a ray in the direction of the camera with respect to the click coordinates 
                Ray pickRay = GetPickRay(this.input.mouseState.X, this.input.mouseState.Y);
            
                // Current distance to selected
                Nullable<float> minDist = null;
                if (BasicModel.selected != null) {
                    BoundingSphere selectedSphere = TransformBoundingSphere(BasicModel.selected.model.Meshes[0].BoundingSphere,
                            BasicModel.selected.model.Meshes[0].ParentBone.Transform * BasicModel.selected.GetWorld());
                    minDist = pickRay.Intersects(selectedSphere);
                }

                // Find my distance and compare
                Matrix[] transforms = new Matrix[this.model.Bones.Count];
                this.model.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in this.model.Meshes)
                {
                    BoundingSphere sphere = TransformBoundingSphere(mesh.BoundingSphere,
                        transforms[mesh.ParentBone.Index] * this.GetWorld());
                    Nullable<float> dist = pickRay.Intersects(sphere);
                    if ((!minDist.HasValue && dist.HasValue) || (dist.HasValue && dist.Value < minDist.Value))
                    {
                        BasicModel.selected = this;
                    }
                }
            }
        }


        private Microsoft.Xna.Framework.Ray GetPickRay(float x, float y)
        {
            Vector3 nearsource = new Vector3((float)x, (float)y, 0f);
            Vector3 farsource = new Vector3((float)x, (float)y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, camera.projection, camera.view, Matrix.CreateTranslation(0, 0, 0));

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, camera.projection, camera.view, Matrix.CreateTranslation(0, 0, 0));

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Microsoft.Xna.Framework.Ray pickRay = new Microsoft.Xna.Framework.Ray(nearPoint, direction);

            return pickRay;
        }

        private static BoundingSphere TransformBoundingSphere(BoundingSphere sphere, Matrix transform)
        {
            BoundingSphere transformedSphere;

            // the transform can contain different scales on the x, y, and z components.
            // this has the effect of stretching and squishing our bounding sphere along
            // different axes. Obviously, this is no good: a bounding sphere has to be a
            // SPHERE. so, the transformed sphere's radius must be the maximum of the 
            // scaled x, y, and z radii.

            // to calculate how the transform matrix will affect the x, y, and z
            // components of the sphere, we'll create a vector3 with x y and z equal
            // to the sphere's radius...
            Vector3 scale3 = new Vector3(sphere.Radius, sphere.Radius, sphere.Radius);

            // then transform that vector using the transform matrix. we use
            // TransformNormal because we don't want to take translation into account.
            scale3 = Vector3.TransformNormal(scale3, transform);

            // scale3 contains the x, y, and z radii of a squished and stretched sphere.
            // we'll set the finished sphere's radius to the maximum of the x y and z
            // radii, creating a sphere that is large enough to contain the original 
            // squished sphere.
            transformedSphere.Radius = Math.Max(scale3.X, Math.Max(scale3.Y, scale3.Z));

            // transforming the center of the sphere is much easier. we can just use 
            // Vector3.Transform to transform the center vector. notice that we're using
            // Transform instead of TransformNormal because in this case we DO want to 
            // take translation into account.
            transformedSphere.Center = Vector3.Transform(sphere.Center, transform);

            return transformedSphere;
        }
    }
}
