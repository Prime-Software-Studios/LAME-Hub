using Microsoft.Extensions.DependencyInjection;

namespace Hotkeys
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddHotkeys(this IServiceCollection services)
		{
			return services.AddSingleton<IHotkeyService, HotkeyService>();
		}
	}
}
