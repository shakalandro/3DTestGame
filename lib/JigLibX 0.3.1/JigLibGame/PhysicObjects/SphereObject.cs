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

    public class SphereObject : PhysicObject
    {

        public SphereObject(Game game, Model model,float radius, Matrix orientation, Vector3 position)
            : base(game, model)
        {
            body = new Body();
            collision = new CollisionSkin(body);
            collision.AddPrimitive(new Sphere(Vector3.Zero * 5.0f,radius), new MaterialProperties(0.5f,0.7f,0.6f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(10.0f);
            body.MoveTo(position + com, orientation);
           // collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
            this.scale = Vector3.One * radius;
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            effect.DiffuseColor = color;
        }

    }
}
