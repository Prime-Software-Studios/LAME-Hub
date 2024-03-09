using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Configuration;

namespace HubHelper
{
    public class HotkeyHandler
    {
        public void HotkeyBinding(Button button, Window window, string hotkeyName, string configPath)
        {
            button.Content = "Binding...";

            string ConfigPath = configPath;
            string HotkeyName = hotkeyName;

            Button Button = button;

            // Subscribe HotkeyBinder to window.KeyDown event
            void KeyDownEventHandler(object sender, KeyEventArgs e)
            {
                HotkeyBinder(e.Key, HotkeyName, Button, window, ConfigPath);

                // Unsubscribe the event handler after the logic is executed
                window.KeyDown -= KeyDownEventHandler;
            }

            // Subscribe the KeyDownEventHandler to window.KeyDown event
            window.KeyDown += KeyDownEventHandler;
        }

        public void HotkeyBinder(Key key, string hotkeyName, Button button, Window window, string configPath)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(configPath);
            config.AppSettings.Settings.Remove(hotkeyName);
            config.AppSettings.Settings.Add(hotkeyName, key.ToString());
            button.Content = "Binded To " + key.ToString();
            config.Save(ConfigurationSaveMode.Full);

        }
    }
}
