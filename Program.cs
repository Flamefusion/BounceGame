// Program.cs - Fixed Advanced Rendering Demo
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
            Console.WriteLine("=== Fixed Advanced Rendering Demo ===\n");
            
            using var game = new AdvancedGameManager("Bounce Game - Advanced Rendering", 1024, 768);
            
            Console.WriteLine("Advanced rendering system initialized!");
            Console.WriteLine("Controls:");
            Console.WriteLine("  WASD - Move camera");
            Console.WriteLine("  Q/E - Zoom in/out");
            Console.WriteLine("  SPACE - Create explosion effect");
            Console.WriteLine("  1/2/3 - Switch between different effects");
            Console.WriteLine("  R - Reset camera");
            Console.WriteLine("  ESC - Exit");
            Console.WriteLine("\nStarting fixed demo...\n");
            
            // Setup the demo world
            SetupFixedWorld(game);
            
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
                
                // Handle input with proper window reference
                HandleFixedInput(game, deltaTime);
                
                // Update game systems
                game.Update(deltaTime);
                
                // Add periodic effects (less frequent)
                if ((DateTime.Now - effectTimer).TotalSeconds >= 5.0) // Every 5 seconds instead of 3
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
            
            Console.WriteLine("Fixed demo shutting down...");
        }
        
        static void SetupFixedWorld(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            
            Console.WriteLine("Setting up fixed demo world...");
            
            // Create sprite sheets for testing
            SetupSpriteSheets(textureManager);
            
            // Create MUCH simpler background
            CreateSimpleBackground(game);
            
            // Create fewer animated entities that are more visible
            CreateVisibleAnimatedEntities(game);
            
            // Create a clear player entity
            CreateVisiblePlayer(game);
            
            // Create fewer initial particle effects
            CreateLimitedEffects(game);
            
            Console.WriteLine($"Fixed world setup complete! Entities: {world.EntityCount}");
        }
        
        static void SetupSpriteSheets(TextureManager textureManager)
        {
            textureManager.LoadTexture("player_sheet", "Resources/player.png");
            var spriteSheet = textureManager.CreateSpriteSheet("player_sprites", "player_sheet");
            
            spriteSheet.AddFrame("player_frame_0", 0, 0, 32, 32);
            spriteSheet.AddFrame("player_frame_1", 32, 0, 32, 32);
            spriteSheet.AddFrame("player_frame_2", 0, 32, 32, 32);
            spriteSheet.AddFrame("player_frame_3", 32, 32, 32, 32);
            
            Console.WriteLine("Sprite sheets created");
        }
        
        static void CreateSimpleBackground(AdvancedGameManager game)
        {
            // Create a simple grid background - much more sparse and subtle
            for (int x = -800; x <= 800; x += 200) // Wider spacing
            {
                for (int y = -600; y <= 600; y += 200) // Wider spacing
                {
                    var entity = game.CreateSpriteEntity(
                        new Vector2(x, y),
                        new Vector2(20, 20), // Much smaller
                        new Vector4(0.2f, 0.2f, 0.3f, 0.3f), // Dark and transparent
                        layer: -10 // Far background
                    );
                }
            }
            
            // Create border markers
            var borderColor = new Vector4(0.4f, 0.4f, 0.5f, 1.0f);
            
            // Corner markers to show world bounds
            game.CreateSpriteEntity(new Vector2(-400, -300), new Vector2(40, 40), borderColor, layer: -1);
            game.CreateSpriteEntity(new Vector2(400, -300), new Vector2(40, 40), borderColor, layer: -1);
            game.CreateSpriteEntity(new Vector2(-400, 300), new Vector2(40, 40), borderColor, layer: -1);
            game.CreateSpriteEntity(new Vector2(400, 300), new Vector2(40, 40), borderColor, layer: -1);
            
            Console.WriteLine("Simple background created");
        }
        
        static void CreateVisibleAnimatedEntities(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            var spriteSheet = textureManager.GetSpriteSheet("player_sprites");
            
            // Create just 3 animated sprites in clear positions
            var positions = new Vector2[]
            {
                new Vector2(-150, 0),   // Left
                new Vector2(0, 100),    // Center-top
                new Vector2(150, 0)     // Right
            };
            
            var colors = new Vector4[]
            {
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), // White
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), // White
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f)  // White
            };
            
            for (int i = 0; i < 3; i++)
            {
                var entity = world.CreateEntity();
                
                world.AddComponent(entity, new Transform(new Vector3(positions[i], 0)));
                
                // Add animated sprite component
                var animSprite = new AnimatedSprite();
                animSprite.SpriteSheet = spriteSheet;
                animSprite.AddAnimation("walk", 0, 3, 3.0f + i, true); // Slower animation
                animSprite.PlayAnimation("walk");
                
                world.AddComponent(entity, animSprite);
                
                // Add sprite for size/color info
                world.AddComponent(entity, new Sprite(
                    new Vector2(80, 80), // Larger and more visible
                    colors[i],
                    layer: 2 // Above background
                ));
            }
            
            Console.WriteLine("Visible animated entities created");
        }
        
        static void CreateVisiblePlayer(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            var textureManager = game.GetTextureManager();
            var spriteSheet = textureManager.GetSpriteSheet("player_sprites");
            
            // Create a bright, large player entity
            var player = game.CreatePhysicsEntity(
                new Vector2(0, -100), // Below center
                new Vector2(60, 60),   // Large and visible
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f), // White, so texture is not tinted
                mass: 1.0f
            );
            
            world.AddComponent(player, new PlayerController(300.0f)); 
            world.AddComponent(player, new CameraTarget()); // Camera will follow this

            // Add an animated sprite to the player
            var animSprite = new AnimatedSprite();
            animSprite.SpriteSheet = spriteSheet;
            animSprite.AddAnimation("idle", 0, 0, 1.0f, true); // idle animation
            animSprite.PlayAnimation("idle");
            world.AddComponent(player, animSprite);
            
            // Modify the sprite layer to be above everything
            var sprite = world.GetComponent<Sprite>(player);
            sprite.Layer = 5; // Top layer
            
            Console.WriteLine("Visible player created");
        }
        
        static void CreateLimitedEffects(AdvancedGameManager game)
        {
            var world = game.GetWorld();
            
            // Create just one fire effect - more visible
            var fireEntity = world.CreateEntity();
            world.AddComponent(fireEntity, new Transform(new Vector3(-200, -150, 0)));
            
            var fireSystem = new ParticleSystem(ParticleConfig.Fire())
            {
                EmissionRate = 10.0f, // Fewer particles
                MaxParticles = 30
            };
            world.AddComponent(fireEntity, fireSystem);
            
            Console.WriteLine("Limited particle effects created");
        }
        
        static void HandleFixedInput(AdvancedGameManager game, float deltaTime)
        {
            var camera = game.GetCamera();
            
            // Camera controls using the game's input methods
            float cameraSpeed = 400.0f; // Faster for more responsive feel
            Vector2 cameraMove = Vector2.Zero;
            
            if (game.IsWPressed()) 
            {
                cameraMove.Y += cameraSpeed * deltaTime;
                Console.WriteLine("W pressed - moving camera up");
            }
            if (game.IsSPressed()) 
            {
                cameraMove.Y -= cameraSpeed * deltaTime;
                Console.WriteLine("S pressed - moving camera down");
            }
            if (game.IsAPressed()) 
            {
                cameraMove.X -= cameraSpeed * deltaTime;
                Console.WriteLine("A pressed - moving camera left");
            }
            if (game.IsDPressed()) 
            {
                cameraMove.X += cameraSpeed * deltaTime;
                Console.WriteLine("D pressed - moving camera right");
            }
            
            if (cameraMove != Vector2.Zero)
            {
                camera.Move(cameraMove);
            }
            
            // Zoom controls
            if (game.IsKeyPressed(0x51)) // Q key
            {
                camera.ZoomBy(1.02f);
                Console.WriteLine($"Q pressed - zooming in, zoom: {camera.Zoom:F2}");
            }
            if (game.IsKeyPressed(0x45)) // E key
            {
                camera.ZoomBy(0.98f);
                Console.WriteLine($"E pressed - zooming out, zoom: {camera.Zoom:F2}");
            }
            
            // Reset camera
            if (game.IsKeyJustPressed(0x52)) // R key
            {
                camera.Position = Vector2.Zero;
                camera.Zoom = 1.0f;
                Console.WriteLine("Camera reset to origin");
            }
            
            // Create explosion effect with SPACE
            if (game.IsSpaceJustPressed())
            {
                CreateExplosionAt(game, camera.Position);
            }
            
            // Effect switching with number keys
            if (game.IsKeyJustPressed(0x31)) // 1 key
            {
                CreateEffectType(game, 1);
                Console.WriteLine("1 pressed - creating fire effect");
            }
            if (game.IsKeyJustPressed(0x32)) // 2 key
            {
                CreateEffectType(game, 2);
                Console.WriteLine("2 pressed - creating smoke effect");
            }
            if (game.IsKeyJustPressed(0x33)) // 3 key
            {
                CreateEffectType(game, 3);
                Console.WriteLine("3 pressed - creating explosion effect");
            }
        }
        
        static void CreateExplosionAt(AdvancedGameManager game, Vector2 position)
        {
            var world = game.GetWorld();
            
            var explosionEntity = world.CreateEntity();
            world.AddComponent(explosionEntity, new Transform(new Vector3(position, 0)));
            
            var explosionSystem = new ParticleSystem(ParticleConfig.Explosion())
            {
                IsEmitting = false,
                AutoDestroy = true,
                MaxParticles = 50 // Fewer particles
            };
            
            world.AddComponent(explosionEntity, explosionSystem);
            explosionSystem.EmitBurst(25); // Smaller burst
            
            Console.WriteLine($"Explosion created at ({position.X:F0}, {position.Y:F0})");
        }
        
        static void CreateEffectType(AdvancedGameManager game, int type)
        {
            var world = game.GetWorld();
            var camera = game.GetCamera();
            var position = camera.Position + new Vector2(
                (new Random().NextSingle() - 0.5f) * 100, // Closer to camera
                (new Random().NextSingle() - 0.5f) * 100
            );
            
            var entity = world.CreateEntity();
            world.AddComponent(entity, new Transform(new Vector3(position, 0)));
            
            ParticleSystem particleSystem = type switch
            {
                1 => new ParticleSystem(ParticleConfig.Fire()) { EmissionRate = 8.0f, MaxParticles = 25 },
                2 => new ParticleSystem(ParticleConfig.Smoke()) { EmissionRate = 5.0f, MaxParticles = 20 },
                3 => new ParticleSystem(ParticleConfig.Explosion()) { IsEmitting = false, AutoDestroy = true },
                _ => new ParticleSystem(ParticleConfig.Fire())
            };
            
            world.AddComponent(entity, particleSystem);
            
            if (type == 3)
            {
                particleSystem.EmitBurst(15); // Smaller bursts
            }
            
            Console.WriteLine($"Effect type {type} created at ({position.X:F0}, {position.Y:F0})");
        }
        
        static void CreateRandomEffect(AdvancedGameManager game)
        {
            var random = new Random();
            var effectType = random.Next(1, 4);
            CreateEffectType(game, effectType);
        }
    }
}
