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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hotkeys;

namespace Hotkeys.Example
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly IHotkeyService hotkeyService;

		public MainWindow(IHotkeyService hotkeyService)
		{
			// IHotkeyService is loaded via DI, See App.xaml.cs for a basic ServiceProvider setup inside of WPF
			this.hotkeyService = hotkeyService;
			
			InitializeComponent();

			// UI Setup
			AddHotkeyButton.Click += AddHotkeyButton_Click;
			HotkeysListBox.ItemsSource = hotkeyService.Hotkeys;
			HotkeysListBox.DisplayMemberPath = "Name";
			HotkeysListBox.MouseDoubleClick += HotkeysListBox_MouseDoubleClick;
		}

		// Routine to remove hotkeys when they are double clicked on in the list
		private void HotkeysListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (HotkeysListBox.SelectedItem is Hotkey hotkey)
			{
				try
				{
					hotkeyService.Remove(hotkey.Key, hotkey.Modifiers, false);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
				finally
				{
					HotkeysListBox.Items.Refresh();
				}
			}
		}

		// Start listening for key setup
		private void AddHotkeyButton_Click(object sender, RoutedEventArgs e)
		{
			// Unregisters all registered hotkeys but does not dispose them
			hotkeyService.Suspend();
			
			AddHotkeyButton.IsEnabled = false;
			AddHotkeyButton.Content = "Listening. Press ESC to cancel";

			this.KeyDown += MainWindow_KeyDown;
		}

		// Process Key Events
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				// Stop bubbling event
				e.Handled = true;

				// Store the keycode here
				Key key = e.Key;

				// Escape pressed. Stop listening for key setup
				if (key == Key.Escape)
				{
					EndIntercept(); 
					return;
				}

				// If ALT is pressed as a modifier, e.Key becomes Key.System and the "real" key is put in e.SystemKey instead
				if (key == Key.System)
				{
					key = e.SystemKey;
				}

				// If this key is directly a modifier, then we can't store it due to WinAPI limitations with RegisterHotKey.
				// A low level keyboard hook can get around this but that is more invasive than we want to be 99% of the time
				if (key.IsModifier())
				{
					return;
				}

				// Setting up a new key. Stop listening for key setup
				EndIntercept();

				// Registers the hotkey and returns a reference
				Hotkey newHotkey = hotkeyService.Add(
					key: key, 
					window: this, 
					modifiers: Keyboard.Modifiers, 
					action: hotkey => MessageBox.Show($"{hotkey.Name} was pressed!"));

				// UI update
				HeardHotkeyLabel.Content = $"Added: {newHotkey.Name}";
				HotkeysListBox.Items.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Stop listening for key setup
		private void EndIntercept()
		{
			// registers all unregistered hotkeys being tracked
			hotkeyService.Resume();

			AddHotkeyButton.IsEnabled = true;
			AddHotkeyButton.Content = "Add Hotkey";
			this.KeyDown -= MainWindow_KeyDown;
		}
	}
}
