// Core/ECS/EntityManager.cs
using System;
using System.Collections.Generic;

namespace BounceGame.Core.ECS
{
    /// <summary>
    /// Manages entity creation, destruction and lifecycle
    /// </summary>
    public class EntityManager
    {
        private uint _nextEntityId = 1; // Start from 1, 0 is reserved for Invalid
        private readonly Queue<uint> _recycledIds = new Queue<uint>();
        private readonly HashSet<uint> _activeEntities = new HashSet<uint>();
        
        public int ActiveEntityCount => _activeEntities.Count;
        
        /// <summary>
        /// Creates a new entity
        /// </summary>
        public Entity CreateEntity()
        {
            uint id;
            
            if (_recycledIds.Count > 0)
            {
                id = _recycledIds.Dequeue();
            }
            else
            {
                id = _nextEntityId++;
            }
            
            _activeEntities.Add(id);
            return new Entity(id);
        }
        
        /// <summary>
        /// Destroys an entity (will be recycled)
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            if (!IsEntityActive(entity))
                throw new ArgumentException($"Entity {entity} is not active");
                
            _activeEntities.Remove(entity.ID);
            _recycledIds.Enqueue(entity.ID);
        }
        
        /// <summary>
        /// Checks if entity is currently active
        /// </summary>
        public bool IsEntityActive(Entity entity)
        {
            return _activeEntities.Contains(entity.ID);
        }
        
        /// <summary>
        /// Gets all active entities
        /// </summary>
        public IEnumerable<Entity> GetActiveEntities()
        {
            foreach (var id in _activeEntities)
            {
                yield return new Entity(id);
            }
        }
    }
}