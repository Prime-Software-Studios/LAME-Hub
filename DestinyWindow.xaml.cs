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
using DiscordRPC.Logging;
using DiscordRPC;
using HubHelper;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Ink;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Nefarius.ViGEm.Client;
using Hotkeys;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.IO;

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

            TextBoxCheck();

            LoadingScreen();

            SeasonalXPDelay.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            FishingDelay.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            RotationRepeatSelectBox.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            LoadoutSwapperDelayBox.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            MovementDelayOne.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            MovementDelayTwo.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            MovementDelayThree.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
            MovementSwapBackDelay.PreviewTextInput += new TextCompositionEventHandler(UIHandler.OnlyAllowNumbers);
        }

        public class GlobalVariables
        {
            public static bool SeasonalXPTitanMode = false;

            public static bool LamentRepeatInfiniteEnabled = false;
            public static bool LamentRepeatHeldEnabled = false;
            public static bool LamentRepeatOnceEnabled = false;
            public static bool LamentRepeatSelectEnabled = false;

            public static bool SeasonalXPEnabled = false;

            public static bool WeaponXPEnabled = false;

            public static bool FishingEnabled = false;

            public static bool PrimarySwapBack = true;
            public static bool SpecialSwapBack = false;
            public static bool HeavySwapBack = false;

            public static bool ShatterdiveMode = false;
        }

        UIHandler UIHandler = new UIHandler();

        HotkeyHandler HotkeyHandler = new HotkeyHandler();

        public DiscordRpcClient client;

        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        const int MOUSEEVENTF_RIGHTUP = 0x10;

        const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
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
        public void HoldKey(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }
        public void ReleaseKey(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        private void NavbarButtonClicked(object sender, RoutedEventArgs e)
        {
            NavbarButtonClicked2(sender, Navbar, canvasParent);
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

                    Username.Content = e.User.Username + "#0001";
                    DisplayName.Content = e.User.DisplayName;
                    UserID.Content = e.User.ID;

                    DisplayName2.Content = e.User.DisplayName;
                    UserPFP2.Source = new BitmapImage(new Uri(avatarURL));

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
            config.Save(ConfigurationSaveMode.Full);
        }
        private void SeasonalXPBind_Click(object sender, RoutedEventArgs e)
        {
            SeasonalXPBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    SeasonalXPBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["SeasonalXPBind"]?.Value == null)
                    {
                        SeasonalXPBind.Content = "Bind";
                    }
                    else
                    {
                        SeasonalXPBind.Content = "Binded To " + config.AppSettings.Settings["SeasonalXPBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Seasonal XP Hotkey",
                        action: hotkey => SeasonalXPToggle()
                        );

                    config.AppSettings.Settings.Remove("SeasonalXPBind");
                    config.AppSettings.Settings.Add("SeasonalXPBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    SeasonalXPBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
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
            WeaponXPBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    WeaponXPBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["WeaponXPBind"]?.Value == null)
                    {
                        WeaponXPBind.Content = "Bind";
                    }
                    else
                    {
                        WeaponXPBind.Content = "Binded To " + config.AppSettings.Settings["WeaponXPBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Weapon XP Hotkey",
                        action: hotkey => WeaponXPToggle()
                        );

                    config.AppSettings.Settings.Remove("WeaponXPBind");
                    config.AppSettings.Settings.Add("WeaponXPBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    WeaponXPBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void FishingXPDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("FishingDelay");
            config.AppSettings.Settings.Add("FishingDelay", FishingDelay.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void RotationRepeatInfinite_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = true;
            GlobalVariables.LamentRepeatHeldEnabled = false;
            GlobalVariables.LamentRepeatOnceEnabled = false;
            GlobalVariables.LamentRepeatSelectEnabled = false;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["ForegroundColor"];
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
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["ForegroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }

        private void RotationRepeatSelect_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.LamentRepeatInfiniteEnabled = false;
            GlobalVariables.LamentRepeatHeldEnabled = false;
            GlobalVariables.LamentRepeatOnceEnabled = false;
            GlobalVariables.LamentRepeatSelectEnabled = true;

            RotationRepeatInfinite.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatOnce.Background = (SolidColorBrush)Resources["BackgroundColor"];
            RotationRepeatSelect.Background = (SolidColorBrush)Resources["ForegroundColor"];
        }
        private void RotationRepeatSelectBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("RotationRepeatSelectBox");
            config.AppSettings.Settings.Add("RotationRepeatSelectBox", RotationRepeatSelectBox.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void LamentRotationBind_Click(object sender, RoutedEventArgs e)
        {
            LamentRotationBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LamentRotationBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LamentBind"]?.Value == null)
                    {
                        LamentRotationBind.Content = "Bind";
                    }
                    else
                    {
                        LamentRotationBind.Content = "Binded To " + config.AppSettings.Settings["LamentBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Lament Hotkey",
                        action: hotkey => Lament()
                        );

                    config.AppSettings.Settings.Remove("LamentBind");
                    config.AppSettings.Settings.Add("LamentBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LamentRotationBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutOneBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutOneBind.Content = "Binding...";

             async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutOneBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutOneBind"]?.Value == null)
                    {
                        LoadoutOneBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutOneBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutOneBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout One Hotkey",
                        action: hotkey => LoadoutOneSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutOneBind");
                    config.AppSettings.Settings.Add("LoadoutOneBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutOneBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutTwoBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutTwoBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutTwoBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutTwoBind"]?.Value == null)
                    {
                        LoadoutTwoBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutTwoBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutTwoBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Two Hotkey",
                        action: hotkey => LoadoutTwoSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutTwoBind");
                    config.AppSettings.Settings.Add("LoadoutTwoBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutTwoBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutThreeBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutThreeBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutThreeBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutThreeBind"]?.Value == null)
                    {
                        LoadoutThreeBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutThreeBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutThreeBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Three Hotkey",
                        action: hotkey => LoadoutThreeSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutThreeBind");
                    config.AppSettings.Settings.Add("LoadoutThreeBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutThreeBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutFourBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutFourBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutFourBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutFourBind"]?.Value == null)
                    {
                        LoadoutFourBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutFourBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutFourBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Four Hotkey",
                        action: hotkey => LoadoutFourSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutFourBind");
                    config.AppSettings.Settings.Add("LoadoutFourBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutFourBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutFiveBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutFiveBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutFiveBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutFiveBind"]?.Value == null)
                    {
                        LoadoutFiveBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutFiveBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutFiveBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Five Hotkey",
                        action: hotkey => LoadoutFiveSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutFiveBind");
                    config.AppSettings.Settings.Add("LoadoutFiveBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutFiveBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutSixBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutSixBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutSixBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutSixBind"]?.Value == null)
                    {
                        LoadoutSixBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutSixBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutSixBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Six Hotkey",
                        action: hotkey => LoadoutSixSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutSixBind");
                    config.AppSettings.Settings.Add("LoadoutSixBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutSixBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutSevenBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutSevenBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutSevenBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutSevenBind"]?.Value == null)
                    {
                        LoadoutSevenBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutSevenBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutSevenBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Seven Hotkey",
                        action: hotkey => LoadoutSevenSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutSevenBind");
                    config.AppSettings.Settings.Add("LoadoutSevenBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutSevenBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutEightBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutEightBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutEightBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutEightBind"]?.Value == null)
                    {
                        LoadoutEightBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutEightBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutEightBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Eight Hotkey",
                        action: hotkey => LoadoutEightSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutEightBind");
                    config.AppSettings.Settings.Add("LoadoutEightBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutEightBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutNineBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutNineBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutNineBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutNineBind"]?.Value == null)
                    {
                        LoadoutNineBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutNineBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutNineBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Nine Hotkey",
                        action: hotkey => LoadoutNineSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutNineBind");
                    config.AppSettings.Settings.Add("LoadoutNineBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutNineBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;

                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutTenBind_Click(object sender, RoutedEventArgs e)
        {
            LoadoutTenBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    LoadoutTenBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["LoadoutTenBind"]?.Value == null)
                    {
                        LoadoutTenBind.Content = "Bind";
                    }
                    else
                    {
                        LoadoutTenBind.Content = "Binded To " + config.AppSettings.Settings["LoadoutTenBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Loadout Ten Hotkey",
                        action: hotkey => LoadoutTenSwap()
                        );

                    config.AppSettings.Settings.Remove("LoadoutTenBind");
                    config.AppSettings.Settings.Add("LoadoutTenBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    LoadoutTenBind.Content = "Binded To " + Keybind.ToString();

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void LoadoutSwapperDelayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("LoadoutSwapDelay");
            config.AppSettings.Settings.Add("LoadoutSwapDelay", LoadoutSwapperDelayBox.Text);
            config.Save(ConfigurationSaveMode.Full);
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
        private void WeaponXPToggle()
        {
            if (GlobalVariables.WeaponXPEnabled)
            {
                GlobalVariables.WeaponXPEnabled = false;
            }
            else if (!GlobalVariables.WeaponXPEnabled)
            {
                GlobalVariables.WeaponXPEnabled = true;
            }
            WeaponXPScript();
        }
        private void WeaponXPScript()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            controller.Connect();

            Task.Run(() =>
            {
                while (GlobalVariables.WeaponXPEnabled)
                {
                    controller.SetSliderValue(Xbox360Slider.RightTrigger, 100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                    System.Threading.Thread.Sleep(200);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, -32768);
                    System.Threading.Thread.Sleep(3000);
                    controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
                }

                if (!GlobalVariables.WeaponXPEnabled)
                {
                    controller.Disconnect();
                }
            });
        }
        private void FishingToggle()
        {
            if (GlobalVariables.FishingEnabled)
            {
                GlobalVariables.FishingEnabled = false;
            }
            else if (!GlobalVariables.FishingEnabled) 
            {
                GlobalVariables.FishingEnabled = true;
            }
            FishingScript();
        }
        private void FishingScript()
        {
            var vigemClient = new ViGEmClient();
            var controller = vigemClient.CreateXbox360Controller();

            controller.Connect();

            controller.SetButtonState(Xbox360Button.X, true);
            Thread.Sleep(1000);
            controller.SetButtonState(Xbox360Button.X, false);

            if (GlobalVariables.FishingEnabled)
            {
                controller.FeedbackReceived += (sender, eventArgs) =>
                {
                    if (eventArgs.LargeMotor == 255)
                    {
                        GlobalVariables.FishingEnabled = false;

                        controller.SetButtonState(Xbox360Button.X, true);
                        Thread.Sleep(int.Parse(config.AppSettings.Settings["FishingDelay"]?.Value ?? "1000"));
                        controller.SetButtonState(Xbox360Button.X, false);

                        GlobalVariables.FishingEnabled = true;
                    }
                };
            }
            else if (!GlobalVariables.FishingEnabled)
            {
                controller.FeedbackReceived -= (sender, eventArgs) =>
                {
                    controller.Disconnect();
                    return;
                };
            }

            Task.Run(() =>
            {
                while (GlobalVariables.FishingEnabled)
                {
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, -32768);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 32767);
                    Thread.Sleep(100);
                    controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
                }

                while (GlobalVariables.FishingEnabled)
                {
                    if (GlobalVariables.FishingEnabled)
                    {
                        controller.FeedbackReceived += (sender, eventArgs) =>
                        {
                            Thread.Sleep(207);

                            if (eventArgs.LargeMotor == 1)
                            {
                                GlobalVariables.FishingEnabled = false;

                                Thread.Sleep(1060);
                                controller.SetButtonState(Xbox360Button.X, true);
                                Thread.Sleep(1000);
                                controller.SetButtonState(Xbox360Button.X, false);

                                GlobalVariables.FishingEnabled = true;
                            }
                        };
                    }
                    else if (!GlobalVariables.FishingEnabled)
                    {
                        controller.FeedbackReceived -= (sender, eventArgs) =>
                        {
                            return;
                        };
                    }
                }
            });
        }
        private void FishingXPBind_Click(object sender, RoutedEventArgs e)
        {
            FishingXPBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    FishingXPBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["FishingXPBind"]?.Value == null)
                    {
                        FishingXPBind.Content = "Bind";
                    }
                    else
                    {
                        FishingXPBind.Content = "Binded To " + config.AppSettings.Settings["FishingXPBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Fishing Hotkey",
                        action: hotkey => FishingToggle()
                        );

                    config.AppSettings.Settings.Remove("FishingXPBind");
                    config.AppSettings.Settings.Add("FishingXPBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    FishingXPBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        public static void MoveMouse(double xPercentage, double yPercentage)
        {
            // Get the screen's width and height
            int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            // Calculate the new position
            int newX = (int)(screenWidth * xPercentage);
            int newY = (int)(screenHeight * yPercentage);

            // Set the cursor to the new position
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(newX, newY);
        }
        private void LoadoutOneSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.06979, 0.35093);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutTwoSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.12500, 0.35833);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutThreeSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.07292, 0.44259);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutFourSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.12656, 0.45370);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutFiveSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.07083, 0.53796);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutSixSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.11927, 0.53796);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutSevenSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.06510, 0.60833);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutEightSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.12344, 0.62130);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutNineSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.07135, 0.72315);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void LoadoutTenSwap()
        {
            HoldKey(Keys.F1);
            ReleaseKey(Keys.F1);
            Thread.Sleep(int.Parse(config.AppSettings.Settings["LoadoutSwapDelay"]?.Value ?? "0"));
            MoveMouse(0.03594, 0.50000);
            Thread.Sleep(100);
            SendLeftClick();
            Thread.Sleep(200);
            MoveMouse(0.12917, 0.73056);
            Thread.Sleep(200);
            SendLeftClick();
            Thread.Sleep(200);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
            Thread.Sleep(50);
            HoldKey(Keys.Escape);
            ReleaseKey(Keys.Escape);
        }
        private void Lament()
        {
            Task.Run(() =>
            {
                if (GlobalVariables.LamentRepeatInfiniteEnabled)
                {
                    if (GlobalVariables.LamentRepeatInfiniteEnabled == false)
                    {
                        GlobalVariables.LamentRepeatInfiniteEnabled = true;
                    }
                    else if (GlobalVariables.LamentRepeatInfiniteEnabled == true)
                    {
                        GlobalVariables.LamentRepeatInfiniteEnabled = false;
                    }
                    while (GlobalVariables.LamentRepeatInfiniteEnabled)
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
                else if (GlobalVariables.LamentRepeatOnceEnabled)
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
                else if (GlobalVariables.LamentRepeatSelectEnabled)
                {
                    Dispatcher.Invoke(() =>
                    {
                        int repeatCount = 0;
                        if (Int32.TryParse(RotationRepeatSelectBox.Text, out repeatCount))
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
                else if (GlobalVariables.LamentRepeatHeldEnabled)
                {

                }
                else
                {
                    System.Windows.MessageBox.Show("You Do Not Have A Loop Option Selected, Please Select A Loop Option");
                }
            });
        }
        private void SuperBind_Click(object sender, RoutedEventArgs e)
        {
            SuperBind.Content = "Binding...";

            async void binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    SuperBind.Content = "Binding Aborted";
                    await Task.Delay(1000);

                    if (config.AppSettings.Settings["SuperBind"]?.Value == null)
                    {
                        SuperBind.Content = "Bind";
                    }
                    else
                    {
                        SuperBind.Content = "Binded To " + config.AppSettings.Settings["SuperBind"].Value;
                    }
                }
                else
                {
                    HotkeyHandler.HotkeyBinding(SuperBind, this, "SuperBind", "LAME Hub.dll");
                }

                this.KeyDown -= binder;
            }

            this.KeyDown += binder;
        }
        private void PrimaryWeaponBind_Click(object sender, RoutedEventArgs e)
        {

            PrimaryWeaponBind.Content = "Binding...";

            async void binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    PrimaryWeaponBind.Content = "Binding Aborted";
                    await Task.Delay(1000);

                    if (config.AppSettings.Settings["PrimaryWeaponBind"]?.Value == null)
                    {
                        PrimaryWeaponBind.Content = "Bind";
                    }
                    else
                    {
                        PrimaryWeaponBind.Content = "Binded To " + config.AppSettings.Settings["PrimaryWeaponBind"].Value;
                    }
                }
                else
                {
                    HotkeyHandler.HotkeyBinding(PrimaryWeaponBind, this, "PrimaryWeaponBind", "LAME Hub.dll");
                }

                this.KeyDown -= binder;
            }

            this.KeyDown += binder;

        }
        private void SpecialWeaponBind_Click(object sender, RoutedEventArgs e)
        {

            SpecialWeaponBind.Content = "Binding...";

            async void binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    SpecialWeaponBind.Content = "Binding Aborted";
                    await Task.Delay(1000);

                    if (config.AppSettings.Settings["SpecialWeaponBind"]?.Value == null)
                    {
                        SpecialWeaponBind.Content = "Bind";
                    }
                    else
                    {
                        SpecialWeaponBind.Content = "Binded To " + config.AppSettings.Settings["SpecialWeaponBind"].Value;
                    }
                }
                else
                {
                    HotkeyHandler.HotkeyBinding(SpecialWeaponBind, this, "SpecialWeaponBind", "LAME Hub.dll");
                }

                this.KeyDown -= binder;
            }

            this.KeyDown += binder;

        }
        private void HeavyWeaponBind_Click(object sender, RoutedEventArgs e)
        {

            HeavyWeaponBind.Content = "Binding...";

            async void binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    HeavyWeaponBind.Content = "Binding Aborted";
                    await Task.Delay(1000);

                    if (config.AppSettings.Settings["HeavyWeaponBind"]?.Value == null)
                    {
                        HeavyWeaponBind.Content = "Bind";
                    }
                    else
                    {
                        HeavyWeaponBind.Content = "Binded To " + config.AppSettings.Settings["HeavyWeaponBind"].Value;
                    }
                }
                else
                {
                    HotkeyHandler.HotkeyBinding(HeavyWeaponBind, this, "HeavyWeaponBind", "LAME Hub.dll");
                }

                this.KeyDown -= binder;
            }

            this.KeyDown += binder;

        }
        private void AirMoveBind_Click(object sender, RoutedEventArgs e)
        {

            AirMoveBind.Content = "Binding...";

            async void binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    AirMoveBind.Content = "Binding Aborted";
                    await Task.Delay(1000);

                    if (config.AppSettings.Settings["AirMoveBind"]?.Value == null)
                    {
                        AirMoveBind.Content = "Bind";
                    }
                    else
                    {
                        AirMoveBind.Content = "Binded To " + config.AppSettings.Settings["AirMoveBind"].Value;
                    }
                }
                else
                {
                    HotkeyHandler.HotkeyBinding(AirMoveBind, this, "AirMoveBind", "LAME Hub.dll");
                }

                this.KeyDown -= binder;
            }

            this.KeyDown += binder;

        }
        async private void LoadingScreen()
        {
            await Task.Delay(6000);
            LoadingImage.Visibility = Visibility.Hidden;
            TitleBar.Visibility = Visibility.Visible;
            Navbar.Visibility = Visibility.Visible;
            canvasParent.Visibility = Visibility.Visible;
        }
        async private void GroundSkate()
        {
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayOne"]?.Value ?? "25"));
            SendLeftClick();
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayTwo"]?.Value ?? "35"));
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayThree"]?.Value ?? "0"));
            if (GlobalVariables.ShatterdiveMode)
            {
                SendKeys.SendWait(config.AppSettings.Settings["AirMoveBind"]?.Value ?? "X");
            }
            else
            {
                SendKeys.SendWait(config.AppSettings.Settings["SuperBind"]?.Value ?? "F");
            }

            if (GlobalVariables.PrimarySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["PrimaryWeaponBind"]?.Value ?? "1");
            }
            else if (GlobalVariables.SpecialSwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["SpecialWeaponBind"]?.Value ?? "2");
            }
            else if (GlobalVariables.HeavySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["HeavyWeaponBind"]?.Value ?? "3");
            }

        }
        async private void EdgeSkate()
        {
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayOne"]?.Value ?? "25"));
            SendRightClick();
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayTwo"]?.Value ?? "35"));
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayThree"]?.Value ?? "0"));
            if (GlobalVariables.ShatterdiveMode)
            {
                SendKeys.SendWait(config.AppSettings.Settings["AirMoveBind"]?.Value ?? "X");
            }
            else
            {
                SendKeys.SendWait(config.AppSettings.Settings["SuperBind"]?.Value ?? "F");
            }

            if (GlobalVariables.PrimarySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["PrimaryWeaponBind"]?.Value ?? "1");
            }
            else if (GlobalVariables.SpecialSwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["SpecialWeaponBind"]?.Value ?? "2");
            }
            else if (GlobalVariables.HeavySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["HeavyWeaponBind"]?.Value ?? "3");
            }
        }
        private void SnapSkate()
        {
            System.Windows.MessageBox.Show("Snapskate is still in development");
        }
        async private void StrandSkate()
        {
            SendRightClick();
            await Task.Delay(int.Parse(config.AppSettings.Settings["MovementDelayTwo"]?.Value ?? "35"));
            SendKeys.SendWait(config.AppSettings.Settings["AirMoveBind"]?.Value ?? "X");
            HoldKey(Keys.Space);
            ReleaseKey(Keys.Space);
            if (GlobalVariables.PrimarySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["PrimaryWeaponBind"]?.Value ?? "1");
            }
            else if (GlobalVariables.SpecialSwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["SpecialWeaponBind"]?.Value ?? "2");
            }
            else if (GlobalVariables.HeavySwapBack)
            {
                SendKeys.SendWait(config.AppSettings.Settings["HeavyWeaponBind"]?.Value ?? "3");
            }
        }
        private void GroundSkateBind_Click(object sender, RoutedEventArgs e)
        {
            GroundSkateBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    GroundSkateBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["GroundSkateBind"]?.Value == null)
                    {
                        GroundSkateBind.Content = "Bind";
                    }
                    else
                    {
                        GroundSkateBind.Content = "Binded To " + config.AppSettings.Settings["GroundSkateBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Groundskate Hotkey",
                        action: hotkey => GroundSkate()
                        );

                    config.AppSettings.Settings.Remove("GroundskateBind");
                    config.AppSettings.Settings.Add("GroundSkateBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    GroundSkateBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void EdgeSkateBind_Click(object sender, RoutedEventArgs e)
        {
            EdgeSkateBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    EdgeSkateBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["EdgeSkateBind"]?.Value == null)
                    {
                        EdgeSkateBind.Content = "Bind";
                    }
                    else
                    {
                        EdgeSkateBind.Content = "Binded To " + config.AppSettings.Settings["EdgeSkateBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Edgeskate Hotkey",
                        action: hotkey => EdgeSkate()
                        );

                    config.AppSettings.Settings.Remove("EdgeSkateBind");
                    config.AppSettings.Settings.Add("EdgeSkateBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    EdgeSkateBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void SnapSkateBind_Click(object sender, RoutedEventArgs e)
        {
            SnapSkateBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    SnapSkateBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["SnapSkateBind"]?.Value == null)
                    {
                        SnapSkateBind.Content = "Bind";
                    }
                    else
                    {
                        SnapSkateBind.Content = "Binded To " + config.AppSettings.Settings["SnapSkateBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Snapskate Hotkey",
                        action: hotkey => SnapSkate()
                        );

                    config.AppSettings.Settings.Remove("SnapSkateBind");
                    config.AppSettings.Settings.Add("SnapSkateBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    SnapSkateBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void StrandSkateBind_Click(object sender, RoutedEventArgs e)
        {
            StrandSkateBind.Content = "Binding...";

            async void Binder(object sender, System.Windows.Input.KeyEventArgs e)
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

                if (e.Key == Key.Escape)
                {
                    StrandSkateBind.Content = "Binding Aborted";
                    await Task.Delay(1000);
                    if (config.AppSettings.Settings["StrandSkateBind"]?.Value == null)
                    {
                        StrandSkateBind.Content = "Bind";
                    }
                    else
                    {
                        StrandSkateBind.Content = "Binded To " + config.AppSettings.Settings["StrandSkateBind"].Value;
                    }

                    this.KeyDown -= Binder;
                }
                else
                {
                    var Keybind = e.Key;

                    Hotkey key = new(
                        key: Keybind,
                        window: this,
                        modifiers: ModifierKeys.None,
                        description: "Strandskate Hotkey",
                        action: hotkey => StrandSkate()
                        );

                    config.AppSettings.Settings.Remove("StrandSkateBind");
                    config.AppSettings.Settings.Add("StrandSkateBind", Keybind.ToString());
                    config.Save(ConfigurationSaveMode.Full);

                    StrandSkateBind.Content = "Binded To " + Keybind;

                    this.KeyDown -= Binder;
                }
            };

            this.KeyDown += Binder;
        }
        private void PrimarySwap_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.PrimarySwapBack = true;
            GlobalVariables.HeavySwapBack = false;
            GlobalVariables.SpecialSwapBack = false;

            PrimarySwap.Background = (SolidColorBrush)Resources["ForegroundColor"];
            SpecialSwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
            HeavySwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }
        private void SpecialSwap_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.PrimarySwapBack = false;
            GlobalVariables.HeavySwapBack = false;
            GlobalVariables.SpecialSwapBack = true;

            PrimarySwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
            SpecialSwap.Background = (SolidColorBrush)Resources["ForegroundColor"];
            HeavySwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
        }
        private void HeavySwap_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.PrimarySwapBack = false;
            GlobalVariables.HeavySwapBack = false;
            GlobalVariables.SpecialSwapBack = true;

            PrimarySwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
            SpecialSwap.Background = (SolidColorBrush)Resources["BackgroundColor"];
            HeavySwap.Background = (SolidColorBrush)Resources["ForegroundColor"];
        }
        private void ShatterMode_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.ShatterdiveMode)
            {
                GlobalVariables.ShatterdiveMode = false;
                ShatterMode.Background = (SolidColorBrush)Resources["BackgroundColor"];
            }
            else if (!GlobalVariables.ShatterdiveMode)
            {
                GlobalVariables.ShatterdiveMode = true;
                ShatterMode.Background = (SolidColorBrush)Resources["ForegroundColor"];
            }
        }
        private void MovementDelayOne_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("MovementDelayOne");
            config.AppSettings.Settings.Add("MovementDelayOne", MovementDelayOne.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void MovementDelayTwo_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("MovementDelayTwo");
            config.AppSettings.Settings.Add("MovementDelayTwo", MovementDelayTwo.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void MovementDelayThree_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("MovementDelayThree");
            config.AppSettings.Settings.Add("MovementDelayThree", MovementDelayThree.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void MovementSwapBackDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.AppSettings.Settings.Remove("SwapBackdelay");
            config.AppSettings.Settings.Add("SwapBackdelay", MovementSwapBackDelay.Text);
            config.Save(ConfigurationSaveMode.Full);
        }
        private void TextBoxCheck()
        {
            if (config.AppSettings.Settings["MovementDelayOne"].Value == null)
            {
                config.AppSettings.Settings.Remove("MovementDelayOne");
                config.AppSettings.Settings.Add("MovementDelayOne", MovementDelayOne.Text);
            }

            if (config.AppSettings.Settings["MovementDelayTwo"].Value == null)
            {
                config.AppSettings.Settings.Remove("MovementDelayTwo");
                config.AppSettings.Settings.Add("MovementDelayTwo", MovementDelayTwo.Text);
            }

            if (config.AppSettings.Settings["MovementDelayThree"].Value == null)
            {
                config.AppSettings.Settings.Remove("MovementDelayThree");
                config.AppSettings.Settings.Add("MovementDelayThree", MovementDelayThree.Text);
            }

            if (config.AppSettings.Settings["SwapBackdelay"].Value == null)
            {
                config.AppSettings.Settings.Remove("SwapBackdelay");
                config.AppSettings.Settings.Add("SwapBackdelay", MovementSwapBackDelay.Text);
            }
        }
        private void InfoNavbar_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.prime-software.xyz/HubInfo.html") { CreateNoWindow = true });
        }
        private void DiscordNavbar_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.thrallway.com") { CreateNoWindow = true });
        }
        private void DonateNavbar_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.ko-fi.com/leopoldprime") { CreateNoWindow = true });
        }

        private void UIUpdater()
        {
            if (config.AppSettings.Settings["SeasonalXPBind"]?.Value == null)
            {
                SeasonalXPBind.Content = "Bind";
            }
            else
            {
                SeasonalXPBind.Content = "Binded To " + config.AppSettings.Settings["SeasonalXPBind"].Value;
            }

            if (config.AppSettings.Settings["SeasonalXPDelay"]?.Value == null)
            {
                SeasonalXPDelay.Text = "2200";
            }
            else
            {
                SeasonalXPDelay.Text = config.AppSettings.Settings["SeasonalXPDelay"].Value;
            }

            if (config.AppSettings.Settings["WeaponXPBind"]?.Value == null)
            {
                WeaponXPBind.Content = "Bind";
            }
            else
            {
                WeaponXPBind.Content = "Binded To " + config.AppSettings.Settings["WeaponXPBind"].Value;
            }

            if (config.AppSettings.Settings["FishingXPBind"]?.Value == null)
            {
                FishingXPBind.Content = "Bind";
            }
            else
            {
                FishingXPBind.Content = "Binded To " + config.AppSettings.Settings["FishingXPBind"].Value;
            }

            if (config.AppSettings.Settings["FishingDelay"]?.Value == null)
            {
                FishingDelay.Text = "1000";
            }
            else
            {
                FishingDelay.Text = config.AppSettings.Settings["FishingDelay"].Value;
            }

            if (config.AppSettings.Settings["LamentBind"]?.Value == null)
            {
                LamentRotationBind.Content = "Bind";
            }
            else
            {
                LamentRotationBind.Content = "Binded To " + config.AppSettings.Settings["LamentBind"].Value;
            }

            if (config.AppSettings.Settings["RotationRepeatSelectBox"]?.Value == null)
            {
                RotationRepeatSelectBox.Text = "1";
            }
            else
            {
                RotationRepeatSelectBox.Text = config.AppSettings.Settings["RotationRepeatSelectBox"].Value;
            }


        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.thrallway.com") { CreateNoWindow = true });
        }

        private void ForegroundInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                try
                {
                    var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(textBox.Text);
                    this.Resources["ForegroundColor"] = new SolidColorBrush(color);
                }
                catch (FormatException)
                {
                    // Handle the exception if the color string is not valid
                }
            }
        }

        private void BackgroundInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                try
                {
                    var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(textBox.Text);
                    this.Resources["BackgroundColor"] = new SolidColorBrush(color);
                }
                catch (FormatException)
                {
                    // Handle the exception if the color string is not valid
                }
            }
        }

        public void NavbarButtonClicked2(object sender, Canvas navbar, Canvas canvasParent)
        {
            // Get the clicked button
            System.Windows.Controls.Button clickedButton = (System.Windows.Controls.Button)sender;

            // Get all buttons in the navbar
            var buttons = navbar.Children.OfType<System.Windows.Controls.Button>();

            // Get all canvases in the parent container
            var canvases = canvasParent.Children.OfType<Canvas>();

            foreach (System.Windows.Controls.Button button in buttons)
            {
                if (button == clickedButton)
                {
                    // Change the background color of the clicked button
                    button.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ForegroundInputBox.Text));

                    // Find the corresponding canvas and set it to visible
                    Canvas correspondingCanvas = canvases.FirstOrDefault(c => c.Name == button.Name + "Canvas");
                    if (correspondingCanvas != null)
                    {
                        correspondingCanvas.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    // Change the background color of the other buttons
                    button.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3e3d3f"));

                    // Find the corresponding canvas and set it to hidden
                    Canvas correspondingCanvas = canvases.FirstOrDefault(c => c.Name == button.Name + "Canvas");
                    if (correspondingCanvas != null)
                    {
                        correspondingCanvas.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        public void OnlyAllowNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
