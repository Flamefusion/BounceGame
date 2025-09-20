// Core/Graphics/Buffer.cs
using System;
using System.Runtime.InteropServices;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// OpenGL buffer wrapper (VBO/EBO)
    /// </summary>
    public class Buffer : IDisposable
    {
        private uint _bufferId;
        private uint _target;
        private bool _disposed = false;

        public uint BufferId => _bufferId;
        public uint Target => _target;

        public Buffer(uint target)
        {
            _target = target;
            uint[] buffers = new uint[1];
            GL.glGenBuffers?.Invoke(1, buffers);
            _bufferId = buffers[0];
            GL.CheckError("Generate buffer");
        }

        public void Bind()
        {
            GL.glBindBuffer?.Invoke(_target, _bufferId);
            GL.CheckError("Bind buffer");
        }

        public void Unbind()
        {
            GL.glBindBuffer?.Invoke(_target, 0);
            GL.CheckError("Unbind buffer");
        }

        public void SetData<T>(T[] data, uint usage = GL.GL_STATIC_DRAW) where T : struct
        {
            Bind();
            
            int elementSize = Marshal.SizeOf<T>();
            IntPtr size = new IntPtr(data.Length * elementSize);
            
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr dataPtr = handle.AddrOfPinnedObject();
                GL.glBufferData?.Invoke(_target, size, dataPtr, usage);
                GL.CheckError("Buffer data");
            }
            finally
            {
                handle.Free();
            }
        }

        public void Dispose()
        {
            if (!_disposed && _bufferId != 0)
            {
                uint[] buffers = { _bufferId };
                GL.glDeleteBuffers?.Invoke(1, buffers);
                _bufferId = 0;
                _disposed = true;
            }
        }
    }
}