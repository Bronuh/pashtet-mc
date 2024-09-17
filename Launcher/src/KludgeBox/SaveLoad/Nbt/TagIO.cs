#region

using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace KludgeBox.SaveLoad.Nbt;

public class TagIO
{
    private static readonly PayloadHandler[] PayloadHandlers = {
		null!, // Unused 0
		new PayloadHandler<byte>(r => r.ReadByte(), (w, v) => w.Write(v)), // 1
		new PayloadHandler<short>(r => r.ReadInt16(), (w, v) => w.Write(v)), // 2
		new PayloadHandler<int>(r => r.ReadInt32(), (w, v) => w.Write(v)), // 3
		new PayloadHandler<long>(r => r.ReadInt64(), (w, v) => w.Write(v)), // 4
		new PayloadHandler<float>(r => r.ReadSingle(), (w, v) => w.Write(v)), // 5
		new PayloadHandler<double>(r => r.ReadDouble(), (w, v) => w.Write(v)), // 6
		new ClassPayloadHandler<byte[]>( // 7
			r => r.ReadBytes(r.ReadInt32()),
			(w, v) => {
				w.Write(v.Length);
				w.Write(v);
			},
			v => (byte[]) v.Clone(),
			() => Array.Empty<byte>()),
		new ClassPayloadHandler<string>( // 8
			r => Encoding.UTF8.GetString(r.BaseStream.ReadByteSpan(r.ReadInt16())),
			(w, v) => {
				var b = Encoding.UTF8.GetBytes(v);
				w.Write((short)b.Length);
				w.Write(b);
			},
			v => v,
			() => ""),
		new ClassPayloadHandler<IList>( // 9
			r => GetHandler(r.ReadByte()).ReadList(r, r.ReadInt32()),
			(w, v) => {
				int id;
				try {
					id = GetPayloadId(v.GetType().GetGenericArguments()[0]);
				}
				catch (IOException) {
					throw new IOException("Invalid NBT list type: " + v.GetType());
				}
				w.Write((byte)id);
				w.Write(v.Count);
				PayloadHandlers![id].WriteList(w, v);
			},
			v => {
				try {
					return GetHandler(GetPayloadId(v.GetType().GetGenericArguments()[0])).CloneList(v);
				}
				catch (IOException) {
					throw new IOException("Invalid NBT list type: " + v.GetType());
				}
			}),
		new ClassPayloadHandler<TagCompound>( // 10
			r => {
				var compound = new TagCompound();
				object? tag;
				while ((tag = ReadTag(r, out string? name)) != null)
					compound.Set(name, tag);

				return compound;
			},
			(w, v) => {
				foreach (var entry in v)
					if (entry.Value != null)
						WriteTag(entry.Key, entry.Value, w);

				w.Write((byte)0);
			},
			v => (TagCompound) v.Clone(),
			() => new TagCompound()),
		new ClassPayloadHandler<int[]>( // 11
			r => {
				var ia = new int[r.ReadInt32()];
				for (int i = 0; i < ia.Length; i++)
					ia[i] = r.ReadInt32();
				return ia;
			},
			(w, v) => {
				w.Write(v.Length);
				foreach (int i in v)
					w.Write(i);
			},
			v => (int[]) v.Clone(),
			() => Array.Empty<int>()),
		
		new ClassPayloadHandler<Array>( // 12
			r => GetHandler(r.ReadByte()).ReadArray(r, r.ReadInt32()),
			(w, v) => {
				int id;
				try {
					id = GetPayloadId(v.GetType().GetElementType());
				}
				catch (IOException) {
					throw new IOException("Invalid NBT array type: " + v.GetType());
				}
				w.Write((byte)id);
				w.Write(v.Length);
				PayloadHandlers![id].WriteArray(w, v);
			},
			v => (Array) v.Clone()
			)
	};

	private static readonly Dictionary<Type, int> PayloadIDs =
		Enumerable.Range(1, PayloadHandlers.Length - 1).ToDictionary(i => PayloadHandlers[i].PayloadType);

	private static readonly PayloadHandler<string> StringHandler = (PayloadHandler<string>)PayloadHandlers[8];
	
	
	private static PayloadHandler GetHandler(int id)
	{
		if (id < 1 || id >= PayloadHandlers.Length)
			throw new IOException("Invalid NBT payload id: " + id);

		return PayloadHandlers[id];
	}

	private static int GetPayloadId(Type t)
	{
		if (PayloadIDs.TryGetValue(t, out int id))
			return id;

		if (t.IsArray)
			return 12;
		
		if (typeof(IList).IsAssignableFrom(t))
			return 9;

		throw new IOException($"Invalid NBT payload type '{t}'");
	}
	
	
	public static object Serialize(object value)
	{
		ArgumentNullException.ThrowIfNull(value);

		// some very quick checks which can save on heavier dict lookups
		if (value is string or int or TagCompound or List<TagCompound>)
			return value;

		var type = value.GetType();

		if (TagSerializer.TryGetSerializer(type, out TagSerializer? serializer))
			return serializer.Serialize(value);

		// does a base level typecheck with throw
		if (GetPayloadId(type) != 9)
			return value;

		var list = (IList)value;
		var elemType = type.GetElementType() ?? type.GetGenericArguments()[0];
		if (TagSerializer.TryGetSerializer(elemType, out serializer))
			return serializer.SerializeList(list);

		if (GetPayloadId(elemType) != 9)
			return list; // already a valid NBT list type

		// list of lists conversion
		var serializedList = new List<IList>(list.Count);
		foreach (var elem in list)
			serializedList.Add((IList)Serialize(elem));

		return serializedList;
	}

	public static bool IsSerializable(Type type)
	{
		var hasTagSerializer = TagSerializer.TryGetSerializer(type, out _);
		var thereIsDefaultPayloadHandler = PayloadIDs.TryGetValue(type, out _);
		var isHandleableArray = type.IsArray && IsSerializable(type.GetElementType());
		var isHandleableList = typeof(IList).IsAssignableFrom(type) && IsSerializable(type.GetGenericArguments()[0]);
		var isExposable = type.IsAssignableTo(typeof(IBinaryExposable));
		
		return hasTagSerializer || thereIsDefaultPayloadHandler || isHandleableArray || isHandleableList || isExposable;
	}

	public static T Deserialize<T>(object? tag)
	{
		if (tag is T t) return t;
		return (T)Deserialize(typeof(T), tag);
	}

	public static object Deserialize(Type type, object? tag)
	{
		ArgumentNullException.ThrowIfNull(type);

		if (type.IsInstanceOfType(tag))
			return tag;

		if (TagSerializer.TryGetSerializer(type, out TagSerializer? serializer)) {
			if (tag == null)
				tag = Deserialize(serializer.TagType, null);

			return serializer.Deserialize(tag);
		}

		// normal nbt type with missing value
		if (tag == null && !type.IsArray) {
			if (type.GetGenericArguments().Length == 0)
				return GetHandler(GetPayloadId(type)).Default();

			if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return Activator.CreateInstance(type)!;
		}

		//list conversion required
		if (tag == null || tag is IList || type.IsArray) {
			if (type.IsArray) {
				// Only 1d arrays reach here
				var elemType = type.GetElementType()!;

				if (tag == null)
					return Array.CreateInstance(elemType, 0);

				var serializedList = (IList)tag;

				if (TagSerializer.TryGetSerializer(elemType, out serializer)) {
					IList array = Array.CreateInstance(elemType, serializedList.Count);
					for (int i = 0; i < serializedList.Count; i++)
						array[i] = serializer.Deserialize(serializedList[i]!);

					return array;
				}

				//create a strongly typed nested array
				IList deserializedArray = Array.CreateInstance(elemType, serializedList.Count);
				for (int i = 0; i < serializedList.Count; i++)
					deserializedArray[i] = Deserialize(elemType, serializedList[i]);

				return deserializedArray;
			}

			if (type.GetGenericArguments().Length == 1) {
				var elemType = type.GetGenericArguments()[0];
				var newListType = typeof(List<>).MakeGenericType(elemType);
				if (type.IsAssignableFrom(newListType)) { //if the desired type is a superclass of List<elemType>
					if (tag == null)
						return Activator.CreateInstance(newListType)!;

					if (TagSerializer.TryGetSerializer(elemType, out serializer))
						return serializer.DeserializeList((IList)tag);

					//create a strongly typed nested list
					var oldList = (IList)tag;
					var newList = (IList)Activator.CreateInstance(newListType, oldList.Count)!;
					foreach (var elem in oldList)
						newList.Add(Deserialize(elemType, elem));

					return newList;
				}
			}
		}

		if (tag == null) // unable to create an empty list subclassing the desired type
			throw new IOException($"Invalid NBT payload type '{type}'");

		throw new InvalidCastException($"Unable to cast object of type '{tag.GetType()}' to type '{type}'");
	}
	
	public static T Clone<T>(T o) where T : notnull => (T)GetHandler(GetPayloadId(o.GetType())).Clone(o);

	public static object? ReadTag(BinaryReader r, out string? name)
	{
		int id = r.ReadByte();
		if (id == 0) {
			name = null;
			return null;
		}

		name = StringHandler.reader(r);
		return ReadTagImpl(id, r);
	}

	public static object? ReadTagImpl(int id, BinaryReader r) => PayloadHandlers[id].Read(r);

	public static void WriteTag(string name, object tag, BinaryWriter w)
	{
		int id = GetPayloadId(tag.GetType());
		w.Write((byte)id);
		StringHandler.writer(w, name);
		PayloadHandlers[id].Write(w, tag);
	}
	
	public static TagCompound Read(BinaryReader reader)
	{
		var tag = ReadTag(reader, out string? name);
		if (tag is not TagCompound compound)
			throw new IOException("Root tag not a TagCompound");

		return compound;
	}
	
	public static TagCompound FromStream(Stream stream)
	{
		return Read(new BigEndianReader(stream));
	}
	
	public static void ToStream(TagCompound root, Stream stream)
	{
		Write(root, new BigEndianWriter(stream));
	}
	
	public static byte[] ToBuffer(TagCompound root)
	{
		using var stream = new MemoryStream(1 << 20);
		ToStream(root, stream);
		return stream.ToArray();
	}
	
	public static TagCompound FromBuffer(byte[] buffer)
	{
		using var stream = new MemoryStream(buffer);
		return FromStream(stream);
	}

	/// <summary>
	/// Writes the TagCompound to the writer. Please don't use this to send TagCompound over the network if you can avoid it. If you have to, consider using <see cref="ToStream(TagCompound, Stream, bool)"/>/<see cref="FromStream(Stream, bool)"/> with <c>compress: true</c>.
	/// </summary>
	public static void Write(TagCompound root, BinaryWriter writer) => WriteTag("", root, writer);
}