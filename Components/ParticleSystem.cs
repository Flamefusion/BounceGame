// Components/ParticleSystem.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using BounceGame.Core.ECS;

namespace BounceGame.Components
{
    /// <summary>
    /// Component for particle system effects
    /// </summary>
    public class ParticleSystem : IComponent
    {
        private readonly List<Particle> _particles;
        private readonly Random _random;

        public Vector2 Position { get; set; }
        public bool IsEmitting { get; set; } = true;
        public bool AutoDestroy { get; set; } = false; // Destroy when no particles left
        
        // Emission properties
        public float EmissionRate { get; set; } = 10.0f; // particles per second
        public int MaxParticles { get; set; } = 100;
        public float EmissionTimer { get; set; } = 0.0f;
        
        // Particle properties
        public ParticleConfig Config { get; set; }

        public int ActiveParticleCount => _particles.Count;
        public IReadOnlyList<Particle> Particles => _particles;

        public ParticleSystem(ParticleConfig config)
        {
            _particles = new List<Particle>();
            _random = new Random();
            Config = config;
        }

        /// <summary>
        /// Update particle system
        /// </summary>
        public void Update(float deltaTime)
        {
            // Update emission
            if (IsEmitting && _particles.Count < MaxParticles)
            {
                EmissionTimer += deltaTime;
                float emissionInterval = 1.0f / EmissionRate;
                
                while (EmissionTimer >= emissionInterval && _particles.Count < MaxParticles)
                {
                    EmitParticle();
                    EmissionTimer -= emissionInterval;
                }
            }

            // Update existing particles
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];
                UpdateParticle(ref particle, deltaTime);

                if (particle.Life <= 0.0f)
                {
                    _particles.RemoveAt(i);
                }
                else
                {
                    _particles[i] = particle;
                }
            }
        }

        private void EmitParticle()
        {
            var particle = new Particle();
            
            // Position with spread
            particle.Position = Position + new Vector2(
                Lerp(-Config.PositionSpread.X, Config.PositionSpread.X, _random.NextSingle()),
                Lerp(-Config.PositionSpread.Y, Config.PositionSpread.Y, _random.NextSingle())
            );

            // Velocity
            float angle = Lerp(Config.MinAngle, Config.MaxAngle, _random.NextSingle());
            float speed = Lerp(Config.MinSpeed, Config.MaxSpeed, _random.NextSingle());
            particle.Velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * speed;

            // Properties
            particle.Life = particle.MaxLife = Lerp(Config.MinLifetime, Config.MaxLifetime, _random.NextSingle());
            particle.Size = Lerp(Config.MinSize, Config.MaxSize, _random.NextSingle());
            particle.Rotation = Lerp(0, MathF.PI * 2, _random.NextSingle());
            particle.AngularVelocity = Lerp(-Config.MaxAngularVelocity, Config.MaxAngularVelocity, _random.NextSingle());
            
            // Color
            particle.Color = Vector4.Lerp(Config.StartColor, Config.EndColor, _random.NextSingle());

            _particles.Add(particle);
        }

        private void UpdateParticle(ref Particle particle, float deltaTime)
        {
            // Update life
            particle.Life -= deltaTime;
            float lifeRatio = 1.0f - (particle.Life / particle.MaxLife);

            // Update position
            particle.Position += particle.Velocity * deltaTime;
            
            // Apply gravity
            particle.Velocity += Config.Gravity * deltaTime;
            
            // Apply drag
            particle.Velocity *= (1.0f - Config.Drag * deltaTime);

            // Update rotation
            particle.Rotation += particle.AngularVelocity * deltaTime;

            // Update color over lifetime
            particle.Color = Vector4.Lerp(Config.StartColor, Config.EndColor, lifeRatio);

            // Update size over lifetime
            float sizeMultiplier = Lerp(Config.StartSizeMultiplier, Config.EndSizeMultiplier, lifeRatio);
            particle.CurrentSize = particle.Size * sizeMultiplier;
        }

        /// <summary>
        /// Emit a burst of particles
        /// </summary>
        public void EmitBurst(int count)
        {
            for (int i = 0; i < count && _particles.Count < MaxParticles; i++)
            {
                EmitParticle();
            }
        }

        /// <summary>
        /// Stop emission and optionally clear existing particles
        /// </summary>
        public void Stop(bool clearParticles = false)
        {
            IsEmitting = false;
            if (clearParticles)
            {
                _particles.Clear();
            }
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Math.Clamp(t, 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Configuration for particle systems
    /// </summary>
    public struct ParticleConfig
    {
        public Vector2 PositionSpread;
        public float MinAngle, MaxAngle;
        public float MinSpeed, MaxSpeed;
        public float MinLifetime, MaxLifetime;
        public float MinSize, MaxSize;
        public float MaxAngularVelocity;
        public Vector2 Gravity;
        public float Drag;
        public Vector4 StartColor, EndColor;
        public float StartSizeMultiplier, EndSizeMultiplier;

        /// <summary>
        /// Create a fire effect configuration
        /// </summary>
        public static ParticleConfig Fire()
        {
            return new ParticleConfig
            {
                PositionSpread = new Vector2(10, 5),
                MinAngle = MathF.PI * 1.3f, // Upward
                MaxAngle = MathF.PI * 1.7f,
                MinSpeed = 30,
                MaxSpeed = 80,
                MinLifetime = 0.5f,
                MaxLifetime = 2.0f,
                MinSize = 8,
                MaxSize = 16,
                MaxAngularVelocity = 2.0f,
                Gravity = new Vector2(0, -50),
                Drag = 0.5f,
                StartColor = new Vector4(1.0f, 0.8f, 0.2f, 1.0f), // Bright orange
                EndColor = new Vector4(1.0f, 0.2f, 0.0f, 0.0f),   // Red fade to transparent
                StartSizeMultiplier = 0.5f,
                EndSizeMultiplier = 1.5f
            };
        }

        /// <summary>
        /// Create a smoke effect configuration
        /// </summary>
        public static ParticleConfig Smoke()
        {
            return new ParticleConfig
            {
                PositionSpread = new Vector2(15, 8),
                MinAngle = MathF.PI * 1.2f,
                MaxAngle = MathF.PI * 1.8f,
                MinSpeed = 20,
                MaxSpeed = 50,
                MinLifetime = 1.0f,
                MaxLifetime = 3.0f,
                MinSize = 12,
                MaxSize = 24,
                MaxAngularVelocity = 1.0f,
                Gravity = new Vector2(0, -20),
                Drag = 0.8f,
                StartColor = new Vector4(0.4f, 0.4f, 0.4f, 0.8f), // Gray
                EndColor = new Vector4(0.6f, 0.6f, 0.6f, 0.0f),   // Lighter gray fade
                StartSizeMultiplier = 0.3f,
                EndSizeMultiplier = 2.0f
            };
        }

        /// <summary>
        /// Create an explosion effect configuration
        /// </summary>
        public static ParticleConfig Explosion()
        {
            return new ParticleConfig
            {
                PositionSpread = new Vector2(5, 5),
                MinAngle = 0,
                MaxAngle = MathF.PI * 2, // All directions
                MinSpeed = 100,
                MaxSpeed = 200,
                MinLifetime = 0.3f,
                MaxLifetime = 1.0f,
                MinSize = 6,
                MaxSize = 14,
                MaxAngularVelocity = 5.0f,
                Gravity = new Vector2(0, -80),
                Drag = 1.5f,
                StartColor = new Vector4(1.0f, 1.0f, 0.8f, 1.0f), // Bright yellow
                EndColor = new Vector4(1.0f, 0.3f, 0.0f, 0.0f),   // Red fade
                StartSizeMultiplier = 1.0f,
                EndSizeMultiplier = 0.2f
            };
        }
    }

    /// <summary>
    /// Individual particle data
    /// </summary>
    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float AngularVelocity;
        public float Life;
        public float MaxLife;
        public float Size;
        public float CurrentSize;
        public Vector4 Color;
    }
}