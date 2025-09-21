// Systems/ParticleRenderSystem.cs
using System;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Components;

namespace BounceGame.Systems
{
    /// <summary>
    /// System that updates and renders particle systems
    /// </summary>
    public class ParticleRenderSystem : ISystem, IRenderSystem, IDisposable
    {
        private readonly World _world;
        private readonly AdvancedRenderer _renderer;
        private bool _disposed = false;

        public int ParticleSystemsUpdated { get; private set; }
        public int ParticlesRendered { get; private set; }

        public ParticleRenderSystem(World world, AdvancedRenderer renderer)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public void Update(float deltaTime)
        {
            ParticleSystemsUpdated = 0;

            foreach (var entity in _world.GetEntitiesWith<ParticleSystem>())
            {
                var particleSystem = _world.GetComponent<ParticleSystem>(entity);
                particleSystem.Update(deltaTime);
                ParticleSystemsUpdated++;

                // Auto-destroy empty systems
                if (particleSystem.AutoDestroy && 
                    !particleSystem.IsEmitting && 
                    particleSystem.ActiveParticleCount == 0)
                {
                    _world.DestroyEntity(entity);
                }
            }
        }

        public void Render()
        {
            ParticlesRendered = 0;

            foreach (var entity in _world.GetEntitiesWith<ParticleSystem>())
            {
                var particleSystem = _world.GetComponent<ParticleSystem>(entity);
                
                // Update system position if it has a transform
                if (_world.TryGetComponent<Transform>(entity, out var transform))
                {
                    particleSystem.Position = new Vector2(transform.Position.X, transform.Position.Y);
                }

                // Render all particles
                foreach (var particle in particleSystem.Particles)
                {
                    var size = new Vector2(particle.CurrentSize, particle.CurrentSize);
                    _renderer!.DrawQuad(particle.Position, size, particle.Color, particle.Rotation);
                    ParticlesRendered++;
                }
            }
        }

        public string GetStats()
        {
            return $"ParticleRenderSystem: {ParticleSystemsUpdated} systems, {ParticlesRendered} particles rendered";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}