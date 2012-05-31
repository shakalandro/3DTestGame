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
        public const float DEFAULT_SIZE = 20.0f;
        public const int DEFAULT_SEGMENTS = 10;
        public const float WAVE_HEIGHT = (float)(DEFAULT_SEGMENTS / 10);
        public const float WAVE_SPEED = WAVE_HEIGHT / 20;
        public static Random r = new Random();

        public Matrix transform;
        public float width;
        public float waveHeight;
        public float length;
        public int segments;

        private VertexBuffer buf;
        private IndexBuffer indexBuf;
        public List<VertexPositionTexture> verts;
        private List<float> vertDirs;
        public List<int> indices;
        public Effect effect;

        public Texture2D texture;

        public ICamera camera
        {
            get
            {
                return (ICamera)this.Game.Services.GetService(typeof(ICamera));
            }
        }

        public Water(Game g) : this(g, Matrix.Identity, DEFAULT_SIZE, DEFAULT_SIZE, WAVE_HEIGHT, DEFAULT_SEGMENTS) {}

        public Water(Game g, Matrix transform, float width, float length, float height, int segments) : base(g)
        {
            this.transform = transform;
            this.width = width;
            this.length = length;
            this.waveHeight = height;
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

            effect = Game.Content.Load<Effect>(@"Effects/PartialReflection");

            base.LoadContent();
        }

        // Sets the vertices if we divide the width and length into n sections.
        // The result has 2n^2 primitives.
        private void setVertices(int n)
        {
            verts = new List<VertexPositionTexture>();
            for (int i = 0; i <= n; i++)
            {
                Vector3 position = new Vector3(0.0f,
                        randHeight(),
                        (float)((i * 1.0 / n) * length));
                position = Vector3.Transform(position, transform);
                verts.Add(new VertexPositionTexture(position, new Vector2(0.0f, (float)(i * 1.0 / n))));
            }
            for (int i = 1; i <= n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    Vector3 position = new Vector3((float)((i * 1.0 / n) * width),
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
                    position = Vector3.Transform(position, transform);
                    verts.Add(new VertexPositionTexture(position, new Vector2((float)(i * 1.0 / n), (float)(j * 1.0 / n))));
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
            return (float)((r.NextDouble() * 2 * waveHeight) - waveHeight);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetVertexBuffer(buf);
            GraphicsDevice.Indices = indexBuf;

            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * camera.view * camera.projection);
            effect.Parameters["xColoredTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verts.Count(), 0, (int)(2 * Math.Pow(segments, 2)));
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                VertexPositionTexture v = verts[i];
                Matrix reverse = Matrix.Invert(transform);
                Vector3 normalPos = Vector3.Transform(v.Position, reverse);
                if (Math.Abs(normalPos.Y) >= waveHeight) {
                    vertDirs[i] *= -1;
                }
                normalPos = new Vector3(normalPos.X, normalPos.Y + vertDirs[i], normalPos.Z);
                v.Position = Vector3.Transform(normalPos, transform);
                verts[i] = v;
            }
            buf.SetData<VertexPositionTexture>(verts.ToArray());
            
            base.Update(gameTime);
        }
    }
}
