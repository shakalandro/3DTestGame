#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace JiggleGame.PhysicObjects
{

    public class CylinderObject : PhysicObject
    {

        public CylinderObject(Game game, float radius, float length, Vector3 position, Model model)
            : base(game, model)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            if (length - 2.0f * radius < 0.0f) 
                throw new ArgumentException("Radius must be at least half length");

            Capsule middle = new Capsule(Vector3.Zero, Matrix.Identity, radius, length - 2.0f * radius);

            float sideLength = 2.0f * radius / (float) Math.Sqrt(2.0d);

            Vector3 sides = new Vector3(-0.5f * sideLength, -0.5f * sideLength, -radius);

            Box supply0 = new Box(sides, Matrix.Identity,
                new Vector3(sideLength, sideLength, length));

            Box supply1 = new Box(Vector3.Transform(sides,Matrix.CreateRotationZ(MathHelper.PiOver4)), 
                Matrix.CreateRotationZ(MathHelper.PiOver4), new Vector3(sideLength, sideLength, length));

            collision.AddPrimitive(middle, new MaterialProperties(0.8f, 0.8f, 0.7f));
            collision.AddPrimitive(supply0, new MaterialProperties(0.8f, 0.8f, 0.7f));
            collision.AddPrimitive(supply1, new MaterialProperties(0.8f, 0.8f, 0.7f));

            body.CollisionSkin = this.collision;

            Vector3 com = SetMass(1.0f);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            #region Manually set body inertia
            float cylinderMass = body.Mass;

            float comOffs = (length - 2.0f * radius) * 0.5f; ;

            float Ixx = 0.5f * cylinderMass * radius * radius + cylinderMass * comOffs * comOffs;
            float Iyy = 0.25f * cylinderMass * radius * radius + (1.0f / 12.0f) * cylinderMass * length * length + cylinderMass * comOffs * comOffs;
            float Izz = Iyy;

            body.SetBodyInertia(Ixx, Iyy, Izz);
            #endregion

            body.MoveTo(position, Matrix.CreateRotationX(MathHelper.PiOver2));
            
            body.EnableBody();

            this.scale = new Vector3(radius, radius, length * 0.5f);
        }


        public override void ApplyEffects(BasicEffect effect)
        {
            effect.DiffuseColor = color;
        }
    }
}


