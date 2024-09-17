#region

using System.IO;

#endregion

namespace KludgeBox.Networking.Serialization;

public interface IBinarySerializer
{
    Type Type { get; }
    void Serialize(BinaryWriter writer, object obj);
    object Deserialize(BinaryReader reader);
}


public interface IBinarySerializer<T> : IBinarySerializer
{
    Type IBinarySerializer.Type => typeof(T);
    new T Deserialize(BinaryReader reader);
    void Serialize(BinaryWriter writer, T obj);
    
    object IBinarySerializer.Deserialize(BinaryReader reader)
    {
        return Deserialize(reader);
    }
    
    void IBinarySerializer.Serialize(BinaryWriter writer, object obj)
    {
        Serialize(writer, (T)obj);
    }
}