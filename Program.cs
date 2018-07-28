using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SearchHighlightedText
{
    static class Program
    {

     private static int hotKeyNumber;

        [STAThread]
        static void Main()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                return;

            Application.EnableVisualStyles();
            NotifyIcon systemTray = new NotifyIcon();
            ContextMenu menu = new ContextMenu();
            MenuItem exit = new MenuItem();
            exit.Index = 0;
            menu.MenuItems.AddRange(new MenuItem[] { exit });
            exit.Text = "Exit";
            exit.Click += new EventHandler(Exit_Click);
            systemTray.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(("SearchHighlightedText.app.ico")));
            systemTray.Text = "Search Highlighted Text";
            systemTray.ContextMenu = menu;
            systemTray.Visible = true;

            hotKeyNumber = HotKeyManager.RegisterHotKey(Keys.C, KeyModifiers.Alt);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(CopyKeyboardSearch);
            Application.Run();
        }

        static void CopyKeyboardSearch(object sender, HotKeyEventArgs e)
        {
            SendKeys.Send("^c");
            Thread.Sleep(100);
            SendKeys.Send("^c");
            Thread.Sleep(100);
            string lastClipboardText = Clipboard.GetText();
            if (lastClipboardText != "")
                Process.Start("https://www.google.com/search?q=" + lastClipboardText.TrimStart(' ').TrimEnd(' ').Replace(" ", "+"));
        }

        static void Exit_Click(object sender, EventArgs e)
        {
            HotKeyManager.UnregisterHotKey(hotKeyNumber);
            Application.Exit();
        }

        //Source: https://stackoverflow.com/questions/3654787/global-hotkey-in-console-application
        public class HotKeyManager
        {
            public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

            public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
            {
                int id = System.Threading.Interlocked.Increment(ref _id);
                RegisterHotKey(_wnd.Handle, id, (uint)modifiers, (uint)key);
                return id;
            }

            public static bool UnregisterHotKey(int id)
            {
                return UnregisterHotKey(_wnd.Handle, id);
            }

            protected static void OnHotKeyPressed(HotKeyEventArgs e)
            {
                if (HotKeyManager.HotKeyPressed != null)
                {
                    HotKeyManager.HotKeyPressed(null, e);
                }
            }

            private static MessageWindow _wnd = new MessageWindow();

            private class MessageWindow : Form
            {
                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == WM_HOTKEY)
                    {
                        HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                        HotKeyManager.OnHotKeyPressed(e);
                    }

                    base.WndProc(ref m);
                }

                private const int WM_HOTKEY = 0x312;
            }

            [DllImport("user32")]
            private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32")]
            private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            private static int _id = 0;
        }


        public class HotKeyEventArgs : EventArgs
        {
            public readonly Keys Key;
            public readonly KeyModifiers Modifiers;

            public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
            {
                this.Key = key;
                this.Modifiers = modifiers;
            }

            public HotKeyEventArgs(IntPtr hotKeyParam)
            {
                uint param = (uint)hotKeyParam.ToInt64();
                Key = (Keys)((param & 0xffff0000) >> 16);
                Modifiers = (KeyModifiers)(param & 0x0000ffff);
            }
        }

        [Flags]
        public enum KeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            NoRepeat = 0x4000
        }


    }
}
