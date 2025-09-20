// Core/Graphics/VertexArray.cs
using System;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// OpenGL Vertex Array Object (VAO) wrapper
    /// </summary>
    public class VertexArray : IDisposable
    {
        private uint _vaoId;
        private bool _disposed = false;

        public uint VAOId => _vaoId;

        public VertexArray()
        {
            uint[] vaos = new uint[1];
            GL.glGenVertexArrays?.Invoke(1, vaos);
            _vaoId = vaos[0];
            GL.CheckError("Generate vertex array");
        }

        public void Bind()
        {
            GL.glBindVertexArray?.Invoke(_vaoId);
            GL.CheckError("Bind vertex array");
        }

        public void Unbind()
        {
            GL.glBindVertexArray?.Invoke(0);
            GL.CheckError("Unbind vertex array");
        }

        /// <summary>
        /// Sets up vertex attribute pointer
        /// </summary>
        public void SetVertexAttribute(uint index, int size, uint type, bool normalized, int stride, int offset)
        {
            GL.glVertexAttribPointer?.Invoke(index, size, type, normalized, stride, new IntPtr(offset));
            GL.glEnableVertexAttribArray?.Invoke(index);
            GL.CheckError($"Set vertex attribute {index}");
        }

        /// <summary>
        /// Helper for common float vertex attributes
        /// </summary>
        public void SetFloatAttribute(uint index, int componentCount, int stride, int offset)
        {
            SetVertexAttribute(index, componentCount, GL.GL_FLOAT, false, stride, offset);
        }

        public void Dispose()
        {
            if (!_disposed && _vaoId != 0)
            {
                uint[] vaos = { _vaoId };
                GL.glDeleteVertexArrays?.Invoke(1, vaos);
                _vaoId = 0;
                _disposed = true;
            }
        }
    }
}