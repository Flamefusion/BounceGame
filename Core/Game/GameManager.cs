// Core/Game/GameManager.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Systems;
using BounceGame.Components;

namespace BounceGame.Core.Game
{
    /// <summary>
    /// Main game manager that coordinates ECS world, systems, and rendering
    /// </summary>
    public class GameManager : IDisposable
    {
        private readonly Window _window;
        private readonly World _world;
        private readonly Renderer _renderer;
        private readonly RenderSystem _renderSystem;
        
        // System collections
        private readonly List<ISystem> _updateSystems;
        private readonly List<IRenderSystem> _renderSystems;
        
        private bool _disposed = false;
        public bool ShouldClose => _window.ShouldClose;
        
        // Game statistics
        public int TotalEntities => _world.EntityCount;
        public string RenderStats => _renderSystem.GetRenderStats();

        public GameManager(string title, int width, int height)
        {
            // Initialize core systems
            _window = new Window(title, width, height);
            _world = new World();
            _renderer = new Renderer(width, height);
            _renderSystem = new RenderSystem(_world, _renderer);
            
            // Initialize system collections
            _updateSystems = new List<ISystem>();
            _renderSystems = new List<IRenderSystem> { _renderSystem };
            
            Console.WriteLine("GameManager initialized successfully");
        }

        /// <summary>
        /// Add a system to the update loop
        /// </summary>
        public void AddSystem(ISystem system)
        {
            _updateSystems.Add(system);
            
            if (system is IInitializableSystem initSystem)
            {
                initSystem.Initialize();
            }
            
            if (system is IRenderSystem renderSystem)
            {
                _renderSystems.Add(renderSystem);
            }
        }

        /// <summary>
        /// Create a basic entity with Transform and Sprite
        /// </summary>
        public Entity CreateSpriteEntity(Vector2 position, Vector2 size, Vector4 color, int layer = 0)
        {
            var entity = _world.CreateEntity();
            
            _world.AddComponent(entity, new Transform(new Vector3(position, 0)));
            _world.AddComponent(entity, new Sprite(size, color, layer));
            
            return entity;
        }

        /// <summary>
        /// Create a physics-enabled entity
        /// </summary>
        public Entity CreatePhysicsEntity(Vector2 position, Vector2 size, Vector4 color, float mass = 1.0f)
        {
            var entity = CreateSpriteEntity(position, size, color);
            _world.AddComponent(entity, new Rigidbody(mass));
            return entity;
        }

        /// <summary>
        /// Main game update loop
        /// </summary>
        public void Update(float deltaTime)
        {
            // Poll window events
            _window.PollEvents();
            
            // Update all systems
            foreach (var system in _updateSystems)
            {
                system.Update(deltaTime);
            }
            
            // Clean up destroyed entities periodically
            _world.CleanupDestroyedEntities();
        }

        /// <summary>
        /// Main rendering loop
        /// </summary>
        public void Render(Vector4? clearColor = null)
        {
            var bgColor = clearColor ?? new Vector4(0.1f, 0.1f, 0.2f, 1.0f);
            _window.Clear(bgColor.X, bgColor.Y, bgColor.Z, bgColor.W);
            
            // Render all render systems
            foreach (var renderSystem in _renderSystems)
            {
                renderSystem.Render();
            }
            
            _window.SwapBuffers();
        }

        /// <summary>
        /// Check if a key is currently pressed
        /// </summary>
        public bool IsKeyPressed(int virtualKey) => _window.IsKeyPressed(virtualKey);
        
        /// <summary>
        /// Check if a key was just pressed this frame
        /// </summary>
        public bool IsKeyJustPressed(int virtualKey) => _window.IsKeyJustPressed(virtualKey);
        
        // Convenience methods for WASD + Space
        public bool IsWPressed() => _window.IsWPressed();
        public bool IsAPressed() => _window.IsAPressed();
        public bool IsSPressed() => _window.IsSPressed();
        public bool IsDPressed() => _window.IsDPressed();
        public bool IsSpacePressed() => _window.IsSpacePressed();
        public bool IsSpaceJustPressed() => _window.IsSpaceJustPressed();

        /// <summary>
        /// Get access to the ECS world for advanced operations
        /// </summary>
        public World GetWorld() => _world;

        /// <summary>
        /// Get debug information about the game state
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Game Debug Info:\n" +
                   $"  Total Entities: {TotalEntities}\n" +
                   $"  {RenderStats}\n" +
                   $"  Update Systems: {_updateSystems.Count}\n" +
                   $"  Render Systems: {_renderSystems.Count}";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // Dispose systems
                foreach (var system in _updateSystems)
                {
                    if (system is IDisposable disposableSystem)
                        disposableSystem.Dispose();
                }
                
                foreach (var system in _renderSystems)
                {
                    if (system is IDisposable disposableSystem)
                        disposableSystem.Dispose();
                }
                
                // Dispose core systems
                _renderSystem?.Dispose();
                _renderer?.Dispose();
                _window?.Dispose();
                
                _disposed = true;
            }
        }
    }
}