// Core/Game/AdvancedGameManager.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Core.Resources;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame.Core.Game
{
    /// <summary>
    /// Advanced game manager with texture support, camera, and particle systems
    /// </summary>
    public class AdvancedGameManager : IDisposable
    {
        private readonly Window _window;
        private readonly World _world;
        private readonly TextureManager _textureManager;
        private readonly AdvancedRenderer _renderer;
        private readonly AdvancedRenderSystem _renderSystem;
        private readonly ParticleRenderSystem _particleRenderSystem;
        private readonly CameraFollowSystem _cameraFollowSystem;
        
        private readonly List<ISystem> _updateSystems;
        private readonly List<IRenderSystem> _renderSystems;
        
        private bool _disposed = false;
        
        public bool ShouldClose => _window.ShouldClose;
        public int TotalEntities => _world.EntityCount;
        public Camera Camera => _renderer.Camera;

        public AdvancedGameManager(string title, int width, int height)
        {
            _window = new Window(title, width, height);
            _world = new World();
            _textureManager = new TextureManager();
            _renderer = new AdvancedRenderer(width, height, _textureManager);
            
            // Create systems
            _renderSystem = new AdvancedRenderSystem(_world, _renderer);
            _particleRenderSystem = new ParticleRenderSystem(_world, _renderer);
            _cameraFollowSystem = new CameraFollowSystem(_world, _renderer.Camera);
            
            _updateSystems = new List<ISystem>
            {
                new MovementSystem(_world),
                new AnimationSystem(_world),
                _particleRenderSystem, // Updates particles
                _cameraFollowSystem
            };
            
            _renderSystems = new List<IRenderSystem>
            {
                _renderSystem,
                _particleRenderSystem
            };
            
            Console.WriteLine("AdvancedGameManager initialized with full rendering pipeline");
        }

        public Entity CreateSpriteEntity(Vector2 position, Vector2 size, Vector4 color, int layer = 0)
        {
            var entity = _world.CreateEntity();
            
            _world.AddComponent(entity, new Transform(new Vector3(position, 0)));
            _world.AddComponent(entity, new Sprite(size, color, layer));
            
            return entity;
        }

        public Entity CreatePhysicsEntity(Vector2 position, Vector2 size, Vector4 color, float mass = 1.0f)
        {
            var entity = CreateSpriteEntity(position, size, color);
            _world.AddComponent(entity, new Rigidbody(mass));
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
            var bgColor = clearColor ?? new Vector4(0.05f, 0.05f, 0.15f, 1.0f);
            _window.Clear(bgColor.X, bgColor.Y, bgColor.Z, bgColor.W);
            
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
        public TextureManager GetTextureManager() => _textureManager;
        public Camera GetCamera() => _renderer.Camera;

        public string GetAdvancedStats()
        {
            return $"Advanced Game Stats:\n" +
                   $"  Entities: {TotalEntities}\n" +
                   $"  {_renderer.GetStats()}\n" +
                   $"  {_renderSystem.GetRenderStats()}\n" +
                   $"  {_particleRenderSystem.GetStats()}\n" +
                   $"  {_textureManager.GetStats()}";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var system in _updateSystems)
                    if (system is IDisposable disposable) disposable.Dispose();
                
                foreach (var system in _renderSystems)
                    if (system is IDisposable disposable) disposable.Dispose();
                
                _renderer?.Dispose();
                _textureManager?.Dispose();
                _window?.Dispose();
                
                _disposed = true;
            }
        }
    }
}