#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Utils;
#endregion

namespace JiggleGame.PhysicObjects
{
    public class HeightmapObject : PhysicObject
    {
        public HeightmapObject(Game game, Model model,Vector2 shift)
            : base(game, model)
        {
            body = new Body(); // just a dummy. The PhysicObject uses its position to get the draw pos
            collision = new CollisionSkin(null);

            HeightMapInfo heightMapInfo = model.Tag as HeightMapInfo;
            Array2D field = new Array2D(heightMapInfo.heights.GetUpperBound(0), heightMapInfo.heights.GetUpperBound(1));

            for (int x = 0; x < heightMapInfo.heights.GetUpperBound(0); x++)
            {
                for (int z = 0; z < heightMapInfo.heights.GetUpperBound(1); z++)
                {
                    field.SetAt(x,z,heightMapInfo.heights[x,z]);  
                }
            }

            // move the body. The body (because its not connected to the collision
            // skin) is just a dummy. But the base class shoudl know where to
            // draw the model.
            body.MoveTo(new Vector3(shift.X,0,shift.Y), Matrix.Identity);
      
            collision.AddPrimitive(new Heightmap(field, shift.X, shift.Y, 1, 1), new MaterialProperties(0.7f,0.7f,0.6f));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(collision);
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            effect.PreferPerPixelLighting = true;
        }

    }
}
