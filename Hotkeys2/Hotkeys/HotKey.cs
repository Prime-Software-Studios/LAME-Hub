using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Hotkeys
{
	/// <summary>
	/// Represents a Global Hotkey. Hotkey is active for the lifetime of the object and is removed when the object is disposed.
	/// </summary>
	public class Hotkey : IDisposable
	{
		private const int WM_HOTKEY = 0x0312;

		private bool disposedValue;

		private readonly IntPtr _handle;
		private readonly int _id;

		private bool _isKeyRegistered;
		private Dispatcher _currentDispatcher;

		private int InteropKey => KeyInterop.VirtualKeyFromKey(Key);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterHotKey")]
		private static extern bool WinAPIRegisterHotkey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "UnregisterHotKey")]
		private static extern bool WinAPIUnregisterHotkey(IntPtr hWnd, int id);

		[DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
		private static extern IntPtr WinAPIGetForegroundWindow();


		/// <summary>
		/// Event that fires when the hotkey is pressed
		/// </summary>
		public event Action<Hotkey>? HotkeyPressed;

		/// <summary>
		/// Key that is being watched
		/// </summary>
		public Key Key { get; private set; }

		/// <summary>
		/// Modifiers required for the hotkey to fire
		/// </summary>
		public ModifierKeys Modifiers { get; private set; }

		/// <summary>
		/// If the key is currently registered or not
		/// </summary>
		public bool IsKeyRegistered { get { return _isKeyRegistered; } }

		/// <summary>
		/// Friendly name for the combo
		/// </summary>
		public string Name
		{
			get
			{
				List<string> parts = new();

				if ((Modifiers & ModifierKeys.Control) != 0)
				{
					parts.Add("CTRL");
				}

				if ((Modifiers & ModifierKeys.Alt) != 0)
				{
					parts.Add("ALT");
				}

				if ((Modifiers & ModifierKeys.Shift) != 0)
				{
					parts.Add("SHIFT");
				}

				if ((Modifiers & ModifierKeys.Windows) != 0)
				{
					parts.Add("WIN");
				}

				parts.Add(Key.ToString());

				return String.Join(" + ", parts);
			}
		}

		/// <summary>
		/// Friendly description for the Hotkey
		/// </summary>
		public string? Description { get; private set; }

		/// <summary>
		/// Register a Hotkey
		/// </summary>
		/// <param name="key">The Key</param>
		/// <param name="window">The window events are dispatched on</param>
		/// <param name="modifiers">Modifier key bit flags</param>
		/// <param name="action">Action to take when hotkey occurs</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		public Hotkey(
			Key key,
			Window window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null)
			: this(
				  key, 
				  new WindowInteropHelper(window), 
				  modifiers, 
				  description,
				  action) { }

		/// <summary>
		/// Register a Hotkey
		/// </summary>
		/// <param name="key">The Key</param>
		/// <param name="window">The window events are dispatched on</param>
		/// <param name="modifiers">Modifier key bit flags</param>
		/// <param name="action">Action to take when hotkey occurs</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		public Hotkey(
			Key key, 
			WindowInteropHelper window, 
			ModifierKeys modifiers, 
			string? description = null, 
			Action<Hotkey>? action = null)
			: this(
				  key, 
				  window.Handle, 
				  modifiers, 
				  description,
				  action) { }

		/// <summary>
		/// Register a Hotkey
		/// </summary>
		/// <param name="key">The Key</param>
		/// <param name="window">The window events are dispatched on</param>
		/// <param name="modifiers">Modifier key bit flags</param>
		/// <param name="action">Action to take when hotkey occurs</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		public Hotkey(
			Key key, 
			IntPtr windowHandle, 
			ModifierKeys modifiers, 
			string? description = null, 
			Action<Hotkey>? action = null)
		{
			Key = key;
			Modifiers = modifiers;
			Description = description;
			_id = GetHashCode();
			_handle = windowHandle == IntPtr.Zero ? WinAPIGetForegroundWindow() : windowHandle;
			_currentDispatcher = Dispatcher.CurrentDispatcher;
			RegisterHotkey();
			ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;

			if (action != null)
				HotkeyPressed += action;
		}

		private void OnHotkeyPressed()
		{
			_currentDispatcher.Invoke(
				delegate
				{
					HotkeyPressed?.Invoke(this);
				});
		}

		public void RegisterHotkey()
		{
			if (Key == Key.None ||
				Key == Key.System)
			{
				MessageBox.Show("Invalid Key Input, Please Try Again");
			}

			if (Key.IsModifier())
			{
                MessageBox.Show("Cant Do Single Key Modifiers 'CTRL / ALT / SHIFT' Sorry");
            }

			if (_isKeyRegistered)
			{
				UnregisterHotkey();
			}

			_isKeyRegistered = WinAPIRegisterHotkey(_handle, _id, Modifiers, InteropKey);

			if (!_isKeyRegistered)
			{
                MessageBox.Show("Failed to register the hotkey, it may already be in use!");
			}
		}

		private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
		{
			if (handled)
			{
				return;
			}

			if (msg.message != WM_HOTKEY || (int)(msg.wParam) != _id)
			{
				return;
			}

			OnHotkeyPressed();
			handled = true;
		}

		public void UnregisterHotkey()
		{
			_isKeyRegistered = !WinAPIUnregisterHotkey(_handle, _id);
		}

		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					try
					{
						ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
					}
					finally
					{
						UnregisterHotkey();
					}
				}

				disposedValue = true;
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
