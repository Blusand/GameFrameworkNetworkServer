using Test;

namespace GameFrameworkServer.Msg;

public class HeartBeatPacket : PacketBase<HeartBeat>
{
    public override int Id => (int)PacketType.SC_HeartBeat;
}