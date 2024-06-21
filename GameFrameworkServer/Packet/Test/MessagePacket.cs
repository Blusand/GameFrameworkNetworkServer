using Test;

namespace GameFrameworkServer.Msg;

public class MessagePacket : PacketBase<Message>
{
    public override int Id => (int)PacketType.Message;

    public void SetMsg(int id, string msg)
    {
        Msg.Id = id;
        Msg.Msg = msg;
    }
}