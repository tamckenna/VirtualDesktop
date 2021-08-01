using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace VirtualDesktop.hotkey
{
    public static class HotKeyManager
    {
        // Events
        public delegate void HotKeyEvent(GlobalHotKey hotkey);
        public static event HotKeyEvent HotKeyFired;

        // Callbacks

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc LowLevelProc = HookCallback;

        // All of the HotKeys
        private static List<GlobalHotKey> HotKeys { get; set; }

        // The build in proc ID for telling windows to hook onto the
        // low level keyboard events with the SetWindowsHookEx function
        private const int WH_KEYBOARD_LL = 13;

        // The system hook ID (for storing this application's hook)
        private static IntPtr HookID = IntPtr.Zero;
        public static bool IsHookSetup { get; private set; }
        public static bool RequiresModifierKey { get; set; }

        static HotKeyManager()
        {
            HotKeys = new List<GlobalHotKey>();
            RequiresModifierKey = true;
        }

        public static void SetupSystemHook()
        {
            HookID = SetHook(LowLevelProc);
            IsHookSetup = true;
        }

        public static void ShutdownSystemHook()
        {
            UnhookWindowsHookEx(HookID);
            IsHookSetup = false;
        }

        public static void AddHotKey(GlobalHotKey hotkey)
        {
            HotKeys.Add(hotkey);
        }

        public static void RemoveHotKey(GlobalHotKey hotkey)
        {
            HotKeys.Remove(hotkey);
        }

        private static void CheckHotKeys()
        {
            if (RequiresModifierKey)
            {
                if (Keyboard.Modifiers != ModifierKeys.None)
                {
                    foreach (GlobalHotKey hotkey in HotKeys)
                    {
                        if (Keyboard.Modifiers == hotkey.Modifier && Keyboard.IsKeyDown(hotkey.Key))
                        {
                            if (hotkey.CanExecute)
                            {
                                hotkey.Callback?.Invoke();
                                HotKeyFired?.Invoke(hotkey);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (GlobalHotKey hotkey in HotKeys)
                {
                    if (Keyboard.Modifiers == hotkey.Modifier && Keyboard.IsKeyDown(hotkey.Key))
                    {
                        if (hotkey.CanExecute)
                        {
                            hotkey.Callback?.Invoke();
                            HotKeyFired?.Invoke(hotkey);
                        }
                    }
                }
            }
        }
        public static List<GlobalHotKey> FindHotKeys(ModifierKeys modifier, Key key)
        {
            List<GlobalHotKey> hotkeys = new List<GlobalHotKey>();
            foreach (GlobalHotKey hotkey in HotKeys)
                if (hotkey.Key == key && hotkey.Modifier == modifier)
                    hotkeys.Add(hotkey);

            return hotkeys;
        }

        public static void AddHotKey(ModifierKeys modifier, Key key, Action callbackMethod, bool canExecute = true)
        {
            AddHotKey(new GlobalHotKey(modifier, key, callbackMethod, canExecute));
        }

        public static void RemoveHotKey(ModifierKeys modifier, Key key, bool removeAllOccourances = false)
        {
            List<GlobalHotKey> originalHotKeys = HotKeys;
            List<GlobalHotKey> toBeRemoved = FindHotKeys(modifier, key);

            if (toBeRemoved.Count > 0)
            {
                if (removeAllOccourances)
                {
                    foreach (GlobalHotKey hotkey in toBeRemoved)
                    {
                        originalHotKeys.Remove(hotkey);
                    }

                    HotKeys = originalHotKeys;
                }
                else
                {
                    RemoveHotKey(toBeRemoved[0]);
                }
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Checks if this is called from keydown only because key ups aren't used.
            if (nCode >= 0)
            {
                CheckHotKeys();

                // Cannot use System.Windows' keys because
                // they dont use the same values as windows
                //int vkCode = Marshal.ReadInt32(lParam);
                //System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)vkCode;
                //Debug.WriteLine(key);
            }

            // I think this tells windows that this app has successfully
            // handled the key events and now other apps can handle it next.
            return CallNextHookEx(HookID, nCode, wParam, lParam);
        }

        #region Native Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
