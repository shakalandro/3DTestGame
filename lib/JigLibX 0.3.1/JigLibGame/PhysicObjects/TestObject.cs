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

    public class TestObject : PhysicObject
    {

        public TestObject(Game game,Model model) : base(game,model)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            Box boxMiddle = new Box(new Vector3(-3, 0 ,-0.5f), Matrix.Identity, new Vector3(6,1,1));
            Box boxLeft = new Box(new Vector3(-3, -3f, -0.5f), Matrix.Identity, new Vector3(1, 4, 1));
            Box boxRight = new Box(new Vector3(2, -3f, -0.5f), Matrix.Identity, new Vector3(1, 4, 1));

            collision.AddPrimitive(boxMiddle, new MaterialProperties(0.2f, 0.7f, 0.6f));
            collision.AddPrimitive(boxLeft, new MaterialProperties(0.2f, 0.7f, 0.6f));
            collision.AddPrimitive(boxRight, new MaterialProperties(0.2f, 0.7f, 0.6f));

            body.CollisionSkin = this.collision;

        
            Vector3 com = SetMass(1.0f);
           // collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));

            body.MoveTo(Vector3.Up * 10, Matrix.Identity);
            
            //body.Immovable = true;
            body.EnableBody();
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            //throw new NotImplementedException();
        }
    }
}


