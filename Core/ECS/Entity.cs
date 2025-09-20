// Core/ECS/Entity.cs
using System;

namespace BounceGame.Core.ECS
{
    /// <summary>
    /// Entity is just a unique identifier
    /// </summary>
    public struct Entity : IEquatable<Entity>
    {
        public readonly uint ID;
        
        public Entity(uint id)
        {
            ID = id;
        }
        
        public bool Equals(Entity other) => ID == other.ID;
        public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);
        public override int GetHashCode() => ID.GetHashCode();
        public override string ToString() => $"Entity({ID})";
        
        public static bool operator ==(Entity left, Entity right) => left.Equals(right);
        public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
        
        public static Entity Invalid => new Entity(0);
        public bool IsValid => ID != 0;
    }
}