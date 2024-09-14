using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using LegacyLauncher.Forms;
using LegacyLauncher.Updaters;

namespace LegacyLauncher
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static int VERSION = 11;
		private bool _ready = false;
		public static MainWindow WINDOW;
		public static UpdateChecker Checker;
		public static string HOME = "CurrentVersion";
		private System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
		int dist = 290;
		int distLeft = 450;

		public static string
			ModsFolder = "CurrentVersion\\minecraft\\mods",
			DisabledModsFolder = "CurrentVersion\\minecraft\\disabledmods",
			CustomModsDir = "\\minecraft\\mods\\1.12.2";
		public static int MaxRam;


		public MainWindow()
		{
			WINDOW = this;
			MaxRam = Utils.GetTotalMemoryInGb();

			HOME = (new DirectoryInfo(Environment.CurrentDirectory).GetFiles().Where(f => f.Name.ToLower() == "unfinished.txt").Count() > 0) ?
				@"TempVersion" : @"CurrentVersion";


			Logger.Log("HOME: " + HOME + "\\");

			ModsFolder = $"{HOME}\\minecraft\\mods";
			DisabledModsFolder = $"{HOME}\\minecraft\\disabledmods";

			InitializeComponent();
			RightPanel.Margin = new Thickness(dist, 0, 0, 0);
			LeftPanel.Margin = new Thickness(0, 0, distLeft, 0);
			Initialize();
			SL_Ram.Maximum = MaxRam-1;

			Checker = new LocalChecker();// new Updaters.AbroChecker(); // По умолчанию
			Title += " v"+VERSION;

			if (App.Args.Length>0)
			{
				string param = App.Args[0].ToLower();

				if (param == "bronuh")
				{
					Checker = new Updaters.BronuhChecker();
					Title = Title + " (BRONUH CHECKER)";
				}
				else if (param == "local")
				{
					Checker = new Updaters.LocalChecker();
					Title = Title + " (LOCAL CHECKER)";
				}
				else if (param == "abro")
				{
					Checker = new Updaters.AbroChecker();
					Title = Title + " (ABRO CHECKER)";
				}
			}

			_ready = true;

			TryReinstall();
			Logger.Log("\n=======================================================\nChecking dirs:");
			CheckDirectory("Usermods");
			CheckDirectory(HOME);
			CheckDirectory($"{HOME}\\minecraft");
			CheckDirectory($"{HOME}{CustomModsDir}");
			CheckDirectory(ModsFolder);
			CheckDirectory(DisabledModsFolder);
			TryGetPass();
			Settings.Storage.Set("LauncherVersion", VERSION);

			UpdateUI(Settings.Storage);

			try
			{
				if (Utils.GetString("http://bronuh.zapto.org:25567/WIP.txt").Trim()=="true")
				{
					Warn.Visibility = Visibility.Visible;
				}
			}
			catch
			{
				Logger.Error("Нипалучилась нифартанула");
			}

			VersionInfo version;

			try
			{
				version = Checker.Check();
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				try
				{
					version = new BronuhChecker().Check();
				}
				catch (Exception ex)
				{
					Logger.Error(ex.Message);
					version = new VersionInfo() {
						ClientVersion = 1,
						LauncherVersion = 1
					};
				}
			}

				if (version.LauncherVersion > VERSION)
			{
				MessageBoxResult result = MessageBox.Show("Обнаружено новое обновление лаунчера ("+ version.LauncherVersion + " > "+ VERSION + ").",
										  "Внимание",
										  MessageBoxButton.OK,
										  MessageBoxImage.Information);
			}
			if (version.ClientVersion > (int)Settings.Storage.Get("Version"))
			{
				MessageBoxResult result = MessageBox.Show("Обнаружено новое обновление (" + version.ClientVersion + "). Для игры на сервере необходимо обновить клиент.",
										  "Внимание",
										  MessageBoxButton.OK,
										  MessageBoxImage.Information);
			}
		}

		public void TryReinstall()
		{
			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			if (directory.GetFiles().Where(f => f.Name == "AbroTech.exe").Count() > 0)
			{
				Updaters.Installer.Reinstall();
			}

			if (directory.GetFiles().Where(f => f.Name == "update.bat").Count() > 0)
			{
				directory.GetFiles().Where(f => f.Name == "update.bat").First().Delete();
			}
		}

		public static void TryGetRam()
		{
			string ram = "2048";
			int parsedRam = 2048;
			try
			{
				Logger.Log("trying to get ram");
				ram = SaveLoad.LoadString($"minecraft\\launcher_ram.txt");
				parsedRam = int.Parse(ram);

				Logger.Log("AMA GOT RAM " + ram);
				
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
			}
			Settings.Storage.Set("RAM", parsedRam);
			WINDOW.SL_Ram.Value = Math.Ceiling(parsedRam/1024.0);
		}
		public static void TryGetPass()
		{
			string pass = "Password";

			try
			{
				Logger.Log("trying to get pass");
				pass = SaveLoad.LoadString($"{HOME}\\minecraft\\sl_password.txt");
				Logger.Log("AMA GOT PASS " + pass);
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
			}

			
			WINDOW.TXT_Password.Password = pass;
		}

		public static void SaveAll()
		{
			Settings.Storage.Set("Console", WINDOW.CB_Console.IsChecked);
			Settings.Storage.Set("Close", WINDOW.CB_Close.IsChecked);
			Settings.Storage.Set("Username", WINDOW.TXT_Username.Text);
			//Settings.Storage.Set("ModsList", Mod.ModsList);
			Settings.Storage.Set("Unstable", WINDOW.CB_Unstable.IsChecked);

			SaveLoad.SaveString(WINDOW.TXT_Password.Password, $"{HOME}\\minecraft\\sl_password.txt");
			InterfaceExecutor.Execute(typeof(ISaveable), "Save");
		}
		

		private void BTN_Start_Click(object sender, RoutedEventArgs e)
		{
			SaveAll();

			Logger.Log("FUCK U");

			//DirectoryInfo customModsDir = new DirectoryInfo(HOME+CustomModsDir);
			try
			{
				Utils.ForceDeleteDirectory(HOME + CustomModsDir);

				CheckDirectory(HOME + CustomModsDir);

				Utils.DirectoryCopy("Usermods", HOME + CustomModsDir, false);
			}
			catch (Exception ex)
			{
				Logger.Error("[User mods]"+ex.Message+" (не критично)");
				MessageBoxResult result = MessageBox.Show(ex.Message,
									  "Внимание",
									  MessageBoxButton.OK,
									  MessageBoxImage.Information);
			}

			


			Process iStartProcess = new Process();
			iStartProcess.StartInfo.FileName = @"start.bat";
			iStartProcess.StartInfo.Arguments = TXT_Username.Text+" "+((int)SL_Ram.Value)+"G";
			iStartProcess.StartInfo.WorkingDirectory =
				(new DirectoryInfo(Environment.CurrentDirectory).GetFiles().Where(f => f.Name.ToLower() == "unfinished.txt").Count() > 0) ?
				@"TempVersion" : @"CurrentVersion";
			iStartProcess.StartInfo.WindowStyle = (bool)CB_Console.IsChecked 
				? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;

			Logger.Log("Запуск "+Path.GetFullPath(iStartProcess.StartInfo.FileName)+" с параметрами "+ iStartProcess.StartInfo.Arguments);

			var info = new Info("Запуск игры...");
			info.Show();

			iStartProcess.Start(); // запускаем программу

			info.Close();

			if ((bool)CB_Close.IsChecked)
			{
				Close();
				Environment.Exit(0);
			}
			
		}

		private void SL_Ram_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_ready)
			{
				return;
			}
			UpdateRam((int)e.NewValue);
		}

		void Initialize()
		{
			TryGetPass();

			InterfaceExecutor.Execute(typeof(ILoadable), "Load");

			string login = "Username";
			if (Settings.Storage.Get("Username") != null)
				if (Settings.Storage.Get("Username").ToString() == "Username" || Settings.Storage.Get("Username").ToString() == "")
				{
					try
					{
						Logger.Log("trying to get login");
						login = SaveLoad.LoadString($"{HOME}\\minecraft\\launcher_login.txt");
						Settings.Storage.Set("Username", login);
						Logger.Log("AMA GOT LOGEN " + login);
						WINDOW.TXT_Username.Text = login;
					}
					catch (Exception e)
					{
						Logger.Error(e.Message);
					}
				}
		}

		void CheckDirectory(string dir)
		{
			Logger.Log("проверка директории: " + dir + "\\");
			if (!Directory.Exists(dir + "\\"))
			{
				Logger.Log("Создана директория: " + dir + "\\");
				Directory.CreateDirectory(dir + "\\");
			}
		}

		void UpdateRam(int value)
		{
			L_Ram.Content = "Используемая память: " + value + "G";
			Settings.Storage.Set("RAM", value);
			SaveAll();
		}

		private void BTN_Mods_Click(object sender, RoutedEventArgs e)
		{
			var form = new Mods();
			form.Tag = Mod.FoundMods;
			form.Show();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveAll();
		}

		private void BTN_Mods_Copy_Click(object sender, RoutedEventArgs e)
		{
			if ((bool)CB_Unstable.IsChecked)
			{
				MessageBoxResult result = MessageBox.Show("Включено обновление до нестабильной версии. Вы уверены, что хотите продолжить?",
									  "Нестабильная версия",
									  MessageBoxButton.YesNo,
									  MessageBoxImage.Question);
				if (result == MessageBoxResult.No)
				{
					return;
				}
				else
				{
					new BronuhChecker().Update();
					return;
				}
			}
			Checker.Update();
			
			//statusForm.Close();
		}

		private void CB_Close_Click(object sender, RoutedEventArgs e)
		{
			Settings.Storage.Set("Close", CB_Close.IsChecked);
			SaveAll();
		}

		private void CB_Console_Click(object sender, RoutedEventArgs e)
		{
			Settings.Storage.Set("Console", CB_Console.IsChecked);
			SaveAll();
		}

		private void RightPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			
		}

		private void BTN_Directory_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(Environment.CurrentDirectory);
		}

		private void BTN_Panel_Click(object sender, RoutedEventArgs e)
		{
			//rightPanelActive = !rightPanelActive;

			ThicknessAnimation buttonAnimation = new ThicknessAnimation();
			if (RightPanel.Margin.Left == 0) 
			{
				buttonAnimation.From = RightPanel.Margin;
				buttonAnimation.To = new Thickness(dist,0,0,0);
			}
			else if(RightPanel.Margin.Left == dist)
			{
				buttonAnimation.From = RightPanel.Margin;
				buttonAnimation.To = new Thickness(0, 0, 0, 0);
			}
			buttonAnimation.AccelerationRatio = 1;
			buttonAnimation.Duration = TimeSpan.FromSeconds(0.35);
			RightPanel.BeginAnimation(Panel.MarginProperty, buttonAnimation);
		}

		private void BTN_PanelLeft_Click(object sender, RoutedEventArgs e)
		{
			string newsData;
			try
			{
				newsData = Utils.GetString("http://bronuh.zapto.org:25567/WhatsNew.txt");
			}
			catch (Exception ex)
			{
				newsData = "====\nНичего интересного.>>>\nВообще. Вот прям ничего, честно. Прекрати читать это, прошу тебя. Зачем ты тратишь время на это? УГАМАНИСЬ!!1";
				Logger.Error(ex.Message);
			}

			string[] news = newsData.Split(new[] {"===="}, StringSplitOptions.RemoveEmptyEntries);

			NewsList.Items.Clear();

			foreach (string article in news)
			{
				NewsList.Items.Add(new Article(article));
			}

			ThicknessAnimation buttonAnimation = new ThicknessAnimation();
			if (LeftPanel.Margin.Right == 0)
			{
				buttonAnimation.From = LeftPanel.Margin;
				buttonAnimation.To = new Thickness(0, 0, distLeft, 0);
			}
			else if (LeftPanel.Margin.Right == distLeft)
			{
				buttonAnimation.From = LeftPanel.Margin;
				buttonAnimation.To = new Thickness(0, 0, 0, 0);
			}
			buttonAnimation.AccelerationRatio = 1;
			buttonAnimation.Duration = TimeSpan.FromSeconds(0.35);
			LeftPanel.BeginAnimation(Panel.MarginProperty, buttonAnimation);
		}

		public void UpdateUI(Data context)
		{
			TXT_Username.Text = (string)(context.Get("Username") ?? "Username");
			SL_Ram.Value = (int) (context.Get("RAM") ?? 2);
			CB_Close.IsChecked = (bool) (context.Get("Close") ?? true);
			CB_Console.IsChecked = (bool)(context.Get("Console") ?? false);
			CB_Unstable.IsChecked = (bool)context.Get("Unstable",false);
			if ((int) context.Get("Version") == 0)
			{
				BTN_Start.IsEnabled = false;
				L_Version.Text = "Версия клиента: не установлен";
			}
			else
			{
				BTN_Start.IsEnabled = true;
				L_Version.Text = "Версия клиента: " + (int)(Settings.Storage.Get("Version"));
			}
			

			UpdateRam((int)SL_Ram.Value);
		}
	}
}
