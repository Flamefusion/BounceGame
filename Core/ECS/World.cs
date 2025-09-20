// Core/ECS/World.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace BounceGame.Core.ECS
{
    /// <summary>
    /// Central ECS coordinator - manages entities, components, and queries
    /// </summary>
    public class World
    {
        private readonly EntityManager _entityManager = new EntityManager();
        private readonly Dictionary<Type, object> _componentManagers = new Dictionary<Type, object>();
        
        public int EntityCount => _entityManager.ActiveEntityCount;
        
        #region Entity Management
        
        /// <summary>
        /// Creates a new entity
        /// </summary>
        public Entity CreateEntity()
        {
            return _entityManager.CreateEntity();
        }
        
        /// <summary>
        /// Destroys an entity and removes all its components
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            // Remove components from all managers
            foreach (var manager in _componentManagers.Values)
            {
                var removeMethod = manager.GetType().GetMethod("RemoveComponent");
                removeMethod?.Invoke(manager, new object[] { entity });
            }
            
            _entityManager.DestroyEntity(entity);
        }
        
        /// <summary>
        /// Checks if entity exists and is active
        /// </summary>
        public bool IsEntityActive(Entity entity)
        {
            return _entityManager.IsEntityActive(entity);
        }
        
        #endregion
        
        #region Component Management
        
        /// <summary>
        /// Gets or creates component manager for type T
        /// </summary>
        private ComponentManager<T> GetComponentManager<T>() where T : class, IComponent
        {
            var type = typeof(T);
            if (!_componentManagers.TryGetValue(type, out object manager))
            {
                manager = new ComponentManager<T>();
                _componentManagers[type] = manager;
            }
            return (ComponentManager<T>)manager;
        }
        
        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        public void AddComponent<T>(Entity entity, T component) where T : class, IComponent
        {
            if (!IsEntityActive(entity))
                throw new ArgumentException($"Entity {entity} is not active");
                
            GetComponentManager<T>().AddComponent(entity, component);
        }
        
        /// <summary>
        /// Removes a component from an entity
        /// </summary>
        public bool RemoveComponent<T>(Entity entity) where T : class, IComponent
        {
            return GetComponentManager<T>().RemoveComponent(entity);
        }
        
        /// <summary>
        /// Gets a component from an entity
        /// </summary>
        public T GetComponent<T>(Entity entity) where T : class, IComponent
        {
            return GetComponentManager<T>().GetComponent(entity);
        }
        
        /// <summary>
        /// Tries to get a component (safe version)
        /// </summary>
        public bool TryGetComponent<T>(Entity entity, out T component) where T : class, IComponent
        {
            return GetComponentManager<T>().TryGetComponent(entity, out component);
        }
        
        /// <summary>
        /// Checks if entity has component of type T
        /// </summary>
        public bool HasComponent<T>(Entity entity) where T : class, IComponent
        {
            return GetComponentManager<T>().HasComponent(entity);
        }
        
        #endregion
        
        #region Queries
        
        /// <summary>
        /// Gets all entities that have component T
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T>() where T : class, IComponent
        {
            return GetComponentManager<T>().GetEntitiesWithComponent();
        }
        
        /// <summary>
        /// Gets all entities that have both components T1 and T2
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T1, T2>() 
            where T1 : class, IComponent 
            where T2 : class, IComponent
        {
            var manager1 = GetComponentManager<T1>();
            var manager2 = GetComponentManager<T2>();
            
            return manager1.GetEntitiesWithComponent()
                          .Where(entity => manager2.HasComponent(entity));
        }
        
        /// <summary>
        /// Gets all entities that have components T1, T2, and T3
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T1, T2, T3>()
            where T1 : class, IComponent 
            where T2 : class, IComponent
            where T3 : class, IComponent
        {
            var manager1 = GetComponentManager<T1>();
            var manager2 = GetComponentManager<T2>();
            var manager3 = GetComponentManager<T3>();
            
            return manager1.GetEntitiesWithComponent()
                          .Where(entity => manager2.HasComponent(entity) && 
                                         manager3.HasComponent(entity));
        }
        
        #endregion
        
        #region Maintenance
        
        /// <summary>
        /// Cleans up components for destroyed entities
        /// Should be called periodically to prevent memory leaks
        /// </summary>
        public void CleanupDestroyedEntities()
        {
            var activeEntities = _entityManager.GetActiveEntities();
            
            foreach (var manager in _componentManagers.Values)
            {
                var cleanupMethod = manager.GetType().GetMethod("CleanupDestroyedEntities");
                cleanupMethod?.Invoke(manager, new object[] { activeEntities });
            }
        }
        
        /// <summary>
        /// Gets debug information about the world state
        /// </summary>
        public string GetDebugInfo()
        {
            var info = $"World Debug Info:\n";
            info += $"  Active Entities: {EntityCount}\n";
            info += $"  Component Managers: {_componentManagers.Count}\n";
            
            foreach (var kvp in _componentManagers)
            {
                var componentCount = kvp.Value.GetType().GetProperty("ComponentCount")?.GetValue(kvp.Value);
                info += $"    {kvp.Key.Name}: {componentCount} components\n";
            }
            
            return info;
        }
        
        #endregion
    }
}