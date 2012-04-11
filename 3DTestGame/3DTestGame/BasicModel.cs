using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DTestGame
{
    class BasicModel
    {
        public Model model {
            get;
            protected set;
        }
        protected Matrix world = Matrix.Identity;

        public BasicModel(Model m)
        {
            this.model = m;
        }
        public virtual void update()
        {

        }
        public void draw(Camera camera)
        {
            
        }
    }
}
