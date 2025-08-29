using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KeyMapper.Models;

// the problem with this class is that it sends the key press as a virtual key press, which means that it does not work with some applications that require physical key presses

public class WindowsKeyPresserViaSendInput
{
    private const uint INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public static void SimulateKeyPress(ModifierKeys modifierKeys, Key actionKey)
    {
        var shift = modifierKeys.HasFlag(ModifierKeys.Shift);
        var ctrl = modifierKeys.HasFlag(ModifierKeys.Control);
        var alt = modifierKeys.HasFlag(ModifierKeys.Alt);
        SimulateKeyPress(actionKey, shift, ctrl, alt);
    }

    public static void SimulateKeyPress(Key key)
    {
        var inputs = new List<INPUT>
        {
            CreateKeyInput(key, false), // key down
            CreateKeyInput(key, true)   // key up
        };
        SendInputs(inputs);
    }

    public static void SimulateKeyPress(Key key, bool shift = false, bool ctrl = false, bool alt = false)
    {
        var inputs = new List<INPUT>();

        // Press modifiers
        if (shift)
            inputs.Add(CreateKeyInput(Key.LeftShift, false));
        if (ctrl)
            inputs.Add(CreateKeyInput(Key.LeftCtrl, false));
        if (alt)
            inputs.Add(CreateKeyInput(Key.LeftAlt, false));
        // Press main key
        inputs.Add(CreateKeyInput(key, false));
        inputs.Add(CreateKeyInput(key, true)); // Release main key
        // Release modifiers
        if (shift)
            inputs.Add(CreateKeyInput(Key.LeftShift, true));
        if (ctrl)
            inputs.Add(CreateKeyInput(Key.LeftCtrl, true));
        if (alt)
            inputs.Add(CreateKeyInput(Key.LeftAlt, true));

        SendInputs(inputs);
    }

    private static INPUT CreateKeyInput(Key key, bool keyUp)
    {
        var code = KeyInterop.VirtualKeyFromKey(key);
        return new INPUT
        {
            type = INPUT_KEYBOARD,
            u = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = (ushort)code,
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
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk; // Virtual-key code
        public ushort wScan; // Hardware scan code
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