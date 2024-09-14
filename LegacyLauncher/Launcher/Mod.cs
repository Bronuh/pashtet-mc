using Launcher;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class Mod : ISaveable, ILoadable
{
	public static List<Mod> ModsList = new List<Mod>();
	public static List<Mod> FoundMods = new List<Mod>();

	public string
		Name, Filename, Description;

	public bool
		Clientside = false, Enabled = false;

	public static Mod GetModByFilename(string filename)
	{
		Mod found = null;

		foreach (var mod in ModsList)
		{
			if (mod.Filename == filename)
			{
				found = mod;
				break;
			}
		}

		if (found is null)
		{
			found = new Mod()
			{
				Filename = filename,
				Name = filename.Replace(".jar", ""),
				Description = "нет"
			};
			Logger.Log("Added new mod: "+found.Name);
			ModsList.Add(found);
		}

		return found;
	}

	public void Load()
	{
		try
		{
			ModsList = SaveLoad.LoadObject<List<Mod>>($"{MainWindow.HOME}\\minecraft\\ModsInfo.xml");
			FoundMods = Utils.ScanMods();
		}
		catch (Exception e)
		{
			Logger.Log(e.Message);
			ModsList = Utils.ScanMods();
			FoundMods = Utils.ScanMods();
		}

		FoundMods = FoundMods is null ? Utils.ScanMods() : FoundMods;

		FoundMods = FoundMods is null ? new List<Mod>() : FoundMods;
		ModsList = ModsList is null ? FoundMods : ModsList;

		//foreach (var mod in ModsList)
		//{
		//	try
		//	{
		//		if (mod.Enabled)
		//		{
		//			mod.Enable();
		//		}
		//		else
		//		{
		//			mod.Disable();
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		Logger.Error(e.Message);
		//	}
		//}

	}


	internal void Disable()
	{
		File.Move(Launcher.MainWindow.ModsFolder+"\\"+Filename, Launcher.MainWindow.DisabledModsFolder + "\\" + Filename);
		Enabled = false;
	}

	internal void Enable()
	{
		File.Move(Launcher.MainWindow.DisabledModsFolder + "\\" + Filename, Launcher.MainWindow.ModsFolder + "\\" + Filename);
		Enabled = true;
	}

	public void Save()
	{
		SaveLoad.SaveObject(ModsList, $"{MainWindow.HOME}\\minecraft\\ModsInfo.xml");
	}
}
