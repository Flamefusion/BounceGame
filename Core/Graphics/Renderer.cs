// Core/Graphics/Renderer.cs - Debug Version with Logging
using System;
using System.Numerics;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Debug version of Renderer with detailed logging
    /// </summary>
    public class DebugRenderer : IDisposable
    {
        private Shader _defaultShader;
        private VertexArray _quadVAO;
        private Buffer _quadVBO;
        private Buffer _quadEBO;
        private bool _disposed = false;
        private Matrix4x4 _projectionMatrix;
        
        public int DrawCallCount { get; private set; }

        public DebugRenderer(int screenWidth, int screenHeight)
        {
            Console.WriteLine($"Initializing DebugRenderer ({screenWidth}x{screenHeight})...");
            
            InitializeQuadRendering();
            CreateDefaultShader();
            SetProjection(screenWidth, screenHeight);
            
            Console.WriteLine("DebugRenderer initialized successfully");
            Console.WriteLine($"Projection matrix created for screen size {screenWidth}x{screenHeight}");
        }

        private void InitializeQuadRendering()
        {
            Console.WriteLine("Setting up quad rendering...");
            
            // Same quad setup as before but with logging
            var vertices = new Vertex[]
            {
                new Vertex(new Vector2(-0.5f, -0.5f), Vector4.One, new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector2( 0.5f, -0.5f), Vector4.One, new Vector2(1.0f, 0.0f)),
                new Vertex(new Vector2( 0.5f,  0.5f), Vector4.One, new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector2(-0.5f,  0.5f), Vector4.One, new Vector2(0.0f, 1.0f))
            };

            var indices = new uint[] { 0, 1, 2, 2, 3, 0 };

            _quadVAO = new VertexArray();
            _quadVBO = new Buffer(GL.GL_ARRAY_BUFFER);
            _quadEBO = new Buffer(GL.GL_ELEMENT_ARRAY_BUFFER);

            _quadVAO.Bind();
            _quadVBO.SetData(vertices);
            _quadEBO.SetData(indices);

            int vertexSize = Vertex.SizeInBytes;
            _quadVAO.SetFloatAttribute(0, 3, vertexSize, 0);
            _quadVAO.SetFloatAttribute(1, 4, vertexSize, sizeof(float) * 3);
            _quadVAO.SetFloatAttribute(2, 2, vertexSize, sizeof(float) * 7);

            _quadVAO.Unbind();
            Console.WriteLine("Quad rendering setup complete");
        }

        private void CreateDefaultShader()
        {
            Console.WriteLine("Creating default shader...");
            
            // Same shaders as before
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
            Console.WriteLine("Default shader created successfully");
        }

        public void SetProjection(int width, int height)
        {
            // Create orthographic projection: left, right, bottom, top, near, far
            float left = -width / 2.0f;
            float right = width / 2.0f;
            float bottom = -height / 2.0f;
            float top = height / 2.0f;
            
            _projectionMatrix = Matrix4x4.CreateOrthographic(width, height, -1.0f, 1.0f);
            
            Console.WriteLine($"Projection set: viewport ({left}, {right}, {bottom}, {top})");
        }

        public void DrawQuad(Vector2 position, Vector2 size, Vector4 color)
        {
            DrawCallCount++;
            
            // Log first few draw calls for debugging
            if (DrawCallCount <= 5)
            {
                Console.WriteLine($"DrawQuad #{DrawCallCount}: pos=({position.X:F1}, {position.Y:F1}), " +
                                $"size=({size.X:F1}, {size.Y:F1}), " +
                                $"color=({color.X:F2}, {color.Y:F2}, {color.Z:F2}, {color.W:F2})");
            }

            // Create model matrix
            var scaleMatrix = Matrix4x4.CreateScale(size.X, size.Y, 1.0f);
            var translationMatrix = Matrix4x4.CreateTranslation(position.X, position.Y, 0.0f);
            var modelMatrix = scaleMatrix * translationMatrix;

            _defaultShader.Use();
            _defaultShader.SetUniform("uProjection", _projectionMatrix);
            _defaultShader.SetUniform("uModel", modelMatrix);
            _defaultShader.SetUniform("uTintColor", color);

            _quadVAO.Bind();
            GL.glDrawElements(GL.GL_TRIANGLES, 6, GL.GL_UNSIGNED_INT, IntPtr.Zero);
            GL.CheckError("Draw quad");
            _quadVAO.Unbind();
        }

        public void ResetDrawCallCount()
        {
            DrawCallCount = 0;
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
                Console.WriteLine("DebugRenderer disposed");
            }
        }
    }
}