// Core/Game/GameManager.cs - Debug Version
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
    /// Debug version of GameManager with detailed logging
    /// </summary>
    public class GameManager : IDisposable
    {
        private readonly Window _window;
        private readonly World _world;
        private readonly DebugRenderer _renderer;
        private readonly RenderSystem _renderSystem;
        
        private readonly List<ISystem> _updateSystems;
        private readonly List<IRenderSystem> _renderSystems;
        
        private bool _disposed = false;
        public bool ShouldClose => _window.ShouldClose;
        public int TotalEntities => _world.EntityCount;
        public string RenderStats => _renderSystem.GetRenderStats();

        public GameManager(string title, int width, int height)
        {
            Console.WriteLine($"Creating GameManager: {title} ({width}x{height})");
            
            _window = new Window(title, width, height);
            Console.WriteLine("Window created successfully");
            
            _world = new World();
            Console.WriteLine("ECS World created successfully");
            
            _renderer = new DebugRenderer(width, height);
            Console.WriteLine("Debug renderer created successfully");
            
            _renderSystem = new RenderSystem(_world, _renderer);
            Console.WriteLine("Render system created successfully");
            
            _updateSystems = new List<ISystem>();
            _renderSystems = new List<IRenderSystem> { _renderSystem };
            
            Console.WriteLine("GameManager initialization complete");
        }

        public void AddSystem(ISystem system)
        {
            _updateSystems.Add(system);
            Console.WriteLine($"Added system: {system.GetType().Name}");
            
            if (system is IRenderSystem renderSystem)
            {
                _renderSystems.Add(renderSystem);
                Console.WriteLine("  (Also added to render systems)");
            }
        }

        public Entity CreateSpriteEntity(Vector2 position, Vector2 size, Vector4 color, int layer = 0)
        {
            var entity = _world.CreateEntity();
            
            var transform = new Transform(new Vector3(position, 0));
            var sprite = new Sprite(size, color, layer);
            
            _world.AddComponent(entity, transform);
            _world.AddComponent(entity, sprite);
            
            Console.WriteLine($"Created sprite entity {entity}: pos=({position.X}, {position.Y}), size=({size.X}, {size.Y})");
            
            return entity;
        }

        public Entity CreatePhysicsEntity(Vector2 position, Vector2 size, Vector4 color, float mass = 1.0f)
        {
            var entity = CreateSpriteEntity(position, size, color);
            var rigidbody = new Rigidbody(mass);
            
            _world.AddComponent(entity, rigidbody);
            Console.WriteLine($"Added Rigidbody to entity {entity} (mass: {mass})");
            
            return entity;
        }

        public void Update(float deltaTime)
        {
            _window.PollEvents();
            
            foreach (var system in _updateSystems)
            {
                system.Update(deltaTime);
            }
            
            _world.CleanupDestroyedEntities();
        }

        public void Render(Vector4? clearColor = null)
        {
            var bgColor = clearColor ?? new Vector4(0.1f, 0.1f, 0.2f, 1.0f);
            _window.Clear(bgColor.X, bgColor.Y, bgColor.Z, bgColor.W);
            
            _renderer.ResetDrawCallCount();
            
            foreach (var renderSystem in _renderSystems)
            {
                renderSystem.Render();
            }
            
            _window.SwapBuffers();
        }

        // Input methods
        public bool IsKeyPressed(int virtualKey) => _window.IsKeyPressed(virtualKey);
        public bool IsKeyJustPressed(int virtualKey) => _window.IsKeyJustPressed(virtualKey);
        public bool IsWPressed() => _window.IsWPressed();
        public bool IsAPressed() => _window.IsAPressed();
        public bool IsSPressed() => _window.IsSPressed();
        public bool IsDPressed() => _window.IsDPressed();
        public bool IsSpacePressed() => _window.IsSpacePressed();
        public bool IsSpaceJustPressed() => _window.IsSpaceJustPressed();

        public World GetWorld() => _world;

        public string GetDebugInfo()
        {
            return $"Game Debug Info:\n" +
                   $"  Total Entities: {TotalEntities}\n" +
                   $"  Draw Calls: {_renderer.DrawCallCount}\n" +
                   $"  {RenderStats}\n" +
                   $"  Update Systems: {_updateSystems.Count}\n" +
                   $"  Render Systems: {_renderSystems.Count}";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _renderSystem?.Dispose();
                _renderer?.Dispose();
                _window?.Dispose();
                _disposed = true;
                Console.WriteLine("GameManager disposed");
            }
        }
    }
}