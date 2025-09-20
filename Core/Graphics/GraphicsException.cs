// Core/Graphics/GraphicsException.cs
using System;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Graphics-specific exception for OpenGL errors
    /// </summary>
    public class GraphicsException : Exception
    {
        public GraphicsException(string message) : base(message) { }
        public GraphicsException(string message, Exception innerException) : base(message, innerException) { }
    }
}