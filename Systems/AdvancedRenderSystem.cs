// Systems/AdvancedRenderSystem.cs
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
    /// Advanced render system with texture and animation support
    /// </summary>
    public class AdvancedRenderSystem : IRenderSystem, IDisposable
    {
        private readonly World _world;
        private readonly AdvancedRenderer _renderer;
        private bool _disposed = false;

        public int EntitiesRendered { get; private set; }
        public int DrawCalls => _renderer.DrawCalls;

        public AdvancedRenderSystem(World world, AdvancedRenderer renderer)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public void Render()
        {
            _renderer.BeginFrame();
            EntitiesRendered = 0;

            // Collect and sort renderable entities
            var renderData = CollectRenderData();
            
            // Sort by layer (back to front)
            renderData.Sort((a, b) => a.Layer.CompareTo(b.Layer));

            // Render all entities
            foreach (var data in renderData)
            {
                RenderEntity(data);
                EntitiesRendered++;
            }
        }

        private List<RenderData> CollectRenderData()
        {
            var renderData = new List<RenderData>();

            // Static sprites
            foreach (var entity in _world.GetEntitiesWith<Transform, Sprite>())
            {
                var transform = _world.GetComponent<Transform>(entity);
                var sprite = _world.GetComponent<Sprite>(entity);

                if (!sprite.Visible)
                    continue;

                renderData.Add(new RenderData
                {
                    Entity = entity,
                    Transform = transform,
                    Sprite = sprite,
                    AnimatedSprite = null,
                    Layer = sprite.Layer,
                    RenderType = RenderType.Static
                });
            }

            // Animated sprites
            foreach (var entity in _world.GetEntitiesWith<Transform, AnimatedSprite>())
            {
                var transform = _world.GetComponent<Transform>(entity);
                var animatedSprite = _world.GetComponent<AnimatedSprite>(entity);

                // Try to get regular sprite for fallback
                _world.TryGetComponent<Sprite>(entity, out var sprite);

                renderData.Add(new RenderData
                {
                    Entity = entity,
                    Transform = transform,
                    Sprite = sprite,
                    AnimatedSprite = animatedSprite,
                    Layer = sprite?.Layer ?? 0,
                    RenderType = RenderType.Animated
                });
            }

            return renderData;
        }

        private void RenderEntity(RenderData data)
        {
            var position = new Vector2(data.Transform.Position.X, data.Transform.Position.Y);
            var rotation = data.Transform.Rotation.Z * (MathF.PI / 180.0f); // Convert to radians
            var worldScale = data.Transform.Scale;

            if (data.RenderType == RenderType.Animated && data.AnimatedSprite != null)
            {
                RenderAnimatedSprite(data, position, worldScale, rotation);
            }
            else if (data.Sprite != null)
            {
                RenderStaticSprite(data, position, worldScale, rotation);
            }
        }

        private void RenderAnimatedSprite(RenderData data, Vector2 position, Vector3 worldScale, float rotation)
        {
            var animSprite = data.AnimatedSprite!;
            var size = data.Sprite?.Size ?? new Vector2(32, 32);
            size = new Vector2(size.X * worldScale.X, size.Y * worldScale.Y);
            
            if (animSprite.SpriteSheet == null)
            {
                // Fall back to colored quad
                var color = data.Sprite?.Color ?? Vector4.One;
                _renderer.DrawQuad(position, size, color, rotation);
                return;
            }

            // Render current animation frame
            var spriteSheet = animSprite.SpriteSheet;
            var frameIndex = animSprite.CurrentFrame;
            var tint = data.Sprite?.Color ?? Vector4.One;

            _renderer.DrawSprite(position, size, tint, spriteSheet, frameIndex, rotation);
        }

        private void RenderStaticSprite(RenderData data, Vector2 position, Vector3 worldScale, float rotation)
        {
            var sprite = data.Sprite!;
            var size = new Vector2(sprite.Size.X * worldScale.X, sprite.Size.Y * worldScale.Y);

            if (sprite.TextureId != 0)
            {
                // TODO: Implement texture rendering from texture ID
                // For now, render as colored quad
                _renderer.DrawQuad(position, size, sprite.Color, rotation);
            }
            else
            {
                // Render as colored quad
                _renderer.DrawQuad(position, size, sprite.Color, rotation);
            }
        }

        public string GetRenderStats()
        {
            return $"AdvancedRenderSystem: {EntitiesRendered} entities, {DrawCalls} draw calls";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        private struct RenderData
        {
            public Entity Entity;
            public Transform Transform;
            public Sprite? Sprite;
            public AnimatedSprite? AnimatedSprite;
            public int Layer;
            public RenderType RenderType;
        }

        private enum RenderType
        {
            Static,
            Animated
        }
    }
}