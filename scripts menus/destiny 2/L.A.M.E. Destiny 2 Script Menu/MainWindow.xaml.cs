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

namespace L.A.M.E._Destiny_2_Script_Menu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]

        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public MainWindow()
        {
            InitializeComponent();
        }

        public static class GlobalVariables
        {
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
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Process.Start(new ProcessStartInfo("../../../../L.A.M.E. Launcher.exe"));
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

            afk_seasonal_canvas.Visibility = Visibility.Visible;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Hidden;
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

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Visible;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Hidden;
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

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Visible;
            auto_lament_canvas.Visibility = Visibility.Hidden;
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

            afk_seasonal_canvas.Visibility = Visibility.Hidden;
            afk_weapon_canvas.Visibility = Visibility.Hidden;
            afk_fishing_canvas.Visibility = Visibility.Hidden;
            auto_lament_canvas.Visibility = Visibility.Visible;
        }

        private void afk_seasonal_titan_Click(object sender, RoutedEventArgs e)
        {
            afk_seasonal_titan.Background = (SolidColorBrush)Resources["ForegroundColor"];
            afk_seasonal_hunter.Background = (SolidColorBrush)Resources["BackgroundColor"];
            GlobalVariables.afk_seasonal_titan_enabled = true;
            GlobalVariables.afk_seasonal_hunter_enabled = false;
        }

        private void afk_seasonal_hunter_Click(object sender, RoutedEventArgs e)
        {
            afk_seasonal_titan.Background = (SolidColorBrush)Resources["BackgroundColor"];
            afk_seasonal_hunter.Background = (SolidColorBrush)Resources["ForegroundColor"];
            GlobalVariables.afk_seasonal_titan_enabled = false;
            GlobalVariables.afk_seasonal_hunter_enabled = true;
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
                        System.Threading.Thread.Sleep(100);
                        controller.SetButtonState(Xbox360Button.LeftShoulder, false);
                        System.Threading.Thread.Sleep(100);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                        System.Threading.Thread.Sleep(300);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        System.Threading.Thread.Sleep(2200);
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
                        System.Threading.Thread.Sleep(100);
                        controller.SetButtonState(Xbox360Button.RightShoulder, false);
                        System.Threading.Thread.Sleep(100);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, -32768);
                        System.Threading.Thread.Sleep(300);
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
                        System.Threading.Thread.Sleep(2200);
                    }

                    if (!GlobalVariables.afk_seasonal_enabled)
                    {
                        controller.Disconnect();
                    }
                });
            }
            else
            {
                MessageBox.Show("You Do Not Have A Class Selected, Please Select A Class");
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

            });
        }

        private void afk_seasonal_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            AFK_Seasonal_XP_Toggle();
        }

        private void afk_weapon_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            AFK_Weapon_XP_Toggle();
        }

        private void auto_lament_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.lament_repeat_infinite_enabled == true)
            {
                if (GlobalVariables.lament_loop_enabled == false)
                {
                    GlobalVariables.lament_loop_enabled = true;
                }
                else if (GlobalVariables.lament_loop_enabled == true)
                {
                    GlobalVariables.lament_loop_enabled = false;
                }
            }
            else if (GlobalVariables.lament_repeat_infinite_enabled == false)
            {
                
            }
            else
            {
                MessageBox.Show("You Do Not Have A Repeat Option Selected, Please Select A Repeat Option");
            }
        }

        private void afk_fishing_toggle_button_Click(object sender, RoutedEventArgs e)
        {
            AFK_Fishing_Toggle();
        }
    }
}