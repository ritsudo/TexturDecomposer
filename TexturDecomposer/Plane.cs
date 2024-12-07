using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TexturDecomposer
{
    public class Plane
    {
        public Vector3 CenterPosition;
        public VertexPositionColor[] _vertices;
        private Color[] colors = {Color.Red, Color.Green, Color.Blue, Color.Yellow};
        private float PlaneSize = 1.0f;

        public int TextureResolutionX = 256;
        public int TextureResolutionY = 256;

        public Plane() {
            ResetPosition();
        }

        public void Move(Vector3 direction, float value)
        {

            Vector3 NewCenterPosition = new Vector3(
                CenterPosition.X + direction.X * value,
                CenterPosition.Y + direction.Y * value,
                CenterPosition.Z + direction.Z * value
            );

            CenterPosition = NewCenterPosition;

            for (int i = 0; i < _vertices.Length; i += 1)
            {
                _vertices[i].Position.X += direction.X * value;
                _vertices[i].Position.Y += direction.Y * value;
                _vertices[i].Position.Z += direction.Z * value;
            }

        }

        public void Rotate(Vector3 axis, float angle)
        {
                // Normalize the rotation axis
                axis = Vector3.Normalize(axis);

                // Create the rotation matrix using MonoGame
                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(axis, angle);

                Vector3 center = CenterPosition; // Assuming 'Position' stores the center of the plane

                // Apply the rotation to each vertex
                for (int i = 0; i < _vertices.Length; i++)
                {
                    // Original vertex position
                    Vector3 originalPosition = new Vector3(
                        _vertices[i].Position.X,
                        _vertices[i].Position.Y,
                        _vertices[i].Position.Z
                    );

                    // Translate vertex to local space (centered at origin)
                    Vector3 localPosition = originalPosition - center;

                    // Transform the vertex position using the rotation matrix
                    Vector3 rotatedPosition = Vector3.Transform(localPosition, rotationMatrix);

                    // Translate vertex back to world space
                    Vector3 worldPosition = rotatedPosition + center;

                    // Update the vertex position
                    _vertices[i].Position.X = worldPosition.X;
                    _vertices[i].Position.Y = worldPosition.Y;
                    _vertices[i].Position.Z = worldPosition.Z;
                }
            
        }

        public void Orient()
        {
            _vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(CenterPosition.X-PlaneSize, CenterPosition.Y+PlaneSize, CenterPosition.Z), colors[0]),
                new VertexPositionColor(new Vector3(CenterPosition.X+PlaneSize, CenterPosition.Y+PlaneSize, CenterPosition.Z), colors[1]),
                new VertexPositionColor(new Vector3(CenterPosition.X-PlaneSize, CenterPosition.Y-PlaneSize, CenterPosition.Z), colors[2]),
                new VertexPositionColor(new Vector3(CenterPosition.X+PlaneSize, CenterPosition.Y-PlaneSize, CenterPosition.Z), colors[3]),
            };
        }

        public void OrientAndRotate(Vector3 axis)
        {
            Orient();
            Rotate(axis, 1.5708f);
        }

        public void ResetPosition()
        {
            CenterPosition = new Vector3(0, 0, 0);

            _vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(-PlaneSize, PlaneSize, 0), colors[0]),
                new VertexPositionColor(new Vector3(PlaneSize, PlaneSize, 0), colors[1]),
                new VertexPositionColor(new Vector3(-PlaneSize, -PlaneSize, 0), colors[2]),
                new VertexPositionColor(new Vector3(PlaneSize, -PlaneSize, 0), colors[3]),
            };
        }

        public readonly short[] _indices =
        {
                0, 1, // Top edge
                1, 3, // Right edge
                3, 2, // Bottom edge
                2, 0 // Left edge
        };

        public void Draw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                _vertices,
                0,
                _vertices.Length,
                _indices,
                0,
                _indices.Length / 2
            );
        }
    }
}
