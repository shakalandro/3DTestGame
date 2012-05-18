using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DTestGame
{
    public class BasicModel : DrawableGameComponent
    {
        public static BasicModel selected;

        public Model model { get; protected set; }
        public Boolean textured;
        public Matrix transform;
        public Vector3 forward;

        public ICamera camera
        {
            get
            {
                return (ICamera)this.Game.Services.GetService(typeof(ICamera));
            }
        }

        public BasicModel(Game game, Model m, Boolean textured) : this(game, m, Matrix.Identity, textured) {}

        public BasicModel(Game game, Model m, Matrix transform, Boolean textured) : base(game)
        {
            this.transform = transform;
            this.model = m;
            this.textured = textured;
            this.forward = this.camera.dir;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = mesh.ParentBone.Transform * GetWorld();
                    ChangeEffect(be);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        public virtual Matrix GetWorld()
        {
            return Matrix.CreateRotationX(-MathHelper.PiOver2) * this.transform;
        }

        public virtual void ChangeEffect(BasicEffect e)
        {
            e.TextureEnabled = this.textured;
        }
    }
}
