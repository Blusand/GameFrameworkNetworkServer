using Test;

namespace GameFrameworkServer.Msg;

public class QuitPacket : PacketBase<Quit>
{
    public override int Id => (int)PacketType.QuitMsg;
}