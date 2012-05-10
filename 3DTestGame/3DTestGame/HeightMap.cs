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


namespace _3DTestGame
{
    /// <summary>
    /// HeightMapInfo is a collection of data about the heightmap. 
    /// </summary>
    public class HeightMap
    {
       
        // TerrainScale is the distance between each entry in the Height property.
        // For example, if TerrainScale is 30, Height[0,0] and Height[1,0] are 30
        // units apart.        
        public float terrainScale;

        // This 2D array of floats tells us the height that each position in the 
        // heightmap is.
        public float[,] heights;

        // the position of the heightmap's -x, -z corner, in worldspace.
        public Vector3 heightmapPosition;

        // including terrain scale
        public int textureWidth;
        public int textureHeight;

        //array of vertexes
        VertexPositionColor[] vertices;

        //array of indices to be drawn
        int[] indices;

        // the constructor will initialize all of the member variables.
        public HeightMap(Texture2D texture, float terrainScale)
        {
           
            this.terrainScale = terrainScale;
         

            //build float array
            textureWidth = texture.Width;
            textureHeight = texture.Height;

            Color[] heightMapColors = new Color[textureWidth * textureHeight];
            texture.GetData(heightMapColors);

            heights = new float[textureWidth, textureHeight];
            for (int x = 0; x < textureWidth; x++)
                for (int y = 0; y < textureHeight; y++)
                {
                   
                    heights[x, y] = heightMapColors[x + y * textureWidth].R / 5.0f;
                }
            SetUpVertices();
            SetUpIndices();
        }

        //sets of a group of vertices to be drawn
        private void SetUpVertices()
        {
            vertices = new VertexPositionColor[textureWidth * textureHeight];
            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    vertices[x + y * textureWidth].Position = new Vector3(x, heights[x,y], -y);
                    vertices[x + y * textureWidth].Color = Color.White;
                }
            }
        }


        //makes three indices for each triangle to be drawn
        private void SetUpIndices()
        {
            indices = new int[(textureWidth - 1) * (textureHeight - 1) * 3];
            int counter = 0;
            for (int y = 0; y < textureHeight - 1; y++)
            {
                for (int x = 0; x < textureWidth - 1; x++)
                {
                    int lowerLeft = x + y * textureWidth;
                    int lowerRight = (x + 1) + y * textureWidth;
                    int topLeft = x + (y + 1) * textureWidth;
                    int topRight = (x + 1) + (y + 1) * textureWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;
                }
            }
        }

        public VertexPositionColor[] getVertices()
        {
            return vertices;
        }

        public int[] getIndices()
        {
            return indices;
        }

        public int getWidth()
        {

            return textureHeight;
        }

        public int getHeight()
        {

            return textureHeight;
        }

        /*
        // This function takes in a position, and tells whether or not the position is 
        // on the heightmap.
        public bool IsOnHeightmap(Vector3 position)
        {
            // first we'll figure out where on the heightmap "position" is...
            Vector3 positionOnHeightmap = position - heightmapPosition;

            // ... and then check to see if that value goes outside the bounds of the
            // heightmap.
            return (positionOnHeightmap.X > 0 &&
                positionOnHeightmap.X < heightmapWidth &&
                positionOnHeightmap.Z > 0 &&
                positionOnHeightmap.Z < heightmapHeight);
        }

        // This function takes in a position, and returns the heightmap's height at that
        // point. Be careful - this function will throw an IndexOutOfRangeException if
        // position isn't on the heightmap!
        // This function is explained in more detail in the accompanying doc.
        public float GetHeight(Vector3 position)
        {
            // the first thing we need to do is figure out where on the heightmap
            // "position" is. This'll make the math much simpler later.
            Vector3 positionOnHeightmap = position - heightmapPosition;

            // we'll use integer division to figure out where in the "heights" array
            // positionOnHeightmap is. Remember that integer division always rounds
            // down, so that the result of these divisions is the indices of the "upper
            // left" of the 4 corners of that cell.
            int left, top;
            left = (int)positionOnHeightmap.X / (int)terrainScale;
            top = (int)positionOnHeightmap.Z / (int)terrainScale;

            // next, we'll use modulus to find out how far away we are from the upper
            // left corner of the cell. Mod will give us a value from 0 to terrainScale,
            // which we then divide by terrainScale to normalize 0 to 1.
            float xNormalized = (positionOnHeightmap.X % terrainScale) / terrainScale;
            float zNormalized = (positionOnHeightmap.Z % terrainScale) / terrainScale;

            // Now that we've calculated the indices of the corners of our cell, and
            // where we are in that cell, we'll use bilinear interpolation to calculuate
            // our height. This process is best explained with a diagram, so please see
            // the accompanying doc for more information.
            // First, calculate the heights on the bottom and top edge of our cell by
            // interpolating from the left and right sides.
            float topHeight = MathHelper.Lerp(
                heights[left, top],
                heights[left + 1, top],
                xNormalized);

            float bottomHeight = MathHelper.Lerp(
                heights[left, top + 1],
                heights[left + 1, top + 1],
                xNormalized);

            // next, interpolate between those two values to calculate the height at our
            // position.
            return MathHelper.Lerp(topHeight, bottomHeight, zNormalized);
        }
         * */
    }
}
