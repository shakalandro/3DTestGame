using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;

namespace _3DTestGame
{
    class PlaneModel : PhysicalModel
    {

        public PlaneModel(Game game, Model m, Boolean t, Vector3 pos)
            : base(game, m, t, pos, 1f, true) {}

        public override void SetSkinAndBody()
        {
            Body = new Body();
            Skin = new CollisionSkin(null);
            Skin.AddPrimitive(new JigLibX.Geometry.Plane(Vector3.Up, 0f), new MaterialProperties(0.2f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(Skin);
        }
    }
}
