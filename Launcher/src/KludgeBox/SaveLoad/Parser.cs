#region

using KludgeBox.Structs;
using Newtonsoft.Json.Linq;

#endregion

namespace KludgeBox.SaveLoad
{
	/// <summary>
	/// Class responsible for parsing values from the strings.
	/// </summary>
	public static class Parser
	{
		/// <summary>
		/// The class is responsible for containing and registration of the different type parsers.
		/// Since C# generates separate static class for every TValue it's can be easily used for quick access.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		public static class Parsers<TValue>
		{
			public static Func<string, TValue> parser;

			/// <summary>
			/// Registers the parsing function for TValue type
			/// </summary>
			/// <param name="func"></param>
			public static void Register(Func<string, TValue> func)
			{
				parser = func;
				_parsers[typeof(TValue)] = (string str) => func(str);
			}
		}
		
		


		private static Dictionary<Type,Func<string, object>> _parsers = new();


		static Parser()
		{
			Parsers<bool>.Register(ParseBool);
			Parsers<int>.Register(ParseInt);
			Parsers<float>.Register(ParseFloat);
			Parsers<double>.Register(ParseDouble);
			Parsers<string>.Register(ParseString);
			Parsers<Vector2>.Register(ParseVec2);
			Parsers<Vector2I>.Register(ParseVec2I);
			Parsers<Rect2>.Register(ParseRect2);
			Parsers<Rect2I>.Register(ParseRect2I);
			Parsers<Color>.Register(ParseColor);
			Parsers<Circle>.Register(ParseCircle);
		}

		public static TValue Parse<TValue>(string str)
		{
			return (TValue)_parsers[typeof(TValue)](str);
		}

		public static object Parse(string str, Type type)
		{
			return _parsers[type](str);
		}

		public static bool CanParse(Type type)
		{
			return _parsers.ContainsKey(type);
		}

		public static bool IsNull(string propertyName)
		{
			if (!SaveLoad.Loader.IsWorking)
				return true;

			var property = SaveLoad.Loader.CurrentObject[propertyName];
			if (property is null)
				return true;

			return IsNull(property);
		}

		public static bool IsNull(JToken token)
		{
			var type = token.Type;
			if (type is JTokenType.Object)
			{
				var obj = (JObject) token;
				if (obj.ContainsKey("isNull"))
					return true;
				return false;
			}

			if (type is JTokenType.Property)
			{
				return IsNull(((JProperty)token).Value);
			}

			return false;
		}

		public static bool ParseBool(string str)
		{
			return Boolean.Parse(str);
		}

		public static int ParseInt(string str)
		{
			return Int32.Parse(str);
		}

		public static float ParseFloat(string str)
		{
			return Single.Parse(str);
		}

		public static double ParseDouble(string str)
		{
			return Double.Parse(str);
		}

		public static string ParseString(string str)
		{
			return str;
		}

		public static Vector2 ParseVec2(string str)
		{
			return (Vector2) GD.StrToVar(str); //Parsers.ParseVec2(str);
		}
		

		public static Vector2I ParseVec2I(string str)
		{
			return (Vector2I) GD.StrToVar(str); //Parsers.ParseVec2I(str);
		}

		public static Rect2 ParseRect2(string str)
		{
			return (Rect2) GD.StrToVar(str); //Parsers.ParseRect2(str);
		}
		
		public static Rect2I ParseRect2I(string str)
		{
			return (Rect2I) GD.StrToVar(str); //Parsers.ParseRect2I(str);
		}

		public static Color ParseColor(string str)
		{
			return (Color)GD.StrToVar(str);  //Parsers.ParseColor(str);
		}

		public static Circle ParseCircle(string str)
		{
			return Circle.Parse(str);
		}
	}
}
