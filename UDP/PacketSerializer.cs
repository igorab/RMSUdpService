using RMSUDPAgent.UDP;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RMSUDPAgent.UDP;

// Disable the warning.
#pragma warning disable SYSLIB0011

public static class PacketSerializer
{
    public static byte[] Serialize(RobotStatePacket packet)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, packet);
            return stream.ToArray();
        }
    }

    public static RobotStatePacket Deserialize(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (RobotStatePacket)formatter.Deserialize(stream);
        }
    }
}
