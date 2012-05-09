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
        public Vector3 position
        {
            get
            {
                return this.body.Position;
            }
            set
            {
                Console.WriteLine(value);
                this.body.MoveTo(value, Matrix.Identity);  
            }
        }
        private float scale;

        private Body body;
        private CollisionSkin skin;

        public PhysicalModel(Game game, Model m, Boolean t, Vector3 pos, float scale) : this(game, m, t, pos, scale, false) {}

        public PhysicalModel(Game game, Model m, Boolean t, Vector3 pos, float scale, Boolean solid) : base(game, m, t)
        {
            this.scale = scale;
            // Mutual composition
            this.body = new Body();
            this.skin = new CollisionSkin(this.body);
            this.body.CollisionSkin = this.skin;
            this.body.Immovable = solid;
            this.skin.AddPrimitive(new Sphere(Vector3.Zero, this.scale * model.Meshes[0].BoundingSphere.Radius), 
                    new MaterialProperties(1f, 0.7f, 0.6f));

            this.position = pos;

            this.skin.ApplyLocalTransform(new Transform(-1 * SetMass(1.0f), Matrix.Identity));
            this.body.EnableBody();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (((ISTestGame)this.Game).DEBUG)
            {
                    // Debug draw the sphere that bounds the model
                    Matrix[] transforms = new Matrix[this.model.Bones.Count];
                    this.model.CopyAbsoluteBoneTransformsTo(transforms);
                    //BoundingSphereRenderer.Draw(TransformBoundingSphere(m.model.Meshes[0].BoundingSphere,
                    //    transforms[m.model.Meshes[0].ParentBone.Index] * m.GetWorld()),
                    //        this.camera.view, this.camera.projection);

                    // Debug draw the collision skin sphere
                    VertexPositionColor[] vpc = this.skin.GetLocalSkinWireframe();     //get the CollisionSkin
                    this.body.TransformWireframe(vpc);           //transform the skin to the Body space
                    ((ISTestGame)this.Game).physDebug.DrawShape(vpc);
            }
            base.Draw(gameTime);
        }

        public override void ChangeEffect(BasicEffect e)
        {
            e.TextureEnabled = this.textured;
            base.ChangeEffect(e);
        }

        public override Matrix GetWorld()
        {
            return
                Matrix.CreateScale(this.scale) *
                skin.GetPrimitiveLocal(0).Transform.Orientation *
                body.Orientation *
                // This line is a hack to correct for the fact that the xna models use the Z axis for up and we use Y
                Matrix.CreateRotationX(-MathHelper.PiOver2) * 
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
