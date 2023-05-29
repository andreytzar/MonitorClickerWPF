using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MonitorClickerWPF
{
    public static class user32imports
    {
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);
        // Activate an application window.
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        // Mouse click Point anywhaere
        [DllImport("USER32.DLL")]
        private static extern bool GetCursorPos(ref Point lpPoint);

        //Send Mouse Event
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);
        [Flags]
        private enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        [DllImport("USER32.DLL")]
        private static extern short GetAsyncKeyState(UInt16 virtualKeyCode);
        //Virtual key codes
        //found at http://msdn.microsoft.com/en-us/library/dd375731(v=VS.85).aspx
        private const UInt16 VK_MBUTTON = 0x04;//middle mouse button
        private const UInt16 VK_LBUTTON = 0x01;//left mouse button
        private const UInt16 VK_RBUTTON = 0x02;//right mouse button

        public static Point GetCurrentMousePosition()
        {
            var res=new Point();
            GetCursorPos(ref res);
            return res;
        }

        public static void SendLeftClick(Point mousePos)
        {
            System.Windows.Forms.Cursor.Position = mousePos;
            mouse_event((int)MouseEventFlags.LEFTDOWN, 0, 00, 0, 0);
            mouse_event( (int)MouseEventFlags.LEFTUP, 0, 00, 0, 0);
        }
        public static void SendRightClick(Point mousePos)
        {
            System.Windows.Forms.Cursor.Position = mousePos;
            mouse_event((int)MouseEventFlags.RIGHTDOWN, 0, 00, 0, 0);
            mouse_event((int)MouseEventFlags.RIGHTUP, 0, 00, 0, 0);
        }

        public static bool IsLeftMouseBtnPressed()
        {
            var res = GetAsyncKeyState(VK_LBUTTON);
            return res == 0;
        }
    }
}
