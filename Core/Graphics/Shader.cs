// Core/Graphics/Shader.cs
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// OpenGL shader wrapper - handles compilation and uniform management
    /// </summary>
    public class Shader : IDisposable
    {
        private uint _programId;
        private bool _disposed = false;

        public uint ProgramId => _programId;

        public Shader(string vertexSource, string fragmentSource)
        {
            uint vertexShader = CompileShader(GL.GL_VERTEX_SHADER, vertexSource);
            uint fragmentShader = CompileShader(GL.GL_FRAGMENT_SHADER, fragmentSource);

            _programId = GL.glCreateProgram?.Invoke() ?? 0u;
            GL.CheckError("Create program");

            GL.glAttachShader?.Invoke(_programId, vertexShader);
            GL.glAttachShader?.Invoke(_programId, fragmentShader);
            GL.CheckError("Attach shaders");

            GL.glLinkProgram?.Invoke(_programId);
            GL.CheckError("Link program");

            // Check link status
            int success = 0;
            GL.glGetProgramiv?.Invoke(_programId, GL.GL_LINK_STATUS, out success);
            if (success == 0)
            {
                int logLength = 0;
                GL.glGetProgramiv?.Invoke(_programId, GL.GL_INFO_LOG_LENGTH, out logLength);
                IntPtr logPtr = Marshal.AllocHGlobal(logLength);
                try
                {
                    int length = 0;
                    GL.glGetProgramInfoLog?.Invoke(_programId, logLength, out length, logPtr);
                    string log = Marshal.PtrToStringAnsi(logPtr, length);
                    throw new Exception($"Shader program linking failed:\n{log}");
                }
                finally
                {
                    Marshal.FreeHGlobal(logPtr);
                }
            }

            // Clean up individual shaders
            GL.glDeleteShader?.Invoke(vertexShader);
            GL.glDeleteShader?.Invoke(fragmentShader);
            GL.CheckError("Delete shaders");

            Console.WriteLine($"Shader compiled and linked successfully (Program ID: {_programId})");
        }

        private uint CompileShader(uint type, string source)
        {
            uint shader = GL.glCreateShader?.Invoke(type) ?? 0u;
            GL.CheckError($"Create shader type {type}");

            string[] sources = { source };
            int[] lengths = { source.Length };
            GL.glShaderSource?.Invoke(shader, 1, sources, lengths);
            GL.CheckError("Shader source");

            GL.glCompileShader?.Invoke(shader);
            GL.CheckError("Compile shader");

            // Check compilation status
            int success = 0;
            GL.glGetShaderiv?.Invoke(shader, GL.GL_COMPILE_STATUS, out success);
            if (success == 0)
            {
                int logLength = 0;
                GL.glGetShaderiv?.Invoke(shader, GL.GL_INFO_LOG_LENGTH, out logLength);
                IntPtr logPtr = Marshal.AllocHGlobal(logLength);
                try
                {
                    int length = 0;
                    GL.glGetShaderInfoLog?.Invoke(shader, logLength, out length, logPtr);
                    string log = Marshal.PtrToStringAnsi(logPtr, length);
                    string shaderType = type == GL.GL_VERTEX_SHADER ? "vertex" : "fragment";
                    throw new Exception($"{shaderType} shader compilation failed:\n{log}");
                }
                finally
                {
                    Marshal.FreeHGlobal(logPtr);
                }
            }

            return shader;
        }

        public void Use()
        {
            GL.glUseProgram?.Invoke(_programId);
            GL.CheckError("Use program");
        }

        public int GetUniformLocation(string name)
        {
            int location = GL.glGetUniformLocation?.Invoke(_programId, name) ?? -1;
            GL.CheckError($"Get uniform location: {name}");
            return location;
        }

        public void SetUniform(string name, System.Numerics.Matrix4x4 matrix)
        {
            int location = GetUniformLocation(name);
            if (location >= 0)
            {
                // Convert Matrix4x4 to float array in column-major order
                float[] matrixArray = {
                    matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                    matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                    matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                    matrix.M14, matrix.M24, matrix.M34, matrix.M44
                };
                GL.glUniformMatrix4fv?.Invoke(location, 1, false, matrixArray);
                GL.CheckError($"Set matrix uniform: {name}");
            }
        }

        public void SetUniform(string name, System.Numerics.Vector4 vector)
        {
            int location = GetUniformLocation(name);
            if (location >= 0)
            {
                GL.glUniform4f?.Invoke(location, vector.X, vector.Y, vector.Z, vector.W);
                GL.CheckError($"Set vector4 uniform: {name}");
            }
        }

        public void SetUniform(string name, int value)
        {
            int location = GetUniformLocation(name);
            if (location >= 0)
            {
                GL.glUniform1i?.Invoke(location, value);
                GL.CheckError($"Set int uniform: {name}");
            }
        }

        public void Dispose()
        {
            if (!_disposed && _programId != 0)
            {
                GL.glDeleteProgram?.Invoke(_programId);
                _programId = 0;
                _disposed = true;
            }
        }
    }
}
