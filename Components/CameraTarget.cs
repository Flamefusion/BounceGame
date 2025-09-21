// Components/CameraTarget.cs
using BounceGame.Core.ECS;
using System.Numerics;

namespace BounceGame.Components
{
    /// <summary>
    /// Component that marks an entity as a camera target
    /// </summary>
    public class CameraTarget : IComponent
    {
        public bool IsActive { get; set; } = true;
        public float FollowSpeed { get; set; } = 5.0f;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        
        public override string ToString()
        {
            return $"CameraTarget(Active: {IsActive}, Speed: {FollowSpeed})";
        }
    }
}
