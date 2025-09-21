using System;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame.Systems
{
    /// <summary>
    /// Fixed movement system that uses Window input instead of Win32 directly
    /// </summary>
    public class FixedMovementSystem : ISystem
    {
        private readonly World _world;
        private readonly Window _window;
        private static readonly Vector3 Gravity = new Vector3(0, -200.0f, 0);
        
        public FixedMovementSystem(World world, Window window)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Update(float deltaTime)
        {
            // Handle player input using Window input methods
            UpdatePlayerMovement(deltaTime);
            
            // Update physics for all rigidbodies
            UpdatePhysics(deltaTime);
            
            // Apply movement to transforms
            UpdateTransforms(deltaTime);
        }
        
        private void UpdatePlayerMovement(float deltaTime)
        {
            foreach (var entity in _world.GetEntitiesWith<PlayerController, Rigidbody>())
            {
                var playerController = _world.GetComponent<PlayerController>(entity);
                var rigidbody = _world.GetComponent<Rigidbody>(entity);
                
                if (!playerController.CanMove)
                    continue;
                
                Vector3 inputForce = Vector3.Zero;
                
                // Use Window input methods instead of Win32 directly
                if (_window.IsWPressed())
                    inputForce.Y += 1.0f;
                if (_window.IsSPressed())
                    inputForce.Y -= 1.0f;
                if (_window.IsAPressed())
                    inputForce.X -= 1.0f;
                if (_window.IsDPressed())
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
                
                // Reset acceleration
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
                
                // Simple boundary constraints
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
    }
}