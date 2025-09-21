// Core/Graphics/Texture.cs
using System;
using System.IO;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// OpenGL texture wrapper with loading capabilities
    /// </summary>
    public class Texture : IDisposable
    {
        private uint _textureId;
        private int _width;
        private int _height;
        private bool _disposed = false;

        public uint TextureId => _textureId;
        public int Width => _width;
        public int Height => _height;
        public bool IsLoaded => _textureId != 0;

        /// <summary>
        /// Create texture from raw pixel data (RGBA format)
        /// </summary>
        public Texture(byte[] pixelData, int width, int height)
        {
            LoadFromPixelData(pixelData, width, height);
        }

        /// <summary>
        /// Create texture from file (basic BMP/TGA support)
        /// </summary>
        public Texture(string filePath)
        {
            LoadFromFile(filePath);
        }

        /// <summary>
        /// Create solid color texture
        /// </summary>
        public static Texture CreateSolid(System.Numerics.Vector4 color, int width = 1, int height = 1)
        {
            var pixelData = new byte[width * height * 4];
            
            byte r = (byte)(color.X * 255);
            byte g = (byte)(color.Y * 255);
            byte b = (byte)(color.Z * 255);
            byte a = (byte)(color.W * 255);
            
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = r;
                pixelData[i + 1] = g;
                pixelData[i + 2] = b;
                pixelData[i + 3] = a;
            }
            
            return new Texture(pixelData, width, height);
        }

        /// <summary>
        /// Create checkerboard pattern texture for testing
        /// </summary>
        public static Texture CreateCheckerboard(int size = 64)
        {
            var pixelData = new byte[size * size * 4];
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = (y * size + x) * 4;
                    bool isWhite = (x / 8 + y / 8) % 2 == 0;
                    
                    byte value = (byte)(isWhite ? 255 : 64);
                    pixelData[index] = value;     // R
                    pixelData[index + 1] = value; // G
                    pixelData[index + 2] = value; // B
                    pixelData[index + 3] = 255;   // A
                }
            }
            
            return new Texture(pixelData, size, size);
        }

        private void LoadFromPixelData(byte[] pixelData, int width, int height)
        {
            if (pixelData == null || pixelData.Length != width * height * 4)
                throw new ArgumentException("Invalid pixel data");

            _width = width;
            _height = height;

            uint[] textures = new uint[1];
            GL.glGenTextures?.Invoke(1, textures);
            _textureId = textures[0];
            GL.CheckError("Generate texture");

            Bind();

            // Set texture parameters
            GL.glTexParameteri?.Invoke(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_NEAREST);
            GL.glTexParameteri?.Invoke(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_NEAREST);
            GL.glTexParameteri?.Invoke(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int)GL.GL_CLAMP_TO_EDGE);
            GL.glTexParameteri?.Invoke(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int)GL.GL_CLAMP_TO_EDGE);
            GL.CheckError("Set texture parameters");

            // Upload pixel data
            var handle = System.Runtime.InteropServices.GCHandle.Alloc(pixelData, System.Runtime.InteropServices.GCHandleType.Pinned);
            try
            {
                var dataPtr = handle.AddrOfPinnedObject();
                GL.glTexImage2D?.Invoke(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGBA, width, height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, dataPtr);
                GL.CheckError("Upload texture data");
            }
            finally
            {
                handle.Free();
            }

            Unbind();
            Console.WriteLine($"Texture loaded: {width}x{height}, ID: {_textureId}");
        }

        private void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Texture file not found: {filePath}");

            // For now, create a placeholder colored texture based on filename
            // In a real implementation, you'd use a library like StbImage or System.Drawing
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var color = GetColorFromFileName(fileName);
            
            Console.WriteLine($"Loading texture from file: {filePath} (using placeholder color)");
            var pixelData = new byte[64 * 64 * 4];
            
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = (byte)(color.X * 255);     // R
                pixelData[i + 1] = (byte)(color.Y * 255); // G
                pixelData[i + 2] = (byte)(color.Z * 255); // B
                pixelData[i + 3] = (byte)(color.W * 255); // A
            }
            
            LoadFromPixelData(pixelData, 64, 64);
        }

        private System.Numerics.Vector4 GetColorFromFileName(string fileName)
        {
            // Simple color mapping based on filename for testing
            return fileName.ToLower() switch
            {
                "ball" or "player" => new System.Numerics.Vector4(1.0f, 0.3f, 0.3f, 1.0f), // Red
                "platform" or "ground" => new System.Numerics.Vector4(0.3f, 0.7f, 0.3f, 1.0f), // Green
                "background" or "sky" => new System.Numerics.Vector4(0.3f, 0.3f, 1.0f, 1.0f), // Blue
                "coin" or "collectible" => new System.Numerics.Vector4(1.0f, 1.0f, 0.3f, 1.0f), // Yellow
                _ => new System.Numerics.Vector4(0.8f, 0.8f, 0.8f, 1.0f) // Gray default
            };
        }

        public void Bind(uint textureUnit = 0)
        {
            if (textureUnit > 0)
            {
                // For multiple texture units (advanced)
                GL.glActiveTexture?.Invoke(GL.GL_TEXTURE0 + textureUnit); // GL_TEXTURE0 + unit
            }
            GL.glBindTexture?.Invoke(GL.GL_TEXTURE_2D, _textureId);
            GL.CheckError("Bind texture");
        }

        public void Unbind()
        {
            GL.glBindTexture?.Invoke(GL.GL_TEXTURE_2D, 0);
            GL.CheckError("Unbind texture");
        }

        public void Dispose()
        {
            if (!_disposed && _textureId != 0)
            {
                uint[] textures = { _textureId };
                GL.glDeleteTextures?.Invoke(1, textures);
                _textureId = 0;
                _disposed = true;
            }
        }
    }
}
