// Core/ECS/ComponentManager.cs
using System;
using System.Collections.Generic;

namespace BounceGame.Core.ECS
{
    /// <summary>
    /// Manages storage and access for a specific component type
    /// </summary>
    public class ComponentManager<T> where T : class, IComponent
    {
        private readonly Dictionary<uint, T> _components = new Dictionary<uint, T>();
        
        public int ComponentCount => _components.Count;
        
        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        public void AddComponent(Entity entity, T component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
                
            if (_components.ContainsKey(entity.ID))
                throw new InvalidOperationException($"Entity {entity} already has component {typeof(T).Name}");
                
            _components[entity.ID] = component;
        }
        
        /// <summary>
        /// Removes a component from an entity
        /// </summary>
        public bool RemoveComponent(Entity entity)
        {
            return _components.Remove(entity.ID);
        }
        
        /// <summary>
        /// Gets a component from an entity
        /// </summary>
        public T GetComponent(Entity entity)
        {
            if (_components.TryGetValue(entity.ID, out T? component) && component != null)
                return component;
            
            throw new InvalidOperationException($"Entity {entity} does not have component {typeof(T).Name}");
        }
        
        /// <summary>
        /// Tries to get a component (safe version)
        /// </summary>
        public bool TryGetComponent(Entity entity, out T? component)
        {
            return _components.TryGetValue(entity.ID, out component);
        }
        
        /// <summary>
        /// Checks if entity has this component type
        /// </summary>
        public bool HasComponent(Entity entity)
        {
            return _components.ContainsKey(entity.ID);
        }
        
        /// <summary>
        /// Gets all entities that have this component
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWithComponent()
        {
            foreach (var entityId in _components.Keys)
            {
                yield return new Entity(entityId);
            }
        }
        
        /// <summary>
        /// Gets all components of this type
        /// </summary>
        public IEnumerable<T> GetAllComponents()
        {
            return _components.Values;
        }
        
        /// <summary>
        /// Gets all entity-component pairs
        /// </summary>
        public IEnumerable<(Entity entity, T component)> GetEntityComponentPairs()
        {
            foreach (var kvp in _components)
            {
                yield return (new Entity(kvp.Key), kvp.Value);
            }
        }
        
        /// <summary>
        /// Cleans up components for destroyed entities
        /// </summary>
        public void CleanupDestroyedEntities(IEnumerable<Entity> activeEntities)
        {
            var activeIds = new HashSet<uint>();
            foreach (var entity in activeEntities)
            {
                activeIds.Add(entity.ID);
            }
            
            var toRemove = new List<uint>();
            foreach (var entityId in _components.Keys)
            {
                if (!activeIds.Contains(entityId))
                {
                    toRemove.Add(entityId);
                }
            }
            
            foreach (var entityId in toRemove)
            {
                _components.Remove(entityId);
            }
        }
    }
}
