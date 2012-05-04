using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;


namespace _3DTestGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicalModel : BasicModel
    {
        private Vector3 position;
        private Vector3 scale;

        public Body body { get; private set; }
        public CollisionSkin skin { get; private set; }

        public PhysicalModel(Model m, Boolean t, Vector3 pos, Vector3 scale) : this(m, t, pos, scale, false) {}

        public PhysicalModel(Model m, Boolean t, Vector3 pos, Vector3 scale, Boolean solid) : base(m, t)
        {
            this.position = pos;
            this.scale = scale;

            // Mutual composition
            this.body = new Body();
            this.skin = new CollisionSkin(this.body);
            this.body.CollisionSkin = this.skin;
            this.body.Immovable = solid;
           
            this.skin.AddPrimitive(new Box(Vector3.Zero, Matrix.Identity, Vector3.Multiply(this.scale, model.Meshes[0].BoundingSphere.Radius)), 
                    new MaterialProperties(0.8f, 0.7f, 0.6f));

            this.body.MoveTo(this.position, Matrix.Identity);

            this.skin.ApplyLocalTransform(new Transform(-1 * SetMass(1.0f), Matrix.Identity));
            this.body.EnableBody();
            //this.body.CollisionSkin.WorldBoundingBox
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.TextureEnabled = this.textured;
                    if (BasicModel.selected == this)
                    {
                        be.DiffuseColor = new Vector3(0.9f, 0f, 0f);
                    }
                    else
                    {
                        be.DiffuseColor = new Vector3(1f, 1f, 1f);
                    }
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        public override Matrix GetWorld()
        {
            return
                Matrix.CreateScale(this.scale) *
                skin.GetPrimitiveLocal(0).Transform.Orientation *
                body.Orientation *
                Matrix.CreateTranslation(this.body.Position);
        }

        private Vector3 SetMass(float mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(
                PrimitiveProperties.MassDistributionEnum.Solid,
                PrimitiveProperties.MassTypeEnum.Mass, mass);
 
            float junk;
            Vector3 com;
            Matrix it;
            Matrix itCoM;

            this.skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
 
            body.BodyInertia = itCoM;
            body.Mass = junk;
 
            return com;
        }
    }
}
