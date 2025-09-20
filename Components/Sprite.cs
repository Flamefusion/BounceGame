// Components/Sprite.cs
using System.Numerics;
using BounceGame.Core.ECS;

namespace BounceGame.Components
{
    /// <summary>
    /// Sprite component for rendering visual representation
    /// </summary>
    public class Sprite : IComponent
    {
        public Vector4 Color { get; set; }
        public Vector2 Size { get; set; }
        public int Layer { get; set; }
        public bool Visible { get; set; }
        
        // Future texture support
        public uint TextureId { get; set; }
        public Vector2 TextureOffset { get; set; }
        public Vector2 TextureScale { get; set; }

        public Sprite(Vector2 size, Vector4? color = null, int layer = 0)
        {
            Size = size;
            Color = color ?? Vector4.One; // Default white
            Layer = layer;
            Visible = true;
            TextureId = 0; // 0 means no texture
            TextureOffset = Vector2.Zero;
            TextureScale = Vector2.One;
        }

        public Sprite(float width, float height, Vector4? color = null, int layer = 0)
            : this(new Vector2(width, height), color, layer)
        {
        }

        /// <summary>
        /// Create a colored sprite
        /// </summary>
        public static Sprite CreateColored(Vector2 size, Vector4 color, int layer = 0)
        {
            return new Sprite(size, color, layer);
        }

        /// <summary>
        /// Create a white sprite (for future texture rendering)
        /// </summary>
        public static Sprite CreateTextured(Vector2 size, uint textureId, int layer = 0)
        {
            return new Sprite(size, Vector4.One, layer) { TextureId = textureId };
        }

        public override string ToString()
        {
            return $"Sprite(Size: {Size}, Color: {Color}, Layer: {Layer}, Visible: {Visible})";
        }
    }
}