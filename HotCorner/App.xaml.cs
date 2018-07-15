﻿using GlobalLowLevelHooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HotCorner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected static Boolean isLocked = false;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MouseHook mouseHook = new MouseHook();

            // Capture mouse events
            mouseHook.MouseMove += new MouseHook.MouseHookCallback(mouseHook_MouseMove);
           
            var bitmap = new Bitmap("./icon.png");
            var iconHandle = bitmap.GetHicon();
            var icon = System.Drawing.Icon.FromHandle(iconHandle);

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            notifyIcon.Icon = icon;
            notifyIcon.Visible = true;

            mouseHook.Install();
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem
            {
                Header = "Exit"
            };
            contextMenu.Items.Add(menuItem);
            contextMenu.IsOpen = true;

            // For now this is everything we need for closing the app. Since the app has no window
            // it will shut down when the context menu loses focus. No need to do anything here.
        }

        private void mouseHook_MouseMove(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
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