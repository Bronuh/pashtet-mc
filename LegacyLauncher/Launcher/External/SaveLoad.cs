using System;
using System.IO;
using System.Xml.Serialization;

public class SaveLoad
{
	/// <summary>
	/// Метод загрузки файла в строку
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static string LoadString(string path)
	{
		string text = "";
		try
		{
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				StreamReader reader = new StreamReader(fs);
				text = reader.ReadToEnd();
			}
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}

		return text;
	}

	/// <summary>
	/// Записывает строку в файл
	/// </summary>
	/// <param name="text">Записываемая строка</param>
	/// <param name="path">Путь к файлу</param>
	public static void SaveString(String text, string path)
	{
		try
		{
			File.WriteAllText(path, text);
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}
	}

	/// <summary>
	/// Загружает объект какого-то там типа Т, если покайфу
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="path"></param>
	/// <returns></returns>
	public static T LoadObject<T>(string path)
	{
		T loadedObject;
		try
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			StreamReader sr = new StreamReader(path);
			loadedObject = (T)xs.Deserialize(sr);
			sr.Dispose();
			return loadedObject;
		}
		catch (Exception e)
		{
			Logger.Error("(LOAD) " + e.Message);
			return default;
		}
	}

	/// <summary>
	/// Сохраняет объект в файл
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="target"></param>
	/// <param name="path"></param>
	public static void SaveObject<T>(T target, string path)
	{
		try
		{
			Logger.Log("Сохранение объекта " + typeof(T));
			XmlSerializer xs = new XmlSerializer(typeof(T));

			StreamWriter writer = new StreamWriter(path);
			xs.Serialize(writer, target);
			writer.Dispose();
		}
		catch (Exception e)
		{
			Logger.Error(e.Message);
		}
	}
}