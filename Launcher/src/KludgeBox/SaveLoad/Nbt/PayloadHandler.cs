#region

using System.Collections;
using System.IO;
using System.Linq;

#endregion

namespace KludgeBox.SaveLoad.Nbt;

public abstract class PayloadHandler
{
    public abstract Type PayloadType { get; }
    public abstract object Default();
    public abstract object Read(BinaryReader r);
    public abstract void Write(BinaryWriter w, object v);
    public abstract IList ReadList(BinaryReader r, int size);
    public abstract Array ReadArray(BinaryReader r, int size);
    public abstract void WriteList(BinaryWriter w, IList list);
    public abstract void WriteArray(BinaryWriter w, Array array);
    public abstract object Clone(object o);
    public abstract IList CloneList(IList list);
}

public class PayloadHandler<T> : PayloadHandler
    where T : notnull
{
    internal Func<BinaryReader, T> reader;
    internal Action<BinaryWriter, T> writer;

    public PayloadHandler(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer)
    {
        this.reader = reader;
        this.writer = writer;
    }

    public override Type PayloadType => typeof(T);
    public override object Read(BinaryReader r) => reader(r);
    public override void Write(BinaryWriter w, object v) => writer(w, (T)v);

    public override IList ReadList(BinaryReader r, int size)
    {
        var list = new List<T>(size);
        for (int i = 0; i < size; i++)
            list.Add(reader(r));

        return list;
    }

    public override Array ReadArray(BinaryReader r, int size)
    {
        var array = new T[size];
        for (int i = 0; i < size; i++)
            array[i] = reader(r);
        
        return array;
    }

    public override void WriteList(BinaryWriter w, IList list)
    {
        foreach (T t in list)
            writer(w, t);
    }
    
    public override void WriteArray(BinaryWriter w, Array array)
    {
        foreach (T t in array)
            writer(w, t);
    }

    public override object Clone(object o) => o;
    public override IList CloneList(IList list) => CloneList((IList<T>)list);
    public virtual IList CloneList(IList<T> list) => new List<T>(list);

    public override object Default() => default(T)!;
}

public class ClassPayloadHandler<T> : PayloadHandler<T> where T : class
{
    private Func<T, T> clone;
    private Func<T>? makeDefault;

    public ClassPayloadHandler(Func<BinaryReader, T> reader, Action<BinaryWriter, T> writer,
        Func<T, T> clone, Func<T>? makeDefault = null) :
        base(reader, writer)
    {
        this.clone = clone;
        this.makeDefault = makeDefault;
    }

    public override object Clone(object o) => clone((T)o);
    public override IList CloneList(IList<T> list) => list.Select(clone).ToList();
    public override object Default() => makeDefault!(); // If makeDefault is null, it's our job to handle default values to ensure this is never called
}