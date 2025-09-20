// Core/Graphics/Window.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// Native Win32 window with OpenGL context
    /// </summary>
    public class Window : IDisposable
    {
        private IntPtr _windowHandle;
        private IntPtr _deviceContext;
        private IntPtr _renderContext;
        private bool _shouldClose = false;
        private readonly string _title;
        private readonly int _width;
        private readonly int _height;
        private Win32.WndProc _windowProcDelegate;
        
        // Input state
        private readonly HashSet<int> _pressedKeys = new HashSet<int>();
        private readonly HashSet<int> _keysThisFrame = new HashSet<int>();
        
        public IntPtr Handle => _windowHandle;
        public bool ShouldClose => _shouldClose;
        public int Width => _width;
        public int Height => _height;
        public string Title => _title;

        public Window(string title, int width, int height)
        {
            _title = title;
            _width = width;
            _height = height;
            
            CreateWindow();
            CreateOpenGLContext();
            GL.LoadFunctions();
            
            // Set up initial OpenGL state
            GL.glViewport?.Invoke(0, 0, width, height);
            GL.glEnable?.Invoke(GL.GL_BLEND);
            GL.glBlendFunc?.Invoke(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.CheckError("Initial OpenGL setup");
            
            Console.WriteLine($"Window created: {title} ({width}x{height})");
            Console.WriteLine("OpenGL context initialized successfully");
        }

        [MemberNotNull(nameof(_windowHandle))]
        [MemberNotNull(nameof(_windowProcDelegate))]
        private void CreateWindow()
        {
            var hInstance = Win32.GetModuleHandle(null);
            
            _windowProcDelegate = new Win32.WndProc(WindowProc);
            var wc = new Win32.WNDCLASSEX
            {
                cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.WNDCLASSEX)),
                style = Win32.CS_HREDRAW | Win32.CS_VREDRAW | Win32.CS_OWNDC,
                lpfnWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(_windowProcDelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = hInstance,
                hIcon = IntPtr.Zero,
                hCursor = Win32.LoadCursor(IntPtr.Zero, Win32.IDC_ARROW),
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null!,
                lpszClassName = "BounceGameWindow",
                hIconSm = IntPtr.Zero
            };

            if (Win32.RegisterClassEx(ref wc) == 0)
                throw new Exception("Failed to register window class");

            // Create window
            _windowHandle = Win32.CreateWindowEx(
                0,
                "BounceGameWindow",
                _title,
                Win32.WS_OVERLAPPEDWINDOW | Win32.WS_VISIBLE,
                100, 100, _width, _height,
                IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

            if (_windowHandle == IntPtr.Zero)
                throw new Exception("Failed to create window");

            Win32.ShowWindow(_windowHandle, Win32.SW_SHOW);
            Win32.UpdateWindow(_windowHandle);
        }

        [MemberNotNull(nameof(_deviceContext))]
        [MemberNotNull(nameof(_renderContext))]
        private void CreateOpenGLContext()
        {
            _deviceContext = Win32.GetDC(_windowHandle);
            if (_deviceContext == IntPtr.Zero)
                throw new Exception("Failed to get device context");

            var pfd = new Win32.PIXELFORMATDESCRIPTOR
            {
                nSize = (ushort)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32.PIXELFORMATDESCRIPTOR)),
                nVersion = 1,
                dwFlags = Win32.PFD_DRAW_TO_WINDOW | Win32.PFD_SUPPORT_OPENGL | Win32.PFD_DOUBLEBUFFER,
                iPixelType = Win32.PFD_TYPE_RGBA,
                cColorBits = 32,
                cDepthBits = 24,
                cStencilBits = 8,
                iLayerType = Win32.PFD_MAIN_PLANE
            };

            int pixelFormat = Win32.ChoosePixelFormat(_deviceContext, ref pfd);
            if (pixelFormat == 0)
                throw new Exception("Failed to choose pixel format");

            if (!Win32.SetPixelFormat(_deviceContext, pixelFormat, ref pfd))
                throw new Exception("Failed to set pixel format");

            _renderContext = Win32.wglCreateContext(_deviceContext);
            if (_renderContext == IntPtr.Zero)
                throw new Exception("Failed to create OpenGL context");

            if (!Win32.wglMakeCurrent(_deviceContext, _renderContext))
                throw new Exception("Failed to make OpenGL context current");
        }

        private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case Win32.WM_CLOSE:
                case Win32.WM_DESTROY:
                    _shouldClose = true;
                    Win32.PostQuitMessage(0);
                    return IntPtr.Zero;
                    
                case Win32.WM_KEYDOWN:
                    int keyDown = wParam.ToInt32();
                    if (!_pressedKeys.Contains(keyDown))
                    {
                        _pressedKeys.Add(keyDown);
                        _keysThisFrame.Add(keyDown);
                    }
                    
                    // Handle escape key
                    if (keyDown == Win32.VK_ESCAPE)
                        _shouldClose = true;
                    break;
                    
                case Win32.WM_KEYUP:
                    int keyUp = wParam.ToInt32();
                    _pressedKeys.Remove(keyUp);
                    break;
            }

            return Win32.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void PollEvents()
        {
            _keysThisFrame.Clear();
            
            while (Win32.PeekMessage(out Win32.MSG msg, IntPtr.Zero, 0, 0, Win32.PM_REMOVE))
            {
                Win32.TranslateMessage(ref msg);
                Win32.DispatchMessage(ref msg);
            }
        }

        public void SwapBuffers()
        {
            Win32.SwapBuffers(_deviceContext);
        }

        public void Clear(float r = 0.0f, float g = 0.0f, float b = 0.0f, float a = 1.0f)
        {
            GL.glClearColor?.Invoke(r, g, b, a);
            GL.glClear?.Invoke(GL.GL_COLOR_BUFFER_BIT);
            GL.CheckError("Clear");
        }

        // Input methods
        public bool IsKeyPressed(int virtualKey)
        {
            return _pressedKeys.Contains(virtualKey);
        }

        public bool IsKeyJustPressed(int virtualKey)
        {
            return _keysThisFrame.Contains(virtualKey);
        }

        // Helper methods for WASD + Space
        public bool IsWPressed() => IsKeyPressed(Win32.VK_W);
        public bool IsAPressed() => IsKeyPressed(Win32.VK_A);
        public bool IsSPressed() => IsKeyPressed(Win32.VK_S);
        public bool IsDPressed() => IsKeyPressed(Win32.VK_D);
        public bool IsSpacePressed() => IsKeyPressed(Win32.VK_SPACE);
        
        public bool IsWJustPressed() => IsKeyJustPressed(Win32.VK_W);
        public bool IsAJustPressed() => IsKeyJustPressed(Win32.VK_A);
        public bool IsSJustPressed() => IsKeyJustPressed(Win32.VK_S);
        public bool IsDJustPressed() => IsKeyJustPressed(Win32.VK_D);
        public bool IsSpaceJustPressed() => IsKeyJustPressed(Win32.VK_SPACE);

        public void Dispose()
        {
            if (_renderContext != IntPtr.Zero)
            {
                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Win32.wglDeleteContext(_renderContext);
                _renderContext = IntPtr.Zero;
            }

            if (_deviceContext != IntPtr.Zero)
            {
                Win32.ReleaseDC(_windowHandle, _deviceContext);
                _deviceContext = IntPtr.Zero;
            }

            _windowHandle = IntPtr.Zero;
        }
    }
}