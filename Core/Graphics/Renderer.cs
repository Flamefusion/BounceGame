// Core/Graphics/Renderer.cs
using System;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Basic 2D renderer for quads/sprites
    /// </summary>
    public class Renderer : IDisposable
    {
        private Shader _defaultShader;
        private VertexArray _quadVAO;
        private Buffer _quadVBO;
        private Buffer _quadEBO;
        private bool _disposed = false;

        // Projection matrix for 2D rendering
        private Matrix4x4 _projectionMatrix;

        public Renderer(int screenWidth, int screenHeight)
        {
            InitializeQuadRendering();
            CreateDefaultShader();
            SetProjection(screenWidth, screenHeight);
            
            Console.WriteLine("Renderer initialized successfully");
        }

        [MemberNotNull(nameof(_quadVAO))]
        [MemberNotNull(nameof(_quadVBO))]
        [MemberNotNull(nameof(_quadEBO))]
        private void InitializeQuadRendering()
        {
            // Create quad vertices (position, color, texCoords)
            var vertices = new Vertex[]
            {
                new Vertex(new Vector2(-0.5f, -0.5f), Vector4.One, new Vector2(0.0f, 0.0f)), // Bottom-left
                new Vertex(new Vector2( 0.5f, -0.5f), Vector4.One, new Vector2(1.0f, 0.0f)), // Bottom-right
                new Vertex(new Vector2( 0.5f,  0.5f), Vector4.One, new Vector2(1.0f, 1.0f)), // Top-right
                new Vertex(new Vector2(-0.5f,  0.5f), Vector4.One, new Vector2(0.0f, 1.0f))  // Top-left
            };

            var indices = new uint[]
            {
                0, 1, 2, // First triangle
                2, 3, 0  // Second triangle
            };

            // Create VAO and buffers
            _quadVAO = new VertexArray();
            _quadVBO = new Buffer(GL.GL_ARRAY_BUFFER);
            _quadEBO = new Buffer(GL.GL_ELEMENT_ARRAY_BUFFER);

            _quadVAO.Bind();

            // Set vertex data
            _quadVBO.SetData(vertices);

            // Set index data
            _quadEBO.SetData(indices);

            // Set up vertex attributes
            int vertexSize = Vertex.SizeInBytes;
            
            // Position attribute (location = 0)
            _quadVAO.SetFloatAttribute(0, 3, vertexSize, 0);
            
            // Color attribute (location = 1)
            _quadVAO.SetFloatAttribute(1, 4, vertexSize, sizeof(float) * 3);
            
            // Texture coordinate attribute (location = 2)
            _quadVAO.SetFloatAttribute(2, 2, vertexSize, sizeof(float) * 7);

            _quadVAO.Unbind();
            GL.CheckError("Initialize quad rendering");
        }

        [MemberNotNull(nameof(_defaultShader))]
        private void CreateDefaultShader()
        {
            string vertexSource = @"
                #version 330 core
                
                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;
                layout (location = 2) in vec2 aTexCoord;
                
                uniform mat4 uProjection;
                uniform mat4 uModel;
                uniform vec4 uTintColor;
                
                out vec4 vColor;
                out vec2 vTexCoord;
                
                void main()
                {
                    gl_Position = uProjection * uModel * vec4(aPosition, 1.0);
                    vColor = aColor * uTintColor;
                    vTexCoord = aTexCoord;
                }";

            string fragmentSource = @"
                #version 330 core
                
                in vec4 vColor;
                in vec2 vTexCoord;
                
                out vec4 FragColor;
                
                void main()
                {
                    FragColor = vColor;
                }";

            _defaultShader = new Shader(vertexSource, fragmentSource);
        }

        public void SetProjection(int width, int height)
        {
            // Create orthographic projection matrix for 2D rendering
            _projectionMatrix = Matrix4x4.CreateOrthographic(width, height, -1.0f, 1.0f);
        }

        /// <summary>
        /// Renders a colored quad at specified position and size
        /// </summary>
        public void DrawQuad(Vector2 position, Vector2 size, Vector4 color)
        {
            // Create model matrix (scale -> translate)
            var scaleMatrix = Matrix4x4.CreateScale(size.X, size.Y, 1.0f);
            var translationMatrix = Matrix4x4.CreateTranslation(position.X, position.Y, 0.0f);
            var modelMatrix = scaleMatrix * translationMatrix;

            _defaultShader.Use();
            _defaultShader.SetUniform("uProjection", _projectionMatrix);
            _defaultShader.SetUniform("uModel", modelMatrix);
            _defaultShader.SetUniform("uTintColor", color);

            _quadVAO.Bind();
            GL.glDrawElements?.Invoke(GL.GL_TRIANGLES, 6, GL.GL_UNSIGNED_INT, IntPtr.Zero);
            GL.CheckError("Draw quad");
            _quadVAO.Unbind();
        }

        /// <summary>
        /// Convenience method for drawing with default white color
        /// </summary>
        public void DrawQuad(Vector2 position, Vector2 size)
        {
            DrawQuad(position, size, Vector4.One);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _defaultShader?.Dispose();
                _quadVAO?.Dispose();
                _quadVBO?.Dispose();
                _quadEBO?.Dispose();
                _disposed = true;
            }
        }
    }
}