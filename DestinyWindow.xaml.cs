using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DiscordRPC.Logging;
using DiscordRPC;
using HubHelper;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Ink;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Nefarius.ViGEm.Client;

namespace LAME_Hub
{
    /// <summary>
    /// Interaction logic for DestinyWindow.xaml
    /// </summary>
    public partial class DestinyWindow : Window
    {

        public DestinyWindow()
        {
            InitializeComponent();

            SeasonalXPDelay.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            FishingDelay.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            RotationRepeatSelectBox.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            LoadoutSwapperDelayBox.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
        }

        public class GlobalVariables
        {
            public static bool binding = false;
            public static bool SeasonalXPTitanMode = false;

            public static bool LamentRepeatInfiniteEnabled = false;
            public static bool LamentRepeatHeldEnabled = false;
            public static bool LamentRepeatOnceEnabled = false;
            public static bool LamentRepeatSelectEnabled = false;

            public static bool SeasonalXPEnabled = false;
        }

        UIHandler UIHandler = new UIHandler();

        HotkeyHandler HotkeyHandler = new HotkeyHandler();

        public DiscordRpcClient client;

        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static IntPtr hookId = IntPtr.Zero;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        private static DestinyWindow? destinyWindow;

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                try
                {
                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;

                    var SeasonalXPBind = config.AppSettings.Settings["SeasonalXPBind"] != null ? (Keys)Enum.Parse(typeof(Keys), config.AppSettings.Settings["SeasonalXPBind"].Value) : default(Keys);

                    if (GlobalVariables.binding)
                    {

                        if (key == SeasonalXPBind)
                        {
                            destinyWindow.SeasonalXPToggle();
                            System.Windows.MessageBox.Show("worked");
                        }
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("Something Failed");
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

        private static void Unhook()
        {
            UnhookWindowsHookEx(hookId);
        }

        private void NavbarButtonClicked(object sender, RoutedEventArgs e)
        {
            UIHandler.NavbarButtonClicked(sender, Navbar, canvasParent);
        }

        private void EnableWindowDragable(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch
            {
                // MessageBox.Show(" Error Code EWDF ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        public string GetAvatarURL(User.AvatarFormat format, User.AvatarSize size)
        {
            format = User.AvatarFormat.PNG;
            size = User.AvatarSize.x128;
            return $"https://cdn.discordapp.com/avatars/{client.CurrentUser.ID}/{client.CurrentUser.Avatar}.{format.ToString().ToLower()}?size={size.ToString().Substring(1)}";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            client = new DiscordRpcClient("1195005916584620212");

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();

            client.OnReady += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    string avatarURL = GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x128);

                    UserPFP.Source = new BitmapImage(new Uri(avatarURL));
                    Username.Content = "Username : " + e.User.Username + "#0001";
                    DisplayName.Content = "Display Name : " + e.User.DisplayName;
                    UserID.Content = "Discord ID : " + e.User.ID;
                });
            };

            client.SetPresence(new RichPresence()
            {
                Details = "Using LAME Hub",
                State = "In The Destiny 2 Script Menu",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "LAME Hub Logo",
                    SmallImageKey = "logo"
                }
            });
        }

        private void SaveThemeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Dispose();
        }

        private void SeasonalXPDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("SeasonalXPDelay");
            config.AppSettings.Settings.Add("SeasonalXPDelay", SeasonalXPDelay.Text);
        }

        private void SeasonalXPBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(SeasonalXPBind, this, "SeasonalXPBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void SeasonalXPTitanMode_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.SeasonalXPTitanMode = true;
            SeasonalXPTitanMode.Background = (SolidColorBrush)Resources["ForegroundColor"];
            SeasonalXPHunterMode.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void SeasonalXPHunterMode_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.SeasonalXPTitanMode = false;
            SeasonalXPHunterMode.Background = (SolidColorBrush)Resources["ForegroundColor"];
            SeasonalXPTitanMode.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void WeaponXPBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(WeaponXPBind, this, "WeaponXPBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void FishingXPDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("FishingDelay");
            config.AppSettings.Settings.Add("FishingDelay", FishingDelay.Text);
        }

        private void RotationRepeatInfinite_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = true;
            GlobalVariables.LamentRepeatHeldEnabled = false;
            GlobalVariables.LamentRepeatOnceEnabled = false;
            GlobalVariables.LamentRepeatSelectEnabled = false;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["ForegroundColor"];
            RotationRepeatHeld.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void RotationRepeatOnce_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = false;
            GlobalVariables.LamentRepeatHeldEnabled = false;
            GlobalVariables.LamentRepeatOnceEnabled = true;
            GlobalVariables.LamentRepeatSelectEnabled = false;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatHeld.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["ForegroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void RotationRepeatHeld_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = false;
            GlobalVariables.LamentRepeatHeldEnabled = true;
            GlobalVariables.LamentRepeatOnceEnabled = false;
            GlobalVariables.LamentRepeatSelectEnabled = false;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatHeld.Background = (SolidColorBrush)Resources["ForegroundColor"];
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void RotationRepeatSelect_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = false;
            GlobalVariables.LamentRepeatHeldEnabled = false;
            GlobalVariables.LamentRepeatOnceEnabled = false;
            GlobalVariables.LamentRepeatSelectEnabled = true;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatHeld.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["ForegroundColor"];
        }

        private void RotationRepeatSelectBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("RotationRepeatSelectBox");
            config.AppSettings.Settings.Add("RotationRepeatSelectBox", RotationRepeatSelectBox.Text);
        }

        private void LamentRotationBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LamentRotationBind, this, "LamentRotationBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutOneBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutOneBind, this, "LoadoutOneBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutTwoBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutTwoBind, this, "LoadoutTwoBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutThreeBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutThreeBind, this, "LoadoutThreeBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutFourBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutFourBind, this, "LoadoutFourBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutFiveBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutFiveBind, this, "LoadoutFiveBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutSixBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutSixBind, this, "LoadoutSixBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutSevenBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutSevenBind, this, "LoadoutSevenBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutEightBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutEightBind, this, "LoadoutEightBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutNineBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutNineBind, this, "LoadoutNineBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutTenBind_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.binding = true;

            HotkeyHandler.HotkeyBinding(LoadoutTenBind, this, "LoadoutTenBind", "LAME Hub.dll");

            GlobalVariables.binding = false;
        }

        private void LoadoutSwapperDelayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("LoadoutSwapperDelayBox");
            config.AppSettings.Settings.Add("LoadoutSwapperDelayBox", LoadoutSwapperDelayBox.Text);
        }

        public void SeasonalXPToggle()
        {
            if (GlobalVariables.SeasonalXPEnabled == false)
            {
                GlobalVariables.SeasonalXPEnabled = true;
            }
            else if (GlobalVariables.SeasonalXPEnabled == true)
            {
                GlobalVariables.SeasonalXPEnabled = false;
            }
            SeasonalXPScript();
        }

        private void SeasonalXPScript()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            if (GlobalVariables.SeasonalXPTitanMode == true)
            {
                controller.Connect();

                Task.Run(() =>
                {
                    while (GlobalVariables.SeasonalXPEnabled)
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

                    if (!GlobalVariables.SeasonalXPEnabled)
                    {
                        controller.Disconnect();
                    }
                });
            }
            else if (GlobalVariables.SeasonalXPTitanMode == false)
            {
                controller.Connect();

                Task.Run(() =>
                {
                    while (GlobalVariables.SeasonalXPEnabled)
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

                    if (!GlobalVariables.SeasonalXPEnabled)
                    {
                        controller.Disconnect();
                    }
                });
            }
            else
            {
                System.Windows.MessageBox.Show("You Do Not Have A Class Selected, Please Select A Class");
                GlobalVariables.SeasonalXPEnabled = false;
            }
        }
    }
}
