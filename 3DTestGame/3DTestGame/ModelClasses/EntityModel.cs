using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;


namespace _3DTestGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class EntityModel : BasicModel
    {
        public Entity entity;

        public EntityModel(Game game, Model m, Entity entity) : base(game, m, true)
        {
            this.entity = entity;
        }

        public override Matrix GetWorld()
        {
            return
                // This line is a hack to correct for the fact that the xna models use the Z axis for up and we use Y
                Matrix.CreateRotationX(-MathHelper.PiOver2) *
                this.entity.WorldTransform;
        }
    }
}
