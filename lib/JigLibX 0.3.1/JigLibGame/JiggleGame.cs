#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;
using JiggleGame.PhysicObjects;
using JigLibX.Vehicles;
using System.Diagnostics;
#endregion

namespace JiggleGame
{

    #region ImmovableSkinPredicate
    class ImmovableSkinPredicate : CollisionSkinPredicate1
    {
        public override bool ConsiderSkin(CollisionSkin skin0)
        {
            if (skin0.Owner != null && !skin0.Owner.Immovable)
                return true;
            else
                return false;
        }
    }
    #endregion

    public class JiggleGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        Model boxModel, sphereModel, capsuleModel, compoundModel, terrainModel,cylinderModel,
            carModel, wheelModel, staticModel, planeModel, pinModel;

        PhysicsSystem physicSystem;
        DebugDrawer debugDrawer;
        Camera camera;

        CarObject carObject;

        ConstraintWorldPoint objectController = new ConstraintWorldPoint();
        ConstraintVelocity damperController = new ConstraintVelocity();

        public JiggleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = true;

            physicSystem = new PhysicsSystem();

            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;
            //physicSystem.CollisionSystem = new CollisionSystemGrid(32, 32, 32, 30, 30, 30);
            //physicSystem.CollisionSystem = new CollisionSystemBrute();
            physicSystem.CollisionSystem = new CollisionSystemSAP();

            physicSystem.EnableFreezing = true;
            physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            physicSystem.CollisionSystem.UseSweepTests = true;

            physicSystem.NumCollisionIterations = 8;
            physicSystem.NumContactIterations = 8;
            physicSystem.NumPenetrationRelaxtionTimesteps = 15;

            camera = new Camera(this);

            FrameRateCounter physStats = new FrameRateCounter(this, this.physicSystem);

            debugDrawer = new DebugDrawer(this);
            debugDrawer.Enabled = false;

            Components.Add(physStats);
            Components.Add(camera);
            Components.Add(debugDrawer);

            physStats.DrawOrder = 2;
            debugDrawer.DrawOrder = 3;

            this.IsMouseVisible = true;
            this.Window.Title = "JigLibX Physic Library " + System.Reflection.Assembly.GetAssembly(typeof(PhysicsSystem)).GetName().Version.ToString();
            
        }

        public DebugDrawer DebugDrawer
        {
            get { return debugDrawer; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        protected override void LoadContent()
        {
            boxModel = content.Load<Model>("content/box");
            sphereModel = content.Load<Model>("content/sphere");
            capsuleModel = content.Load<Model>("content/capsule");
            carModel = content.Load<Model>("content/car");
            wheelModel = content.Load<Model>("content/wheel");
            staticModel = content.Load<Model>("content/staticmesh");
            planeModel = content.Load<Model>("content/plane");
            pinModel = content.Load<Model>("content/pin");
            compoundModel = content.Load<Model>("content/compound");
            cylinderModel = content.Load<Model>("content/cylinder");

            try
            {
                // some video card can't handle the >16 bit index type of the terrain
                terrainModel = content.Load<Model>("content/terrain");
                HeightmapObject heightmapObj = new HeightmapObject(this, terrainModel,Vector2.Zero);
                this.Components.Add(heightmapObj);
            }
            catch(Exception)
            {
                // if that happens just createa a ground plane 
                PlaneObject planeObj = new PlaneObject(this, planeModel, 15.0f);
                this.Components.Add(planeObj);
            }

            TriangleMeshObject triObj = new TriangleMeshObject(this, staticModel, Matrix.Identity, Vector3.Zero);
            this.Components.Add(triObj);

            carObject = new CarObject(this, carModel, wheelModel, true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f,0.05f, 0.45f, 0.3f, 1, 520.0f, physicSystem.Gravity.Length());
            carObject.Car.Chassis.Body.MoveTo(new Vector3(-5, -13, 5), Matrix.Identity);
            carObject.Car.EnableCar();
            carObject.Car.Chassis.Body.AllowFreezing = false;
            this.Components.Add(carObject);

            camera.Position = Vector3.Down * 12 + Vector3.Backward * 30.0f;

            CreateScene6();

            base.LoadContent();
        }

        #region JigLibX Scenes
        private void CreateScene0()
        {
            // Newton was here

            BoxObject holder = new BoxObject(this, boxModel, new Vector3(5, 1, 1), Matrix.Identity, new Vector3(-10, -5, 5));
            holder.PhysicsBody.Immovable = true;
            this.Components.Add(holder);

            for (int i = 0; i < 5; i++)
            {
                SphereObject obj = new SphereObject(this, sphereModel, 0.5f, Matrix.Identity, new Vector3(-12 + i, -8, 5));
                obj.PhysicsBody.CollisionSkin.SetMaterialProperties(0, new MaterialProperties(1.0f, 0.2f, 0.2f));
                obj.PhysicsBody.AllowFreezing = false;

                ConstraintMaxDistance maxDist1 = new ConstraintMaxDistance(holder.PhysicsBody, new Vector3(-2 + i, -0.5f, 0.5f), obj.PhysicsBody, Vector3.Up * 0.5f, 3f);
                ConstraintMaxDistance maxDist2 = new ConstraintMaxDistance(holder.PhysicsBody, new Vector3(-2 + i, -0.5f, -0.5f), obj.PhysicsBody, Vector3.Up * 0.5f, 3f);
                maxDist1.EnableConstraint();
                maxDist2.EnableConstraint();

                this.Components.Add(obj);

                if (i == 4)
                    obj.PhysicsBody.MoveTo(new Vector3(-6, -6, 5), Matrix.Identity);
                if (i == 3)
                    obj.PhysicsBody.MoveTo(new Vector3(-7, -6, 5), Matrix.Identity);

            }
        }

        private void CreateScene1(int dim)
        {
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                    if (y % 2 == 0)
                        this.Components.Add(new BoxObject(this, boxModel, new Vector3(1, 1, 1), Matrix.Identity, new Vector3(x * 1.01f - 10.0f, y * 1.01f - 14.5f, 25)));
                    else
                        this.Components.Add(new BoxObject(this, boxModel, new Vector3(1, 1, 1), Matrix.Identity, new Vector3(x * 1.01f - 10.5f, y * 1.01f - 14.5f, 25)));
            }

        }

        private void CreateScene2()
        {
            for (int i = 0; i < 20; i++)
            {
                this.Components.Add(SpawnPrimitive(new Vector3(2, 3 * i + 10, 2), Matrix.Identity));
            }
            for (int i = 0; i < 20; i++)
            {
                this.Components.Add(SpawnPrimitive(new Vector3(2, 3 * i + 10, -2), Matrix.Identity));
            }
            for (int i = 0; i < 20; i++)
            {
                this.Components.Add(SpawnPrimitive(new Vector3(-2, 3 * i + 10, -2), Matrix.Identity));
            }
            for (int i = 0; i < 20; i++)
            {
                this.Components.Add(SpawnPrimitive(new Vector3(-2, 3 * i + 10, -2), Matrix.Identity));
            }
        }

        private void CreateScene4(int dim)
        {
            for (int x = 0; x < dim; x++)
            {
                BoxObject obj = new BoxObject(this, boxModel, Vector3.One, Matrix.Identity, new Vector3(0, x * 1.01f-14.0f, 25));
                this.Components.Add(obj);
            }
        }

        private void CreateScene5(int dim)
        {
            for (int x = 0; x < dim; x++)
            {
                for (int e = x; e < dim; e++)
                {
                    this.Components.Add(new BoxObject(this, boxModel, Vector3.One, Matrix.Identity, new Vector3(e - 0.5f * x, x * 1.01f - 14, 25)));
                }
            }
        }

        private void CreateScene6()
        {
            for (int i = 0; i < 10; i += 2)
            {
                BoxObject boxObj0 = new BoxObject(this, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(0, i * 1f - 14, 1));
                BoxObject boxObj1 = new BoxObject(this, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(1, i * 1f - 14, 1));
                BoxObject boxObj2 = new BoxObject(this, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(2, i * 1f - 14, 1));
                this.Components.Add(boxObj0); this.Components.Add(boxObj1); this.Components.Add(boxObj2);

                BoxObject boxObj3 = new BoxObject(this, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f - 14, 0));
                BoxObject boxObj4 = new BoxObject(this, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f - 14, 1));
                BoxObject boxObj5 = new BoxObject(this, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f - 14, 2));
                this.Components.Add(boxObj3); this.Components.Add(boxObj4); this.Components.Add(boxObj5);
            }

            for (int i = 0; i < 10; i++)
            {
                CylinderObject cyl = new CylinderObject(this, 0.5f, 1.0f, new Vector3(5, i * 1.01f - 14.2f, 0), cylinderModel);
                this.Components.Add(cyl);
            }

            RagdollObject rgd;

            // professional stuntmen, noone gets hurt!

            for (int e = 0; e < 2; e++)
            {
                for (int i = 0; i < 2; i++)
                {
                    rgd = new RagdollObject(this, capsuleModel, sphereModel, boxModel, RagdollObject.RagdollType.Simple, 1.0f);
                    rgd.Position = new Vector3(e * 2, -14, 10 + i * 2);
                    rgd.PutToSleep();
                }
            }


            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 3; y++)
                    if (y % 2 == 0)
                        this.Components.Add(new BoxObject(this, boxModel, new Vector3(1, 1, 1), Matrix.Identity, new Vector3(x * 1.01f - 10.0f, y * 1.01f - 14.5f, 0)));
                    else
                        this.Components.Add(new BoxObject(this, boxModel, new Vector3(1, 1, 1), Matrix.Identity, new Vector3(x * 1.01f - 10.5f, y * 1.01f - 14.5f, 0)));
            }
        }

        private void CreateScene7()
        {
            Matrix rotM = Matrix.CreateRotationY(0.5f);

            for (int i = 0; i < 15; i += 2)
            {
                BoxObject boxObj0 = new BoxObject(this, boxModel, new Vector3(1, 1, 4),rotM, new Vector3(0, i - 10, 25));
                BoxObject boxObj2 = new BoxObject(this, boxModel, new Vector3(1, 1, 4), rotM, new Vector3(2, i - 10, 25));
                this.Components.Add(boxObj0); this.Components.Add(boxObj2);

                BoxObject boxObj3 = new BoxObject(this, boxModel, new Vector3(4, 1, 1), rotM, new Vector3(1, i + 1 - 10, 24));
                BoxObject boxObj5 = new BoxObject(this, boxModel, new Vector3(4, 1, 1), rotM, new Vector3(1, i + 1 - 10, 26));
                this.Components.Add(boxObj3); this.Components.Add(boxObj5);
            }
        }

        private void CreateScene8()
        {

            for (int e = 0; e < 5; e++)
            {
                for (int i = e; i < 5; i++)
                {
                    BowlingPin bp = new BowlingPin(this, pinModel, Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(i * e), new Vector3(e, -14.2f, i));
                    this.Components.Add(bp);
                    bp.PhysicsBody.SetDeactivationTime(5.0f);
                }
            }


        }

        private void CreateScene9()
        {
            RagdollObject rgd;

            // professional stuntmen, noone gets hurt!

            for (int e = 0; e < 5; e++)
            {
                for (int i = 0; i < 5; i++)
                {
                    rgd = new RagdollObject(this, capsuleModel, sphereModel, boxModel, RagdollObject.RagdollType.Simple, 1.0f);
                    rgd.Position = new Vector3(e * 2, -14, 10 + i * 2);
                    rgd.PutToSleep();
                }
            }
        }

        private void CreateScene3()
        {
            // add a chain
            List<BoxObject> chainBoxes = new List<BoxObject>();

            for (int i = 0; i < 25; i++)
            {
                BoxObject boxObject = new BoxObject(this, boxModel, Vector3.One, Matrix.Identity, new Vector3(i, 25 - i, 0));
                if (i == 0) boxObject.PhysicsBody.Immovable = true;
                chainBoxes.Add(boxObject);
            }

            for (int i = 1; i < 25; i++)
            {
                HingeJoint hingeJoint = new HingeJoint();
                hingeJoint.Initialise(chainBoxes[i - 1].PhysicsBody, chainBoxes[i].PhysicsBody, Vector3.Backward,
                    new Vector3(0.5f, -0.5f, 0.0f), 0.5f, 90.0f, 90.0f, 0.0f, 0.2f);
                hingeJoint.EnableController();
                hingeJoint.EnableHinge();
            }

            foreach (BoxObject obj in chainBoxes)
            {
                this.Components.Add(obj);
            }

        }
        #endregion

        protected override void UnloadContent()
        {
           content.Unload();
           base.UnloadContent();
        }

        #region Update

        bool singleStep = false;
        bool leftButton = false;

        // for picking
        float camPickDistance = 0.0f;
        bool middleButton = false;
        int oldWheel = 0;

        Stopwatch sw = new Stopwatch();

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();


            #region Picking Objects with the mouse
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (middleButton == false)
                {
                    Vector3 ray = RayTo(mouseState.X, mouseState.Y);
                    float frac; CollisionSkin skin;
                    Vector3 pos, normal;

                    ImmovableSkinPredicate pred = new ImmovableSkinPredicate();

                    physicSystem.CollisionSystem.SegmentIntersect(out frac, out skin, out pos, out normal,
                        new Segment(camera.Position, ray * 1000.0f), pred);

                    if (skin != null && (skin.Owner != null))
                    {
                        if (!skin.Owner.Immovable)
                        {
                            Vector3 delta = pos - skin.Owner.Position;
                            delta = Vector3.Transform(delta, Matrix.Transpose(skin.Owner.Orientation));

                            camPickDistance = (camera.Position - pos).Length();
                            oldWheel = mouseState.ScrollWheelValue;

                            skin.Owner.SetActive();
                            objectController.Destroy();
                            damperController.Destroy();
                            objectController.Initialise(skin.Owner, delta, pos);
                            damperController.Initialise(skin.Owner, ConstraintVelocity.ReferenceFrame.Body, Vector3.Zero, Vector3.Zero);
                            objectController.EnableConstraint();
                            damperController.EnableConstraint();
                        }
                    }

                    middleButton = true;
                }

                if (objectController.IsConstraintEnabled && (objectController.Body != null))
                {
                    Vector3 delta = objectController.Body.Position - camera.Position;
                    Vector3 ray = RayTo(mouseState.X, mouseState.Y); ray.Normalize();
                    float deltaWheel = mouseState.ScrollWheelValue - oldWheel;
                    camPickDistance += deltaWheel * 0.01f;
                    Vector3 result = camera.Position + camPickDistance * ray;
                    oldWheel = mouseState.ScrollWheelValue;
                    objectController.WorldPosition = result;
                    objectController.Body.SetActive();
                }
            }
            else
            {
                objectController.DisableConstraint();
                damperController.DisableConstraint();
                middleButton = false;
            }
            #endregion

            if (mouseState.LeftButton == ButtonState.Pressed && leftButton == false)
            {
                PhysicObject physObj = SpawnPrimitive(camera.Position, Matrix.CreateRotationX(0.5f));
                physObj.PhysicsBody.Velocity = (camera.Target - camera.Position) * 20.0f;
                Components.Add(physObj);
                leftButton = true;
            }

            if (mouseState.LeftButton == ButtonState.Released) leftButton = false;

            Keys[] pressedKeys = keyState.GetPressedKeys();

            if (pressedKeys.Length != 0)
            {
                switch (pressedKeys[0])
                {
                    case Keys.D1: ResetScene(); CreateScene1(9); break;
                    case Keys.D2: ResetScene(); CreateScene2(); break;
                    case Keys.D3: ResetScene(); CreateScene3(); break;
                    case Keys.D4: ResetScene(); CreateScene4(12); break;
                    case Keys.D5: ResetScene(); CreateScene5(10); break;
                    case Keys.D6: ResetScene(); CreateScene6(); break;
                    case Keys.D7: ResetScene(); CreateScene7(); break;
                    case Keys.D8: ResetScene(); CreateScene8(); break;
                    case Keys.D9: ResetScene(); CreateScene9(); break;
                    case Keys.D0: ResetScene(); CreateScene0(); break;
                }
            }

            debugDrawer.Enabled = keyState.IsKeyDown(Keys.C);

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.Down))
            {
                if (keyState.IsKeyDown(Keys.Up))
                    carObject.Car.Accelerate = 1.0f;
                else
                    carObject.Car.Accelerate = -1.0f;
            }
            else
                carObject.Car.Accelerate = 0.0f;

            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Right))
            {
                if (keyState.IsKeyDown(Keys.Left))
                    carObject.Car.Steer = 1.0f;
                else
                    carObject.Car.Steer = -1.0f;
            }
            else
                carObject.Car.Steer = 0.0f;

            if (keyState.IsKeyDown(Keys.B))
                carObject.Car.HBrake = 1.0f;
            else
                carObject.Car.HBrake = 0.0f;

            
            if (singleStep == true && keyState.IsKeyDown(Keys.Space) == false)
            {
                // don't intergrate so we can step at will
            }
            else
            {
                float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (timeStep < 1.0f / 60.0f) physicSystem.Integrate(timeStep);
                else physicSystem.Integrate(1.0f / 60.0f);
            }

            base.Update(gameTime);
        }
        #endregion

        #region ResetScene
        private void ResetScene()
        {
            List<PhysicObject> toBeRemoved = new List<PhysicObject>();
            foreach (GameComponent gc in this.Components)
            {
                if (gc is PhysicObject && !(gc is HeightmapObject) 
                    && !(gc is CarObject) && !(gc is TriangleMeshObject)
                    && !(gc is PlaneObject))
                {
                    PhysicObject physObj = gc as PhysicObject;
                    toBeRemoved.Add(physObj);
                }
            }

            foreach (PhysicObject physObj in toBeRemoved)
            {
                physObj.PhysicsBody.DisableBody();
                this.Components.Remove(physObj);
            }

            int count = physicSystem.Controllers.Count;
            for (int i = 0; i < count; i++) physicSystem.Controllers[0].DisableController();
            count = physicSystem.Constraints.Count;
            for (int i = 0; i < count; i++) physicSystem.RemoveConstraint(physicSystem.Constraints[0]);

            //count = physicSystem.Constraints.Count;

            //for (int i = 0; i < count; i++)
            //    physicSystem.Controllers[0].DisableController();

            //count = physicSystem.Constraints.Count;

            //for (int i = 0; i < count; i++)
            //    physicSystem.Constraints[0].DisableConstraint;


        }
        #endregion

        public void RestoreRenderState()
        {
            this.graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            this.graphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
            this.graphics.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            this.graphics.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            this.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
        }

        private Vector3 RayTo(int x, int y)
        {
            Vector3 nearSource = new Vector3(x, y, 0);
            Vector3 farSource = new Vector3(x, y, 1);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = graphics.GraphicsDevice.Viewport.Unproject(nearSource, camera.Projection, camera.View, world);
            Vector3 farPoint = graphics.GraphicsDevice.Viewport.Unproject(farSource, camera.Projection, camera.View, world);

            Vector3 direction = farPoint - nearPoint;
            //direction.Normalize();
            return direction;
        }
        

        Random random = new Random();
        private PhysicObject SpawnPrimitive(Vector3 pos,Matrix ori)
        {
            int prim = random.Next(3);
            PhysicObject physicObj;

            float a = 1.0f + (float)random.NextDouble() * 1.0f;
            float b = a + (float)random.NextDouble() * 0.5f;
            float c = 2.0f / a / b;

            switch (prim)
            {
                case 0:
                    physicObj = new BoxObject(this, boxModel, new Vector3(a, b, c), ori, pos);
                    break;
                case 1:
                    physicObj = new SphereObject(this, sphereModel, 0.5f, ori, pos);
                    break;
                case 2:
                    physicObj = new CapsuleObject(this, capsuleModel, 0.5f,1f, ori, pos);
                    break; 
                default:
                    physicObj = new SphereObject(this, sphereModel, (float)random.Next(5, 15), ori, pos);
                    break;
            }
            return physicObj;
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
          //  graphics.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            base.Draw(gameTime);
        }
    }
}
