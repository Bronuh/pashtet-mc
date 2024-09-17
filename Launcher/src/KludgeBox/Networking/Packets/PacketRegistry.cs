#region

using KludgeBox.Collections;
using KludgeBox.Core;

#endregion

namespace KludgeBox.Networking.Packets;

public class PacketRegistry : TypeRegistry
{
    public PacketRegistry() : base(typeof(NetPacket))
    {
        RegisterType(typeof(PacketRegistrySynchronizationPacket));
    }
    
    public void ScanPackets()
    {
        var packets = ReflectionExtensions.FindTypesWithAttributes(typeof(GamePacketAttribute));
        foreach (Type packet in packets)
        {
            RegisterType(packet);
        }
    }
}