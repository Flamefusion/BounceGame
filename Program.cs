// Program.cs - ECS Test Example
using System;
using System.Numerics;
using BounceGame.Core.ECS;
using BounceGame.Components;

namespace BounceGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ECS Foundation Test ===\n");
            
            // Create world
            var world = new World();
            
            // Test 1: Basic entity creation
            Console.WriteLine("1. Creating entities...");
            var entity1 = world.CreateEntity();
            var entity2 = world.CreateEntity();
            var entity3 = world.CreateEntity();
            
            Console.WriteLine($"Created entities: {entity1}, {entity2}, {entity3}");
            Console.WriteLine($"Active entities: {world.EntityCount}\n");
            
            // Test 2: Adding components
            Console.WriteLine("2. Adding components...");
            world.AddComponent(entity1, new Transform(new Vector3(1, 2, 3)));
            world.AddComponent(entity1, new Rigidbody(mass: 2.0f));
            
            world.AddComponent(entity2, new Transform(new Vector3(4, 5, 6)));
            // entity2 has no rigidbody
            
            world.AddComponent(entity3, new Transform(new Vector3(7, 8, 9)));
            world.AddComponent(entity3, new Rigidbody(mass: 0.5f, useGravity: false));
            
            Console.WriteLine("Components added successfully!\n");
            
            // Test 3: Component queries
            Console.WriteLine("3. Testing queries...");
            
            Console.WriteLine("Entities with Transform:");
            foreach (var entity in world.GetEntitiesWith<Transform>())
            {
                var transform = world.GetComponent<Transform>(entity);
                Console.WriteLine($"  {entity}: {transform}");
            }
            
            Console.WriteLine("\nEntities with Rigidbody:");
            foreach (var entity in world.GetEntitiesWith<Rigidbody>())
            {
                var rigidbody = world.GetComponent<Rigidbody>(entity);
                Console.WriteLine($"  {entity}: {rigidbody}");
            }
            
            Console.WriteLine("\nEntities with both Transform AND Rigidbody:");
            foreach (var entity in world.GetEntitiesWith<Transform, Rigidbody>())
            {
                var transform = world.GetComponent<Transform>(entity);
                var rigidbody = world.GetComponent<Rigidbody>(entity);
                Console.WriteLine($"  {entity}: Pos={transform.Position}, Mass={rigidbody.Mass}");
            }
            
            // Test 4: Component manipulation
            Console.WriteLine("\n4. Testing component manipulation...");
            
            if (world.TryGetComponent<Rigidbody>(entity1, out var rb1) && rb1 != null)
            {
                Console.WriteLine($"Before force: {rb1}");
                rb1.AddForce(new Vector3(10, 0, 0));
                Console.WriteLine($"After force:  {rb1}");
            }
            
            // Test 5: Error handling
            Console.WriteLine("\n5. Testing error handling...");
            
            try
            {
                // This should throw - entity2 doesn't have rigidbody
                var rb = world.GetComponent<Rigidbody>(entity2);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Expected error caught: {ex.Message}");
            }
            
            // Safe version
            if (world.TryGetComponent<Rigidbody>(entity2, out var rb2))
            {
                Console.WriteLine("Entity2 has rigidbody");
            }
            else
            {
                Console.WriteLine("Entity2 does not have rigidbody (safe check)");
            }
            
            // Test 6: Entity destruction
            Console.WriteLine("\n6. Testing entity destruction...");
            Console.WriteLine($"Before destruction: {world.EntityCount} entities");
            
            world.DestroyEntity(entity2);
            Console.WriteLine($"After destroying entity2: {world.EntityCount} entities");
            
            // Clean up orphaned components
            world.CleanupDestroyedEntities();
            
            // Test 7: Debug info
            Console.WriteLine("\n7. World debug info:");
            Console.WriteLine(world.GetDebugInfo());
            
            Console.WriteLine("\n=== ECS Foundation Test Complete ===");
            Console.WriteLine("Press any key to continue...");
            Console.WriteLine("Program finished.");
        }
    }
}