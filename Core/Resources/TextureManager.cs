// Core/Resources/TextureManager.cs
using System;
using System.Collections.Generic;
using BounceGame.Core.Graphics;

namespace BounceGame.Core.Resources
{
    /// <summary>
    /// Manages texture loading and caching
    /// </summary>
    public class TextureManager : IDisposable
    {
        private readonly Dictionary<string, Texture> _textures;
        private readonly Dictionary<string, SpriteSheet> _spriteSheets;
        private bool _disposed = false;

        public TextureManager()
        {
            _textures = new Dictionary<string, Texture>();
            _spriteSheets = new Dictionary<string, SpriteSheet>();
            
            // Create default textures
            CreateDefaultTextures();
        }

        private void CreateDefaultTextures()
        {
            // White 1x1 texture for solid colors
            _textures["white"] = Texture.CreateSolid(System.Numerics.Vector4.One, 1, 1);
            
            // Checkerboard for testing
            _textures["checkerboard"] = Texture.CreateCheckerboard(64);
            
            // Some basic colored textures
            _textures["red"] = Texture.CreateSolid(new System.Numerics.Vector4(1, 0, 0, 1), 32, 32);
            _textures["green"] = Texture.CreateSolid(new System.Numerics.Vector4(0, 1, 0, 1), 32, 32);
            _textures["blue"] = Texture.CreateSolid(new System.Numerics.Vector4(0, 0, 1, 1), 32, 32);
            
            Console.WriteLine("Default textures created");
        }

        /// <summary>
        /// Load texture from file
        /// </summary>
        public Texture LoadTexture(string name, string filePath)
        {
            if (_textures.ContainsKey(name))
            {
                Console.WriteLine($"Texture '{name}' already loaded");
                return _textures[name];
            }

            try
            {
                var texture = new Texture(filePath);
                _textures[name] = texture;
                Console.WriteLine($"Loaded texture '{name}' from {filePath}");
                return texture;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load texture '{name}': {ex.Message}");
                // Return white texture as fallback
                return GetTexture("white");
            }
        }

        /// <summary>
        /// Create texture from pixel data
        /// </summary>
        public Texture CreateTexture(string name, byte[] pixelData, int width, int height)
        {
            if (_textures.ContainsKey(name))
            {
                _textures[name].Dispose();
            }

            var texture = new Texture(pixelData, width, height);
            _textures[name] = texture;
            return texture;
        }

        /// <summary>
        /// Get existing texture
        /// </summary>
        public Texture GetTexture(string name)
        {
            if (_textures.TryGetValue(name, out var texture))
            {
                return texture;
            }

            Console.WriteLine($"Texture '{name}' not found, returning white texture");
            return _textures["white"];
        }

        /// <summary>
        /// Create sprite sheet from existing texture
        /// </summary>
        public SpriteSheet CreateSpriteSheet(string name, string textureName)
        {
            var texture = GetTexture(textureName);
            var spriteSheet = new SpriteSheet(texture);
            _spriteSheets[name] = spriteSheet;
            Console.WriteLine($"Created sprite sheet '{name}' from texture '{textureName}'");
            return spriteSheet;
        }

        /// <summary>
        /// Get existing sprite sheet
        /// </summary>
        public SpriteSheet? GetSpriteSheet(string name)
        {
            return _spriteSheets.TryGetValue(name, out var spriteSheet) ? spriteSheet : null;
        }

        /// <summary>
        /// Check if texture exists
        /// </summary>
        public bool HasTexture(string name)
        {
            return _textures.ContainsKey(name);
        }

        /// <summary>
        /// Get all loaded texture names
        /// </summary>
        public IEnumerable<string> GetTextureNames()
        {
            return _textures.Keys;
        }

        /// <summary>
        /// Get memory usage statistics
        /// </summary>
        public string GetStats()
        {
            int totalPixels = 0;
            foreach (var texture in _textures.Values)
            {
                totalPixels += texture.Width * texture.Height;
            }

            int estimatedMemoryMB = (totalPixels * 4) / (1024 * 1024); // RGBA = 4 bytes per pixel

            return $"TextureManager: {_textures.Count} textures, {_spriteSheets.Count} sprite sheets, ~{estimatedMemoryMB} MB";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var texture in _textures.Values)
                {
                    texture.Dispose();
                }
                _textures.Clear();

                foreach (var spriteSheet in _spriteSheets.Values)
                {
                    spriteSheet.Dispose();
                }
                _spriteSheets.Clear();

                _disposed = true;
                Console.WriteLine("TextureManager disposed");
            }
        }
    }
}
