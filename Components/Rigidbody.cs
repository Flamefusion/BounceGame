// Components/Rigidbody.cs
using System.Numerics;
using BounceGame.Core.ECS;

namespace BounceGame.Components
{
    /// <summary>
    /// Rigidbody component - physics properties
    /// </summary>
    public class Rigidbody : IComponent
    {
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Mass { get; set; } = 1.0f;
        public float Drag { get; set; } = 0.1f; // Air resistance
        public bool IsKinematic { get; set; } = false; // If true, not affected by physics
        public bool UseGravity { get; set; } = true;
        
        public Rigidbody(float mass = 1.0f, bool useGravity = true)
        {
            Mass = mass;
            UseGravity = useGravity;
        }
        
        /// <summary>
        /// Applies a force to the rigidbody
        /// </summary>
        public void AddForce(Vector3 force)
        {
            if (!IsKinematic)
            {
                Acceleration += force / Mass;
            }
        }
        
        /// <summary>
        /// Applies an impulse (instant velocity change)
        /// </summary>
        public void AddImpulse(Vector3 impulse)
        {
            if (!IsKinematic)
            {
                Velocity += impulse / Mass;
            }
        }
        
        public override string ToString()
        {
            return $"Rigidbody(Vel: {Velocity}, Mass: {Mass}, Kinematic: {IsKinematic})";
        }
    }
}