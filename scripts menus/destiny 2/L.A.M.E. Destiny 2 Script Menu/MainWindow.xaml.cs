using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System.Configuration;
using System.IO;

namespace L.A.M.E._Destiny_2_Script_Menu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static IntPtr hookId = IntPtr.Zero;

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        const int MOUSEEVENTF_RIGHTUP = 0x10;

        const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private System.Windows.Controls.Button? m_button;

        Configuration config = ConfigurationManager.OpenExeConfiguration("L.A.M.E. Destiny 2 Script Menu.exe");

        private static MainWindow mainWindow;

        public MainWindow()
        {
            InitializeComponent();

            UI_Config_Updater();

            SetHook();

            mainWindow = this;

            lament_select_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            delay_one_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            delay_two_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            delay_three_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            swap_back_delay_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            jump_delay_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);
            dash_delay_box.PreviewTextInput += new TextCompositionEventHandler(OnlyAllowNumbers);

            KeyDown += Seasonal_Hotkey_Binder;
            KeyDown += Weapon_Hotkey_Binder;
            KeyDown += Fishing_Hotkey_Binder;
            KeyDown += Super_Key_Binder;
            KeyDown += Dive_Key_Binder;
            KeyDown += Snapskate_Hotkey_Binder;
            KeyDown += Shatterskate_Hotkey_Binder;
            KeyDown += Wellskate_Hotkey_Binder;
            KeyDown += Lament_Hotkey_Binder;

        }

        public static class GlobalVariables
        {
            public static bool handle_hotkeys = true;

            public static bool seasonal_binding = false;
            public static bool weapon_binding = false;
            public static bool fishing_binding = false;
            public static bool super_binding = false;
            public static bool dive_binding = false;
            public static bool snapskate_binding = false;
            public static bool shatterskate_binding = false;
            public static bool wellskate_binding = false;
            public static bool lament_binding = false;

            public static bool afk_weapon_enabled = false;
            public static bool afk_fishing_enabled = false;
            public static bool afk_seasonal_enabled = false;

            public static bool afk_seasonal_titan_enabled = false;
            public static bool afk_seasonal_hunter_enabled = false;

            public static bool lament_repeat_infinite_enabled = false;
            public static bool lament_repeat_held_enabled = false;
            public static bool lament_repeat_none_enabled = false;
            public static bool lament_repeat_select_enabled = false;
            public static bool lament_loop_enabled = false;

            public static bool afk_fishing_afk_enabled = false;

            public static bool dash_after_skate = false;
            public static bool jump_after_skate = false;
            public static bool swap_back_after_skate = false;

        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration("L.A.M.E. Destiny 2 Script Menu.exe");

                    var seasonal_xp_hotkey = config.AppSettings.Settings["seasonal_xp_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["seasonal_xp_hotkey"].Value) : default(Keys);
                    var weapon_xp_hotkey = config.AppSettings.Settings["weapon_xp_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["weapon_xp_hotkey"].Value) : default(Keys);
                    var fishing_hotkey = config.AppSettings.Settings["fishing_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["fishing_hotkey"].Value) : default(Keys);
                    var snapskate_hotkey = config.AppSettings.Settings["snapskate_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["snapskate_hotkey"].Value) : default(Keys);
                    var shatterskate_hotkey = config.AppSettings.Settings["shatterskate_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["shatterskate_hotkey"].Value) : default(Keys);
                    var wellskate_hotkey = config.AppSettings.Settings["wellskate_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["wellskate_hotkey"].Value) : default(Keys);
                    var lament_hotkey = config.AppSettings.Settings["lament_hotkey"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["lament_hotkey"].Value) : default(Keys);

                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;

                    if (GlobalVariables.handle_hotkeys)
                    {

                        if (key == seasonal_xp_hotkey)
                        {
                            mainWindow.AFK_Seasonal_XP_Toggle();
                        }
                        else if (key == weapon_xp_hotkey)
                        {
                            mainWindow.AFK_Weapon_XP_Toggle();
                        }
                        else if (key == fishing_hotkey)
                        {
                            mainWindow.AFK_Fishing_Toggle();
                        }
                        else if (key == snapskate_hotkey)
                        {
                            mainWindow.Snapskate();
                        }
                        else if (key == shatterskate_hotkey)
                        {
                            mainWindow.Shatterskate();
                        }
                        else if (key == wellskate_hotkey)
                        {
                            mainWindow.Wellskate();
                        }
                        else if (key == lament_hotkey)
                        {
                             mainWindow.Lament();
                        }
                        else
                        {

                        }
                    }
                }
                catch
                {

                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private IntPtr SetHook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private void OnlyAllowNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        public void FocusApplication(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);

            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
            }
            else
            {
                System.Windows.MessageBox.Show("Window not found!");
            }
        }

        public void HoldKey(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        public void ReleaseKey(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public void SendLeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void SendRightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        private void Seasonal_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.seasonal_binding = true;
        }

        private void Seasonal_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.seasonal_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("seasonal_xp_hotkey");
                config.AppSettings.Settings.Add("seasonal_xp_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["seasonal_xp_hotkey"].Value;
                GlobalVariables.seasonal_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Weapon_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.weapon_binding = true;
        }

        private void Weapon_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.weapon_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("weapon_xp_hotkey");
                config.AppSettings.Settings.Add("weapon_xp_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["weapon_xp_hotkey"].Value;
                GlobalVariables.weapon_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Fishing_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.fishing_binding = true;
        }

        private void Fishing_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.fishing_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("fishing_hotkey");
                config.AppSettings.Settings.Add("fishing_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["fishing_hotkey"].Value;
                GlobalVariables.fishing_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Super_Key_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.super_binding = true;
        }

        private void Super_Key_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.super_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("super_key");
                config.AppSettings.Settings.Add("super_key", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["super_key"].Value;
                GlobalVariables.super_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Dive_Key_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.dive_binding = true;
        }

        private void Dive_Key_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.dive_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("dive_key");
                config.AppSettings.Settings.Add("dive_key", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["dive_key"].Value;
                GlobalVariables.dive_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Snapskate_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.snapskate_binding = true;
        }

        private void Snapskate_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.snapskate_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("snapskate_hotkey");
                config.AppSettings.Settings.Add("snapskate_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["snapskate_hotkey"].Value;
                GlobalVariables.snapskate_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Shatterskate_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.shatterskate_binding = true;
        }

        private void Shatterskate_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.shatterskate_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("shatterskate_hotkey");
                config.AppSettings.Settings.Add("shatterskate_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["shatterskate_hotkey"].Value;
                GlobalVariables.shatterskate_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Wellskate_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.wellskate_binding = true;
        }

        private void Wellskate_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.wellskate_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("wellskate_hotkey");
                config.AppSettings.Settings.Add("wellskate_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["wellskate_hotkey"].Value;
                GlobalVariables.wellskate_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Lament_Hotkey_Binding(object sender, RoutedEventArgs e)
        {
            GlobalVariables.handle_hotkeys = false;
            m_button = (System.Windows.Controls.Button)sender;
            m_button.Content = "Binding...";
            GlobalVariables.lament_binding = true;
        }

        private void Lament_Hotkey_Binder(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var keyName = e.Key.ToString();
            if (GlobalVariables.lament_binding && m_button != null)
            {
                config.AppSettings.Settings.Remove("lament_hotkey");
                config.AppSettings.Settings.Add("lament_hotkey", e.Key.ToString());
                config.Save(ConfigurationSaveMode.Modified);
                m_button.Content = config.AppSettings.Settings["lament_hotkey"].Value;
                GlobalVariables.lament_binding = false;
                GlobalVariables.handle_hotkeys = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void scripts_button_Click(object sender, RoutedEventArgs e)
        {
            scripts_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            scripts_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            scripts_canvas.Visibility = Visibility.Visible;
            release_notes_canvas.Visibility = Visibility.Hidden;
            credits_canvas.Visibility = Visibility.Hidden;
        }

        private void release_notes_button_Click(object sender, RoutedEventArgs e)
        {
            release_notes_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            scripts_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            scripts_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            scripts_canvas.Visibility = Visibility.Hidden;
            release_notes_canvas.Visibility = Visibility.Visible;
            credits_canvas.Visibility = Visibility.Hidden;
        }

        private void credits_button_Click(object sender, RoutedEventArgs e)
        {
            credits_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            scripts_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            scripts_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            release_notes_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            scripts_canvas.Visibility = Visibility.Hidden;
            release_notes_canvas.Visibility = Visibility.Hidden;
            credits_canvas.Visibility = Visibility.Visible;
        }

        private void discord_button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.thrallway.com") { CreateNoWindow = true });
        }

        private void donate_button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.ko-fi.com/leopoldprime") { CreateNoWindow = true });
        }

        private void afk_seasonal_button_Click(object sender, RoutedEventArgs e)
        {
            afk_seasonal_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            afk_seasonal_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            afk_weapon_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_weapon_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_fishing_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_fishing_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            automatic_lament_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            movement_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            movement_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            afk_seasonal_canvas.Visibility = Visibility.Visible;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Hidden;
            movement_canvas.Visibility = Visibility.Hidden;
        }

        private void afk_weapon_button_Click(object sender, RoutedEventArgs e)
        {
            afk_weapon_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            afk_weapon_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_fishing_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_fishing_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            automatic_lament_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            movement_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            movement_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Visible;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Hidden;
            movement_canvas.Visibility = Visibility.Hidden;
        }

        private void afk_fishing_button_Click(object sender, RoutedEventArgs e)
        {
            afk_fishing_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            afk_fishing_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_weapon_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_weapon_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            automatic_lament_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            movement_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            movement_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Visible;
            auto_lament_canvas.Visibility = Visibility.Hidden;
            movement_canvas.Visibility = Visibility.Hidden;
        }

        private void automatic_lament_button_Click(object sender, RoutedEventArgs e)
        {
            afk_fishing_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_fishing_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_seasonal_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_weapon_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_weapon_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            movement_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            movement_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Visible;
            movement_canvas.Visibility = Visibility.Hidden;
        }

        private void afk_seasonal_titan_Click(object sender, RoutedEventArgs e)
        {
            afk_seasonal_titan.Background = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_hunter.Background = (SolidColorBrush)Resources["BackgroundColor"];
            GlobalVariables.afk_seasonal_titan_enabled = true;
            GlobalVariables.afk_seasonal_hunter_enabled = false;

            config.AppSettings.Settings.Remove("afk_seasonal_titan");
            config.AppSettings.Settings.Add("afk_seasonal_titan", "true");
        }

        private void afk_seasonal_hunter_Click(object sender, RoutedEventArgs e)
        {
            afk_seasonal_titan.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_seasonal_hunter.Background = (SolidColorBrush)Resources["ForegroundColor"];
            GlobalVariables.afk_seasonal_titan_enabled = false;
            GlobalVariables.afk_seasonal_hunter_enabled = true;

            config.AppSettings.Settings.Remove("afk_seasonal_hunter");
            config.AppSettings.Settings.Add("afk_seasonal_hunter", "true");
        }

        private void lament_repeat_infinite_button_Click(object sender, RoutedEventArgs e)
        {
            lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            GlobalVariables.lament_repeat_infinite_enabled = true;
            GlobalVariables.lament_repeat_held_enabled = false;
            GlobalVariables.lament_repeat_none_enabled = false;
            GlobalVariables.lament_repeat_select_enabled = false;

            config.AppSettings.Settings.Remove("lament_repeat_infinite");
            config.AppSettings.Settings.Add("lament_repeat_infinite", "true");
        }

        private void lament_repeat_none_button_Click(object sender, RoutedEventArgs e)
        {
            lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_none_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            GlobalVariables.lament_repeat_infinite_enabled = false;
            GlobalVariables.lament_repeat_held_enabled = false;
            GlobalVariables.lament_repeat_none_enabled = true;
            GlobalVariables.lament_repeat_select_enabled = false;

            config.AppSettings.Settings.Remove("lament_repeat_none");
            config.AppSettings.Settings.Add("lament_repeat_none", "true");

        }

        private void lament_repeat_select_button_Click(object sender, RoutedEventArgs e)
        {
            lament_repeat_select_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            GlobalVariables.lament_repeat_infinite_enabled = false;
            GlobalVariables.lament_repeat_held_enabled = false;
            GlobalVariables.lament_repeat_none_enabled = false;
            GlobalVariables.lament_repeat_select_enabled = true;

            config.AppSettings.Settings.Remove("lament_repeat_select");
            config.AppSettings.Settings.Add("lament_repeat_select", "true");
        }

        private void lament_repeat_held_button_Click(object sender, RoutedEventArgs e)
        {
            lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_held_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            GlobalVariables.lament_repeat_infinite_enabled = false;
            GlobalVariables.lament_repeat_held_enabled = true;
            GlobalVariables.lament_repeat_none_enabled = false;
            GlobalVariables.lament_repeat_select_enabled = false;

            config.AppSettings.Settings.Remove("lament_repeat_held");
            config.AppSettings.Settings.Add("lament_repeat_held", "true");
        }

        public void AFK_Seasonal_XP_Toggle()
        {
            if (GlobalVariables.afk_seasonal_enabled == false)
            {
                GlobalVariables.afk_seasonal_enabled = true;
            }
            else if (GlobalVariables.afk_seasonal_enabled == true)
            {
                GlobalVariables.afk_seasonal_enabled = false;
            }
            AFK_Seasonal_XP();
        }

        public void AFK_Weapon_XP_Toggle()
        {
            if (GlobalVariables.afk_weapon_enabled == false)
            {
                GlobalVariables.afk_weapon_enabled = true;
            }
            else if (GlobalVariables.afk_weapon_enabled == true)
            {
                GlobalVariables.afk_weapon_enabled = false;
            }
            AFK_Weapon_XP();
        }

        public void AFK_Fishing_Toggle()
        {
            if (GlobalVariables.afk_fishing_enabled == false)
            {
                GlobalVariables.afk_fishing_enabled = true;
            }
            else if (GlobalVariables.afk_fishing_enabled == true)
            {
                GlobalVariables.afk_fishing_enabled = false;
            }
            AFK_Fishing();
        }

        public void AFK_Seasonal_XP()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            if (GlobalVariables.afk_seasonal_titan_enabled == true)
            {
                controller.Connect();

                Task.Run(() =>
                {
                    while (GlobalVariables.afk_seasonal_enabled)
                    {
                        controller.SetButtonState(Xbox360Button.LeftShoulder, true);
                        Thread.Sleep(100);
                        controller.SetButtonState(Xbox360Button.LeftShoulder, false);
                        Thread.Sleep(100);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                        Thread.Sleep(300);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        Thread.Sleep(int.Parse(config.AppSettings.Settings["seasonal_xp_delay"]?.Value ?? "0"));
                    }

                    if (!GlobalVariables.afk_seasonal_enabled)
                    {
                        controller.Disconnect();
                    }
                });
            }
            else if (GlobalVariables.afk_seasonal_hunter_enabled == true)
            {
                controller.Connect();

                Task.Run(() =>
                {
                    while (GlobalVariables.afk_seasonal_enabled)
                    {
                        controller.SetButtonState(Xbox360Button.RightShoulder, true);
                        Thread.Sleep(100);
                        controller.SetButtonState(Xbox360Button.RightShoulder, false);
                        Thread.Sleep(100);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                        Thread.Sleep(300);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        Thread.Sleep(int.Parse(config.AppSettings.Settings["seasonal_xp_delay"]?.Value ?? "0"));
                    }

                    if (!GlobalVariables.afk_seasonal_enabled)
                    {
                        controller.Disconnect();
                    }
                });
            }
            else
            {
                System.Windows.MessageBox.Show("You Do Not Have A Class Selected, Please Select A Class");
                GlobalVariables.afk_seasonal_enabled = false;
            }
        }

        private void AFK_Weapon_XP()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            controller.Connect();

            Task.Run(() =>
            {
                while (GlobalVariables.afk_weapon_enabled)
                {
                    controller.SetSliderValue(Xbox360Slider.RightTrigger, 100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                    System.Threading.Thread.Sleep(200);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, -32768);
                    System.Threading.Thread.Sleep(3000);
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
                }

                if (!GlobalVariables.afk_weapon_enabled)
                {
                    controller.Disconnect();
                }
            });
        }

        private void AFK_Fishing()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            controller.Connect();

            controller.SetButtonState(Xbox360Button.X, true);
            Thread.Sleep(1000);
            controller.SetButtonState(Xbox360Button.X, false);

            if (GlobalVariables.afk_fishing_enabled)
            {
                controller.FeedbackReceived += (sender, eventArgs) =>
                {
                    if (eventArgs.LargeMotor == 255)
                    {
                        GlobalVariables.afk_fishing_afk_enabled = false;

                        controller.SetButtonState(Xbox360Button.X, true);
                        Thread.Sleep(1000);
                        controller.SetButtonState(Xbox360Button.X, false);

                        GlobalVariables.afk_fishing_afk_enabled = true;
                    }
                };
            }
            else if (!GlobalVariables.afk_fishing_enabled)
            {
                controller.FeedbackReceived -= (sender, eventArgs) =>
                {
                    controller.Disconnect();
                    return;
                };
            }

            Task.Run(() =>
            {
                while (GlobalVariables.afk_fishing_afk_enabled)
                {
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, -32768);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 32767);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                }

                while (GlobalVariables.afk_fishing_enabled)
                {
                    if (GlobalVariables.afk_fishing_enabled)
                    {
                        controller.FeedbackReceived += (sender, eventArgs) =>
                        {
                            Thread.Sleep(207);

                            if (eventArgs.LargeMotor == 1)
                            {
                                GlobalVariables.afk_fishing_afk_enabled = false;

                                Thread.Sleep(1060);
                                controller.SetButtonState(Xbox360Button.X, true);
                                Thread.Sleep(1000);
                                controller.SetButtonState(Xbox360Button.X, false);

                                GlobalVariables.afk_fishing_afk_enabled = true;
                            }
                        };
                    }
                    else if (!GlobalVariables.afk_fishing_enabled)
                    {
                        controller.FeedbackReceived -= (sender, eventArgs) =>
                        {
                            return;
                        };
                    }
                }
            });
        }

        private void Lament()
        {
            Task.Run(() =>
            {
                if (GlobalVariables.lament_repeat_infinite_enabled)
                {
                    if (GlobalVariables.lament_loop_enabled == false)
                    {
                        GlobalVariables.lament_loop_enabled = true;
                    }
                    else if (GlobalVariables.lament_loop_enabled == true)
                    {
                        GlobalVariables.lament_loop_enabled = false;
                    }
                    while (GlobalVariables.lament_loop_enabled)
                    {
                        HoldKey(Keys.C);
                        Thread.Sleep(300);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        ReleaseKey(Keys.C);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendRightClick();
                        Thread.Sleep(600);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(300);
                        SendLeftClick();
                        Thread.Sleep(1250);
                    }
                }
                else if (GlobalVariables.lament_repeat_none_enabled)
                {
                    HoldKey(Keys.C);
                    Thread.Sleep(300);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    ReleaseKey(Keys.C);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendRightClick();
                    Thread.Sleep(600);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(300);
                    SendLeftClick();
                    Thread.Sleep(1250);
                }
                else if (GlobalVariables.lament_repeat_select_enabled)
                {
                    Dispatcher.Invoke(() =>
                    {
                        int repeatCount = 0;
                        if (Int32.TryParse(lament_select_box.Text, out repeatCount))
                        {
                            for (int i = 0; i < repeatCount; i++)
                            {
                                HoldKey(Keys.C);
                                Thread.Sleep(300);
                                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                                ReleaseKey(Keys.C);
                                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendRightClick();
                                Thread.Sleep(600);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(300);
                                SendLeftClick();
                                Thread.Sleep(1250);
                            }
                        }
                    });

                }
                else if (GlobalVariables.lament_repeat_held_enabled)
                {

                }
                else
                {
                    System.Windows.MessageBox.Show("You Do Not Have A Loop Option Selected, Please Select A Loop Option");
                }
            });
        }

        private void Wellskate()
        {
            Task.Run(() =>
            {
                HoldKey(Keys.D3);
                ReleaseKey(Keys.D3);
                Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_one"]?.Value ?? "0"));
                SendRightClick();
                Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_two"]?.Value ?? "0"));
                HoldKey(Keys.Space);
                ReleaseKey(Keys.Space);
                Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_three"]?.Value ?? "0"));
                SendKeys.SendWait(config.AppSettings.Settings["super_key"].Value);

                if (GlobalVariables.swap_back_after_skate)
                {
                    Thread.Sleep(int.Parse(config.AppSettings.Settings["swap_back_after_delay"]?.Value ?? "0"));
                    SendKeys.SendWait(config.AppSettings.Settings["swap_back_button"].Value);
                }

                if (GlobalVariables.jump_after_skate)
                {
                    Thread.Sleep(int.Parse(config.AppSettings.Settings["jump_after_delay"]?.Value ?? "0"));
                    HoldKey(Keys.Space);
                    ReleaseKey(Keys.Space);
                }

                if (GlobalVariables.dash_after_skate)
                {
                    Thread.Sleep(int.Parse(config.AppSettings.Settings["dash_after_delay"]?.Value ?? "0" ));
                    SendKeys.SendWait(config.AppSettings.Settings["dive_key"].Value);
                }
            });
        }

        private void Shatterskate()
        {
            HoldKey(Keys.D3);
            Thread.Sleep(500);
            ReleaseKey(Keys.D3);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_one"]?.Value ?? "0"));
            SendRightClick();
            Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_two"]?.Value ?? "0"));
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["delay_three"]?.Value ?? "0"));
            SendKeys.Send(config.AppSettings.Settings["dive_key"].Value);

            if (GlobalVariables.swap_back_after_skate)
            {
                Thread.Sleep(int.Parse(config.AppSettings.Settings["swap_back_after_delay"]?.Value ?? "0"));
                SendKeys.Send(config.AppSettings.Settings["swap_back_button"].Value);
            }

            if (GlobalVariables.jump_after_skate)
            {
                Thread.Sleep(int.Parse(config.AppSettings.Settings["jump_after_delay"]?.Value ?? "0"));
                HoldKey(Keys.Space);
                ReleaseKey(Keys.Space);
            }
        }

        private void Snapskate()
        {
            System.Windows.MessageBox.Show("Snapskate is currently in development, please use Shatterskate or Wellskate for now.");
        }

        private void afk_seasonal_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Seasonal_Hotkey_Binding(sender, e);
        }

        private void afk_weapon_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Weapon_Hotkey_Binding(sender, e);
        }

        private void auto_lament_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Lament_Hotkey_Binding(sender, e);
        }

        private void afk_fishing_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Fishing_Hotkey_Binding(sender, e);
        }

        private void super_bind_button_Click(object sender, RoutedEventArgs e)
        {
            Super_Key_Binding(sender, e);
        }

        private void dive_bind_button_Click(object sender, RoutedEventArgs e)
        {
            Dive_Key_Binding(sender, e);
        }

        private void movement_snapskate_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Snapskate_Hotkey_Binding(sender, e);
        }

        private void movement_shatterskate_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Shatterskate_Hotkey_Binding(sender, e);
        }

        private void movement_wellskate_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            Wellskate_Hotkey_Binding(sender, e);
        }

        private void movement_button_Click(object sender, RoutedEventArgs e)
        {
            afk_fishing_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_fishing_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_seasonal_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_weapon_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            afk_weapon_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            automatic_lament_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            automatic_lament_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            movement_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            movement_button.Background = (SolidColorBrush)Resources["ForegroundColor"];

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Hidden;
            movement_canvas.Visibility = Visibility.Visible;
        }

        private void dash_delay_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("dash_after_delay");
            config.AppSettings.Settings.Add("dash_after_delay", dash_delay_box.Text);
        }

        private void jump_delay_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("jump_after_delay");
            config.AppSettings.Settings.Add("jump_after_delay", jump_delay_box.Text);
        }

        private void swap_back_delay_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("swap_back_delay");
            config.AppSettings.Settings.Add("swap_back_delay", swap_back_delay_box.Text);
        }

        private void delay_three_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("delay_three");
            config.AppSettings.Settings.Add("delay_three", delay_three_box.Text);
        }

        private void delay_two_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("delay_two");
            config.AppSettings.Settings.Add("delay_two", delay_two_box.Text);
        }

        private void delay_one_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("delay_one");
            config.AppSettings.Settings.Add("delay_one", delay_one_box.Text);
        }

        private void swap_back_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.swap_back_after_skate == true)
            {
                GlobalVariables.swap_back_after_skate = false;
                swap_back_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];

                config.AppSettings.Settings.Remove("swap_back_after_skate");
                config.AppSettings.Settings.Add("swap_back_after_skate", "false");
            }
            else if (GlobalVariables.swap_back_after_skate == false)
            {
                GlobalVariables.swap_back_after_skate = true;
                swap_back_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];

                config.AppSettings.Settings.Remove("swap_back_after_skate");
                config.AppSettings.Settings.Add("swap_back_after_skate", "true");
            }
        }

        private void dash_after_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.dash_after_skate == true)
            {
                GlobalVariables.dash_after_skate = false;
                dash_after_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];

                config.AppSettings.Settings.Remove("dash_after_skate");
                config.AppSettings.Settings.Add("dash_after_skate", "false");
            }
            else if (GlobalVariables.dash_after_skate == false)
            {
                GlobalVariables.dash_after_skate = true;
                dash_after_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];

                config.AppSettings.Settings.Remove("dash_after_skate");
                config.AppSettings.Settings.Add("dash_after_skate", "true");
            }
        }

        private void jump_after_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.jump_after_skate == true)
            {
                GlobalVariables.jump_after_skate = false;
                jump_after_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];

                config.AppSettings.Settings.Remove("jump_after_skate");
                config.AppSettings.Settings.Add("jump_after_skate", "false");
            }
            else if (GlobalVariables.jump_after_skate == false)
            {
                GlobalVariables.jump_after_skate = true;
                jump_after_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];

                config.AppSettings.Settings.Remove("jump_after_skate");
                config.AppSettings.Settings.Add("jump_after_skate", "true");
            }
        }

        private void swap_back_choice_toggle_one_Click(object sender, RoutedEventArgs e)
        {
            swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["ForegroundColor"];
            swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["BackgroundColor"];
            swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["BackgroundColor"];

            config.AppSettings.Settings.Remove("swap_back_button");
            config.AppSettings.Settings.Add("swap_back_button", "1");
        }

        private void swap_back_choice_toggle_two_Click(object sender, RoutedEventArgs e)
        {
            swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["BackgroundColor"];
            swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["ForegroundColor"];
            swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["BackgroundColor"];

            config.AppSettings.Settings.Remove("swap_back_button");
            config.AppSettings.Settings.Add("swap_back_button", "2");
        }

        private void swap_back_choice_toggle_three_Click(object sender, RoutedEventArgs e)
        {
            swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["BackgroundColor"];
            swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["BackgroundColor"];
            swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["ForegroundColor"];

            config.AppSettings.Settings.Remove("swap_back_button");
            config.AppSettings.Settings.Add("swap_back_button", "3");
        }

        private void seasonal_xp_delaye_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("seasonal_xp_delay");
            config.AppSettings.Settings.Add("seasonal_xp_delay", seasonal_xp_delaye_box.Text);
        }

        private void UI_Config_Updater()
        {
            if (config.AppSettings.Settings["seasonal_xp_hotkey"]?.Value == "null")
            {
                afk_seasonal_toggle_button.Content = "Unbinded";
            }
            else
            {
                afk_seasonal_toggle_button.Content = config.AppSettings.Settings["seasonal_xp_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["weapon_xp_hotkey"]?.Value == "null")
            {
                afk_weapon_toggle_button.Content = "Unbinded";
            }
            else
            {
                afk_weapon_toggle_button.Content = config.AppSettings.Settings["weapon_xp_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["fishing_hotkey"]?.Value == "null")
            {
                afk_fishing_toggle_button.Content = "Unbinded";
            }
            else
            {
                afk_fishing_toggle_button.Content = config.AppSettings.Settings["fishing_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["super_key"]?.Value == "null")
            {
                super_bind_button.Content = "Unbinded";
            }
            else
            {
                super_bind_button.Content = config.AppSettings.Settings["super_key"]?.Value;
            }

            if (config.AppSettings.Settings["dive_key"]?.Value == "null")
            {
                dive_bind_button.Content = "Unbinded";
            }
            else
            {
                dive_bind_button.Content = config.AppSettings.Settings["dive_key"]?.Value;
            }

            if (config.AppSettings.Settings["snapskate_hotkey"]?.Value == "null")
            {
                movement_snapskate_toggle_button.Content = "Unbinded";
            }
            else
            {
                movement_snapskate_toggle_button.Content = config.AppSettings.Settings["snapskate_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["shatterskate_hotkey"]?.Value == "null")
            {
                movement_shatterskate_toggle_button.Content = "Unbinded";
            }
            else
            {
                movement_shatterskate_toggle_button.Content = config.AppSettings.Settings["shatterskate_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["wellskate_hotkey"]?.Value == "null")
            {
                movement_wellskate_toggle_button.Content = "Unbinded";
            }
            else
            {
                movement_wellskate_toggle_button.Content = config.AppSettings.Settings["wellskate_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["lament_hotkey"]?.Value == "null")
            {
                auto_lament_toggle_button.Content = "Unbinded";
            }
            else
            {
                auto_lament_toggle_button.Content = config.AppSettings.Settings["lament_hotkey"]?.Value;
            }

            if (config.AppSettings.Settings["swap_back_button"]?.Value == "1")
            {
                swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["ForegroundColor"];
                swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (config.AppSettings.Settings["swap_back_button"]?.Value == "2")
            {
                swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["ForegroundColor"];
                swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (config.AppSettings.Settings["swap_back_button"]?.Value == "3")
            {
                swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["ForegroundColor"];
            }
            else
            {
                swap_back_choice_toggle_one.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_two.Background = (SolidColorBrush)Resources["BackgroundColor"];
                swap_back_choice_toggle_three.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }

            if (config.AppSettings.Settings["lament_repeat_infinite"]?.Value == "true")
            {
                lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
                lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (config.AppSettings.Settings["lament_repeat_held"]?.Value == "true")
            {
                lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_held_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
                lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (config.AppSettings.Settings["lament_repeat_none"]?.Value == "true")
            {
                lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_none_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
                lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (config.AppSettings.Settings["lament_repeat_select"]?.Value == "true")
            {
                lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_select_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            }
            else
            {
                lament_repeat_infinite_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_held_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_none_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
                lament_repeat_select_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }

            if (config.AppSettings.Settings["swap_back_after_skate"]?.Value == "true")
            {
                swap_back_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];
                GlobalVariables.swap_back_after_skate = true;
            }
            else if (config.AppSettings.Settings["swap_back_after_skate"]?.Value == "false")
            {
                swap_back_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];
                GlobalVariables.swap_back_after_skate = false;
            }

            if (config.AppSettings.Settings["jump_after_skate"]?.Value == "true")
            {
                jump_after_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];
                GlobalVariables.jump_after_skate = true;
            }
            else if (config.AppSettings.Settings["jump_after_skate"]?.Value == "false")
            {
                jump_after_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];
                GlobalVariables.jump_after_skate = false;
            }

            if (config.AppSettings.Settings["dash_after_skate"]?.Value == "true")
            {
                dash_after_toggle.Background = (SolidColorBrush)Resources["ForegroundColor"];
                GlobalVariables.dash_after_skate = true;
            }
            else if (config.AppSettings.Settings["dash_after_skate"]?.Value == "false")
            {
                dash_after_toggle.Background = (SolidColorBrush)Resources["BackgroundColor"];
                GlobalVariables.dash_after_skate = false;
            }

            if (config.AppSettings.Settings["afk_seasonal_titan"]?.Value == "true")
            {
                afk_seasonal_titan.Background = (SolidColorBrush)Resources["ForegroundColor"];
                afk_seasonal_hunter.Background = (SolidColorBrush)Resources["BackgroundColor"];
                GlobalVariables.afk_seasonal_titan_enabled = true;
                GlobalVariables.afk_seasonal_hunter_enabled = false;
            }
            else if (config.AppSettings.Settings["afk_seasonal_hunter"]?.Value == "true")
            {
                afk_seasonal_titan.Background = (SolidColorBrush)Resources["BackgroundColor"];
                afk_seasonal_hunter.Background = (SolidColorBrush)Resources["ForegroundColor"];
                GlobalVariables.afk_seasonal_titan_enabled = false;
                GlobalVariables.afk_seasonal_hunter_enabled = true;
            }
            else
            {
                afk_seasonal_titan.Background = (SolidColorBrush)Resources["BackgroundColor"];
                afk_seasonal_hunter.Background = (SolidColorBrush)Resources["BackgroundColor"];
                GlobalVariables.afk_seasonal_titan_enabled = false;
                GlobalVariables.afk_seasonal_hunter_enabled = false;
            }

            seasonal_xp_delaye_box.Text = config.AppSettings.Settings["seasonal_xp_delay"].Value;
            delay_one_box.Text = config.AppSettings.Settings["delay_one"].Value;
            delay_two_box.Text = config.AppSettings.Settings["delay_two"].Value;
            delay_three_box.Text = config.AppSettings.Settings["delay_three"].Value;
            swap_back_delay_box.Text = config.AppSettings.Settings["swap_back_delay"].Value;
            jump_delay_box.Text = config.AppSettings.Settings["jump_after_delay"].Value;
            dash_delay_box.Text = config.AppSettings.Settings["dash_after_delay"].Value;
        }
    }
}