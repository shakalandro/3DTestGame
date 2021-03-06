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
        public static Random r = new Random();
        public readonly Boolean DEBUG = true;
        public readonly int RINGS_TO_WIN = 20;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Camera camera;
        public UserInput input;

        //character space
        public Space space;

        //doodad space
        public Space space2;

        public BasicModel terrain;

        public ControllableModel cube;

        public Water water;
        public Water waterfall;

        public SpriteBatch hud;
        public SpriteFont hudFont;
        public int numRingsHit;
        public int totalRings;
        public bool gameOver;


        public ISTestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            numRingsHit = 0;
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

            water = new Water(this, Matrix.CreateTranslation(new Vector3(-20, -3, -100)), 122, 122, 2, 8);

            Matrix waterfallTranslate = Matrix.CreateTranslation(new Vector3(5f, 25.5f, -85.5f));
            Matrix waterfallRotate = Matrix.CreateRotationX(MathHelper.PiOver2 - MathHelper.PiOver4 / 4);
            waterfall = new Water(this, waterfallRotate * waterfallTranslate, 20, 40, 0.5f, 8);

            Components.Add(water);
            Components.Add(waterfall);
            Components.Add(this.input);
            Components.Add(this.camera);
            Services.AddService(typeof(ICamera), this.camera);
            Services.AddService(typeof(IInput), this.input);

            hud = new SpriteBatch(this.GraphicsDevice);

            gameOver = false;

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
            hudFont = Content.Load<SpriteFont>(@"hudFont");

            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            //space2 = new Space();
            //space2.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            terrain = new BasicModel(this, Content.Load<Model>(@"Models/terrain"), true);
            StaticMesh terrainMesh = GetTerrainMesh(terrain, Matrix.Identity);
           
            //terrainMesh.Material.StaticFriction = 0;
            //terrainMesh.Material.KineticFriction = 1;
            space.Add(terrainMesh);
            terrainMesh.Tag = terrain;

            // StaticMesh terrainMesh2 = GetTerrainMesh(terrain, Matrix.Identity);
            // space2.Add(terrainMesh2);

            // ferns
            for (int i = 0; i < 20; i++)
            {
                Vector3 position = getSafeSpawn(0);
                Console.WriteLine(position);
                BasicModel tree = new BasicModel(this, Content.Load<Model>(@"Models/fern"), position, true);
                StaticMesh treeMesh = GetTerrainMesh(tree, Matrix.CreateTranslation(position));
                Components.Add(tree);
            }

            // palm trees
            for (int i = 0; i < 5; i++)
            {
                Vector3 position = getSafeSpawn(0);
                Console.WriteLine(position);
                BasicModel tree = new BasicModel(this, Content.Load<Model>(@"Models/palmTree"), position, true);
                StaticMesh treeMesh = GetTerrainMesh(tree, Matrix.CreateTranslation(position));
                Components.Add(tree);
            }

            //adding character
            cube = new ControllableModel(this, Content.Load<Model>(@"Models/character"),
                    new Box(new Vector3(0, 10, 0), 1, 1, 1, 2));
            space.Add(cube.entity);
            cube.entity.Material.KineticFriction = 1;
            cube.entity.Tag = cube;

            //Adding skydome
            Matrix skyDomeRotate = Matrix.CreateRotationX(MathHelper.Pi);
            BasicModel skyDome = new BasicModel(this, Content.Load<Model>(@"Models/skyDome"), skyDomeRotate, true);

            //adding rings
            totalRings = 16;
            for (int i = 0; i < totalRings; i++ )
            {
                makeCoin(getSafeSpawn(r.Next(25,35)));
            }

            // baobab trees
            for (int i = 0; i < 5; i++)
            {
                Vector3 position = getSafeSpawn(0);
                BasicModel tree = new BasicModel(this, Content.Load<Model>(@"Models/baobabTree"),position,true);
                Cylinder treeBox =  new Cylinder(position, 10,3, 1);

                treeBox.Mass = float.PositiveInfinity;
                space.Add(treeBox); 
                treeBox.Tag = tree;
                Components.Add(tree);
            }
            
            Components.Add(skyDome);
            Components.Add(terrain);
            Components.Add(cube);

            // turn off backface culling
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;
        }

        public Vector3 getSafeSpawn(int spawnHeight)
        {
           int zoneNum = r.Next(0,3);
           if(zoneNum == 0) { //zone 1
               return new Vector3(r.Next(0, 70), spawnHeight, r.Next(15, 60));
           }else if(zoneNum == 1) { // zone 2
               return new Vector3(r.Next(-80, 0), spawnHeight, r.Next(0, 60));
           }else { // zone 3
               return new Vector3(r.Next(-80, -25), spawnHeight, r.Next(-70, 0));
           }

        }

        public void makeCoin(Vector3 spawnVec)
        {
            totalRings++;
            CoinModel oneRing = new CoinModel(this, Content.Load<Model>(@"Models/bigRing"),
            new Box(spawnVec, 2, 6, 2, 1));
            space.Add(oneRing.entity);
            oneRing.entity.Tag = oneRing;
            Components.Add(oneRing);
            oneRing.entity.CollisionInformation.Events.InitialCollisionDetected += coinHit; 
        }

        // Returns a static mesh for the given model with the XNA/Blender rotation hack applied
        private StaticMesh GetTerrainMesh(BasicModel t, Matrix transform)
        {
            Vector3[] vertices;
            int[] indices;
            TriangleMesh.GetVerticesAndIndicesFromModel(terrain.model, out vertices, out indices);
            // rotate the computed vertices because of the blender to xna axis issue
            for (int i = 0; i < vertices.Count(); i++)
            {
                vertices[i] = Vector3.Transform(vertices[i], Matrix.CreateRotationX(-MathHelper.Pi / 2));
            }
            return new StaticMesh(vertices, indices, new AffineTransform(transform.Translation));
        }

        //Removes the coin from the game if it contacts the character 

        //coin is sender , other is whatever hits it
        public void coinHit(EntityCollidable sender, Collidable other, CollidablePairHandler pair) 
        {
            var otherEntityInformation = other as EntityCollidable;
            if (otherEntityInformation != null && otherEntityInformation.Entity.Tag == cube)
            {
                Components.Remove((CoinModel)sender.Entity.Tag);
                space.Remove(sender.Entity);
                Console.WriteLine("removed ring");
                numRingsHit++;
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
            //space2.Update();


            //check end game condition
            gameOver = numRingsHit >= RINGS_TO_WIN;

            if (r.Next(3000) < 10)
            {
                makeCoin(getSafeSpawn(r.Next(25, 35)));
            }

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

            hud.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            hud.DrawString(hudFont, "3D Test Game by: Roy,Gabe,Sean", Vector2.Zero, Color.White);

            if (gameOver)
            {
                hud.DrawString(hudFont, "You Won! Congrats!!!", new Vector2(0, 100), Color.White);
            }

            hud.DrawString(hudFont, "Coins: " + numRingsHit + "/" + RINGS_TO_WIN, new Vector2(GraphicsDevice.Viewport.Bounds.Width - 130, 0), Color.White);
            hud.End();

            this.GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
