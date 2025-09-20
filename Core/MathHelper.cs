// Core/MathHelper.cs
using System;

namespace BounceGame.Core
{
    /// <summary>
    /// Math utility functions
    /// </summary>
    public static class MathHelper
    {
        public const float PI = (float)Math.PI;
        public const float PIOver2 = PI / 2.0f;
        public const float PIOver4 = PI / 4.0f;
        public const float TwoPI = PI * 2.0f;
        
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * PI / 180.0f;
        }
        
        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static float RadiansToDegrees(float radians)
        {
            return radians * 180.0f / PI;
        }
        
        /// <summary>
        /// Clamps a value between min and max
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
        
        /// <summary>
        /// Linear interpolation
        /// </summary>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp(t, 0.0f, 1.0f);
        }
    }
}