// README.md - Setup Instructions
/*
# Bounce Game - OpenGL Foundation

## What We Built

### ECS System ✅
- Entity management with ID recycling
- Component storage with type safety  
- Clean query API: `world.GetEntitiesWith<Transform, Rigidbody>()`
- Memory management and cleanup

### OpenGL Foundation ✅
- Native Win32 window creation (full control)
- OpenGL 3.3 Core context with function loading
- Comprehensive error checking and debugging
- Wrapper classes for Shader, Buffer, VertexArray
- Basic 2D renderer with orthographic projection

## Current Features

### Window System
- Native window with proper OpenGL context
- Input handling for WASD + Space + Escape
- Frame rate monitoring and delta time
- Graceful shutdown and cleanup

### Rendering System  
- Shader compilation and linking with error reporting
- Vertex Array Objects (VAO) management
- Buffer Objects (VBO/EBO) for vertex data
- 2D orthographic projection matrix
- Colored quad rendering

### Test Program
- Interactive red square that moves with WASD
- Color cycling with spacebar
- Grid background and border decoration
- Real-time FPS display and position tracking

## Next Steps

### Phase 1: Core Game Systems
- [ ] Add Sprite component to ECS
- [ ] Create SpriteRenderer system  
- [ ] Implement basic physics system
- [ ] Add collision detection (AABB)

### Phase 2: Ball Mechanics
- [ ] Ball controller component
- [ ] Size changing mechanics
- [ ] Physics properties per size
- [ ] Environment interaction system

### Phase 3: Level System
- [ ] Level loading from data files
- [ ] Platform and obstacle entities
- [ ] Collectible system
- [ ] Goal/completion detection

## Architecture Benefits

✅ **Modular Design**: Each system is independent
✅ **Type Safety**: Compile-time component checking  
✅ **Performance Ready**: Dictionary storage easily upgrades to arrays
✅ **Full Control**: Native OpenGL gives complete graphics control
✅ **Debugging**: Comprehensive error checking and logging

## Running the Test

1. Compile and run the program
2. Use WASD to move the red square
3. Press Space to cycle through colors
4. Watch console for FPS and position updates
5. Press Escape to exit

The foundation is solid and ready for game-specific features!
*/