using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MYRIAM
{
    internal partial class Console_Banners
    {

        // Import the GetConsoleScreenBufferInfo function from the Windows API
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        // Define the structure required by GetConsoleScreenBufferInfo function
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }


        static void Console_Resize(int minwidth = 68)
        {
            // Get the handle to the console screen buffer
            IntPtr handle = GetStdHandle(-11); // -11 refers to the standard output device

            // Retrieve the console screen buffer information
            if (GetConsoleScreenBufferInfo(handle, out CONSOLE_SCREEN_BUFFER_INFO info))
            {
                // Check if the width is smaller than minimum width
                if (info.srWindow.Right - info.srWindow.Left + 1 < minwidth)
                {
                    // Adjust the width to minimum width characters
                    int height = info.srWindow.Bottom - info.srWindow.Top + 1;
                    SMALL_RECT rect;
                    rect.Left = info.srWindow.Left;
                    rect.Top = info.srWindow.Top;
                    rect.Right = (short)(rect.Left + minwidth - 1);
                    rect.Bottom = (short)(rect.Top + height - 1);

                    // Set the new console screen buffer size
                    Console.SetWindowSize(minwidth, height);
                    //Console.SetBufferSize(minwidth, height);
                    SetConsoleWindowInfo(handle, true, ref rect);
                }
            }
        }

        // Import the SetConsoleWindowInfo function from the Windows API
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, ref SMALL_RECT lpConsoleWindow);
    }
}
