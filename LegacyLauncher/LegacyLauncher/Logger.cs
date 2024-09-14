using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class Logger
{
	public static List<LogMessage> GlobalLog = new List<LogMessage>();
	public static List<LogMessage> ErrorsLog = new List<LogMessage>();
	public static List<LogMessage> WarningsLog = new List<LogMessage>();
	public static List<LogMessage> SuccessesLog = new List<LogMessage>();
	public static List<LogMessage> MessagesLog = new List<LogMessage>();
	public static List<LogMessage> DebugsLog = new List<LogMessage>();
	public static List<LogMessage> SavePool = new List<LogMessage>();

	public void Save()
	{
		if (!Directory.Exists("logs\\"))
		{
			Trace.WriteLine("Создана директория: " + "logs\\");
			Console.WriteLine("Создана директория: " + "logs\\");
			Directory.CreateDirectory("logs\\");
		}
		string log = "\n";
		foreach (LogMessage logMessage in SavePool)
		{
			log += logMessage.fullText + "\n";
		}

		Logger.Log("Сохранение Лога...");
		SaveLoad.SaveObject<string>(log, $"logs\\Log " +
			$"{DateTime.Now.ToShortDateString().Replace(".", "-")} " +
			$"{DateTime.Now.ToLongTimeString().Replace(":", ".")}.txt");
		Logger.Success("Сохранение завершено");
		SavePool = new List<LogMessage>();
	}

	public static void Log(String text)
	{
		new LogMessage(text, LogMessage.Type.Message);
	}

	public static void Error(String text)
	{
		new LogMessage(text, LogMessage.Type.Error);
	}

	public static void Warning(String text)
	{
		new LogMessage(text, LogMessage.Type.Warning);
	}

	public static void Success(String text)
	{
		new LogMessage(text, LogMessage.Type.Success);
	}

	public static void Debug(String text)
	{
		new LogMessage(text, LogMessage.Type.Debug);
	}
}

public class LogMessage
{
	public String text;
	public Type type;
	public DateTime time;
	public string fullText;

	public enum Type
	{
		Error,
		Warning,
		Message,
		Success,
		Debug
	}

	public LogMessage(string text)
	{
		this.text = text;
		type = Type.Message;
		time = DateTime.Now;

		string _type = type == Type.Message ? "" : type.ToString().ToUpper();
		fullText = "[" + time.ToLongTimeString() + "] " + "(" + Thread.CurrentThread.Name + ") " + _type + ": " + text;
		Console.ForegroundColor = ConsoleColor.White;
		Trace.WriteLine(fullText);
		Console.WriteLine(fullText);
		Logger.GlobalLog.Add(this);
		Logger.MessagesLog.Add(this);
	}

	public LogMessage(string text, Type type)
	{
		this.text = text;
		this.type = type;
		time = DateTime.Now;

		string _type = type == Type.Message ? "" : type.ToString().ToUpper();
		fullText = "[" + time.ToLongTimeString() + "] " + "(" + Thread.CurrentThread.Name + ") " + _type + ": " + text;

		Logger.GlobalLog.Add(this);
		Logger.SavePool.Add(this);

		if (type == Type.Message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Logger.MessagesLog.Add(this);
		}

		if (type == Type.Error)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Logger.ErrorsLog.Add(this);
		}

		if (type == Type.Warning)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Logger.WarningsLog.Add(this);
		}

		if (type == Type.Success)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Logger.SuccessesLog.Add(this);
		}

		if (type == Type.Debug)
		{
			if (Settings.DEBUG)
				Console.ForegroundColor = ConsoleColor.DarkGray;

			Logger.DebugsLog.Add(this);
		}

		if (type == Type.Debug && !Settings.DEBUG)
		{
			return;
		}

		Trace.WriteLine(fullText);
		Console.WriteLine(fullText);
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write("");
	}
}