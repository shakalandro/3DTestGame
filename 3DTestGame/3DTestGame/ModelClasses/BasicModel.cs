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

        public ICamera camera
        {
            get
            {
                return (ICamera)this.Game.Services.GetService(typeof(ICamera));
            }
        }

        public BasicModel(Game game, Model m, Boolean textured) : base(game)
        {
            this.model = m;
            this.textured = textured;
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
            return Matrix.Identity;
        }

        public virtual void ChangeEffect(BasicEffect e) {}
    }
}
