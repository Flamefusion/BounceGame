// Program.cs - Debug Diagnostic Version
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
            Console.WriteLine("=== ECS + OpenGL Debug Diagnostic ===\n");
            
            using var game = new GameManager("Bounce Game - Debug Mode", 1024, 768);
            
            Console.WriteLine("GameManager created successfully");
            
            // Add systems
            var movementSystem = new MovementSystem(game.GetWorld());
            game.AddSystem(movementSystem);
            
            Console.WriteLine("MovementSystem added");
            
            // Create test entities with detailed logging
            SetupDebugWorld(game);
            
            // Game loop timing
            var lastTime = DateTime.Now;
            int frameCount = 0;
            var fpsTimer = DateTime.Now;
            var debugTimer = DateTime.Now;
            
            Console.WriteLine("\nStarting main game loop...");
            
            // Main game loop
            while (!game.ShouldClose)
            {
                // Calculate delta time
                var currentTime = DateTime.Now;
                float deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;
                
                // Update game logic
                game.Update(deltaTime);
                
                // Handle input with debug info
                HandleDebugInput(game);
                
                // Render everything
                game.Render();
                
                // Debug info every few seconds
                if ((DateTime.Now - debugTimer).TotalSeconds >= 3.0)
                {
                    PrintDetailedDebugInfo(game);
                    debugTimer = DateTime.Now;
                }
                
                // FPS counter
                frameCount++;
                if ((DateTime.Now - fpsTimer).TotalSeconds >= 1.0)
                {
                    Console.WriteLine($"FPS: {frameCount} | Entities: {game.TotalEntities}");
                    frameCount = 0;
                    fpsTimer = DateTime.Now;
                }
            }
            
            Console.WriteLine("Game loop ended, shutting down...");
        }
        
        static void SetupDebugWorld(GameManager game)
        {
            Console.WriteLine("\n--- Setting up debug world ---");
            var world = game.GetWorld();
            
            // Create a simple test entity first
            Console.WriteLine("Creating test entity at (0, 0)...");
            var testEntity = game.CreateSpriteEntity(
                position: new Vector2(0, 0),
                size: new Vector2(100, 100),
                color: new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Bright red
                layer: 0
            );
            
            Console.WriteLine($"Test entity created: {testEntity}");
            
            // Verify components were added
            if (world.HasComponent<Transform>(testEntity))
            {
                var transform = world.GetComponent<Transform>(testEntity);
                Console.WriteLine($"  Transform: {transform}");
            }
            else
            {
                Console.WriteLine("  ERROR: Test entity missing Transform component!");
            }
            
            if (world.HasComponent<Sprite>(testEntity))
            {
                var sprite = world.GetComponent<Sprite>(testEntity);
                Console.WriteLine($"  Sprite: {sprite}");
            }
            else
            {
                Console.WriteLine("  ERROR: Test entity missing Sprite component!");
            }
            
            // Create player entity
            Console.WriteLine("Creating player entity...");
            var player = game.CreatePhysicsEntity(
                position: new Vector2(100, 100),
                size: new Vector2(50, 50),
                color: new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // Bright green
                mass: 1.0f
            );
            
            world.AddComponent(player, new PlayerController(speed: 300.0f));
            Console.WriteLine($"Player entity created: {player}");
            
            // Create some corner entities to test coordinate system
            Console.WriteLine("Creating corner test entities...");
            
            // Top-left
            var topLeft = game.CreateSpriteEntity(
                position: new Vector2(-400, 300),
                size: new Vector2(50, 50),
                color: new Vector4(1.0f, 1.0f, 0.0f, 1.0f) // Yellow
            );
            Console.WriteLine($"Top-left entity: {topLeft}");
            
            // Bottom-right  
            var bottomRight = game.CreateSpriteEntity(
                position: new Vector2(400, -300),
                size: new Vector2(50, 50),
                color: new Vector4(0.0f, 1.0f, 1.0f, 1.0f) // Cyan
            );
            Console.WriteLine($"Bottom-right entity: {bottomRight}");
            
            Console.WriteLine($"Total entities created: {world.EntityCount}");
            Console.WriteLine("--- Debug world setup complete ---\n");
        }
        
        static void HandleDebugInput(GameManager game)
        {
            if (game.IsSpaceJustPressed())
            {
                Console.WriteLine("SPACE pressed - Creating new entity...");
                
                var random = new Random();
                var position = new Vector2(
                    random.Next(-200, 201),
                    random.Next(-100, 101)
                );
                
                var entity = game.CreatePhysicsEntity(
                    position: position,
                    size: new Vector2(30, 30),
                    color: new Vector4(1.0f, 0.5f, 0.0f, 1.0f) // Orange
                );
                
                Console.WriteLine($"New entity created: {entity} at position {position}");
                Console.WriteLine($"Total entities now: {game.TotalEntities}");
            }
        }
        
        static void PrintDetailedDebugInfo(GameManager game)
        {
            Console.WriteLine("\n=== DETAILED DEBUG INFO ===");
            
            var world = game.GetWorld();
            Console.WriteLine($"Total entities: {world.EntityCount}");
            
            // Check entities with Transform + Sprite
            var renderableEntities = world.GetEntitiesWith<Transform, Sprite>();
            int renderableCount = 0;
            
            Console.WriteLine("Entities with Transform + Sprite:");
            foreach (var entity in renderableEntities)
            {
                var transform = world.GetComponent<Transform>(entity);
                var sprite = world.GetComponent<Sprite>(entity);
                
                Console.WriteLine($"  {entity}: Pos=({transform.Position.X:F1}, {transform.Position.Y:F1}), " +
                                $"Size=({sprite.Size.X:F1}, {sprite.Size.Y:F1}), " +
                                $"Color=({sprite.Color.X:F1}, {sprite.Color.Y:F1}, {sprite.Color.Z:F1}, {sprite.Color.W:F1}), " +
                                $"Visible={sprite.Visible}, Layer={sprite.Layer}");
                renderableCount++;
            }
            
            Console.WriteLine($"Total renderable entities: {renderableCount}");
            Console.WriteLine(game.GetDebugInfo());
            Console.WriteLine("=== END DEBUG INFO ===\n");
        }
    }
}