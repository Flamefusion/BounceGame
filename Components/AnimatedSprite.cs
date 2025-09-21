// Components/AnimatedSprite.cs
using System;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;

namespace BounceGame.Components
{
    /// <summary>
    /// Component for animated sprites using sprite sheets
    /// </summary>
    public class AnimatedSprite : IComponent
    {
        public SpriteSheet? SpriteSheet { get; set; }
        public string CurrentAnimation { get; set; } = "";
        public float AnimationSpeed { get; set; } = 10.0f; // frames per second
        public bool IsPlaying { get; set; } = true;
        public bool Loop { get; set; } = true;
        public int CurrentFrame { get; set; } = 0;
        public float FrameTimer { get; set; } = 0.0f;
        
        // Animation definitions
        private readonly System.Collections.Generic.Dictionary<string, AnimationClip> _animations;
        
        public AnimatedSprite()
        {
            _animations = new System.Collections.Generic.Dictionary<string, AnimationClip>();
        }

        /// <summary>
        /// Add an animation clip
        /// </summary>
        public void AddAnimation(string name, int startFrame, int endFrame, float speed = 10.0f, bool loop = true)
        {
            _animations[name] = new AnimationClip
            {
                Name = name,
                StartFrame = startFrame,
                EndFrame = endFrame,
                Speed = speed,
                Loop = loop
            };
        }

        /// <summary>
        /// Play an animation
        /// </summary>
        public void PlayAnimation(string name)
        {
            if (_animations.TryGetValue(name, out var animation))
            {
                CurrentAnimation = name;
                CurrentFrame = animation.StartFrame;
                FrameTimer = 0.0f;
                AnimationSpeed = animation.Speed;
                Loop = animation.Loop;
                IsPlaying = true;
            }
        }

        /// <summary>
        /// Get current animation clip
        /// </summary>
        public AnimationClip? GetCurrentAnimation()
        {
            return _animations.TryGetValue(CurrentAnimation, out var animation) ? animation : null;
        }

        /// <summary>
        /// Update animation (called by AnimationSystem)
        /// </summary>
        public void UpdateAnimation(float deltaTime)
        {
            if (!IsPlaying || SpriteSheet == null)
                return;

            var currentAnim = GetCurrentAnimation();
            if (currentAnim == null)
                return;

            FrameTimer += deltaTime;
            float frameTime = 1.0f / AnimationSpeed;

            if (FrameTimer >= frameTime)
            {
                FrameTimer -= frameTime;
                CurrentFrame++;

                if (CurrentFrame > currentAnim.Value.EndFrame)
                {
                    if (Loop)
                    {
                        CurrentFrame = currentAnim.Value.StartFrame;
                    }
                    else
                    {
                        CurrentFrame = currentAnim.Value.EndFrame;
                        IsPlaying = false;
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"AnimatedSprite(Anim: '{CurrentAnimation}', Frame: {CurrentFrame}, Playing: {IsPlaying})";
        }
    }

    /// <summary>
    /// Represents an animation clip within a sprite sheet
    /// </summary>
    public struct AnimationClip
    {
        public string Name;
        public int StartFrame;
        public int EndFrame;
        public float Speed;
        public bool Loop;

        public int FrameCount => EndFrame - StartFrame + 1;
    }
}