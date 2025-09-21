// Systems/AnimationSystem.cs
using System;
using BounceGame.Core.ECS;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame.Systems
{
    /// <summary>
    /// System that updates animated sprites
    /// </summary>
    public class AnimationSystem : ISystem
    {
        private readonly World _world;

        public AnimationSystem(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _world.GetEntitiesWith<AnimatedSprite>())
            {
                var animatedSprite = _world.GetComponent<AnimatedSprite>(entity);
                animatedSprite.UpdateAnimation(deltaTime);
            }
        }
    }
}