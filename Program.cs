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
        private static int hotKeyNumber2;
        private static int beta;
        private static int gamma;
        private static int alpha;
        private static int delta;

        private static int hotKeyNumberImage;
        private static int hotKeyNumberImage2;
        private static int insertIncreaseArrow;
        private static int insertIncreaseArrow2;
        private static int insertDecreaseArrow;
        private static int insertDecreaseArrow2;
        private static int insertForwardArrow;
        private static int insertForwardArrow2;

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
            hotKeyNumberImage = HotKeyManager.RegisterHotKey(Keys.X, KeyModifiers.Alt);
            alpha = HotKeyManager.RegisterHotKey(Keys.A, KeyModifiers.Alt);
            beta = HotKeyManager.RegisterHotKey(Keys.B, KeyModifiers.Alt);
            delta = HotKeyManager.RegisterHotKey(Keys.D, KeyModifiers.Alt);
            gamma = HotKeyManager.RegisterHotKey(Keys.G, KeyModifiers.Alt);
            insertIncreaseArrow = HotKeyManager.RegisterHotKey(Keys.Up, KeyModifiers.Alt);
            insertDecreaseArrow = HotKeyManager.RegisterHotKey(Keys.Down, KeyModifiers.Alt);
            insertForwardArrow = HotKeyManager.RegisterHotKey(Keys.Right, KeyModifiers.Alt);

            hotKeyNumber2 = HotKeyManager.RegisterHotKey(Keys.Oem4, KeyModifiers.Control);
            hotKeyNumberImage2 = HotKeyManager.RegisterHotKey(Keys.Oem6, KeyModifiers.Control);
            insertIncreaseArrow2 = HotKeyManager.RegisterHotKey(Keys.OemQuotes, KeyModifiers.Control);
            insertDecreaseArrow2 = HotKeyManager.RegisterHotKey(Keys.OemSemicolon, KeyModifiers.Control);
            insertForwardArrow2 = HotKeyManager.RegisterHotKey(Keys.L, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(CopyKeyboardSearch);

            Application.Run();
        }

        static void CopyKeyboardSearch(object sender, HotKeyEventArgs e)
        {
            switch (e.Key)
            {

                case Keys.Down: 
                case Keys.OemSemicolon:
                    Thread.Sleep(200);
                    SendKeys.Send('\u2193'.ToString());
                    //SendKeys.Send(' '.ToString());
                    break;

                case Keys.Up:
                case Keys.OemQuotes:
                    Thread.Sleep(200);
                    SendKeys.Send('\u2191'.ToString());
                    //SendKeys.Send(' '.ToString());
                    break;

                case Keys.L:
                case Keys.Right:
                    Thread.Sleep(200);
                    SendKeys.Send('\u2192'.ToString());
                    SendKeys.Send(' '.ToString());
                    break;

                case Keys.Oem4:
                case Keys.C:
                    //Thread.Sleep(500);
                    SendKeys.Send("^c");
                    Thread.Sleep(200);//SendKeys.Send("^c");                    
                    string lastClipboardText = Clipboard.GetText();
                    if (lastClipboardText != "")
                        Process.Start("https://www.google.com/search?q=" +
                                      lastClipboardText.TrimStart(' ').TrimEnd(' ').Replace(" ", "+"));
                    break;
                case Keys.Oem6:
                case Keys.X:
                    //Thread.Sleep(500);
                    //SendKeys.Send("^c");
                    SendKeys.Send("^c");
                    Thread.Sleep(200);
                    string imageText = Clipboard.GetText();
                    if (imageText != "")
                        Process.Start("https://www.google.com/search?tbm=isch&q=" +
                                      imageText.TrimStart(' ').TrimEnd(' ').Replace(" ", "+"));
                    break;

                case Keys.A:
                    Thread.Sleep(200);
                    SendKeys.Send('\u03b1'.ToString());
                    break;

                case Keys.B:
                    Thread.Sleep(200);
                    SendKeys.Send('\u03b2'.ToString());
                    break;

                case Keys.D:
                    Thread.Sleep(200);
                    SendKeys.Send('\u03b4'.ToString());
                    break;

                case Keys.G:
                    Thread.Sleep(200);
                    SendKeys.Send('\u03b3'.ToString());
                    break;

                default:
                    break;
            }

        }

        static void Exit_Click(object sender, EventArgs e)
        {
            HotKeyManager.UnregisterHotKey(hotKeyNumber);
            HotKeyManager.UnregisterHotKey(hotKeyNumberImage);
            HotKeyManager.UnregisterHotKey(alpha);
            HotKeyManager.UnregisterHotKey(beta);
            HotKeyManager.UnregisterHotKey(delta);
            HotKeyManager.UnregisterHotKey(gamma);
            HotKeyManager.UnregisterHotKey(insertIncreaseArrow);
            HotKeyManager.UnregisterHotKey(insertDecreaseArrow);
            HotKeyManager.UnregisterHotKey(insertForwardArrow);
            HotKeyManager.UnregisterHotKey(hotKeyNumber2);
            HotKeyManager.UnregisterHotKey(hotKeyNumberImage2);
            HotKeyManager.UnregisterHotKey(insertIncreaseArrow2);
            HotKeyManager.UnregisterHotKey(insertDecreaseArrow2);
            HotKeyManager.UnregisterHotKey(insertForwardArrow2);
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
