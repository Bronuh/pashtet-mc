using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher.Updaters
{
	class Installer
	{
		static string old, current;


		public static void Reinstall()
		{

			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			// TryRestartAsAdmin();

			MessageBoxResult result = MessageBox.Show("Обнаружен старый клиент. Будет произведена автоматическая настройка.",
								  "Внимание",
								  MessageBoxButton.OK,
								  MessageBoxImage.Information);
			if (directory.GetDirectories().Where(d => d.Name == "minecraft").Count() > 0)
			{
				MainWindow.TryGetRam();
				try
				{
					var Login = SaveLoad.LoadString("minecraft\\launcher_login.txt");
					Settings.Storage.Set("Username", Login);
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
				}
			}


			var info = new Launcher.Forms.Info("Идет настройка клиента, подождите...");
			info.Show();

			Utils.DirectoryCopy(".", @".\" + MainWindow.HOME, true);


			foreach (DirectoryInfo dir in directory.GetDirectories())
			{
				if (dir.Name != MainWindow.HOME)
				{
					try
					{
						Utils.ForceDeleteDirectory(dir.FullName);
						//dir.Delete(true);
					}
					catch (Exception e)
					{
						Logger.Error(e.Message);
						Warn(e.Message);
					}
				}
			}

			foreach (FileInfo file in directory.GetFiles())
			{
				if (file.Name != "Launcher.exe")
				{
					try
					{
						file.IsReadOnly = false;
						file.Delete();
					}
					catch (Exception e)
					{
						Logger.Error(e.Message);
					}
				}
			}

			info.Close();
			Settings.Storage.Set("Version", 1);

			result = MessageBox.Show("Переустановка завершена.",
									  "Переустановка",
									  MessageBoxButton.OK,
									  MessageBoxImage.Information);
		}


		public static void Install(VersionInfo version)
		{
			var info = new Launcher.Forms.Info("Установка...\nЯ не завис честно, просто много работы");
			info.Show();
			Logger.Log("Установка...\nЯ не завис честно, просто много работы");

			PrepareDirectories();


			ExtractUpdate();


			SaveOld();


			RestoreUserFiles();


			Settings.Storage.Set("Version", version.ClientVersion);
			new Mod().Load();
			MainWindow.WINDOW.UpdateUI(Settings.Storage);
			MainWindow.SaveAll();
			info.Close();

			MessageBoxResult result = MessageBox.Show("Обновление завершено",
									  "Обновление",
									  MessageBoxButton.OK,
									  MessageBoxImage.Information);
		}











		private static void ExtractUpdate()
		{
			Logger.Log("Распаковка архива...");
			try
			{
				ZipFile.ExtractToDirectory(@"Update.zip", ".\\Update");
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				Warn(e.Message);
			}

			File.Delete(@"Update.zip");
		}




		private static void PrepareDirectories()
		{
			Logger.Log("Подготовка папок");


			Utils.CheckDirectory("OldVersion");
			Utils.CheckDirectory("Update");

			Logger.Log("Удаление OldVersion");
			try
			{
				Utils.ForceDeleteDirectory("OldVersion");
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				Warn(e.Message);
			}

			Logger.Log("Удаление Update");
			try
			{
				Utils.ForceDeleteDirectory("Update");
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				Warn(e.Message);
			}


			Utils.CheckDirectory("Update");


			Utils.CheckDirectory("TempVersion");
			if (MainWindow.HOME != "TempVersion")
			{
				Logger.Log("Удаление TempVersion");
				try
				{
					Utils.ForceDeleteDirectory("TempVersion");
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					Warn(e.Message);
				}
			}
			else
			{
				Logger.Log("Удаление CurrentVersion");
				try
				{
					Utils.ForceDeleteDirectory("CurrentVersion");
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					Warn(e.Message);
				}
			}
		}




		private static void SaveOld()
		{
			old = "OldVersion";
			current = $"{MainWindow.HOME}";

			try
			{
				Directory.Move($"{MainWindow.HOME}", "OldVersion");
				Utils.DirectoryCopy("Update", "CurrentVersion",true);
				try
				{
					Utils.ForceDeleteDirectory("Update");
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					Warn(e.Message);
				}

				MainWindow.HOME = "CurrentVersion";
				if (new DirectoryInfo(Environment.CurrentDirectory).GetFiles().Where(f => f.Name.ToLower() == "unfinished.txt").Count() > 0)
				{
					File.Delete("unfinished.txt");
				}
				current = "CurrentVersion";
			}
			catch (Exception e)
			{
				Directory.Move("Update", "TempVersion");
				old = "CurrentVersion";
				current = "TempVersion";
				MainWindow.HOME = current;
				SaveLoad.SaveString("UNFINISHED", "Unfinished.txt");

				Logger.Error(e.Message);
			}
		}



		private static void TryRestartAsAdmin()
		{
			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

			if (!hasAdministrativeRight)
			{
				MessageBoxResult result = MessageBox.Show("Обнаружен старый клиент. Необходим перезапуск с правами администратора.",
								  "Внимание",
								  MessageBoxButton.OK,
								  MessageBoxImage.Warning);
				ProcessStartInfo processInfo = new ProcessStartInfo(); //создаем новый процесс
				processInfo.Verb = "runas"; //в данном случае указываем, что процесс должен быть запущен с правами администратора
				processInfo.FileName = Process.GetCurrentProcess().MainModule.FileName; //указываем исполняемый файл (программу) для запуска
				try
				{
					Process.Start(processInfo); //пытаемся запустить процесс
				}
				catch (Win32Exception)
				{
					//
				}
				MainWindow.WINDOW.Close();
				Environment.Exit(0);
			}
			else
			{
				
			}
		}


		private static void RestoreUserFiles()
		{

			string[] dirs = { 
				"\\minecraft\\saves",
				"\\minecraft\\config",
				"\\minecraft\\resourcepacks",
				"\\minecraft\\XaeroWaypoints",
				"\\minecraft\\XaeroWorldMap"
			};
			string[] files = { 
				"\\minecraft\\options.txt",
				"\\minecraft\\optionsof.txt",
				"\\minecraft\\optionsshaders.txt"
			};

			foreach (var dir in dirs)
			{
				try
				{
					Utils.DirectoryCopy($"{old}{dir}", $"{current}{dir}", false);
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					Warn(e.Message);
				}
			}

			foreach (var file in files)
			{
				try
				{
					File.Copy($"{old}{file}", $"{current}{file}", true);
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
					Warn(e.Message);
				}
			}
		}






		private static void Warn(string msg)
		{
			//Logger.Warning(msg);
			//MessageBoxResult result = MessageBox.Show(msg,
			//						  "Внимание",
			//						  MessageBoxButton.OK,
			//						  MessageBoxImage.Warning);
		}
	}
}
