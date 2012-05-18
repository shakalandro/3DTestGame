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

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Camera camera;
        public UserInput input;

        public Space space;
        public BasicModel terrain;

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
            this.Window.AllowUserResizing = true;

            // Create Services
            this.input = new UserInput(this);
            this.camera = new Camera(this, new Vector3(25f, 25f, 25f),
                new Vector3(-1f, -1f, -1f), Vector3.Up);

            Components.Add(this.input);
            Components.Add(this.camera);
            Services.AddService(typeof(ICamera), this.camera);
            Services.AddService(typeof(IInput), this.input);

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

            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            terrain = new BasicModel(this, Content.Load<Model>(@"Models/terrainTextured"), true);
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(terrain.model, out vertices, out indices);
            // rotate the computed vertices because of the blender to xna axis issue
            for (int i = 0; i < vertices.Count(); i++)
            {
                vertices[i] = Vector3.Transform(vertices[i], Matrix.CreateRotationX(-MathHelper.PiOver2));
            }
            var terrainMeshEntity = new StaticMesh(vertices, indices, new AffineTransform(new Vector3(0, -40, 0)));
            terrain.transform = terrainMeshEntity.WorldTransform.Matrix;
            space.Add(terrainMeshEntity);

            ControllableModel cube = new ControllableModel(this, Content.Load<Model>(@"Models/cube"),
                    new Box(new Vector3(0, 0, 0), 1, 1, 1, 1));
            space.Add(cube.entity);

            Components.Add(terrain);
            Components.Add(cube);
            /*maybe add later----- game logic ------*/
            //Hook an event handler to an entity to handle some game logic.
            //Refer to the Entity Events documentation for more information.
            //Box deleterBox = new Box(new Vector3(5, 2, 0), 3, 3, 3);
            //space.Add(deleterBox);
           // deleterBox.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;


            //Go through the list of entities in the space and create a graphical representation for them.
            /*
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
            */
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

            base.Draw(gameTime);
        }
    }
}
