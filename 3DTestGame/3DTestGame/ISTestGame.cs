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
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Utils;
using JigLibX.Math;

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
        public PhysicsSystem phys;
        public DebugDrawer physDebug;


        //Heightmap Related
        Texture2D terrain;            ///////
        HeightMap heightMap;

        //rendering related
        VertexBuffer vertexBuffer;      //vertex information for the graphics device
        Effect effect;             //effect to draw with

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
            this.phys = new PhysicsSystem();
            this.phys.CollisionSystem = new CollisionSystemSAP();
            this.input = new UserInput(this);
            this.camera = new Camera(this, new Vector3(50f, 50f, 50f),
                new Vector3(-1f, -1f, -1f), Vector3.Up);
            this.physDebug = new DebugDrawer(this, this.camera);
            this.physDebug.Enabled = true;
            Components.Add(this.input);
            Components.Add(this.camera);
            Services.AddService(typeof(ICamera), this.camera);
            Services.AddService(typeof(IInput), this.input);
            Components.Add(this.physDebug);


            /*---------------------------------*/
            //make heightmap
            terrain = Content.Load<Texture2D>(@"Models/Heightmap2");
            heightMap = new HeightMap(terrain, 30);

            //make buffer for heightmap
            //set vertex data in buffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), heightMap.getVertices().Length, BufferUsage.None);
            vertexBuffer.SetData(heightMap.getVertices());

            //initialize basic effect(tells graphics device to get ready to draw the effect)
            effect = Content.Load<Effect>(@"Models/effects");

            /*---------------------------------*/

            Components.Add(new HeightMapModel2(this, heightMap, false,
                    new Vector3(0f, 0f, 0f), 1.0f));
            Components.Add(new PlaneModel(this, this.Content.Load<Model>(@"Models/terrain2"), false,
                    new Vector3(0f, 0f, 0f)));
            Components.Add(new MobileModel(this, this.Content.Load<Model>(@"Models/cobra"), false,
                    new Vector3(40f, 40f, 0f), 0.5f, false, 0.01f));

            // Turn off backface culling for now
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;

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

            // TODO: use this.Content to load your game content here
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

            float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);

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


            /*-----------Height Map drawing code (if needed)*/

            //NOTE: can turn this into basic effect later (or remove for meshes)
            Matrix worldMatrix = Matrix.CreateTranslation(-heightMap.getWidth() / 2.0f, 0, heightMap.getHeight() / 2.0f);
            effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            effect.Parameters["xView"].SetValue(camera.view);
            effect.Parameters["xProjection"].SetValue(camera.projection);
            effect.Parameters["xWorld"].SetValue(worldMatrix);

            //begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration); IndexElementSize.SixteenBits
                GraphicsDevice.DrawUserIndexedPrimitives(Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, heightMap.getVertices(), 0, heightMap.getVertices().Length, heightMap.getIndices(), 0, heightMap.getIndices().Length / 3, VertexPositionColor.VertexDeclaration);
            }


            /*-----------------------------Microsoft.Xna.Framework.Graphics----*/




            base.Draw(gameTime);
        }
    }
}
