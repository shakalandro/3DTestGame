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
using JigLibX.Collision;
using JigLibX.Physics;

namespace _3DTestGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public ICamera camera
        {
            get
            {
                return (ICamera)this.Game.Services.GetService(typeof(ICamera));
            }
        }
        public List<BasicModel> models;
        private UserInput input;
        private DebugDrawer physDebug;

        public ModelManager(Game game) : base(game)
        {
            this.models = new List<BasicModel>();
            this.input = ((ISTestGame)this.Game).input;
            this.physDebug = ((ISTestGame)this.Game).physDebug;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Console.WriteLine("Initilized Manager");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            models.Add(new PhysicalModel(Game.Content.Load<Model>(@"Models/terrain2"), false,
                    new Vector3(0f, 0f, 0f), 1.0f, true));
            models.Add(new PhysicalModel(Game.Content.Load<Model>(@"Models/cobra"), false,
                    new Vector3(0f, 5f, 0f), 0.5f, false));

            BoundingSphereRenderer.Initialize(GraphicsDevice, 45);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            CheckMouseClick();
            for (int i = 0; i < models.Count; i++ )
            {
                models[i].Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicModel m in models)
            {
                m.Draw(this.camera);
                // Debug draw the sphere that bounds the model
                Matrix[] transforms = new Matrix[m.model.Bones.Count];
                m.model.CopyAbsoluteBoneTransformsTo(transforms);
                //BoundingSphereRenderer.Draw(TransformBoundingSphere(m.model.Meshes[0].BoundingSphere,
                //    transforms[m.model.Meshes[0].ParentBone.Index] * m.GetWorld()),
                //        this.camera.view, this.camera.projection);
                if (m is PhysicalModel)
                {
                    // Debug draw the collision skin sphere
                    VertexPositionColor[] vpc = ((PhysicalModel)m).skin.GetLocalSkinWireframe();     //get the CollisionSkin
                    ((PhysicalModel)m).body.TransformWireframe(vpc);           //transform the skin to the Body space
                    this.physDebug.DrawShape(vpc);
                }
            }

            base.Draw(gameTime);
        }

        private void CheckMouseClick()
        {
            if (this.input.leftClick())
            {
                // Create a ray in the direction of the camera with respect to the click coordinates 
                Ray pickRay = GetPickRay(this.input.mouseState.X, this.input.mouseState.Y);

                // Find the Model that is closest to the camera along that ray
                BasicModel closest = null;
                float selectedDistance = float.MaxValue;
                for (int i = 0; i < this.models.Count; i++)
                {
                    BasicModel bm = this.models[i];
                    Matrix[] transforms = new Matrix[bm.model.Bones.Count];
                    bm.model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (ModelMesh mesh in bm.model.Meshes) {
                        BoundingSphere sphere = TransformBoundingSphere(mesh.BoundingSphere,
                            transforms[mesh.ParentBone.Index] * bm.GetWorld());
                        Nullable<float> dist = pickRay.Intersects(sphere);
                        if (dist.HasValue && dist.Value < selectedDistance)
                        {
                            closest = bm;
                            selectedDistance = dist.Value;
                        }
                    }
                }
                BasicModel.selected = closest;
            }
        }


        private Ray GetPickRay(float x, float y)
        {
            Vector3 nearsource = new Vector3((float)x, (float)y, 0f);
            Vector3 farsource = new Vector3((float)x, (float)y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, camera.projection, camera.view, Matrix.CreateTranslation(0,0,0));

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, camera.projection, camera.view, Matrix.CreateTranslation(0, 0, 0));

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

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
