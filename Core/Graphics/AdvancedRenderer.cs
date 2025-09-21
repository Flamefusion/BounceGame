// Core/Graphics/AdvancedRenderer.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;
using BounceGame.Core.Resources;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Advanced renderer with texture support and camera system
    /// </summary>
    public class AdvancedRenderer : IDisposable
    {
        private Shader _spriteShader;
        private VertexArray _quadVAO;
        private Buffer _quadVBO;
        private Buffer _quadEBO;
        private Camera _camera;
        private TextureManager _textureManager;
        private bool _disposed = false;

        // Render statistics
        public int DrawCalls { get; private set; }
        public int VerticesRendered { get; private set; }
        public Camera Camera => _camera;

        public AdvancedRenderer(int screenWidth, int screenHeight, TextureManager textureManager)
        {
            _textureManager = textureManager ?? throw new ArgumentNullException(nameof(textureManager));
            _camera = new Camera(screenWidth, screenHeight);
            
            InitializeQuadRendering();
            CreateSpriteShader();
            
            Console.WriteLine($"AdvancedRenderer initialized ({screenWidth}x{screenHeight})");
        }

        [MemberNotNull(nameof(_quadVAO))]
        [MemberNotNull(nameof(_quadVBO))]
        [MemberNotNull(nameof(_quadEBO))]
        private void InitializeQuadRendering()
        {
            // Create textured quad vertices
            var vertices = new TexturedVertex[]
            {
                new TexturedVertex(new Vector2(-0.5f, -0.5f), Vector4.One, new Vector2(0.0f, 0.0f)),
                new TexturedVertex(new Vector2( 0.5f, -0.5f), Vector4.One, new Vector2(1.0f, 0.0f)),
                new TexturedVertex(new Vector2( 0.5f,  0.5f), Vector4.One, new Vector2(1.0f, 1.0f)),
                new TexturedVertex(new Vector2(-0.5f,  0.5f), Vector4.One, new Vector2(0.0f, 1.0f))
            };

            var indices = new uint[] { 0, 1, 2, 2, 3, 0 };

            _quadVAO = new VertexArray();
            _quadVBO = new Buffer(GL.GL_ARRAY_BUFFER);
            _quadEBO = new Buffer(GL.GL_ELEMENT_ARRAY_BUFFER);

            _quadVAO.Bind();
            _quadVBO.SetData(vertices);
            _quadEBO.SetData(indices);

            int vertexSize = TexturedVertex.SizeInBytes;
            _quadVAO.SetFloatAttribute(0, 2, vertexSize, 0);                    // Position
            _quadVAO.SetFloatAttribute(1, 4, vertexSize, sizeof(float) * 2);    // Color
            _quadVAO.SetFloatAttribute(2, 2, vertexSize, sizeof(float) * 6);    // TexCoords

            _quadVAO.Unbind();
        }

        [MemberNotNull(nameof(_spriteShader))]
        private void CreateSpriteShader()
        {
            string vertexSource = @"
                #version 330 core
                
                layout (location = 0) in vec2 aPosition;
                layout (location = 1) in vec4 aColor;
                layout (location = 2) in vec2 aTexCoord;
                
                uniform mat4 uViewProjection;
                uniform mat4 uModel;
                uniform vec4 uTintColor;
                uniform vec4 uTexCoordTransform; // (offsetU, offsetV, scaleU, scaleV)
                
                out vec4 vColor;
                out vec2 vTexCoord;
                
                void main()
                {
                    gl_Position = uViewProjection * uModel * vec4(aPosition, 0.0, 1.0);
                    vColor = aColor * uTintColor;
                    
                    // Apply texture coordinate transformation for sprite sheets
                    vTexCoord = aTexCoord * uTexCoordTransform.zw + uTexCoordTransform.xy;
                }";

            string fragmentSource = @"
                #version 330 core
                
                in vec4 vColor;
                in vec2 vTexCoord;
                
                uniform sampler2D uTexture;
                uniform bool uUseTexture;
                
                out vec4 FragColor;
                
                void main()
                {
                    if (uUseTexture) {
                        vec4 texColor = texture(uTexture, vTexCoord);
                        FragColor = texColor * vColor;
                    } else {
                        FragColor = vColor;
                    }
                }";

            _spriteShader = new Shader(vertexSource, fragmentSource);
        }

        /// <summary>
        /// Begin rendering frame
        /// </summary>
        public void BeginFrame()
        {
            DrawCalls = 0;
            VerticesRendered = 0;
        }

        /// <summary>
        /// Draw a colored quad (no texture)
        /// </summary>
        public void DrawQuad(Vector2 position, Vector2 size, Vector4 color, float rotation = 0.0f)
        {
            DrawTexturedQuad(position, size, color, null, Vector4.Zero, rotation);
        }

        /// <summary>
        /// Draw a textured quad
        /// </summary>
        public void DrawTexturedQuad(Vector2 position, Vector2 size, Vector4 tint, 
            Texture? texture = null, Vector4 texCoordTransform = default, float rotation = 0.0f)
        {
            // Skip if not visible by camera
            if (!_camera.IsRectVisible(position, size))
                return;

            DrawCalls++;
            VerticesRendered += 4;

            // Create model matrix
            var scaleMatrix = Matrix4x4.CreateScale(size.X, size.Y, 1.0f);
            var rotationMatrix = Matrix4x4.CreateRotationZ(rotation);
            var translationMatrix = Matrix4x4.CreateTranslation(position.X, position.Y, 0.0f);
            var modelMatrix = scaleMatrix * rotationMatrix * translationMatrix;

            // Set up shader
            _spriteShader.Use();
            _spriteShader.SetUniform("uViewProjection", _camera.ViewProjectionMatrix);
            _spriteShader.SetUniform("uModel", modelMatrix);
            _spriteShader.SetUniform("uTintColor", tint);

            // Handle texture
            if (texture != null && texture.IsLoaded)
            {
                _spriteShader.SetUniform("uUseTexture", 1);
                texture.Bind(0);
                _spriteShader.SetUniform("uTexture", 0);
                
                // Set texture coordinate transform (for sprite sheets)
                if (texCoordTransform == default)
                    texCoordTransform = new Vector4(0, 0, 1, 1); // No transform
                _spriteShader.SetUniform("uTexCoordTransform", texCoordTransform);
            }
            else
            {
                _spriteShader.SetUniform("uUseTexture", 0);
                // Bind white texture to avoid issues
                _textureManager.GetTexture("white")?.Bind(0);
            }

            // Render quad
            _quadVAO.Bind();
            GL.glDrawElements?.Invoke(GL.GL_TRIANGLES, 6, GL.GL_UNSIGNED_INT, IntPtr.Zero);
            GL.CheckError("Draw textured quad");
            _quadVAO.Unbind();

            // Unbind texture
            if (texture != null)
                texture.Unbind();
        }

        /// <summary>
        /// Draw a sprite from a sprite sheet
        /// </summary>
        public void DrawSprite(Vector2 position, Vector2 size, Vector4 tint, 
            SpriteSheet spriteSheet, string frameName, float rotation = 0.0f)
        {
            var frame = spriteSheet.GetFrame(frameName);
            if (frame == null)
            {
                // Fall back to colored quad if frame not found
                DrawQuad(position, size, tint, rotation);
                return;
            }

            // Convert frame UV to texture coordinate transform
            var texTransform = new Vector4(
                frame.Value.UV.X, // offset U
                frame.Value.UV.Y, // offset V
                frame.Value.UV.Z - frame.Value.UV.X, // scale U
                frame.Value.UV.W - frame.Value.UV.Y  // scale V
            );

            DrawTexturedQuad(position, size, tint, spriteSheet.Texture, texTransform, rotation);
        }

        /// <summary>
        /// Draw a sprite from a sprite sheet by frame index
        /// </summary>
        public void DrawSprite(Vector2 position, Vector2 size, Vector4 tint, 
            SpriteSheet spriteSheet, int frameIndex, float rotation = 0.0f)
        {
            var frame = spriteSheet.GetFrame(frameIndex);
            if (frame == null)
            {
                DrawQuad(position, size, tint, rotation);
                return;
            }

            var texTransform = new Vector4(
                frame.Value.UV.X,
                frame.Value.UV.Y,
                frame.Value.UV.Z - frame.Value.UV.X,
                frame.Value.UV.W - frame.Value.UV.Y
            );

            DrawTexturedQuad(position, size, tint, spriteSheet.Texture, texTransform, rotation);
        }

        /// <summary>
        /// Set viewport size (call when window is resized)
        /// </summary>
        public void SetViewport(int width, int height)
        {
            _camera.SetViewport(width, height);
            GL.glViewport?.Invoke(0, 0, width, height);
            GL.CheckError("Set viewport");
        }

        /// <summary>
        /// Get rendering statistics
        /// </summary>
        public string GetStats()
        {
            var (viewMin, viewMax) = _camera.GetViewBounds();
            return $"AdvancedRenderer: {DrawCalls} draws, {VerticesRendered} verts, " +
                   $"Cam: ({_camera.Position.X:F1}, {_camera.Position.Y:F1}), " +
                   $"Zoom: {_camera.Zoom:F2}, View: ({viewMin.X:F0},{viewMin.Y:F0}) to ({viewMax.X:F0},{viewMax.Y:F0})";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _spriteShader?.Dispose();
                _quadVAO?.Dispose();
                _quadVBO?.Dispose();
                _quadEBO?.Dispose();
                _disposed = true;
                Console.WriteLine("AdvancedRenderer disposed");
            }
        }
    }

    /// <summary>
    /// Vertex structure for textured rendering
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct TexturedVertex
    {
        public Vector2 Position;
        public Vector4 Color;
        public Vector2 TexCoords;

        public TexturedVertex(Vector2 position, Vector4 color, Vector2 texCoords)
        {
            Position = position;
            Color = color;
            TexCoords = texCoords;
        }

        public static int SizeInBytes => System.Runtime.InteropServices.Marshal.SizeOf<TexturedVertex>();
    }
}
