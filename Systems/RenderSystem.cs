// Systems/RenderSystem.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Components;

namespace BounceGame.Systems
{
    /// <summary>
    /// System responsible for rendering sprites using the OpenGL renderer
    /// </summary>
    public class RenderSystem : IRenderSystem
    {
        private readonly World _world;
        private readonly DebugRenderer _renderer;
        private bool _disposed = false;

        // Render statistics
        public int EntitiesRendered { get; private set; }
        public int DrawCalls { get; private set; }

        public RenderSystem(World world, DebugRenderer renderer)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        /// <summary>
        /// Render all entities with Transform and Sprite components
        /// </summary>
        public void Render()
        {
            EntitiesRendered = 0;
            DrawCalls = 0;

            // Get all entities with both Transform and Sprite components
            var renderableEntities = _world.GetEntitiesWith<Transform, Sprite>().ToList();
            
            if (renderableEntities.Count == 0)
                return;

            // Create a list of render data for sorting
            var renderData = new List<RenderData>();
            
            foreach (var entity in renderableEntities)
            {
                var transform = _world.GetComponent<Transform>(entity);
                var sprite = _world.GetComponent<Sprite>(entity);
                
                // Skip invisible sprites
                if (!sprite.Visible)
                    continue;

                renderData.Add(new RenderData
                {
                    Entity = entity,
                    Transform = transform,
                    Sprite = sprite,
                    Position = new Vector2(transform.Position.X, transform.Position.Y),
                    WorldSize = new Vector2(sprite.Size.X * transform.Scale.X, sprite.Size.Y * transform.Scale.Y)
                });
            }

            // Sort by layer (back to front)
            renderData.Sort((a, b) => a.Sprite.Layer.CompareTo(b.Sprite.Layer));

            // Render all sprites
            foreach (var data in renderData)
            {
                RenderSprite(data);
                EntitiesRendered++;
                DrawCalls++;
            }
        }

        private void RenderSprite(RenderData data)
        {
            // For now, render as colored quad
            // Future: Add texture support here
            if (data.Sprite.TextureId == 0)
            {
                // Render as colored quad
                _renderer.DrawQuad(data.Position, data.WorldSize, data.Sprite.Color);
            }
            else
            {
                // Future: Render textured quad
                // For now, fall back to colored rendering
                _renderer.DrawQuad(data.Position, data.WorldSize, data.Sprite.Color);
            }
        }

        /// <summary>
        /// Get render statistics as a formatted string
        /// </summary>
        public string GetRenderStats()
        {
            return $"RenderSystem: {EntitiesRendered} entities, {DrawCalls} draw calls";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // RenderSystem doesn't own the renderer, so we don't dispose it
                _disposed = true;
            }
        }

        /// <summary>
        /// Internal data structure for sorting and batching
        /// </summary>
        private struct RenderData
        {
            public Entity Entity;
            public Transform Transform;
            public Sprite Sprite;
            public Vector2 Position;
            public Vector2 WorldSize;
        }
    }
}