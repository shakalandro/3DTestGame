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
/*
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Utils;
using JigLibX.Math;
 */

/*------BEPU includes ------*/
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.DataStructures;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace _3DTestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ISTestGame : Microsoft.Xna.Framework.Game
    {
        public readonly Boolean DEBUG = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;
        public UserInput input;

        //public PhysicsSystem phys;

        public DebugDrawer physDebug;


        /*-----BEPU Physics fields-----*/
        Space space;
        public Model terrain;
        public Model cube;
        Box testBox;

        

        /*-------------*/

        public ISTestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            /*
            this.phys = new PhysicsSystem();
            this.phys.CollisionSystem = new CollisionSystemSAP();
             */
            this.input = new UserInput(this);
            this.camera = new Camera(this, new Vector3(25f, 25f, 25f),
                new Vector3(-1f, -1f, -1f), Vector3.Up);
            this.physDebug = new DebugDrawer(this, this.camera);
            this.physDebug.Enabled = true;
            Components.Add(this.input);
            Components.Add(this.camera);
            Services.AddService(typeof(ICamera), this.camera);
            Services.AddService(typeof(IInput), this.input);
            Components.Add(this.physDebug);

            /*-----BEPU Physics stuff-----*/

            /*
            Components.Add(new PlaneModel(this, this.Content.Load<Model>(@"Models/terrainUntextured"), false,

                    new Vector3(0f, 0f, 0f)));
            Components.Add(new MobileModel(this, this.Content.Load<Model>(@"Models/cobra"), false,
                    new Vector3(0f, 10f, 0f), 0.5f, false, 0.01f));
            
            // Turn off backface culling for now
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;
            */
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);



            /*-----BEPU Physics content loading-----*/
            terrain = Content.Load<Model>(@"Models/terrainUntextured");
            cube = Content.Load<Model>(@"Models/cube");

            space = new Space();

            //set the gravity of space
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            //Sample 
            //Box ground = new Box(Vector3.Zero, 30, 1, 30);
            //space.Add(ground);


            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(terrain, out vertices, out indices);
            //Give the mesh information to a new StaticMesh.  
            //Give it a transformation which scoots it down below the kinematic box entity we created earlier.
            var mesh = new StaticMesh(vertices, indices, new AffineTransform(new Vector3(0, -40, 0)));

            //Add it to the space!
            space.Add(mesh);
            //Make it visible too.
            Components.Add(new StaticModel(terrain, mesh.WorldTransform.Matrix, this));

            testBox = new Box(new Vector3(0, 12, 0), 1, 1, 1, 1);
            space.Add(testBox);
            

            /*maybe add later----- game logic ------*/
            //Hook an event handler to an entity to handle some game logic.
            //Refer to the Entity Events documentation for more information.
            //Box deleterBox = new Box(new Vector3(5, 2, 0), 3, 3, 3);
            //space.Add(deleterBox);
           // deleterBox.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;


            //Go through the list of entities in the space and create a graphical representation for them.
            foreach (Entity e in space.Entities)
            {
                Box box = e as Box;
                if (box != null) //This won't create any graphics for an entity that isn't a box since the model being used is a box.
                {

                    Matrix scaling = Matrix.CreateScale(box.Width, box.Height, box.Length); //Since the cube model is 1x1x1, it needs to be scaled to match the size of each individual box.
                    EntityModel model = new ControledModel(e, cube, scaling, this);
                    //Add the drawable game component for this entity to the game.
                    Components.Add(model);
                    e.Tag = model; //set the object tag of this entity to the model so that it's easy to delete the graphics component later if the entity is removed.
                }
            }

        }





        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

           
            //Steps the simulation forward one time step.
            space.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
