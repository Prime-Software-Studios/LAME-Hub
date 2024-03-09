using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Hotkeys
{
	/// <summary>
	/// Hotkey service interface for DI
	/// </summary>
	public interface IHotkeyService : IDisposable
	{
		/// <summary>
		/// Registered Hotkeys
		/// </summary>
		public IReadOnlyList<Hotkey> Hotkeys { get; }

		/// <summary>
		/// Add a hotkey
		/// </summary>
		/// <param name="key">The key to watch</param>
		/// <param name="window">The window to dispatch events on</param>
		/// <param name="modifiers">Optional modifier keys</param>
		/// <param name="description">Optional description for the hotkey</param>
		/// <param name="action">Optional action to fire when hotkey occurs</param>
		/// <param name="force">Force registration of the hotkey even if we already have a hotkey with this key combination</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		/// <exception cref="ArgumentException">Occurs if the window handle or key supplied is invalid</exception>
		public Hotkey Add(
			Key key, 
			IntPtr window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false);

		/// <summary>
		/// Add a hotkey
		/// </summary>
		/// <param name="key">The key to watch</param>
		/// <param name="window">The window to dispatch events on</param>
		/// <param name="modifiers">Optional modifier keys</param>
		/// <param name="description">Optional description for the hotkey</param>
		/// <param name="action">Optional action to fire when hotkey occurs</param>
		/// <param name="force">Force registration of the hotkey even if we already have a hotkey with this key combination</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		/// <exception cref="ArgumentException">Occurs if the window handle or key supplied is invalid</exception>
		public Hotkey Add(
			Key key, 
			WindowInteropHelper window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false);

		/// <summary>
		/// Add a hotkey
		/// </summary>
		/// <param name="key">The key to watch</param>
		/// <param name="window">The window to dispatch events on</param>
		/// <param name="modifiers">Optional modifier keys</param>
		/// <param name="description">Optional description for the hotkey</param>
		/// <param name="action">Optional action to fire when hotkey occurs</param>
		/// <param name="force">Force registration of the hotkey even if we already have a hotkey with this key combination</param>
		/// <exception cref="RegisterHotkeyException">Occurs if the hotkey cannot be registered</exception>
		/// <exception cref="ArgumentException">Occurs if the window handle or key supplied is invalid</exception>
		public Hotkey Add(
			Key key, 
			Window window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false);

		/// <summary>
		/// Removes a hotkey. Modifiers must match!
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="modifiers">Optional modifier keys</param>
		/// <param name="failSilently">Silence exceptions if the modifier is not found</param>
		/// <exception cref="KeyNotFoundException">Occurs if the key and modifier combination was not found in the tracked hotkeys. Can be silenced.</exception>
		/// <exception cref="ArgumentException">Occurs if the window handle or key supplied is invalid</exception>
		public void Remove(
			Key key, 
			ModifierKeys modifiers = ModifierKeys.None, 
			bool failSilently = false);

		/// <summary>
		/// Suspend all hotkeys we currently have
		/// </summary>
		public void Suspend();

		/// <summary>
		/// Resume all hotkeys we currently have
		/// </summary>
		public void Resume();
	}

	/// <summary>
	/// Singleton Hotkey Service
	/// </summary>
	public class HotkeyService : IHotkeyService
	{
		private List<Hotkey> hotkeys;
		private bool disposedValue;

		/// <inheritdoc/>
		public IReadOnlyList<Hotkey> Hotkeys
		{
			get { return hotkeys.AsReadOnly(); }
		}

		/// <summary>
		/// Singleton Hotkey Service
		/// </summary>
		public HotkeyService()
		{
			this.hotkeys = new();
		}

		/// <inheritdoc/>
		public Hotkey Add(
			Key key, 
			IntPtr window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false)
		{
			if (window == IntPtr.Zero)
			{
				throw new ArgumentException("Cannot register hotkeys without a valid Window Handle!");
			}

			Hotkey? exists = hotkeys
				.Where(x => x.Key == key)
				.Where(x => x.Modifiers == modifiers)
				.FirstOrDefault();
			if (exists is not null)
			{
				if (force)
				{
					hotkeys.Remove(exists);
					exists.Dispose();
				}
				else
				{
					throw new RegisterHotkeyException("We already have a hotkey registered with this key combination and \"force\" was false!");
				}
			}

			Hotkey newHotkey = new(
				key,
				window,
				modifiers,
				description,
				action);

			hotkeys.Add(newHotkey);

			return newHotkey;
		}

		/// <inheritdoc/>
		public Hotkey Add(
			Key key, 
			WindowInteropHelper window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false)
			=> Add(
				key, 
				window.Handle, 
				modifiers, 
				description, 
				action, 
				force);

		/// <inheritdoc/>
		public Hotkey Add(
			Key key, 
			Window window, 
			ModifierKeys modifiers = ModifierKeys.None, 
			string? description = null, 
			Action<Hotkey>? action = null, 
			bool force = false)
			=> Add(
				key, 
				new WindowInteropHelper(window), 
				modifiers, 
				description, 
				action, 
				force);

		/// <inheritdoc/>
		public void Remove(
			Key key, 
			ModifierKeys modifiers = ModifierKeys.None, 
			bool failSilently = false)
		{
			Hotkey? exists = hotkeys
				.Where(x => x.Key == key)
				.Where(x => x.Modifiers == modifiers)
				.FirstOrDefault();
			if (exists is null)
			{
				if (failSilently)
				{
					return;
				}

				throw new KeyNotFoundException("Could not find a hotkey we have registered with that key combination!");
			}

			hotkeys.Remove(exists);
			exists.Dispose();
		}

		/// <inheritdoc/>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					foreach (Hotkey hotkey in hotkeys)
					{
						hotkey.Dispose();
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

		public void Suspend()
		{
			hotkeys
				.Where(x => x.IsKeyRegistered)
				.ToList()
				.ForEach(x => x.UnregisterHotkey());
		}

		public void Resume()
		{
			hotkeys
				.Where(x => !x.IsKeyRegistered)
				.ToList()
				.ForEach(x => x.RegisterHotkey());
		}
	}
}
