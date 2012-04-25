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

    public class BoxObject : PhysicObject
    {

        public BoxObject(Game game,Model model,Vector3 sideLengths, Matrix orientation, Vector3 position) : base(game,model)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            collision.AddPrimitive(new Box(- 0.5f * sideLengths, orientation, sideLengths), new MaterialProperties(0.8f, 0.8f, 0.7f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(1.0f);
            body.MoveTo(position, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
            this.scale = sideLengths;
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            effect.DiffuseColor = color;
        }
    }
}


