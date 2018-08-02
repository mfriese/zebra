using GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using zebra;

namespace HotCorner
{
    public partial class App : System.Windows.Application
    {
        protected static List<KeyboardHook.VKeys> releasedKeys = new List<KeyboardHook.VKeys>();

        protected static MouseHook mouseHook = null;

        protected static KeyboardHook keyHook = null;

        protected static HotCorners Corners { get; } = new HotCorners();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mouseHook = new MouseHook();
            keyHook = new KeyboardHook();

            // Capture mouse events
            mouseHook.MouseMove += MouseMove;
            keyHook.KeyUp += KeyHook_KeyUp;

            // Show an Icon in the tray
            var bitmap = new Bitmap("./icon.png");
            var iconHandle = bitmap.GetHicon();
            var icon = System.Drawing.Icon.FromHandle(iconHandle);

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += NotifyIcon_Click;
            notifyIcon.Icon = icon;
            notifyIcon.Visible = true;

            // Attach yourself to the system
            mouseHook.Install();
            keyHook.Install();

            // Add Actions for hot corners
            Corners.RegisterHandler(CORNER.UPPER_LEFT, () => ArrangeWindows());
            Corners.RegisterHandler(CORNER.UPPER_RIGHT, () => ShowDesktop());
        }

        void NotifyIcon_Click(object sender, EventArgs e)
        {
            mouseHook.Uninstall();
            keyHook.Uninstall();
            this.Shutdown();
        }

        private void MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Corners.TriggerOnHit(new System.Drawing.Point()
            {
                X = mouseStruct.pt.x,
                Y = mouseStruct.pt.y
            });

            releasedKeys.Clear();
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

        public void ShowDesktop()
        {
            INPUT[] sequence = {
                User32Util.MakeInput(KeyCode.LWIN, false),
                User32Util.MakeInput(KeyCode.KEY_D, false),
                User32Util.MakeInput(KeyCode.KEY_D, true),
                User32Util.MakeInput(KeyCode.LWIN, true)
            };

            User32Util.SendInputSequence(sequence);
        }

        private void ArrangeWindows()
        {
            INPUT[] sequence = {
                User32Util.MakeInput(KeyCode.LWIN, false),
                User32Util.MakeInput(KeyCode.TAB, false),
                User32Util.MakeInput(KeyCode.TAB, true),
                User32Util.MakeInput(KeyCode.LWIN, true)
            };

            User32Util.SendInputSequence(sequence);
        }
    }
}
