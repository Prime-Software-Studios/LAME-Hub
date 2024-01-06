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
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Security.Policy;
using System.Threading;
using System.Management;
using Microsoft.Win32;

namespace L.A.M.E._Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DependenciesChecker();
        }

        public enum LauncherStatus
        {
            Ready,
            CheckingDependencies,
            DownloadingDependencies,

        }

        public bool IsDotNetRuntimeInstalled()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost"))
            {
                if (key != null && key.GetValue("Version") != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsViGEmBusDriverInstalled()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SystemDriver WHERE Name = 'ViGEmBus'");
            var drivers = searcher.Get();

            return drivers.Count > 0;
        }

        private async void DependenciesChecker()
        {
            SlideLogo();
            await Task.Delay(3500);
            if (IsDotNetRuntimeInstalled())
            {
            }
            else if (!IsDotNetRuntimeInstalled())
            {
                launching_label_checking.Visibility = Visibility.Hidden;
                launching_label_downloading.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                launching_label_downloading.Visibility = Visibility.Hidden;
                launching_label_installing_net.Visibility = Visibility.Visible;
                Process installerProccess = new Process();
                installerProccess.StartInfo.FileName = "installers/net_installer.exe";
                installerProccess.Start();
                installerProccess.WaitForExit();
            }

            if (IsViGEmBusDriverInstalled())
            {
            }
            else if (!IsViGEmBusDriverInstalled())
            {
                launching_label_checking.Visibility = Visibility.Hidden;
                launching_label_installing_net.Visibility = Visibility.Hidden;
                launching_label_installing_vigem.Visibility = Visibility.Visible;
                Process installerProccess = new Process();
                installerProccess.StartInfo.FileName = "installers/vigem_installer.exe";
                installerProccess.Start();
                installerProccess.WaitForExit();
            }
            launching_label_checking.Visibility = Visibility.Hidden;
            launching_label_installing_vigem.Visibility = Visibility.Hidden;
            launching_label_launching.Visibility = Visibility.Visible;
            await Task.Delay(3500);
            launching_canvas.Visibility = Visibility.Hidden;
            titlebar.Visibility = Visibility.Visible;
            release_notes_canvas.Visibility = Visibility.Visible;

        }

        public void SlideLogo()
        {
            // Create a new TranslateTransform and assign it to the Logo's RenderTransform property
            launching_logo.RenderTransformOrigin = new Point(0.5, 0.5);
            TranslateTransform translateTransform = new TranslateTransform();
            launching_logo.RenderTransform = translateTransform;

            // Create a new DoubleAnimation to animate the TranslateTransform
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = +1000, // Slide off screen to the left
                Duration = new Duration(TimeSpan.FromSeconds(3)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);
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

        private void games_button_Click(object sender, RoutedEventArgs e)
        {
            games_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            games_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            discord_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            discord_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            donate_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            donate_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            games_canvas.Visibility = Visibility.Visible;
            release_notes_canvas.Visibility = Visibility.Hidden;
            credits_canvas.Visibility = Visibility.Hidden;
        }

        private void release_notes_button_Click(object sender, RoutedEventArgs e)
        {
            release_notes_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            games_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            games_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            discord_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            discord_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            donate_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            donate_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            games_canvas.Visibility = Visibility.Hidden;
            release_notes_canvas.Visibility = Visibility.Visible;
            credits_canvas.Visibility = Visibility.Hidden;
        }

        private void credits_button_Click(object sender, RoutedEventArgs e)
        {
            credits_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            credits_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            games_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            games_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            release_notes_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            release_notes_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            discord_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            discord_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            donate_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            donate_button.Background = (SolidColorBrush)Resources["BackgroundColor"];

            games_canvas.Visibility = Visibility.Hidden;
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

        private void destiny_button_Click(object sender, RoutedEventArgs e)
        {
            destiny_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            destiny_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            destiny_button.BorderBrush = (SolidColorBrush)Resources["BackgroundColor"];
            warframe_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            warframe_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            warframe_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];
            rust_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            rust_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            rust_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];

            game_destiny_canvas.Visibility = Visibility.Visible;
            game_warframe_canvas.Visibility = Visibility.Hidden;
            game_rust_canvas.Visibility = Visibility.Hidden;
        }

        private void warframe_button_Click(object sender, RoutedEventArgs e)
        {
            warframe_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            warframe_button.Background = (SolidColorBrush)Resources["ForegroundColor"]; 
            warframe_button.BorderBrush = (SolidColorBrush)Resources["BackgroundColor"];
            destiny_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            destiny_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            destiny_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];
            rust_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            rust_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            rust_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];

            game_destiny_canvas.Visibility = Visibility.Hidden;
            game_warframe_canvas.Visibility = Visibility.Visible;
            game_rust_canvas.Visibility = Visibility.Hidden;
        }

        private void rust_button_Click(object sender, RoutedEventArgs e)
        {
            rust_button.Foreground = (SolidColorBrush)Resources["BackgroundColor"];
            rust_button.Background = (SolidColorBrush)Resources["ForegroundColor"];
            rust_button.BorderBrush = (SolidColorBrush)Resources["BackgroundColor"];
            destiny_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            destiny_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            destiny_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];
            warframe_button.Foreground = (SolidColorBrush)Resources["ForegroundColor"];
            warframe_button.Background = (SolidColorBrush)Resources["BackgroundColor"];
            warframe_button.BorderBrush = (SolidColorBrush)Resources["ForegroundColor"];

            game_destiny_canvas.Visibility = Visibility.Hidden;
            game_warframe_canvas.Visibility = Visibility.Hidden;
            game_rust_canvas.Visibility = Visibility.Visible;
        }

        private void game_destiny_launch_button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("scripts/destiny2/L.A.M.E. Destiny 2 Script Menu.exe"));
            this.Close();
        }
    }
}