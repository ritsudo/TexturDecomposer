using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexturDecomposer
{
    public static class Trapezoid
    {
        //TODO FIX MATH

        public static void drawVertexPositionTexture(Plane controlPlane, BasicEffect basicEffect, BasicEffect worldEffect, GraphicsDevice graphicsDevice)
        {
            VertexPositionTexture[] textures = getVertexPositionTexture(controlPlane, worldEffect, graphicsDevice);
            // Define the triangle indices (two triangles make up the trapezoid)
            short[] indices = new short[] {
                0, 1, 2,    // First triangle (top-left, top-right, bottom-left)
                1, 3, 2     // Second triangle (top-right, bottom-right, bottom-left)
            };


            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(
                new Vector3(0, 0, 2),
                Vector3.Zero,
                Vector3.Up
                );
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0,
                graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height,
                0,
                1.0f,
                1000.0f);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;  // Disable back-face culling (we want to draw both triangles)
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                
                // Draw using the vertices and indices (2 triangles forming the trapezoid)
                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    textures, 0, 4, // We have 4 vertices
                    indices, 0, 2   // We have 2 triangles
                );
            }

        }

        private static VertexPositionTexture[] getVertexPositionTexture(Plane controlPlane, BasicEffect cubeBasicEffect, GraphicsDevice graphicsDevice)
        {
            // Define the 4 corner points of the trapezoid (screen space)
            Vector3 topLeft = new Vector3(0, 0, 0);    // Screen-space coordinates
            Vector3 topRight = new Vector3(controlPlane.TextureResolutionX, 0, 0);   // Top-right point of the trapezoid
            Vector3 bottomLeft = new Vector3(0, controlPlane.TextureResolutionY, 0); // Bottom-left point
            Vector3 bottomRight = new Vector3(controlPlane.TextureResolutionX, controlPlane.TextureResolutionY, 0);// Bottom-right point

            // Define the texture coordinates corresponding to each corner
            Vector2 topLeftTexCoord = ProjectToScreen(controlPlane._vertices[0].Position, cubeBasicEffect.World, cubeBasicEffect.View, cubeBasicEffect.Projection, graphicsDevice.Viewport);   // Corresponds to the top-left corner of the texture
            Vector2 topRightTexCoord = ProjectToScreen(controlPlane._vertices[1].Position, cubeBasicEffect.World, cubeBasicEffect.View, cubeBasicEffect.Projection, graphicsDevice.Viewport);  // Top-right corner of the texture
            Vector2 bottomLeftTexCoord = ProjectToScreen(controlPlane._vertices[2].Position, cubeBasicEffect.World, cubeBasicEffect.View, cubeBasicEffect.Projection, graphicsDevice.Viewport);// Bottom-left of the texture
            Vector2 bottomRightTexCoord = ProjectToScreen(controlPlane._vertices[3].Position, cubeBasicEffect.World, cubeBasicEffect.View, cubeBasicEffect.Projection, graphicsDevice.Viewport);// Bottom-right of the texture

            return new VertexPositionTexture[] {
                new VertexPositionTexture(topLeft, topLeftTexCoord),
                new VertexPositionTexture(topRight, topRightTexCoord),
                new VertexPositionTexture(bottomLeft, bottomLeftTexCoord),
                new VertexPositionTexture(bottomRight, bottomRightTexCoord)
            };
        }

        public static Vector2 ProjectToScreen(Vector3 vertexPosition, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            // Transform the vertex position using the world, view, and projection matrices
            Vector4 transformedVertex = Vector4.Transform(vertexPosition, world * view * projection);

            // Perform perspective division to get normalized device coordinates (NDC)
            if (transformedVertex.W != 0)
            {
                transformedVertex /= transformedVertex.W;
            }

            // Convert NDC (-1 to 1 range) to screen space (pixel coordinates)
            float screenX = (transformedVertex.X + 1f) / 2f;
            float screenY = (1f - transformedVertex.Y) / 2f;  // Invert Y-axis for screen coordinates

            if (screenX < 0.0f)
            {
                screenX = 0.0f;
            }

            if (screenY < 0.0f)
            {
                screenY = 0.0f;
            }

            if (screenX >= 1.0f)
            {
                screenX = 1.0f;
            }

            if (screenY >= 1.0f)
            {
                screenY = 1.0f;
            }

            // Return the screen coordinates as a Vector2 (X, Y)
            return new Vector2(screenX, screenY);
        }

        public static Texture2D DrawPrimitivesToRenderTarget(GraphicsDevice graphicsDevice, Plane controlPlane, RenderTarget2D renderTarget, BasicEffect basicEffect, BasicEffect worldEffect)
        {
            
                // Set the render target to the current one (renderTargets[i])
                graphicsDevice.SetRenderTarget(renderTarget);
                graphicsDevice.Clear(Color.Transparent); // Clear the render target before drawing

                // Render the primitives. You can customize this based on the render target.
                // For example, each render target could have a different transformation or primitive.
                drawVertexPositionTexture(controlPlane, basicEffect, worldEffect, graphicsDevice);

                // Reset to drawing on the back buffer (the screen)
                graphicsDevice.SetRenderTarget(null);

                // Store the rendered texture for later use (to display or analyze)
                return renderTarget;
        }
    }
}
