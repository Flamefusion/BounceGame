// Systems/CameraFollowSystem.cs
using System;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Components;

namespace BounceGame.Systems
{
    /// <summary>
    /// System that makes camera follow target entities
    /// </summary>
    public class CameraFollowSystem : ISystem
    {
        private readonly World _world;
        private readonly Camera _camera;

        public CameraFollowSystem(World world, Camera camera)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }

        public void Update(float deltaTime)
        {
            // Find active camera targets
            foreach (var entity in _world.GetEntitiesWith<Transform, CameraTarget>())
            {
                var transform = _world.GetComponent<Transform>(entity);
                var cameraTarget = _world.GetComponent<CameraTarget>(entity);
                
                if (!cameraTarget.IsActive)
                    continue;
                
                var targetPosition = new Vector2(transform.Position.X, transform.Position.Y) + cameraTarget.Offset;
                
                // Smoothly move camera toward target
                _camera.MoveTowards(targetPosition, cameraTarget.FollowSpeed * 100.0f, deltaTime);
                
                // Only follow first active target
                break;
            }
        }
    }
}