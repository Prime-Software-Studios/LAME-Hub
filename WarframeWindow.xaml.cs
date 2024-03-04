using DiscordRPC;
using DiscordRPC.Logging;
using HubHelper;
using Microsoft.VisualBasic.ApplicationServices;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LAME_Hub
{
    /// <summary>
    /// Interaction logic for WarframeWindow.xaml
    /// </summary>
    /// 

    public enum NotificationPlatform
    {
        iOS,
        Android
    }

    public enum NotificationsEnabled
    {
        True,
        False
    }

    public partial class WarframeWindow : Window
    {
        HotkeyHandler HotkeyHandler = new HotkeyHandler();

        UIHandler UIHandler = new UIHandler();

        public DiscordRpcClient client;

        private NotificationPlatform platform;

        private NotificationsEnabled state;

        public WarframeWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
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

        public string GetAvatarURL(DiscordRPC.User.AvatarFormat format, DiscordRPC.User.AvatarSize size)
        {
            format = DiscordRPC.User.AvatarFormat.PNG;
            size = DiscordRPC.User.AvatarSize.x128;
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
                    string avatarURL = GetAvatarURL(DiscordRPC.User.AvatarFormat.PNG, DiscordRPC.User.AvatarSize.x128);

                    UserPFP2.Source = new BitmapImage(new Uri(avatarURL));
                    //Username.Content = e.User.Username + "#0001";
                    //DisplayName.Content = e.User.DisplayName;
                    //DisplayName2.Content = e.User.DisplayName;
                    //UserID.Content = e.User.ID;
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


        private void Notifications()
        {
            while(state == NotificationsEnabled.True)
            {

            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (state == NotificationsEnabled.True)
            {
                state = NotificationsEnabled.False;
            }
            else if (state == NotificationsEnabled.False)
            {
                state = NotificationsEnabled.True;
            }

            Notifications();
        }
    }
}
