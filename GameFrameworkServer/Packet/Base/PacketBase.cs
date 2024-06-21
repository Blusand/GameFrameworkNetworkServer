using Google.Protobuf;

namespace GameFrameworkServer.Msg;

public abstract class PacketBase
{
    public abstract int Id { get; }
    public abstract int HeaderLength { get; }
    public abstract int PacketLength { get; }
    public abstract byte[] Serialize();
    public abstract void Deserialize(byte[] bytes, int offset, int packetLength);
}

public abstract class PacketBase<T> : PacketBase where T : class, IMessage, new()
{
    /// <summary>
    /// 包头长度
    /// </summary>
    public override int HeaderLength => 2 * sizeof(int);

    /// <summary>
    /// 消息体长度
    /// </summary>
    public override int PacketLength => Msg.CalculateSize();

    public T Msg { get; private set; } = new T();

    public override byte[] Serialize()
    {
        byte[] bytes = new byte[HeaderLength + PacketLength];
        int index = 0;
        // 包头
        BitConverter.GetBytes(Id).CopyTo(bytes, index);
        index += sizeof(int);
        BitConverter.GetBytes(PacketLength).CopyTo(bytes, index);
        index += sizeof(int);
        // 包体
        Msg.ToByteArray().CopyTo(bytes, index);
        return bytes;
    }

    public override void Deserialize(byte[] bytes, int offset, int packetLength)
    {
        Msg = Msg.Descriptor.Parser.ParseFrom(bytes, offset, packetLength) as T;
    }
}