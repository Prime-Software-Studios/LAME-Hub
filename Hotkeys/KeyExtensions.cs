using System.Windows.Input;

namespace Hotkeys
{
	public static class KeyExtensions
	{
		public static bool IsModifier(this Key key)
		{
			switch (key)
			{
				case Key.LeftCtrl:
				case Key.RightCtrl:
				case Key.LeftShift:
				case Key.RightShift:
				case Key.LeftAlt:
				case Key.RightAlt:
				case Key.LWin:
				case Key.RWin:
					return true;
				default:
					return false;
			}
		}
	}
}
