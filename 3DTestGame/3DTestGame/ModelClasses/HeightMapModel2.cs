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
    public class HeightMapModel2 : PhysicalModel
    {

        #region Constructor

        public HeightMapModel2(Game game, HeightMap info, Boolean t, Vector3 pos, float scale)
            : base(game , null, t, pos, scale)
        {
              // Game game, Model m, Boolean t, Vector3 pos, float scale, Boolean solid
            //Game game, HeightMap m, Boolean t, Vector3 pos, float scale

            this.Visible = false;
            Body = new Body();
            Skin = new CollisionSkin(null);
            //Skin.CollisionType = (int)CollisionTypes.Terrain;

            Array2D field = new Array2D(info.heights.GetUpperBound(0), info.heights.GetUpperBound(1));

            for (int x = 0; x < info.heights.GetUpperBound(0); x++)
            {
                for (int z = 0; z < info.heights.GetUpperBound(1); z++)
                {
                    field.SetAt(x, z, info.heights[x, z]);
                }
            }

            Body.MoveTo(new Vector3(info.heightmapPosition.X, info.heightmapPosition.Y, info.heightmapPosition.Y), Matrix.Identity);

            Skin.AddPrimitive(new Heightmap(field, info.heightmapPosition.X, info.heightmapPosition.Y, scale, scale), (int)MaterialTable.MaterialID.NotBouncyRough);

            Body.Immovable = true;

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(Skin);
        }

        #endregion

    }
}