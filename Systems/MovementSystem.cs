// Systems/MovementSystem.cs
using System;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame.Systems
{
    /// <summary>
    /// System that handles player input and basic physics movement
    /// </summary>
    public class MovementSystem : ISystem
    {
        private readonly World _world;
        private static readonly Vector3 Gravity = new Vector3(0, -200.0f, 0); // pixels/second²
        
        public MovementSystem(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public void Update(float deltaTime)
        {
            // Handle player input
            UpdatePlayerMovement(deltaTime);
            
            // Update physics for all rigidbodies
            UpdatePhysics(deltaTime);
            
            // Apply movement to transforms
            UpdateTransforms(deltaTime);
        }
        
        private void UpdatePlayerMovement(float deltaTime)
        {
            // This is a bit of a hack - we need access to input
            // In a real system, we'd pass input state or have an InputSystem
            // For now, we'll use Win32 API directly
            
            foreach (var entity in _world.GetEntitiesWith<PlayerController, Rigidbody>())
            {
                var playerController = _world.GetComponent<PlayerController>(entity);
                var rigidbody = _world.GetComponent<Rigidbody>(entity);
                
                if (!playerController.CanMove)
                    continue;
                
                Vector3 inputForce = Vector3.Zero;
                
                // Check input using Win32 API
                if (IsKeyPressed(Win32.VK_W))
                    inputForce.Y += 1.0f;
                if (IsKeyPressed(Win32.VK_S))
                    inputForce.Y -= 1.0f;
                if (IsKeyPressed(Win32.VK_A))
                    inputForce.X -= 1.0f;
                if (IsKeyPressed(Win32.VK_D))
                    inputForce.X += 1.0f;
                
                // Normalize and apply speed
                if (inputForce.LengthSquared() > 0)
                {
                    inputForce = Vector3.Normalize(inputForce) * playerController.Speed;
                    rigidbody.AddForce(inputForce);
                }
                
                // Apply drag to player for responsive controls
                rigidbody.Velocity *= (1.0f - rigidbody.Drag * deltaTime);
            }
        }
        
        private void UpdatePhysics(float deltaTime)
        {
            foreach (var entity in _world.GetEntitiesWith<Rigidbody>())
            {
                var rigidbody = _world.GetComponent<Rigidbody>(entity);
                
                if (rigidbody.IsKinematic)
                    continue;
                
                // Apply gravity
                if (rigidbody.UseGravity)
                {
                    rigidbody.AddForce(Gravity * rigidbody.Mass);
                }
                
                // Update velocity from acceleration
                rigidbody.Velocity += rigidbody.Acceleration * deltaTime;
                
                // Apply general drag
                rigidbody.Velocity *= (1.0f - rigidbody.Drag * deltaTime * 0.5f);
                
                // Reset acceleration (forces are applied each frame)
                rigidbody.Acceleration = Vector3.Zero;
            }
        }
        
        private void UpdateTransforms(float deltaTime)
        {
            foreach (var entity in _world.GetEntitiesWith<Transform, Rigidbody>())
            {
                var transform = _world.GetComponent<Transform>(entity);
                var rigidbody = _world.GetComponent<Rigidbody>(entity);
                
                // Update position from velocity
                transform.Position += rigidbody.Velocity * deltaTime;
                
                // Simple boundary constraints (bounce off edges)
                const float boundary = 400.0f;
                if (Math.Abs(transform.Position.X) > boundary)
                {
                    transform.Position = new Vector3(
                        Math.Sign(transform.Position.X) * boundary,
                        transform.Position.Y,
                        transform.Position.Z
                    );
                    rigidbody.Velocity = new Vector3(-rigidbody.Velocity.X * 0.8f, rigidbody.Velocity.Y, rigidbody.Velocity.Z);
                }
                
                const float verticalBoundary = 300.0f;
                if (Math.Abs(transform.Position.Y) > verticalBoundary)
                {
                    transform.Position = new Vector3(
                        transform.Position.X,
                        Math.Sign(transform.Position.Y) * verticalBoundary,
                        transform.Position.Z
                    );
                    rigidbody.Velocity = new Vector3(rigidbody.Velocity.X, -rigidbody.Velocity.Y * 0.8f, rigidbody.Velocity.Z);
                }
            }
        }
        
        // Helper method to check key state
        // In a real implementation, this would be injected or handled by an InputSystem
        private bool IsKeyPressed(int virtualKey)
        {
            return (Win32.GetAsyncKeyState(virtualKey) & 0x8000) != 0;
        }
    }
}
