using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KeyMapper.Models
{
    public class WindowsKeyPresserViaScanCode
    {
        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;

        public static void SimulateKeyPress(Key key)
        {
            var inputs = new List<INPUT>
        {
            CreateVkInput(key, false), // key down
            CreateVkInput(key, true)   // key up
        };
            SendInputs(inputs);
        }

        public static void SimulateKeyPress(ModifierKeys modifierKeys, Key key)
        {
            var inputs = new List<INPUT>();

            // Press modifiers (VK-based)
            if (modifierKeys.HasFlag(ModifierKeys.Control))
                inputs.Add(CreateVkInput(Key.LeftCtrl, false));
            if (modifierKeys.HasFlag(ModifierKeys.Shift))
                inputs.Add(CreateVkInput(Key.LeftShift, false));
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
                inputs.Add(CreateVkInput(Key.LeftAlt, false));

            // Press main key
            inputs.Add(CreateVkInput(key, false));
            inputs.Add(CreateVkInput(key, true));

            // Release modifiers (reverse order)
            if (modifierKeys.HasFlag(ModifierKeys.Alt))
                inputs.Add(CreateVkInput(Key.LeftAlt, true));
            if (modifierKeys.HasFlag(ModifierKeys.Shift))
                inputs.Add(CreateVkInput(Key.LeftShift, true));
            if (modifierKeys.HasFlag(ModifierKeys.Control))
                inputs.Add(CreateVkInput(Key.LeftCtrl, true));

            SendInputs(inputs);
        }

        private static INPUT CreateVkInput(Key key, bool keyUp)
        {
            ushort vk = (ushort)KeyInterop.VirtualKeyFromKey(key);

            return new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = 0,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
        }

        private static void SendInputs(List<INPUT> inputs)
        {
            var result = SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf<INPUT>());
            if (result == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}
