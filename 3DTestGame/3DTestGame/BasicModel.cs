﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace _3DTestGame
{
    public class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        public Boolean textured;

        public BasicModel(Model m, Boolean textured)
        {
            this.model = m;
            this.textured = textured;
        }

        public virtual void Update() {}

        public virtual void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.TextureEnabled = this.textured;
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
    }
}
