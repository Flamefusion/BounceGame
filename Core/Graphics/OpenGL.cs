// Core/Graphics/OpenGL.cs
using System;
using System.Runtime.InteropServices;

namespace BounceGame.Core.Graphics
{
    /// <summary>
    /// OpenGL function bindings and constants for 3.3 Core
    /// </summary>
    public static class GL
    {
        // OpenGL Constants
        public const uint GL_VERTEX_SHADER = 0x8B31;
        public const uint GL_FRAGMENT_SHADER = 0x8B30;
        public const uint GL_COMPILE_STATUS = 0x8B81;
        public const uint GL_LINK_STATUS = 0x8B82;
        public const uint GL_INFO_LOG_LENGTH = 0x8B84;
        public const uint GL_ARRAY_BUFFER = 0x8892;
        public const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893;
        public const uint GL_STATIC_DRAW = 0x88E4;
        public const uint GL_DYNAMIC_DRAW = 0x88E8;
        public const uint GL_TRIANGLES = 0x0004;
        public const uint GL_UNSIGNED_INT = 0x1405;
        public const uint GL_FLOAT = 0x1406;
        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const uint GL_DEPTH_TEST = 0x0B71;
        public const uint GL_BLEND = 0x0BE2;
        public const uint GL_SRC_ALPHA = 0x0302;
        public const uint GL_ONE_MINUS_SRC_ALPHA = 0x0303;
        public const uint GL_TEXTURE_2D = 0x0DE1;
        public const uint GL_TEXTURE_MIN_FILTER = 0x2801;
        public const uint GL_TEXTURE_MAG_FILTER = 0x2800;
        public const uint GL_NEAREST = 0x2600;
        public const uint GL_LINEAR = 0x2601;
        public const uint GL_TEXTURE_WRAP_S = 0x2802;
        public const uint GL_TEXTURE_WRAP_T = 0x2803;
        public const int GL_CLAMP_TO_EDGE = 0x812F;
        public const uint GL_TEXTURE0 = 0x84C0;
        public const uint GL_RGBA = 0x1908;
        public const uint GL_UNSIGNED_BYTE = 0x1401;
        public const uint GL_NO_ERROR = 0;

        // Function delegates
        public delegate void glViewportDelegate(int x, int y, int width, int height);
        public delegate void glClearColorDelegate(float red, float green, float blue, float alpha);
        public delegate void glClearDelegate(uint mask);
        public delegate void glEnableDelegate(uint cap);
        public delegate void glBlendFuncDelegate(uint sfactor, uint dfactor);
        public delegate uint glCreateShaderDelegate(uint type);
        public delegate void glShaderSourceDelegate(uint shader, int count, string[] source, int[] length);
        public delegate void glCompileShaderDelegate(uint shader);
        public delegate void glGetShaderivDelegate(uint shader, uint pname, out int param);
        public delegate void glGetShaderInfoLogDelegate(uint shader, int bufSize, out int length, IntPtr infoLog);
        public delegate uint glCreateProgramDelegate();
        public delegate void glAttachShaderDelegate(uint program, uint shader);
        public delegate void glLinkProgramDelegate(uint program);
        public delegate void glGetProgramivDelegate(uint program, uint pname, out int param);
        public delegate void glGetProgramInfoLogDelegate(uint program, int bufSize, out int length, IntPtr infoLog);
        public delegate void glUseProgramDelegate(uint program);
        public delegate void glDeleteShaderDelegate(uint shader);
        public delegate void glDeleteProgramDelegate(uint program);
        public delegate void glGenVertexArraysDelegate(int n, uint[] arrays);
        public delegate void glBindVertexArrayDelegate(uint array);
        public delegate void glGenBuffersDelegate(int n, uint[] buffers);
        public delegate void glBindBufferDelegate(uint target, uint buffer);
        public delegate void glBufferDataDelegate(uint target, IntPtr size, IntPtr data, uint usage);
        public delegate void glVertexAttribPointerDelegate(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);
        public delegate void glEnableVertexAttribArrayDelegate(uint index);
        public delegate void glDrawElementsDelegate(uint mode, int count, uint type, IntPtr indices);
        public delegate void glDeleteVertexArraysDelegate(int n, uint[] arrays);
        public delegate void glDeleteBuffersDelegate(int n, uint[] buffers);
        public delegate int glGetUniformLocationDelegate(uint program, string name);
        public delegate void glUniformMatrix4fvDelegate(int location, int count, bool transpose, float[] value);
        public delegate void glUniform1iDelegate(int location, int v0);
        public delegate void glUniform4fDelegate(int location, float v0, float v1, float v2, float v3);
        public delegate uint glGetErrorDelegate();
        public delegate void glGenTexturesDelegate(int n, uint[] textures);
        public delegate void glBindTextureDelegate(uint target, uint texture);
        public delegate void glTexImage2DDelegate(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);
        public delegate void glTexParameteriDelegate(uint target, uint pname, int param);
        public delegate void glActiveTextureDelegate(uint texture);
        public delegate void glDeleteTexturesDelegate(int n, uint[] textures);

        // Function pointers
        public static glViewportDelegate? glViewport;
        public static glClearColorDelegate? glClearColor;
        public static glClearDelegate? glClear;
        public static glEnableDelegate? glEnable;
        public static glBlendFuncDelegate? glBlendFunc;
        public static glCreateShaderDelegate? glCreateShader;
        public static glShaderSourceDelegate? glShaderSource;
        public static glCompileShaderDelegate? glCompileShader;
        public static glGetShaderivDelegate? glGetShaderiv;
        public static glGetShaderInfoLogDelegate? glGetShaderInfoLog;
        public static glCreateProgramDelegate? glCreateProgram;
        public static glAttachShaderDelegate? glAttachShader;
        public static glLinkProgramDelegate? glLinkProgram;
        public static glGetProgramivDelegate? glGetProgramiv;
        public static glGetProgramInfoLogDelegate? glGetProgramInfoLog;
        public static glUseProgramDelegate? glUseProgram;
        public static glDeleteShaderDelegate? glDeleteShader;
        public static glDeleteProgramDelegate? glDeleteProgram;
        public static glGenVertexArraysDelegate? glGenVertexArrays;
        public static glBindVertexArrayDelegate? glBindVertexArray;
        public static glGenBuffersDelegate? glGenBuffers;
        public static glBindBufferDelegate? glBindBuffer;
        public static glBufferDataDelegate? glBufferData;
        public static glVertexAttribPointerDelegate? glVertexAttribPointer;
        public static glEnableVertexAttribArrayDelegate? glEnableVertexAttribArray;
        public static glDrawElementsDelegate? glDrawElements;
        public static glDeleteVertexArraysDelegate? glDeleteVertexArrays;
        public static glDeleteBuffersDelegate? glDeleteBuffers;
        public static glGetUniformLocationDelegate? glGetUniformLocation;
        public static glUniformMatrix4fvDelegate? glUniformMatrix4fv;
        public static glUniform1iDelegate? glUniform1i;
        public static glUniform4fDelegate? glUniform4f;
        public static glGetErrorDelegate? glGetError;
        public static glGenTexturesDelegate? glGenTextures;
        public static glBindTextureDelegate? glBindTexture;
        public static glTexImage2DDelegate? glTexImage2D;
        public static glTexParameteriDelegate? glTexParameteri;
        public static glActiveTextureDelegate? glActiveTexture;
        public static glDeleteTexturesDelegate? glDeleteTextures;

        /// <summary>
        /// Load OpenGL function pointers
        /// </summary>
        public static void LoadFunctions()
        {
            glViewport = GetFunction<glViewportDelegate>("glViewport");
            glClearColor = GetFunction<glClearColorDelegate>("glClearColor");
            glClear = GetFunction<glClearDelegate>("glClear");
            glEnable = GetFunction<glEnableDelegate>("glEnable");
            glBlendFunc = GetFunction<glBlendFuncDelegate>("glBlendFunc");
            glCreateShader = GetFunction<glCreateShaderDelegate>("glCreateShader");
            glShaderSource = GetFunction<glShaderSourceDelegate>("glShaderSource");
            glCompileShader = GetFunction<glCompileShaderDelegate>("glCompileShader");
            glGetShaderiv = GetFunction<glGetShaderivDelegate>("glGetShaderiv");
            glGetShaderInfoLog = GetFunction<glGetShaderInfoLogDelegate>("glGetShaderInfoLog");
            glCreateProgram = GetFunction<glCreateProgramDelegate>("glCreateProgram");
            glAttachShader = GetFunction<glAttachShaderDelegate>("glAttachShader");
            glLinkProgram = GetFunction<glLinkProgramDelegate>("glLinkProgram");
            glGetProgramiv = GetFunction<glGetProgramivDelegate>("glGetProgramiv");
            glGetProgramInfoLog = GetFunction<glGetProgramInfoLogDelegate>("glGetProgramInfoLog");
            glUseProgram = GetFunction<glUseProgramDelegate>("glUseProgram");
            glDeleteShader = GetFunction<glDeleteShaderDelegate>("glDeleteShader");
            glDeleteProgram = GetFunction<glDeleteProgramDelegate>("glDeleteProgram");
            glGenVertexArrays = GetFunction<glGenVertexArraysDelegate>("glGenVertexArrays");
            glBindVertexArray = GetFunction<glBindVertexArrayDelegate>("glBindVertexArray");
            glGenBuffers = GetFunction<glGenBuffersDelegate>("glGenBuffers");
            glBindBuffer = GetFunction<glBindBufferDelegate>("glBindBuffer");
            glBufferData = GetFunction<glBufferDataDelegate>("glBufferData");
            glVertexAttribPointer = GetFunction<glVertexAttribPointerDelegate>("glVertexAttribPointer");
            glEnableVertexAttribArray = GetFunction<glEnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
            glDrawElements = GetFunction<glDrawElementsDelegate>("glDrawElements");
            glDeleteVertexArrays = GetFunction<glDeleteVertexArraysDelegate>("glDeleteVertexArrays");
            glDeleteBuffers = GetFunction<glDeleteBuffersDelegate>("glDeleteBuffers");
            glGetUniformLocation = GetFunction<glGetUniformLocationDelegate>("glGetUniformLocation");
            glUniformMatrix4fv = GetFunction<glUniformMatrix4fvDelegate>("glUniformMatrix4fv");
            glUniform1i = GetFunction<glUniform1iDelegate>("glUniform1i");
            glUniform4f = GetFunction<glUniform4fDelegate>("glUniform4f");
            glGetError = GetFunction<glGetErrorDelegate>("glGetError");
            glGenTextures = GetFunction<glGenTexturesDelegate>("glGenTextures");
            glBindTexture = GetFunction<glBindTextureDelegate>("glBindTexture");
            glTexImage2D = GetFunction<glTexImage2DDelegate>("glTexImage2D");
            glTexParameteri = GetFunction<glTexParameteriDelegate>("glTexParameteri");
            glActiveTexture = GetFunction<glActiveTextureDelegate>("glActiveTexture");
            glDeleteTextures = GetFunction<glDeleteTexturesDelegate>("glDeleteTextures");
        }

        private static IntPtr opengl32Module = Win32.GetModuleHandle("opengl32.dll");

        private static T GetFunction<T>(string name) where T : class
        {
            var ptr = Win32.wglGetProcAddress(name);
            if (ptr == IntPtr.Zero)
            {
                ptr = Win32.GetProcAddress(opengl32Module, name);
            }

            if (ptr == IntPtr.Zero)
                throw new Exception($"Failed to load OpenGL function: {name}");
            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }

        /// <summary>
        /// Check for OpenGL errors and throw exception if found
        /// </summary>
        public static void CheckError(string operation = "")
        {
            if (glGetError == null) return;
            uint error = glGetError();
            if (error != GL_NO_ERROR)
            {
                string errorString = error switch
                {
                    0x0500 => "GL_INVALID_ENUM",
                    0x0501 => "GL_INVALID_VALUE",
                    0x0502 => "GL_INVALID_OPERATION",
                    0x0505 => "GL_OUT_OF_MEMORY",
                    0x0506 => "GL_INVALID_FRAMEBUFFER_OPERATION",
                    _ => $"Unknown error: 0x{error:X}"
                };
                
                throw new Exception($"OpenGL Error: {errorString}" + 
                    (string.IsNullOrEmpty(operation) ? "" : $" during {operation}"));
            }
        }
    }

    /// <summary>
    /// Win32 API functions for window and OpenGL context creation
    /// </summary>
    internal static partial class Win32
    {
        // Window class constants
        public const int CS_OWNDC = 0x0020;
        public const int CS_VREDRAW = 0x0001;
        public const int CS_HREDRAW = 0x0002;
        
        // Window style constants
        public const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
        public const uint WS_VISIBLE = 0x10000000;
        
        // ShowWindow constants
        public const int SW_SHOW = 5;
        
        // Message constants
        public const uint WM_CLOSE = 0x0010;
        public const uint WM_DESTROY = 0x0002;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        
        // Virtual key codes
        public const int VK_ESCAPE = 0x1B;
        public const int VK_SPACE = 0x20;
        public const int VK_W = 0x57;
        public const int VK_A = 0x41;
        public const int VK_S = 0x53;
        public const int VK_D = 0x44;

        // Pixel format constants
        public const int PFD_DRAW_TO_WINDOW = 0x00000004;
        public const int PFD_SUPPORT_OPENGL = 0x00000020;
        public const int PFD_DOUBLEBUFFER = 0x00000001;
        public const int PFD_TYPE_RGBA = 0;
        public const int PFD_MAIN_PLANE = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string? lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public ushort nSize;
            public ushort nVersion;
            public uint dwFlags;
            public byte iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public byte iLayerType;
            public byte bReserved;
            public uint dwLayerMask;
            public uint dwVisibleMask;
            public uint dwDamageMask;
        }

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX wc);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName,
            uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu,
            IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("gdi32.dll")]
        public static extern int ChoosePixelFormat(IntPtr hdc, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll")]
        public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr hDC);

        [DllImport("opengl32.dll")]
        public static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hglrc);

        [DllImport("opengl32.dll")]
        public static extern bool wglDeleteContext(IntPtr hglrc);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglGetProcAddress(string lpszProc);

        [DllImport("gdi32.dll")]
        public static extern bool SwapBuffers(IntPtr hdc);

        public const int IDC_ARROW = 32512;
        public const uint PM_REMOVE = 0x0001;
    }
}