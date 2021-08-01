using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using VirtualDesktop.hotkey;

namespace VirtualDesktop
{
    public partial class App : Application
    {
        protected const string GLOBAL_CONF_FILE = @"C:\opt\conf\VirtualDesktop.conf";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            HotKeyManager.SetupSystemHook();
            HotKeyManager.AddHotKey(ModifierKeys.Alt, Key.Left, staSwitchDesktopLeft);
            HotKeyManager.AddHotKey(ModifierKeys.Alt, Key.Right, staSwitchDesktopRight);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            HotKeyManager.ShutdownSystemHook();
        }

        protected void switchDesktop(int movement)
        {
            int currentDesktopIndex = Desktop.FromDesktop(Desktop.Current);
            int farLeftDesktopIndex = 0;
            int farRightDesktopIndex = Desktop.Count - 1;
            int newDesktopIndex = currentDesktopIndex + movement;

            // Wrap Desktop Indexes
            if (newDesktopIndex < 0) { newDesktopIndex = farRightDesktopIndex; }
            else if (newDesktopIndex > farRightDesktopIndex) { newDesktopIndex = farLeftDesktopIndex; }

            // Make Desktop Visible
            Desktop.FromIndex(newDesktopIndex).MakeVisible();
            Desktop.Current.SetForeground(this.getFocusAppName());
        }

        protected void staSwitchDesktopLeft()
        {
            var thread = new Thread(switchDesktopLeft);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected void staSwitchDesktopRight()
        {
            var thread = new Thread(switchDesktopRight);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        protected void switchDesktopLeft(object state){ this.switchDesktop(-1); }

        protected void switchDesktopRight(object state){ this.switchDesktop(1); }

        protected string getFocusAppName()
        {
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(GLOBAL_CONF_FILE)) { data.Add(row.Split('=')[0], string.Join("=",row.Split('=').Skip(1).ToArray())); }
            return data["FocusApp"];
        }
    }
}
