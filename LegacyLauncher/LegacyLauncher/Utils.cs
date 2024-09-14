using LegacyLauncher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading;

class Utils
{

	public static void InstallLauncher(VersionInfo version)
	{
		string command = "timeout /t 5 /nobreak >nul\ndel launcher.exe\nren tmp_Launcher.exe LegacyLauncher.exe";
		SaveLoad.SaveString(command,"update.bat");


		Settings.Storage.Set("LauncherVersion", version.LauncherVersion);

		Process iStartProcess = new Process();
		iStartProcess.StartInfo.FileName = @"update.bat";
		iStartProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		Logger.Log("Запуск " + Path.GetFullPath(iStartProcess.StartInfo.FileName) + " с параметрами " + iStartProcess.StartInfo.Arguments);
		iStartProcess.Start();
		MainWindow.WINDOW.Close();
	}

	public static void CheckDirectory(string dir)
	{
		if (!Directory.Exists(dir + "\\"))
		{
			Logger.Log("Создана директория: " + dir + "\\");
			Directory.CreateDirectory(dir + "\\");
		}
	}

	public static void ForceDeleteDirectory(string path)
	{
		var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

		foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
		{
			info.Attributes = FileAttributes.Normal;
		}

		directory.Delete(true);
	}
	public static void DirectoryCopy(string sourceDirName, string destDirName, bool overwrite)
	{
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);

		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}

		DirectoryInfo[] dirs = dir.GetDirectories();

		// If the destination directory doesn't exist, create it.       
		Directory.CreateDirectory(destDirName);

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
		foreach (FileInfo file in files)
		{
			string tempPath = Path.Combine(destDirName, file.Name);
			Logger.Log("Копирование: "+file.Name);
			if (file.Name!=exeName)
			{
				try
				{
					file.CopyTo(tempPath, overwrite);
				}
				catch (Exception e)
				{
					Logger.Error(e.Message);
				}
			}
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (true)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string tempPath = Path.Combine(destDirName, subdir.Name);
				if (subdir.Name!="CurrentVersion"&&subdir.Name!="TempVersion"&&subdir.Name!="OldVersion")
				{
					DirectoryCopy(subdir.FullName, tempPath, true);
				}
			}
		}
	}

	

	public static int GetTotalMemoryInGb()
	{
		ManagementObjectSearcher ramMonitor =    //запрос к WMI для получения памяти ПК
		new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");
		int memory = 0;
		foreach (ManagementObject objram in ramMonitor.Get())
		{
			ulong totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
			memory = (int)Math.Round((double)totalRam / 1024 / 1024);
		}
		return memory;
	}


	public static string GetString(string url)
	{
		string data = "";
		using (WebClient webClient = new WebClient())
		{
			data = webClient.DownloadString(url);
		}
		return data;
	}

	public static List<Mod> ScanMods()
	{
		List<Mod> mods = new List<Mod>();

		try
		{
			string[] enabledModFiles = Directory.GetFiles(LegacyLauncher.MainWindow.ModsFolder, "*.jar");
			string[] disabledModFiles = Directory.GetFiles(LegacyLauncher.MainWindow.DisabledModsFolder, "*.jar");

			foreach (var file in enabledModFiles)
			{
				Mod mod = Mod.GetModByFilename(Path.GetFileName(file));
				mod.Enabled = true;
				mods.Add(mod);
			}

			foreach (var file in disabledModFiles)
			{
				Mod mod = Mod.GetModByFilename(Path.GetFileName(file));
				mod.Enabled = false;
				mods.Add(mod);
			}
		}
		catch (Exception e)
		{
			Trace.WriteLine(e.Message);
		}

		return mods;
	}
}

