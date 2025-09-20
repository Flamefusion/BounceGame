// Components/PlayerController.cs
using BounceGame.Core.ECS;

namespace BounceGame.Components
{
    /// <summary>
    /// Component that marks an entity as player-controllable
    /// </summary>
    public class PlayerController : IComponent
    {
        public float Speed { get; set; }
        public bool CanMove { get; set; }
        
        public PlayerController(float speed = 200.0f)
        {
            Speed = speed;
            CanMove = true;
        }
        
        public override string ToString()
        {
            return $"PlayerController(Speed: {Speed}, CanMove: {CanMove})";
        }
    }
}