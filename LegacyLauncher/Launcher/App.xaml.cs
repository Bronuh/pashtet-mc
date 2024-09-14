using System.Windows;

namespace Launcher
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static string[] Args;
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Args = e.Args;
		}
	}
}
