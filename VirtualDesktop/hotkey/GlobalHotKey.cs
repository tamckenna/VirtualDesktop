using System;
using System.Windows.Input;

namespace VirtualDesktop.hotkey
{
    public class GlobalHotKey
    {
        public ModifierKeys Modifier { get; set; }
        public Key Key { get; set; }
        public Action Callback { get; set; }
        public bool CanExecute { get; set; }

        public GlobalHotKey(ModifierKeys modifier, Key key, Action callbackMethod, bool canExecute = true)
        {
            this.Modifier = modifier;
            this.Key = key;
            this.Callback = callbackMethod;
            this.CanExecute = canExecute;
        }
    }
}