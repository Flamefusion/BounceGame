// Components/Transform.cs
using System.Numerics;
using BounceGame.Core.ECS;

namespace BounceGame.Components
{
    /// <summary>
    /// Transform component - position, rotation, scale in world space
    /// </summary>
    public class Transform : IComponent
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; } // Euler angles in degrees
        public Vector3 Scale { get; set; }
        
        public Transform(Vector3 position = default, Vector3 rotation = default, Vector3? scale = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale ?? Vector3.One;
        }
        
        /// <summary>
        /// Creates a transformation matrix from this transform
        /// </summary>
        public Matrix4x4 GetTransformMatrix()
        {
            var translation = Matrix4x4.CreateTranslation(Position);
            var rotationX = Matrix4x4.CreateRotationX(Core.MathHelper.DegreesToRadians(Rotation.X));
            var rotationY = Matrix4x4.CreateRotationY(Core.MathHelper.DegreesToRadians(Rotation.Y));
            var rotationZ = Matrix4x4.CreateRotationZ(Core.MathHelper.DegreesToRadians(Rotation.Z));
            var scale = Matrix4x4.CreateScale(Scale);
            
            return scale * rotationZ * rotationY * rotationX * translation;
        }
        
        public override string ToString()
        {
            return $"Transform(Pos: {Position}, Rot: {Rotation}, Scale: {Scale})";
        }
    }
}