using Google.Protobuf;
using Test;

namespace GameFrameworkServer.Msg;

public class QuitPacket : PacketBase
{
    public override int Id => (int)PacketType.QuitMsg;
    protected override IMessage Msg { get; set; } = new Quit();
}