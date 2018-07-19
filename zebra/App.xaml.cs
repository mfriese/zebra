using GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace HotCorner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected static Boolean isLocked = false;

        protected static List<KeyboardHook.VKeys> releasedKeys = new List<KeyboardHook.VKeys>();

        protected static MouseHook mouseHook = null;

        protected static KeyboardHook keyHook = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mouseHook = new MouseHook();
            keyHook = new KeyboardHook();

            // Capture mouse events
            mouseHook.MouseMove += new MouseHook.MouseHookCallback(mouseHook_MouseMove);
            keyHook.KeyUp += KeyHook_KeyUp;

            var bitmap = new Bitmap("./icon.png");
            var iconHandle = bitmap.GetHicon();
            var icon = System.Drawing.Icon.FromHandle(iconHandle);

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(NotifyIcon_Click);
            notifyIcon.Icon = icon;
            notifyIcon.Visible = true;

            mouseHook.Install();
            keyHook.Install();
        }

        private void KeyHook_KeyUp(KeyboardHook.VKeys key)
        {
            if (key != KeyboardHook.VKeys.ESCAPE)
            {
                releasedKeys.Clear();
                return;
            }

            releasedKeys.Add(key);

            if (releasedKeys.Count > 2)
            {
                releasedKeys.Clear();
                INPUT[] sequence = {
                    User32Util.MakeInput(KeyCode.ALT, false),
                    User32Util.MakeInput(KeyCode.F4, false),
                    User32Util.MakeInput(KeyCode.F4, true),
                    User32Util.MakeInput(KeyCode.ALT, true)
                };
                User32Util.SendInputSequence(sequence);
            }
        }

        void NotifyIcon_Click(object sender, EventArgs e)
        {
            mouseHook.Uninstall();
            keyHook.Uninstall();
            this.Shutdown();
        }

        private void mouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Console.WriteLine(String.Format("{0} : {1}", mouseStruct.pt.x, mouseStruct.pt.y));

            // Unlock and exit if mouse is outside the hot area
            if (mouseStruct.pt.x > 4 || mouseStruct.pt.y > 4)
            {
                isLocked = false;
                return;
            }

            // Do not allow restarts. If hot corner is locked stop.
            if (isLocked)
                return;

            // Hot corner was not locked, lock it now.
            isLocked = true;

            INPUT[] sequence = {
                User32Util.MakeInput(KeyCode.LWIN, false),
                User32Util.MakeInput(KeyCode.TAB, false),
                User32Util.MakeInput(KeyCode.TAB, true),
                User32Util.MakeInput(KeyCode.LWIN, true)
            };

            // Send the keystroke to windows, activate Mission Control.
            User32Util.SendInputSequence(sequence);
        }
    }
}
