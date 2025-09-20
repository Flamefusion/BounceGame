// Program.cs - OpenGL Foundation Test
using System;
using System.Numerics;
using BounceGame.Core.Graphics;

namespace BounceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== OpenGL Foundation Test ===\n");
            
            // Create window and initialize OpenGL
            using var window = new Window("Bounce Game - OpenGL Test", 800, 600);
            using var renderer = new Renderer(800, 600);
            
            Console.WriteLine("Window and renderer created successfully!");
            Console.WriteLine("Controls:");
            Console.WriteLine("  WASD - Move red square");
            Console.WriteLine("  SPACE - Change color");
            Console.WriteLine("  ESC - Exit");
            Console.WriteLine("\nStarting render loop...\n");
            
            // Game state
            var playerPos = new Vector2(0, 0);
            var playerSize = new Vector2(50, 50);
            var playerColor = new Vector4(1.0f, 0.3f, 0.3f, 1.0f); // Red
            var backgroundColor = new Vector4(0.1f, 0.1f, 0.2f, 1.0f); // Dark blue
            
            float speed = 200.0f; // pixels per second
            bool colorChanged = false;
            
            var lastTime = DateTime.Now;
            int frameCount = 0;
            var fpsTimer = DateTime.Now;
            
            // Main game loop
            while (!window.ShouldClose)
            {
                // Calculate delta time
                var currentTime = DateTime.Now;
                float deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;
                
                // Poll window events and input
                window.PollEvents();
                
                // Handle input
                HandleInput(window, ref playerPos, ref playerColor, ref colorChanged, speed, deltaTime);
                
                // Keep player in bounds
                playerPos.X = Math.Clamp(playerPos.X, -400 + playerSize.X/2, 400 - playerSize.X/2);
                playerPos.Y = Math.Clamp(playerPos.Y, -300 + playerSize.Y/2, 300 - playerSize.Y/2);
                
                // Render
                window.Clear(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W);
                
                // Draw some background elements
                DrawBackground(renderer);
                
                // Draw player
                renderer.DrawQuad(playerPos, playerSize, playerColor);
                
                // Present frame
                window.SwapBuffers();
                
                // FPS counter
                frameCount++;
                if ((DateTime.Now - fpsTimer).TotalSeconds >= 1.0)
                {
                    Console.WriteLine($"FPS: {frameCount}, Player: ({playerPos.X:F1}, {playerPos.Y:F1})");
                    frameCount = 0;
                    fpsTimer = DateTime.Now;
                }
            }
            
            Console.WriteLine("\nShutting down gracefully...");
        }
        
        static void HandleInput(Window window, ref Vector2 playerPos, ref Vector4 playerColor, 
                               ref bool colorChanged, float speed, float deltaTime)
        {
            float movement = speed * deltaTime;
            
            // Movement with WASD
            if (window.IsWPressed())
                playerPos.Y += movement;
            if (window.IsSPressed()) 
                playerPos.Y -= movement;
            if (window.IsAPressed())
                playerPos.X -= movement;
            if (window.IsDPressed())
                playerPos.X += movement;
            
            // Color change with space (just pressed, not held)
            if (window.IsSpaceJustPressed() && !colorChanged)
            {
                // Cycle through some colors
                if (Math.Abs(playerColor.X - 1.0f) < 0.1f) // Red
                    playerColor = new Vector4(0.3f, 1.0f, 0.3f, 1.0f); // Green
                else if (Math.Abs(playerColor.Y - 1.0f) < 0.1f) // Green  
                    playerColor = new Vector4(0.3f, 0.3f, 1.0f, 1.0f); // Blue
                else if (Math.Abs(playerColor.Z - 1.0f) < 0.1f) // Blue
                    playerColor = new Vector4(1.0f, 1.0f, 0.3f, 1.0f); // Yellow
                else if (Math.Abs(playerColor.X - playerColor.Y) < 0.1f && Math.Abs(playerColor.Y - 1.0f) < 0.1f) // Yellow
                    playerColor = new Vector4(1.0f, 0.3f, 1.0f, 1.0f); // Magenta
                else
                    playerColor = new Vector4(1.0f, 0.3f, 0.3f, 1.0f); // Back to red
                
                colorChanged = true;
                Console.WriteLine($"Color changed to: ({playerColor.X:F1}, {playerColor.Y:F1}, {playerColor.Z:F1})");
            }
            
            if (!window.IsSpacePressed())
                colorChanged = false;
        }
        
        static void DrawBackground(Renderer renderer)
        {
            // Draw some background decoration
            var gridColor = new Vector4(0.2f, 0.2f, 0.3f, 1.0f);
            var gridSize = new Vector2(20, 20);
            
            // Draw a simple grid pattern
            for (int x = -400; x <= 400; x += 100)
            {
                for (int y = -300; y <= 300; y += 100)
                {
                    renderer.DrawQuad(new Vector2(x, y), gridSize, gridColor);
                }
            }
            
            // Draw border
            var borderColor = new Vector4(0.4f, 0.4f, 0.5f, 1.0f);
            var borderThickness = 10.0f;
            
            // Top and bottom borders
            renderer.DrawQuad(new Vector2(0, 300 - borderThickness/2), new Vector2(800, borderThickness), borderColor);
            renderer.DrawQuad(new Vector2(0, -300 + borderThickness/2), new Vector2(800, borderThickness), borderColor);
            
            // Left and right borders  
            renderer.DrawQuad(new Vector2(-400 + borderThickness/2, 0), new Vector2(borderThickness, 600), borderColor);
            renderer.DrawQuad(new Vector2(400 - borderThickness/2, 0), new Vector2(borderThickness, 600), borderColor);
        }
    }
}
