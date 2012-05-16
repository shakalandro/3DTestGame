using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BEPUphysics.Entities;

namespace _3DTestGame
{
    /// <summary>
    /// Component that draws a model following the position and orientation of a BEPUphysics entity.
    /// </summary>
    public class ControledModel : EntityModel
    {
       
        private static float DEFAULT_SPEED = .33f;

        private IInput input
        {
            get
            {
                return (IInput)this.Game.Services.GetService(typeof(IInput));
            }
        }

        /// <summary>
        /// Creates a new EntityModel.
        /// </summary>
        /// <param name="entity">Entity to attach the graphical representation to.</param>
        /// <param name="model">Graphical representation to use for the entity.</param>
        /// <param name="transform">Base transformation to apply to the model before moving to the entity.</param>
        /// <param name="game">Game to which this component will belong.</param>
        public ControledModel(Entity entity, Model model, Matrix transform, Game game)
            : base(entity , model, transform, game)
        {}

        public override void Update(GameTime gameTime)
        {
            
            entity.AngularVelocity = Vector3.Zero;
            if (input.down3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, new Vector3(0f, 0f, DEFAULT_SPEED));
            }
            if (input.up3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, new Vector3(0f, 0f, -DEFAULT_SPEED));
            }
            if (input.left3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, new Vector3(-DEFAULT_SPEED, 0f, 0f));
            }
            if (input.right3())
            {
                entity.LinearVelocity = Vector3.Add(entity.LinearVelocity, new Vector3(DEFAULT_SPEED, 0f, 0f));
            }
            
            if (input.spaceKey()) {
                entity.LinearMomentum = Vector3.Add(entity.LinearMomentum, new Vector3(0f, 2f, 0f));
            }
             
             

            base.Update(gameTime);
        }
       
    }
}
