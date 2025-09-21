// Core/Graphics/SpriteSheet.cs
using System;
using System.Collections.Generic;
using System.Numerics;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Represents a sprite sheet with multiple frames/sprites
    /// </summary>
    public class SpriteSheet : IDisposable
    {
        private readonly Texture _texture;
        private readonly Dictionary<string, SpriteFrame> _frames;
        private bool _disposed = false;

        public Texture Texture => _texture;
        public int FrameCount => _frames.Count;

        public SpriteSheet(Texture texture)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
            _frames = new Dictionary<string, SpriteFrame>();
        }

        /// <summary>
        /// Add a frame to the sprite sheet
        /// </summary>
        public void AddFrame(string name, int x, int y, int width, int height)
        {
            var frame = new SpriteFrame
            {
                Name = name,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                // Calculate UV coordinates (0-1 range)
                UV = new Vector4(
                    x / (float)_texture.Width,        // U min
                    y / (float)_texture.Height,       // V min
                    (x + width) / (float)_texture.Width,   // U max
                    (y + height) / (float)_texture.Height  // V max
                )
            };

            _frames[name] = frame;
            Console.WriteLine($"Added frame '{name}': ({x}, {y}, {width}, {height})");
        }

        /// <summary>
        /// Add frames in a grid pattern
        /// </summary>
        public void AddGridFrames(int frameWidth, int frameHeight, int startX = 0, int startY = 0)
        {
            int frameIndex = 0;
            
            for (int y = startY; y < _texture.Height - frameHeight; y += frameHeight)
            {
                for (int x = startX; x < _texture.Width - frameWidth; x += frameWidth)
                {
                    AddFrame($"frame_{frameIndex}", x, y, frameWidth, frameHeight);
                    frameIndex++;
                }
            }
            
            Console.WriteLine($"Added {frameIndex} grid frames ({frameWidth}x{frameHeight})");
        }

        /// <summary>
        /// Get a frame by name
        /// </summary>
        public SpriteFrame? GetFrame(string name)
        {
            return _frames.TryGetValue(name, out var frame) ? frame : null;
        }

        /// <summary>
        /// Get frame by index
        /// </summary>
        public SpriteFrame? GetFrame(int index)
        {
            if (index < 0 || index >= _frames.Count)
                return null;

            var frameArray = new SpriteFrame[_frames.Count];
            _frames.Values.CopyTo(frameArray, 0);
            return frameArray[index];
        }

        /// <summary>
        /// Get all frame names
        /// </summary>
        public IEnumerable<string> GetFrameNames()
        {
            return _frames.Keys;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Note: We don't dispose the texture as it might be shared
                _frames.Clear();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a single frame within a sprite sheet
    /// </summary>
    public struct SpriteFrame
    {
        public string Name;
        public int X, Y;           // Position in sprite sheet
        public int Width, Height;  // Size in pixels
        public Vector4 UV;         // UV coordinates (min_u, min_v, max_u, max_v)

        public override string ToString()
        {
            return $"SpriteFrame('{Name}': {X},{Y} {Width}x{Height})";
        }
    }
}
