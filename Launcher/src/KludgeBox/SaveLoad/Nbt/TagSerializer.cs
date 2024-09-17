#nullable enable

#region

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KludgeBox.Core;

#endregion

namespace KludgeBox.SaveLoad.Nbt;

public abstract class TagSerializer
{
	public abstract Type Type { get; }
	public abstract Type TagType { get; }

	public abstract object Serialize(object value);
	public abstract object Deserialize(object tag);
	public abstract IList SerializeList(IList value);
	public abstract IList DeserializeList(IList value);

	private static IDictionary<Type, TagSerializer> serializers = new Dictionary<Type, TagSerializer>();
	private static IDictionary<string, Type> typeNameCache = new Dictionary<string, Type>();

	static TagSerializer()
	{
		Reload();
	}

	internal static void Reload()
	{
		serializers.Clear();
		typeNameCache.Clear();

		var scannedSerializers = ReflectionExtensions.FindAllTypesThatDeriveFrom<TagSerializer>();

		foreach (var type in scannedSerializers)
		{
			try
			{
				if(!type.IsAbstract)
					AddSerializer((TagSerializer)Activator.CreateInstance(type)!);
			}
			catch
			{
				Log.Debug($"Failed to instantiate TagSerializer '{type.FullName}'.");
			}
		}
			
	}

	public static bool TryGetSerializer(Type type, [NotNullWhen(true)] out TagSerializer? serializer)
	{
		if (serializers.TryGetValue(type, out serializer))
			return true;

		if (type.IsArray && type.GetArrayRank() > 1) {
			serializers[type] = serializer = new MultiDimArraySerializer(type);
			return true;
		}

		if (typeof(TagSerializable).IsAssignableFrom(type)) {
			var sType = typeof(TagSerializableSerializer<>).MakeGenericType(type);
			serializers[type] = serializer = (TagSerializer)Activator.CreateInstance(sType)!;
			return true;
		}

		return false;
	}

	internal static void AddSerializer(TagSerializer serializer)
	{
		serializers.Add(serializer.Type, serializer);
	}

	public static Type? GetType(string name)
	{
		if (typeNameCache.TryGetValue(name, out Type? type))
			return type;

		type = Type.GetType(name);
		if (type != null)
			return typeNameCache[name] = type;

		foreach (var asm in ReflectionExtensions.GetAllAssemblies()) {
			type = asm.GetType(name);
			if (type != null)
				return typeNameCache[name] = type;
		}

		return null;
	}
}

public abstract class TagSerializer<T, S> : TagSerializer
	where T : notnull
	where S : notnull
{
	public override Type Type => typeof(T);
	public override Type TagType => typeof(S);

	public abstract S Serialize(T value);
	public abstract T Deserialize(S tag);

	public override object Serialize(object value)
	{
		return Serialize((T)value);
	}

	public override object Deserialize(object tag)
	{
		return Deserialize((S)tag);
	}

	public override IList SerializeList(IList value)
	{
		return ((IList<T>)value).Select(Serialize).ToList();
	}

	public override IList DeserializeList(IList value)
	{
		return ((IList<S>)value).Select(Deserialize).ToList();
	}
}

public class UShortTagSerializer : TagSerializer<ushort, short>
{
	public override short Serialize(ushort value) => (short)value;
	public override ushort Deserialize(short tag) => (ushort)tag;
}

public class UIntTagSerializer : TagSerializer<uint, int>
{
	public override int Serialize(uint value) => (int)value;
	public override uint Deserialize(int tag) => (uint)tag;
}

public class ULongTagSerializer : TagSerializer<ulong, long>
{
	public override long Serialize(ulong value) => (long)value;
	public override ulong Deserialize(long tag) => (ulong)tag;
}

public class BoolTagSerializer : TagSerializer<bool, byte>
{
	public override byte Serialize(bool value) => (byte)(value ? 1 : 0);
	public override bool Deserialize(byte tag) => tag != 0;
}

public class Vector2TagSerializer : TagSerializer<Vector2, TagCompound>
{
	public override TagCompound Serialize(Vector2 value) => new TagCompound {
		["x"] = value.X,
		["y"] = value.Y,
	};

	public override Vector2 Deserialize(TagCompound tag) => new Vector2(tag.GetReal("x"), tag.GetReal("y"));
}

public class Vector3TagSerializer : TagSerializer<Vector3, TagCompound>
{
	public override TagCompound Serialize(Vector3 value) => new TagCompound {
		["x"] = value.X,
		["y"] = value.Y,
		["z"] = value.Z,
	};

	public override Vector3 Deserialize(TagCompound tag) => new Vector3(tag.GetReal("x"), tag.GetReal("y"), tag.GetReal("z"));
}

public class ColorSerializer : TagSerializer<Color, int>
{
	public override int Serialize(Color value)
	{
		return (int)value.ToRgba32();
	}

	public override Color Deserialize(int tag)
	{
		return new Color((uint)tag);
	}
}


public class RectangleSerializer : TagSerializer<Rect2, TagCompound>
{
	public override TagCompound Serialize(Rect2 value) => new TagCompound {
		["x"] = value.Position.X,
		["y"] = value.Position.Y,
		["width"] = value.Size.X,
		["height"] = value.Size.Y
	};

	public override Rect2 Deserialize(TagCompound tag) => new Rect2(tag.GetReal("x"), tag.GetReal("y"), tag.GetReal("width"), tag.GetReal("height"));
}

public class VersionSerializer : TagSerializer<Version, string>
{
	public override string Serialize(Version value) => value.ToString(); // Since 1.0 and 1.0.0 are different, it's simpler to just use ToString than implement all the branching logic.

	public override Version Deserialize(string tag) => new Version(tag);
}
