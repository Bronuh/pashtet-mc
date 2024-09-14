
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class Extensions
{
	public static string ToLine(this List<string> list)
	{
		string respond = "";
		foreach (string word in list)
			respond += word + (word == list[list.Count - 1] ? "" : ", ");

		return respond;
	}

	public static T GetRandom<T>(this List<T> list)
	{
		return list[new Random().Next(0, list.Count)];
	}

	public static IEnumerable<List<T>> ToChunks<T>(this IEnumerable<T> items, int chunkSize)
	{
		List<T> chunk = new List<T>(chunkSize);
		foreach (var item in items)
		{
			chunk.Add(item);
			if (chunk.Count == chunkSize)
			{
				yield return chunk;
				chunk = new List<T>(chunkSize);
			}
		}
		if (chunk.Any())
			yield return chunk;
	}
	public static void EachAsync<T>(this List<T> list, Action<T> action)
	{
		int threads = Environment.ProcessorCount;
		int listCount = list.Count;
		int perThread = (int)Math.Ceiling((double)listCount / threads);

		List<Task> tasks = new List<Task>();
		var parts = list.ToChunks(perThread);

		foreach (var part in parts)
		{
			tasks.Add(Task.Factory.StartNew(() =>
			{
				foreach (var item in part)
				{
					action(item);
				}
			}));
		}
		Task.WaitAll(tasks.ToArray());
	}

	public static Stream ToStream(this string s)
	{
		var stream = new MemoryStream();
		var writer = new StreamWriter(stream);
		writer.Write(s);
		writer.Flush();
		stream.Position = 0;
		return stream;
	}
}
