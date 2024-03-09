
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HubHelper
{
    public class UIHandler
    {

        public void NavbarButtonClicked(object sender, Canvas navbar, Canvas canvasParent)
        {
            // Get the clicked button
            Button clickedButton = (Button)sender;

            // Get all buttons in the navbar
            var buttons = navbar.Children.OfType<Button>();

            // Get all canvases in the parent container
            var canvases = canvasParent.Children.OfType<Canvas>();

            foreach (Button button in buttons)
            {
                if (button == clickedButton)
                {
                    // Change the background color of the clicked button
                    button.Foreground = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#539db5"));

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
                    button.Foreground = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#3e3d3f"));

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
