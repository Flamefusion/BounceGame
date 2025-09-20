// Systems/ISystem.cs
using BounceGame.Core.ECS;

namespace BounceGame.Systems
{
    /// <summary>
    /// Base interface for all systems in the ECS
    /// </summary>
    public interface ISystem
    {
        void Update(float deltaTime);
    }
    
    /// <summary>
    /// Interface for systems that need initialization
    /// </summary>
    public interface IInitializableSystem : ISystem
    {
        void Initialize();
    }
    
    /// <summary>
    /// Interface for systems that handle rendering
    /// </summary>
    public interface IRenderSystem
    {
        void Render();
    }
}