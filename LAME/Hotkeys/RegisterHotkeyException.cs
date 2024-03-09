using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotkeys
{
	/// <summary>
	/// An exception that occurs when a hotkey could not be registered
	/// </summary>
	public sealed class RegisterHotkeyException : Exception
	{
		/// <summary>
		/// An exception that occurs when a hotkey could not be registered
		/// </summary>
		/// <param name="message"></param>
		public RegisterHotkeyException(string message) : base(message) { }
	}
}
