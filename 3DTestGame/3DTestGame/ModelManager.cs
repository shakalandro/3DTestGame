using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3DTestGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {

        List<BasicModel> models;

        public ModelManager(Game game) : base(game)
        {
            this.models = new List<BasicModel>();
            Console.WriteLine("Manager made");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            Console.WriteLine("Initilized Manager");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            models.Add(new BasicModel(Game.Content.Load<Model>(@"Models/terrain2"), false));
            models.Add(new BasicModel(Game.Content.Load<Model>(@"Models/fern"), true));
            //models.Add(new PhysicalModel(Game.Content.Load<Model>(@"Models/p1_wedge"), true,
            //        new Vector3(3f, 0f, 0f), new Vector3(0.02f, 0.02f, 0.02f), true));
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < models.Count; i++ )
            {
                models[i].Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicModel m in models)
            {
                m.Draw(((ISTestGame)this.Game).camera);
            }

            base.Draw(gameTime);
        }
    }
}
