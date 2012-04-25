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

    public class BowlingPin : PhysicObject
    {

        public BowlingPin(Game game, Model model, Matrix orientation, Vector3 position)
            : base(game, model)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            // add a capsule for the main corpus
            Primitive capsule = new Capsule(Vector3.Zero, Matrix.Identity, 0.1f, 1.3f);
            // add a small box at the buttom
            Primitive box = new Box(new Vector3(-0.1f,-0.1f,-0.1f), Matrix.Identity, Vector3.One * 0.2f);
            // add a sphere in the middle
            Primitive sphere = new Sphere(new Vector3(0.0f, 0.0f, 0.3f), 0.3f);

            collision.AddPrimitive(capsule, new MaterialProperties(0.1f, 0.5f, 0.5f));
            collision.AddPrimitive(box, new MaterialProperties(0.1f, 0.5f, 0.5f));
            collision.AddPrimitive(sphere, new MaterialProperties(0.1f, 0.5f, 0.5f));

            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(0.5f);
   
            body.MoveTo(position, orientation);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            body.EnableBody();
            this.scale = Vector3.One * 10.0f;
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            effect.DiffuseColor = color;
        }
    }
}