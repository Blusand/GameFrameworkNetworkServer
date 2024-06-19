using Google.Protobuf;
using Test;

namespace GameFrameworkServer.Msg;

public class HeartBeatPacket : PacketBase
{
    public override int Id => (int)PacketType.SC_HeartBeat;
    protected override IMessage Msg { get; set; } = new HeartBeat();
}