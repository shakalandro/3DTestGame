using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3DTestGame
{
    public class Water : DrawableGameComponent
    {
        public const float DEFAULT_HEIGHT = -38.0f;
        public const float DEFAULT_SIZE = 20.0f;
        public const int DEFAULT_SEGMENTS = 10;
        public const float WAVE_HEIGHT = (float)(DEFAULT_SEGMENTS / 10);
        public const float WAVE_SPEED = WAVE_HEIGHT / 20;
        public static Random r = new Random();

        public float width;
        public float height;
        public float length;
        public int segments;

        private VertexBuffer buf;
        private IndexBuffer indexBuf;
        public List<VertexPositionTexture> verts;
        private List<float> vertDirs;
        public List<int> indices;
        public BasicEffect effect;

        public Texture2D texture;

        public ICamera camera
        {
            get
            {
                return (ICamera)this.Game.Services.GetService(typeof(ICamera));
            }
        }

        public Water(Game g) : this(g, DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_HEIGHT, DEFAULT_SEGMENTS) {}

        public Water(Game g, float width, float length, float height, int segments) : base(g)
        {
            this.width = width;
            this.length = length;
            this.height = height;
            this.segments = segments;
            verts = new List<VertexPositionTexture>();
            vertDirs = new List<float>();
            indices = new List<int>();
        }

        protected override void LoadContent()
        {
            setVertices(segments);

            buf = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), verts.Count, BufferUsage.None);
            indexBuf = new IndexBuffer(GraphicsDevice, typeof(int), indices.Count, BufferUsage.WriteOnly);
            indexBuf.SetData<int>(indices.ToArray());

            texture = Game.Content.Load<Texture2D>(@"Textures/water");

            effect = new BasicEffect(GraphicsDevice);

            base.LoadContent();
        }

        // Sets the vertices if we divide the width and length into n sections.
        // The result has 2n^2 primitives.
        private void setVertices(int n)
        {
            verts = new List<VertexPositionTexture>();
            for (int i = 0; i <= n; i++)
            {
                Vector3 pos = new Vector3(0.0f,
                        (float)(height + r.NextDouble() * 2),
                        (float)((i * 1.0 / n) * length));
                verts.Add(new VertexPositionTexture(pos, new Vector2(0.0f, (float)(i * 1.0 / n))));
            }
            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    Vector3 pos = new Vector3((float)((i * 1.0 / n) * width),
                            randHeight(),
                            (float)((j * 1.0 / n) * length));
                    if (j != 0)
                    {
                        indices.Add((i - 1) * (n + 1) + j);
                        indices.Add(i * (n + 1) + j - 1);
                        indices.Add(i * (n + 1) + j);
                    }
                    if (j != n)
                    {
                        indices.Add((i - 1) * (n + 1) + j + 1);
                        indices.Add((i - 1) * (n + 1) + j);
                        indices.Add(i * (n + 1) + j); 
                    }
                    verts.Add(new VertexPositionTexture(pos, new Vector2((float)(i * 1.0 / n), (float)(j * 1.0 / n))));
                }
            }
            float nonZeroDelta = WAVE_SPEED / 10;
            for (int i = 0; i < verts.Count; i++)
            {
                vertDirs.Add((float)((r.NextDouble() + nonZeroDelta) * 2 * WAVE_SPEED - (WAVE_SPEED + nonZeroDelta)));
            }
        }

        private Color randColor()
        {
            return new Color((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
        }

        private float randHeight()
        {
            return (float)(height + (r.NextDouble() * 2 * WAVE_HEIGHT) - WAVE_HEIGHT);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetVertexBuffer(buf);
            GraphicsDevice.Indices = indexBuf;

            effect.World = Matrix.Identity;
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.Texture = texture;
            effect.TextureEnabled = true;

            foreach (EffectPass p in effect.CurrentTechnique.Passes) {
                p.Apply();

                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verts.Count(), 0, (int)(2 * Math.Pow(segments, 2)));
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                VertexPositionTexture v = verts[i];
                if (Math.Abs(v.Position.Y - height) >= WAVE_HEIGHT) {
                    vertDirs[i] *= -1;
                }
                v.Position = new Vector3(v.Position.X, v.Position.Y + vertDirs[i], v.Position.Z);
                verts[i] = v;
            }
            buf.SetData<VertexPositionTexture>(verts.ToArray());
            
            base.Update(gameTime);
        }
    }
}
