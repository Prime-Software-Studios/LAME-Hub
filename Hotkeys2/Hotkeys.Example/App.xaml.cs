using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Hotkeys.Example
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly ServiceProvider serviceProvider;

		public App()
		{
			ServiceCollection services = new ServiceCollection();
			ConfigureServices(services);
			serviceProvider = services.BuildServiceProvider();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddHotkeys();

			services.AddSingleton<MainWindow>();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			Window mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			serviceProvider.Dispose();
			base.OnExit(e);
		}
	}
}
