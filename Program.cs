// Program.cs - ECS + OpenGL Integration Demo
using System;
using System.Numerics;
using BounceGame.Core.Game;
using BounceGame.Core.ECS;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ECS + OpenGL Integration Demo ===\n");
            
            using var game = new GameManager("Bounce Game - ECS Integration", 1024, 768);
            
            // Add systems to the game
            var movementSystem = new MovementSystem(game.GetWorld());
            game.AddSystem(movementSystem);
            
            Console.WriteLine("Game initialized with ECS + OpenGL integration");
            Console.WriteLine("Controls:");
            Console.WriteLine("  WASD - Move player (red square)");
            Console.WriteLine("  SPACE - Add new entity");
            Console.WriteLine("  ESC - Exit");
            Console.WriteLine("\nStarting game loop...\n");
            
            // Create initial entities
            SetupGameWorld(game);
            
            // Game loop timing
            var lastTime = DateTime.Now;
            int frameCount = 0;
            var fpsTimer = DateTime.Now;
            
            // Main game loop
            while (!game.ShouldClose)
            {
                // Calculate delta time
                var currentTime = DateTime.Now;
                float deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;
                
                // Update game logic
                game.Update(deltaTime);
                
                // Handle input
                HandleGameInput(game);
                
                // Render everything
                game.Render();
                
                // FPS counter and debug info
                frameCount++;
                if ((DateTime.Now - fpsTimer).TotalSeconds >= 1.0)
                {
                    Console.WriteLine($"FPS: {frameCount}");
                    Console.WriteLine(game.GetDebugInfo());
                    Console.WriteLine(); // Empty line for readability
                    
                    frameCount = 0;
                    fpsTimer = DateTime.Now;
                }
            }
            
            Console.WriteLine("Game shutting down gracefully...");
        }
        
        static void SetupGameWorld(GameManager game)
        {
            var world = game.GetWorld();
            
            // Create player entity (red, controllable)
            var player = game.CreatePhysicsEntity(
                position: new Vector2(0, 0),
                size: new Vector2(50, 50),
                color: new Vector4(1.0f, 0.3f, 0.3f, 1.0f), // Red
                mass: 1.0f
            );
            
            // Tag the player for the movement system
            world.AddComponent(player, new PlayerController(speed: 300.0f));
            
            Console.WriteLine($"Player entity created: {player}");
            
            // Create some background entities
            CreateBackground(game);
            
            // Create some dynamic entities
            CreateDynamicEntities(game);
            
            Console.WriteLine($"Game world setup complete. Total entities: {world.EntityCount}");
        }
        
        static void CreateBackground(GameManager game)
        {
            // Create a grid of background squares
            var gridColor = new Vector4(0.2f, 0.2f, 0.4f, 1.0f); // Dark blue
            var gridSize = new Vector2(30, 30);
            
            for (int x = -400; x <= 400; x += 100)
            {
                for (int y = -300; y <= 300; y += 100)
                {
                    game.CreateSpriteEntity(
                        position: new Vector2(x, y),
                        size: gridSize,
                        color: gridColor,
                        layer: -1 // Behind everything
                    );
                }
            }
            
            // Create border entities
            var borderColor = new Vector4(0.5f, 0.5f, 0.6f, 1.0f);
            var borderThickness = 20.0f;
            
            // Top and bottom borders
            game.CreateSpriteEntity(new Vector2(0, 384), new Vector2(1024, borderThickness), borderColor, layer: 1);
            game.CreateSpriteEntity(new Vector2(0, -384), new Vector2(1024, borderThickness), borderColor, layer: 1);
            
            // Left and right borders
            game.CreateSpriteEntity(new Vector2(-512, 0), new Vector2(borderThickness, 768), borderColor, layer: 1);
            game.CreateSpriteEntity(new Vector2(512, 0), new Vector2(borderThickness, 768), borderColor, layer: 1);
        }
        
        static void CreateDynamicEntities(GameManager game)
        {
            var random = new Random();
            
            // Create some colorful entities at random positions
            var colors = new Vector4[]
            {
                new Vector4(0.3f, 1.0f, 0.3f, 1.0f), // Green
                new Vector4(0.3f, 0.3f, 1.0f, 1.0f), // Blue
                new Vector4(1.0f, 1.0f, 0.3f, 1.0f), // Yellow
                new Vector4(1.0f, 0.3f, 1.0f, 1.0f), // Magenta
                new Vector4(0.3f, 1.0f, 1.0f, 1.0f), // Cyan
            };
            
            for (int i = 0; i < 10; i++)
            {
                var position = new Vector2(
                    random.Next(-300, 301),
                    random.Next(-200, 201)
                );
                
                var size = new Vector2(
                    random.Next(20, 61),
                    random.Next(20, 61)
                );
                
                var color = colors[random.Next(colors.Length)];
                
                // Some entities have physics, some don't
                if (i < 5)
                {
                    var entity = game.CreatePhysicsEntity(position, size, color, mass: 0.5f);
                    
                    // Add some initial velocity
                    var world = game.GetWorld();
                    var rigidbody = world.GetComponent<Rigidbody>(entity);
                    rigidbody.Velocity = new Vector3(
                        (random.NextSingle() - 0.5f) * 100,
                        (random.NextSingle() - 0.5f) * 100,
                        0
                    );
                }
                else
                {
                    game.CreateSpriteEntity(position, size, color, layer: 0);
                }
            }
        }
        
        static void HandleGameInput(GameManager game)
        {
            // Handle adding new entities with space
            if (game.IsSpaceJustPressed())
            {
                var random = new Random();
                var position = new Vector2(
                    random.Next(-200, 201),
                    random.Next(-100, 101)
                );
                
                var colors = new Vector4[]
                {
                    new Vector4(1.0f, 0.5f, 0.5f, 1.0f),
                    new Vector4(0.5f, 1.0f, 0.5f, 1.0f),
                    new Vector4(0.5f, 0.5f, 1.0f, 1.0f),
                };
                
                var color = colors[random.Next(colors.Length)];
                
                game.CreatePhysicsEntity(
                    position: position,
                    size: new Vector2(25, 25),
                    color: color,
                    mass: 0.3f
                );
                
                Console.WriteLine($"New entity created at {position}. Total: {game.TotalEntities}");
            }
        }
    }
}
