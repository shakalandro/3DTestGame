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
        // The position of the physics body
        public Vector3 Position
        {
            get
            {
                return this.Body.Position;
            }
            set
            {
                Console.WriteLine(value);
                this.Body.MoveTo(value, Matrix.Identity);  
            }
        }

        // The mass of the physics body
        private float _mass;
        public float Mass
        {
            get { return _mass; }
            set
            {
                // Set the new value
                _mass = value;

                // Fix transforms
                Vector3 com = SetMass(value);
                if (Skin != null)
                {
                    Skin.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
                }
            }
        }

        // The rotation of the body
        public Matrix Rotation
        {
            get { return Body.Orientation; }
            set { Body.MoveTo(Body.Position, value); }
        }

        // Whether the body is solid and immovable
        public bool Immovable
        {
            get { return Body.Immovable; }
            set { Body.Immovable = value; }
        }

        // The velocity of the body
        public Vector3 Velocity
        {
            get { return Body.Velocity; }
            set { Body.Velocity = value; }
        }

        private float Scale;
        

        protected Body Body;
        protected CollisionSkin Skin;

        public PhysicalModel(Game game, Model m, Boolean t, Vector3 pos, float scale) : this(game, m, t, pos, scale, false) {}

        public PhysicalModel(Game game, Model m, Boolean t, Vector3 pos, float scale, Boolean solid) : base(game, m, t)
        {
            this.Scale = scale;
            _mass = 1.0f;
            SetSkinAndBody();
            this.Immovable = solid;
            this.Position = pos;
        }

        public virtual void SetSkinAndBody()
        {
            this.Body = new Body();
            this.Skin = new CollisionSkin(this.Body);
            this.Body.CollisionSkin = this.Skin;
            if(model != null) {
                this.Skin.AddPrimitive(new Sphere(Vector3.Zero, this.Scale * model.Meshes[0].BoundingSphere.Radius),
                    new MaterialProperties(1f, 0.7f, 0.6f));
            }
            this.Skin.ApplyLocalTransform(new Transform(-1 * SetMass(1.0f), Matrix.Identity));
            this.Body.EnableBody();
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
                    VertexPositionColor[] vpc = this.Skin.GetLocalSkinWireframe();     //get the CollisionSkin
                    this.Body.TransformWireframe(vpc);           //transform the skin to the Body space
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
                Matrix.CreateScale(this.Scale) *
                Skin.GetPrimitiveLocal(0).Transform.Orientation *
                Body.Orientation *
                // This line is a hack to correct for the fact that the xna models use the Z axis for up and we use Y
                Matrix.CreateRotationX(-MathHelper.PiOver2) * 
                Matrix.CreateTranslation(this.Body.Position);
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

            this.Skin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);
 
            Body.BodyInertia = itCoM;
            Body.Mass = junk;
 
            return com;
        }
    }
}
