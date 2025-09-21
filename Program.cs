// Program.cs - Advanced Rendering Demo
using System;
using System.Numerics;
using BounceGame.Core.Game;
using BounceGame.Core.ECS;
using BounceGame.Core.Graphics;
using BounceGame.Core.Resources;
using BounceGame.Components;
using BounceGame.Systems;

namespace BounceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Advanced Rendering Demo ===\n");
            
            // Create the advanced game manager
            using var game = new AdvancedGameManager("Bounce Game - Advanced Rendering", 1024, 768);
            
            Console.WriteLine("Advanced rendering system initialized!");
            Console.WriteLine("Controls:");
            Console.WriteLine("  WASD - Move camera");
            Console.WriteLine("  Q/E - Zoom in/out");
            Console.WriteLine("  SPACE - Create explosion effect");
            Console.WriteLine("  1/2/3 - Switch between different effects");
            Console.WriteLine("  R - Reset camera");
            Console.WriteLine("  ESC - Exit");
            Console.WriteLine("\nStarting advanced demo...\n");
            
            // Setup the demo world
            SetupAdvancedWorld(game);
            
            // Game loop timing
            var lastTime = DateTime.Now;
            int frameCount = 0;
            var fpsTimer = DateTime.Now;
            var effectTimer = DateTime.Now;
            
            // Main game loop
            while (!game.ShouldClose)
            {
                var currentTime = DateTime.Now;
                float deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;
                
                // Handle input
                HandleAdvancedInput(game, deltaTime);
                
                // Update game systems
                game.Update(deltaTime);
                
                // Add periodic effects
                if ((DateTime.Now - effectTimer).TotalSeconds >= 3.0)
                {
                    CreateRandomEffect(game);
                    effectTimer = DateTime.Now;
                }
                
                // Render everything
                game.Render();
                
                // Performance monitoring
                frameCount++;
                if ((DateTime.Now - fpsTimer).TotalSeconds >= 2.0)
                {
                    Console.WriteLine($"FPS: {frameCount / 2}");
                    Console.WriteLine(game.GetAdvancedStats());
                    Console.WriteLine(); // Empty line
                    
                    frameCount = 0;
                    fpsTimer = DateTime.Now;
                }
            }
            
            Console.WriteLine("Advanced demo shutting down...");
        }
        
        static void SetupAdvancedWorld(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            
            Console.WriteLine("Setting up advanced demo world...");
            
            // Create some sprite sheets for testing
            SetupSpriteSheets(textureManager);
            
            // Create textured background
            CreateTexturedBackground(game);
            
            // Create animated entities
            CreateAnimatedEntities(game);
            
            // Create initial particle effects
            CreateInitialEffects(game);
            
            // Create a player entity with camera following
            CreateCameraFollowPlayer(game);
            
            Console.WriteLine($"Advanced world setup complete! Entities: {world.EntityCount}");
        }
        
        static void SetupSpriteSheets(TextureManager textureManager)
        {
            // Create a test sprite sheet from checkerboard texture
            var spriteSheet = textureManager.CreateSpriteSheet("test_sprites", "checkerboard");
            
            // Add frames in a 2x2 grid (each frame is 32x32 from 64x64 texture)
            spriteSheet.AddFrame("frame_0", 0, 0, 32, 32);
            spriteSheet.AddFrame("frame_1", 32, 0, 32, 32);
            spriteSheet.AddFrame("frame_2", 0, 32, 32, 32);
            spriteSheet.AddFrame("frame_3", 32, 32, 32, 32);
            
            Console.WriteLine("Sprite sheets created");
        }
        
        static void CreateTexturedBackground(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            
            // Create background tiles using different textures
            for (int x = -1000; x <= 1000; x += 100)
            {
                for (int y = -800; y <= 800; y += 100)
                {
                    // Vary the background colors
                    var distance = MathF.Sqrt(x * x + y * y);
                    var hue = (distance * 0.001f) % 1.0f;
                    var color = HSVToRGB(hue, 0.3f, 0.8f);
                    
                    var entity = game.CreateSpriteEntity(
                        new Vector2(x, y),
                        new Vector2(80, 80),
                        new Vector4(color.X, color.Y, color.Z, 0.6f), // Semi-transparent
                        layer: -2 // Far background
                    );
                }
            }
        }
        
        static void CreateAnimatedEntities(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            var spriteSheet = textureManager.GetSpriteSheet("test_sprites");
            
            // Create some animated sprites
            for (int i = 0; i < 5; i++)
            {
                var entity = world.CreateEntity();
                
                var position = new Vector2(
                    (i - 2) * 200,
                    100
                );
                
                world.AddComponent(entity, new Transform(new Vector3(position, 0)));
                
                // Add animated sprite component
                var animSprite = new AnimatedSprite();
                animSprite.SpriteSheet = spriteSheet;
                animSprite.AddAnimation("spin", 0, 3, 5.0f + i, true); // Different speeds
                animSprite.PlayAnimation("spin");
                
                world.AddComponent(entity, animSprite);
                
                // Also add regular sprite for size/color info
                world.AddComponent(entity, new Sprite(
                    new Vector2(64, 64),
                    new Vector4(1.0f, 0.8f, 0.6f, 1.0f),
                    layer: 1
                ));
                
                // Add some physics for movement
                var rigidbody = new Rigidbody(1.0f);
                rigidbody.Velocity = new Vector3(
                    (i - 2) * 30, // Horizontal spread
                    50 + i * 20,  // Vertical variation
                    0
                );
                world.AddComponent(entity, rigidbody);
            }
            
            Console.WriteLine("Animated entities created");
        }
        
        static void CreateInitialEffects(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            
            // Create a fire effect
            var fireEntity = world.CreateEntity();
            world.AddComponent(fireEntity, new Transform(new Vector3(-300, -200, 0)));
            
            var fireSystem = new ParticleSystem(ParticleConfig.Fire())
            {
                EmissionRate = 15.0f,
                MaxParticles = 50
            };
            world.AddComponent(fireEntity, fireSystem);
            
            // Create a smoke effect
            var smokeEntity = world.CreateEntity();
            world.AddComponent(smokeEntity, new Transform(new Vector3(300, -200, 0)));
            
            var smokeSystem = new ParticleSystem(ParticleConfig.Smoke())
            {
                EmissionRate = 8.0f,
                MaxParticles = 30
            };
            world.AddComponent(smokeEntity, smokeSystem);
            
            Console.WriteLine("Initial particle effects created");
        }
        
        static void CreateCameraFollowPlayer(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            
            // Create player entity
            var player = game.CreatePhysicsEntity(
                new Vector2(0, 0),
                new Vector2(40, 40),
                new Vector4(1.0f, 0.3f, 0.3f, 1.0f), // Red
                mass: 1.0f
            );
            
            world.AddComponent(player, new PlayerController(400.0f)); // Faster movement
            world.AddComponent(player, new CameraTarget()); // Mark for camera following
            
            Console.WriteLine("Camera follow player created");
        }
        
        static void HandleAdvancedInput(AdvancedGameManager game, float deltaTime)
        {
            var camera = game.GetCamera();
            
            // Camera controls
            float cameraSpeed = 300.0f;
            Vector2 cameraMove = Vector2.Zero;
            
            if (game.IsWPressed()) cameraMove.Y += cameraSpeed * deltaTime;
            if (game.IsSPressed()) cameraMove.Y -= cameraSpeed * deltaTime;
            if (game.IsAPressed()) cameraMove.X -= cameraSpeed * deltaTime;
            if (game.IsDPressed()) cameraMove.X += cameraSpeed * deltaTime;
            
            if (cameraMove != Vector2.Zero)
            {
                camera.Move(cameraMove);
            }
            
            // Zoom controls
            if (game.IsKeyPressed(0x51)) // Q key
                camera.ZoomBy(1.02f);
            if (game.IsKeyPressed(0x45)) // E key
                camera.ZoomBy(0.98f);
            
            // Reset camera
            if (game.IsKeyPressed(0x52)) // R key
            {
                camera.Position = Vector2.Zero;
                camera.Zoom = 1.0f;
                Console.WriteLine("Camera reset");
            }
            
            // Create explosion effect with SPACE
            if (game.IsSpaceJustPressed())
            {
                CreateExplosionAt(game, camera.Position);
            }
            
            // Effect switching with number keys
            if (game.IsKeyJustPressed(0x31)) // 1 key
                CreateEffectType(game, 1);
            if (game.IsKeyJustPressed(0x32)) // 2 key
                CreateEffectType(game, 2);
            if (game.IsKeyJustPressed(0x33)) // 3 key
                CreateEffectType(game, 3);
        }
        
        static void CreateExplosionAt(AdvancedGameManager game, Vector2 position)
        {
            var world = game.GetWorld();
            
            var explosionEntity = world.CreateEntity();
            world.AddComponent(explosionEntity, new Transform(new Vector3(position, 0)));
            
            var explosionSystem = new ParticleSystem(ParticleConfig.Explosion())
            {
                IsEmitting = false, // One-time burst
                AutoDestroy = true,
                MaxParticles = 100
            };
            
            world.AddComponent(explosionEntity, explosionSystem);
            
            // Emit burst immediately
            explosionSystem.EmitBurst(50);
            
            Console.WriteLine($"Explosion created at ({position.X:F0}, {position.Y:F0})");
        }
        
        static void CreateEffectType(AdvancedGameManager game, int type)
        {
            var world = game.GetWorld();
            var camera = game.GetCamera();
            var position = camera.Position + new Vector2(
                (new Random().NextSingle() - 0.5f) * 200,
                (new Random().NextSingle() - 0.5f) * 200
            );
            
            var entity = world.CreateEntity();
            world.AddComponent(entity, new Transform(new Vector3(position, 0)));
            
            ParticleSystem particleSystem = type switch
            {
                1 => new ParticleSystem(ParticleConfig.Fire()) { EmissionRate = 20.0f, MaxParticles = 60 },
                2 => new ParticleSystem(ParticleConfig.Smoke()) { EmissionRate = 12.0f, MaxParticles = 40 },
                3 => new ParticleSystem(ParticleConfig.Explosion()) { IsEmitting = false, AutoDestroy = true },
                _ => new ParticleSystem(ParticleConfig.Fire())
            };
            
            world.AddComponent(entity, particleSystem);
            
            if (type == 3)
            {
                particleSystem.EmitBurst(30);
            }
            
            Console.WriteLine($"Effect type {type} created at ({position.X:F0}, {position.Y:F0})");
        }
        
        static void CreateRandomEffect(AdvancedGameManager game)
        {
            var random = new Random();
            var effectType = random.Next(1, 4);
            CreateEffectType(game, effectType);
        }
        
        // Helper function to convert HSV to RGB
        static Vector3 HSVToRGB(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - MathF.Abs((h * 6) % 2 - 1));
            float m = v - c;
            
            Vector3 rgb = (h * 6) switch
            {
                >= 0 and < 1 => new Vector3(c, x, 0),
                >= 1 and < 2 => new Vector3(x, c, 0),
                >= 2 and < 3 => new Vector3(0, c, x),
                >= 3 and < 4 => new Vector3(0, x, c),
                >= 4 and < 5 => new Vector3(x, 0, c),
                _ => new Vector3(c, 0, x)
            };
            
            return rgb + new Vector3(m, m, m);
        }
    }
}
