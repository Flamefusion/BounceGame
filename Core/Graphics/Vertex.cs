// Core/Graphics/Vertex.cs
using System.Numerics;
using System.Runtime.InteropServices;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Standard vertex structure for 2D rendering
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector4 Color;
        public Vector2 TexCoords;

        public Vertex(Vector3 position, Vector4 color, Vector2 texCoords)
        {
            Position = position;
            Color = color;
            TexCoords = texCoords;
        }

        public Vertex(Vector2 position, Vector4 color, Vector2 texCoords)
        {
            Position = new Vector3(position, 0.0f);
            Color = color;
            TexCoords = texCoords;
        }

        public static int SizeInBytes => Marshal.SizeOf<Vertex>();
    }
}
