// Core/Graphics/Win32.cs - Additional Win32 function for input
using System.Runtime.InteropServices;

namespace BounceGame.Core.Graphics
{
    partial class Win32
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
    }
}