using System.DirectoryServices;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KeyMapper.Models
{
    public class WindowsKeyPresserViaMessage
    {
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;
        private const uint MAPVK_VK_TO_VSC = 0x0;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern bool SetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private static ushort GetVirtualKey(Key key) => (ushort)KeyInterop.VirtualKeyFromKey(key);

        public static void SimulateKeyPress(ModifierKeys modifierKeys, Key mainKey)
        {
            IntPtr fgWindow = GetForegroundWindow();
            uint targetThread = GetWindowThreadProcessId(fgWindow, out _);
            uint currentThread = GetWindowThreadProcessId(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, out _);
            AttachThreadInput(currentThread, targetThread, true);
            IntPtr focused = GetFocus();
            AttachThreadInput(currentThread, targetThread, false);

            if (focused == IntPtr.Zero)
                return;

            // backup keyboard state
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            // set modifiers down in keyboard state
            SetModifiersState(modifierKeys, true, keyboardState);
            SetKeyboardState(keyboardState);

            // post modifier keydown messages
            if (modifierKeys.HasFlag(ModifierKeys.Control)) PressKey(focused, Key.LeftCtrl);
            if (modifierKeys.HasFlag(ModifierKeys.Shift)) PressKey(focused, Key.LeftShift);
            if (modifierKeys.HasFlag(ModifierKeys.Alt)) PressKey(focused, Key.LeftAlt);

            // press main key
            PressKey(focused, mainKey);
            Thread.Sleep(5); // ensure app detects modifier state
            ReleaseKey(focused, mainKey);

            // release modifiers
            if (modifierKeys.HasFlag(ModifierKeys.Control)) ReleaseKey(focused, Key.LeftCtrl);
            if (modifierKeys.HasFlag(ModifierKeys.Shift)) ReleaseKey(focused, Key.LeftShift);
            if (modifierKeys.HasFlag(ModifierKeys.Alt)) ReleaseKey(focused, Key.LeftAlt);

            // restore original keyboard state
            SetModifiersState(modifierKeys, false, keyboardState);
            SetKeyboardState(keyboardState);
        }

        private static void PressKey(IntPtr focused, Key key)
        {
            if (key == Key.None) return;
            var vk = GetVirtualKey(key);
            var lParam = BuildLParam(vk, keyUp: false, wasDown: false);
            PostMessage(focused, WM_KEYDOWN, (IntPtr)vk, lParam);
        }

        private static void ReleaseKey(IntPtr focused, Key key)
        {
            if (key == Key.None) return;
            var vk = GetVirtualKey(key);
            var lParam = BuildLParam(vk, keyUp: true, wasDown: true);
            PostMessage(focused, WM_KEYUP, (IntPtr)vk, lParam);
        }

        private static IntPtr BuildLParam(uint vkCode, bool keyUp, bool wasDown)
        {
            uint scanCode = MapVirtualKey(vkCode, MAPVK_VK_TO_VSC);
            uint lParam = 0;
            lParam |= 1u;                     // repeat count
            lParam |= (scanCode & 0xFF) << 16;
            bool isExtended = vkCode == 0xA3 || vkCode == 0xA1 || // Right Ctrl / Alt
                              vkCode == 0x25 || vkCode == 0x26 || vkCode == 0x27 || vkCode == 0x28; // arrows
            if (isExtended) lParam |= 1u << 24;
            if (wasDown) lParam |= 1u << 30;
            if (keyUp) lParam |= 1u << 31;
            return (IntPtr)lParam;
        }

        private static void SetModifiersState(ModifierKeys modifierKeys, bool down, byte[] keyboardState)
        {
            if (modifierKeys.HasFlag(ModifierKeys.Control))
                SetModifierKeyState(Key.LeftCtrl, down, keyboardState);
            if (modifierKeys.HasFlag(ModifierKeys.Shift))
                SetModifierKeyState(Key.LeftShift, down, keyboardState);
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
                SetModifierKeyState(Key.LeftAlt, down, keyboardState);
        }

        private static void SetModifierKeyState(Key key, bool down, byte[] keyboardState)
        {
            int vk = KeyInterop.VirtualKeyFromKey(key);
            if (down) keyboardState[vk] |= 0x80;
            else keyboardState[vk] &= 0x7F;
        }
    }
}