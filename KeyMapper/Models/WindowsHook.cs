using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KeyMapper.Models
{
    public sealed class WindowsHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int LLKHF_INJECTED = 0x10;
        private const int LLKHF_LOWER_IL_INJECTED = 0x02;
        private const int WM_KEYDOWN = 0x0100;

        private readonly IntPtr _hookId;
        private readonly LowLevelKeyboardProc _hookProc;

        public WindowsHook()
        {
            _hookProc = HookCallback;
            _hookId = SetHook(_hookProc);
        }

        public event EventHandler<KeyComboEventArgs>? KeysPressed;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                if (module == null)
                    throw new InvalidOperationException("Failed to get process module.");
                var hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(module.ModuleName), 0);
                if (hookId == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to set Windows hook.");
                return hookId;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (int)wParam;
                KBDLLHOOKSTRUCT kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                if ((kbStruct.flags & (LLKHF_INJECTED | LLKHF_LOWER_IL_INJECTED)) == 0 && msg == WM_KEYDOWN) // this key was not injected (in essence this is a physical press, rather then the one sent by other app)
                {
                    var actionKey = KeyInterop.KeyFromVirtualKey((int)kbStruct.vkCode);
                    var modifier = IsModifierKey(actionKey);
                    if (!modifier)
                    {
                        var modifierKeys = Keyboard.Modifiers;
                        var keyCombo = new KeyCombo(modifierKeys, actionKey);
                        var args = new KeyComboEventArgs(keyCombo);
                        KeysPressed?.Invoke(this, args);
                        if (args.Handled)
                            return 1;
                    }
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt;
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                UnhookWindowsHookEx(_hookId);
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        ~WindowsHook()
        {
            Dispose();
        }

        #endregion
    }

    public class KeyComboEventArgs : EventArgs
    {
        private KeyCombo _keyCombo;
        private bool _handled;

        public KeyComboEventArgs(KeyCombo keyCombo)
        {
            this._keyCombo = keyCombo;
        }

        public KeyCombo KeyCombo
        {
            get { return _keyCombo; }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode; // Virtual-key code
        public uint scanCode; // Hardware scan code
        public uint flags; // Event flags (e.g., injected input)
        public uint time; // Timestamp
        public IntPtr dwExtraInfo; // Extra info
    }
}
