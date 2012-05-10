using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using JigLibX.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DTestGame
{
    // A physics object that simulates all the individual polygons in a model
    public class HeightMapModel : PhysicalModel
    {
        // TODO: Need to add png parameter
        public HeightMapModel(Game game, Model m, Boolean t, Vector3 pos, float scale)
            : base(game, m, t, pos, scale, true)
        {
            Body = new Body(); // just a dummy. The PhysicObject uses its position to get the draw pos
            Skin = new CollisionSkin(null);

            HeightMapInfo heightMapInfo = this.model.Tag as HeightMapInfo;
            Array2D field = new Array2D(heightMapInfo.heights.GetUpperBound(0), heightMapInfo.heights.GetUpperBound(1));

            for (int x = 0; x < heightMapInfo.heights.GetUpperBound(0); x++)
            {
                for (int z = 0; z < heightMapInfo.heights.GetUpperBound(1); z++)
                {
                    field.SetAt(x, z, heightMapInfo.heights[x, z]);
                }
            }

            // move the body. The body (because its not connected to the collision
            // skin) is just a dummy. But the base class shoudl know where to
            // draw the model.
            Body.MoveTo(this.Position, Matrix.Identity);

            Skin.AddPrimitive(new Heightmap(field, 0f, 0f, 1, 1), new MaterialProperties(0.7f, 0.7f, 0.6f));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(this.Skin);
        }

        public override void ChangeEffect(BasicEffect e)
        {
            e.PreferPerPixelLighting = true;
        }
    }
}