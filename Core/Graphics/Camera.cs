// Core/Graphics/Camera.cs
using System;
using System.Numerics;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// 2D camera with position, zoom, and viewport management
    /// </summary>
    public class Camera
    {
        private Vector2 _position;
        private float _zoom = 1.0f;
        private float _rotation = 0.0f;
        private int _viewportWidth;
        private int _viewportHeight;
        private Matrix4x4 _viewMatrix;
        private Matrix4x4 _projectionMatrix;
        private bool _matricesDirty = true;

        public Vector2 Position
        {
            get => _position;
            set { _position = value; _matricesDirty = true; }
        }

        public float Zoom
        {
            get => _zoom;
            set { _zoom = Math.Max(0.1f, value); _matricesDirty = true; }
        }

        public float Rotation
        {
            get => _rotation;
            set { _rotation = value; _matricesDirty = true; }
        }

        public int ViewportWidth => _viewportWidth;
        public int ViewportHeight => _viewportHeight;
        public Matrix4x4 ViewMatrix => GetViewMatrix();
        public Matrix4x4 ProjectionMatrix => GetProjectionMatrix();
        public Matrix4x4 ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;

        public Camera(int viewportWidth, int viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _position = Vector2.Zero;
            UpdateMatrices();
        }

        /// <summary>
        /// Set viewport size (call when window is resized)
        /// </summary>
        public void SetViewport(int width, int height)
        {
            _viewportWidth = width;
            _viewportHeight = height;
            _matricesDirty = true;
        }

        /// <summary>
        /// Move camera by offset
        /// </summary>
        public void Move(Vector2 offset)
        {
            Position += offset;
        }

        /// <summary>
        /// Smoothly move camera toward target position
        /// </summary>
        public void MoveTowards(Vector2 target, float speed, float deltaTime)
        {
            var direction = target - Position;
            var distance = direction.Length();
            
            if (distance > 0.1f)
            {
                var moveDistance = Math.Min(speed * deltaTime, distance);
                Position += Vector2.Normalize(direction) * moveDistance;
            }
        }

        /// <summary>
        /// Zoom by factor (1.1 = zoom in 10%, 0.9 = zoom out 10%)
        /// </summary>
        public void ZoomBy(float factor)
        {
            Zoom *= factor;
        }

        /// <summary>
        /// Convert screen coordinates to world coordinates
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            // Convert from screen space (0,0 top-left) to NDC (-1,-1 bottom-left)
            var ndc = new Vector2(
                (screenPos.X / _viewportWidth) * 2.0f - 1.0f,
                1.0f - (screenPos.Y / _viewportHeight) * 2.0f
            );

            // Apply inverse view-projection transform
            if (!Matrix4x4.Invert(ViewProjectionMatrix, out var invViewProj))
                return Vector2.Zero;

            var worldPos4 = Vector4.Transform(new Vector4(ndc, 0, 1), invViewProj);
            return new Vector2(worldPos4.X, worldPos4.Y);
        }

        /// <summary>
        /// Convert world coordinates to screen coordinates
        /// </summary>
        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            var worldPos4 = new Vector4(worldPos, 0, 1);
            var screenPos4 = Vector4.Transform(worldPos4, ViewProjectionMatrix);
            
            // Convert from NDC to screen space
            return new Vector2(
                (_viewportWidth * (screenPos4.X + 1.0f)) * 0.5f,
                (_viewportHeight * (1.0f - screenPos4.Y)) * 0.5f
            );
        }

        /// <summary>
        /// Get the camera's view bounds in world space
        /// </summary>
        public (Vector2 min, Vector2 max) GetViewBounds()
        {
            var halfWidth = _viewportWidth / (2.0f * Zoom);
            var halfHeight = _viewportHeight / (2.0f * Zoom);
            
            return (
                new Vector2(Position.X - halfWidth, Position.Y - halfHeight),
                new Vector2(Position.X + halfWidth, Position.Y + halfHeight)
            );
        }

        /// <summary>
        /// Check if a point is visible by the camera
        /// </summary>
        public bool IsPointVisible(Vector2 point, float margin = 0.0f)
        {
            var (min, max) = GetViewBounds();
            return point.X >= min.X - margin && point.X <= max.X + margin &&
                   point.Y >= min.Y - margin && point.Y <= max.Y + margin;
        }

        /// <summary>
        /// Check if a rectangle is visible by the camera
        /// </summary>
        public bool IsRectVisible(Vector2 center, Vector2 size)
        {
            var (viewMin, viewMax) = GetViewBounds();
            var halfSize = size * 0.5f;
            
            var rectMin = center - halfSize;
            var rectMax = center + halfSize;
            
            return rectMax.X >= viewMin.X && rectMin.X <= viewMax.X &&
                   rectMax.Y >= viewMin.Y && rectMin.Y <= viewMax.Y;
        }

        private Matrix4x4 GetViewMatrix()
        {
            if (_matricesDirty)
                UpdateMatrices();
            return _viewMatrix;
        }

        private Matrix4x4 GetProjectionMatrix()
        {
            if (_matricesDirty)
                UpdateMatrices();
            return _projectionMatrix;
        }

        private void UpdateMatrices()
        {
            // Create view matrix (inverse of camera transform)
            var translation = Matrix4x4.CreateTranslation(-_position.X, -_position.Y, 0);
            var rotation = Matrix4x4.CreateRotationZ(-_rotation);
            var scale = Matrix4x4.CreateScale(_zoom, _zoom, 1.0f);
            
            _viewMatrix = translation * rotation * scale;

            // Create orthographic projection matrix
            var halfWidth = _viewportWidth * 0.5f;
            var halfHeight = _viewportHeight * 0.5f;
            
            _projectionMatrix = Matrix4x4.CreateOrthographic(
                _viewportWidth, _viewportHeight, -100.0f, 100.0f);

            _matricesDirty = false;
        }
    }
}