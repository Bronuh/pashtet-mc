using Launcher;
using System;


public class Settings : ILoadable, ISaveable
{
	public static bool DEBUG = true;
	public static Data Storage;

	public void Load()
	{
		try
		{
			Storage = SaveLoad.LoadObject<Data>($"{MainWindow.HOME}\\minecraft\\Profile.xml");
		}
		catch (Exception e)
		{
			Logger.Log(e.Message);
		}
		if (Storage is null)
		{
			Storage = new Data();

			Storage.Set("RAM", 2);
			Storage.Set("Username", "Username");
			Storage.Set("Version", 0);
			Storage.Set("Close", true);
			Storage.Set("Console", false);
		}
		Launcher.MainWindow.WINDOW.UpdateUI(Storage);
	}

	public void Save()
	{
		Storage.PreSave();
		SaveLoad.SaveObject(Storage, $"{MainWindow.HOME}\\minecraft\\Profile.xml");
	}
}

