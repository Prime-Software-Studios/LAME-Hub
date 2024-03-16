using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HubHelper;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Diagnostics;
using DiscordRPC;
using DiscordRPC.Logging;
using System.Configuration;

namespace LAME_Hub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HotkeyHandler HotkeyHandler = new HotkeyHandler();

        UIHandler UIHandler = new UIHandler();

        public DiscordRpcClient client;

        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration("LAME Hub.dll");

        public MainWindow()
        {
            InitializeComponent();

            LoadingScreen();
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
            try
            {
                this.Close();
            }
            catch
            {
                System.Windows.MessageBox.Show(" Error Code CBC ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavbarButtonClicked(object sender, RoutedEventArgs e)
        {
            UIHandler.NavbarButtonClicked(sender, Navbar, canvasParent);
        }

        private void DestinyCanvasLaunchButton_Click(object sender, RoutedEventArgs e)
        {
            DestinyWindow destinyWindow = new DestinyWindow();
            destinyWindow.Show();
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

                    UserPFP2.Source = new BitmapImage(new Uri(avatarURL));
                    Username.Content = e.User.Username + "#0001";
                    DisplayName.Content = e.User.DisplayName;
                    DisplayName2.Content = e.User.DisplayName;
                    UserID.Content = e.User.ID;
                });
            };

            client.SetPresence(new RichPresence()
            {
                Details = "Using LAME Hub",
                State = "In The LAME Hub Main Menu",
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "LAME Hub Logo",
                    SmallImageKey = "logo"
                    
                }
            });

           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            client.Dispose();
        }

        private void SaveThemeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DiscordNavbar_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.thrallway.com") { CreateNoWindow = true });
        }

        private void DonateNavbar_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.ko-fi.com/leopoldprime") { CreateNoWindow = true });
        }

        async private void LoadingScreen()
        {
            await Task.Delay(6000);
            LoadingImage.Visibility = Visibility.Hidden;
            TitleBar.Visibility = Visibility.Visible;
            Navbar.Visibility = Visibility.Visible;
            canvasParent.Visibility = Visibility.Visible;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WarframeWindow warframeWindow = new WarframeWindow();
            warframeWindow.Show();
            this.Close();
        }
    }
}