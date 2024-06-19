using Google.Protobuf;

namespace GameFrameworkServer.Msg;

public abstract class PacketBase
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public abstract int Id { get; }

    /// <summary>
    /// 消息长度
    /// </summary>
    public int PacketLength => Msg.CalculateSize();

    private int HeaderLength => 2 * sizeof(int);

    /// <summary>
    /// 消息体
    /// </summary>
    protected abstract IMessage Msg { get; set; }

    public byte[] Serialize()
    {
        byte[] bytes = new byte[HeaderLength + PacketLength];
        int index = 0;
        BitConverter.GetBytes(Id).CopyTo(bytes, index);
        index += sizeof(int);
        BitConverter.GetBytes(PacketLength).CopyTo(bytes, index);
        index += sizeof(int);
        Msg.ToByteArray().CopyTo(bytes, index);
        return bytes;
    }

    public void Deserialize(byte[] bytes, int offset, int packetLength)
    {
        Msg = Msg.Descriptor.Parser.ParseFrom(bytes, offset, packetLength);
    }
}