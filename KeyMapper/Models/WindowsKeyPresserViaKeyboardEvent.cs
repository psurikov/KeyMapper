using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyMapper.Models
{
    public class WindowsKeyPresserViaKeyboardEvent
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYUP = 0x0002;

        public static void SimulateKeyPress(ModifierKeys modifiers, Key key)
        {
            // Press modifiers
            if (modifiers.HasFlag(ModifierKeys.Control)) PressKey(KeyInterop.VirtualKeyFromKey(Key.LeftCtrl));
            if (modifiers.HasFlag(ModifierKeys.Alt)) PressKey(KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            if (modifiers.HasFlag(ModifierKeys.Shift)) PressKey(KeyInterop.VirtualKeyFromKey(Key.LeftShift));
            if (modifiers.HasFlag(ModifierKeys.Windows)) PressKey(KeyInterop.VirtualKeyFromKey(Key.LWin));

            // Press main key
            int vk = KeyInterop.VirtualKeyFromKey(key);
            PressKey(vk);
            ReleaseKey(vk);

            // Release modifiers (reverse order)
            if (modifiers.HasFlag(ModifierKeys.Windows)) ReleaseKey(KeyInterop.VirtualKeyFromKey(Key.LWin));
            if (modifiers.HasFlag(ModifierKeys.Shift)) ReleaseKey(KeyInterop.VirtualKeyFromKey(Key.LeftShift));
            if (modifiers.HasFlag(ModifierKeys.Alt)) ReleaseKey(KeyInterop.VirtualKeyFromKey(Key.LeftAlt));
            if (modifiers.HasFlag(ModifierKeys.Control)) ReleaseKey(KeyInterop.VirtualKeyFromKey(Key.LeftCtrl));
        }

        private static void PressKey(int vk)
        {
            keybd_event((byte)vk, 0, 0, UIntPtr.Zero);
        }

        private static void ReleaseKey(int vk)
        {
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
