using Google.Protobuf;
using Test;

namespace GameFrameworkServer.Msg;

public class MessagePacket : PacketBase
{
    public override int Id => (int)PacketType.Message;
    protected override IMessage Msg { get; set; } = new Message();

    public void SetMsg(int id, string msg)
    {
        Message message = GetMsg();
        message.Id = Id;
        message.Msg = msg;
    }

    public Message GetMsg()
    {
        return Msg as Message;
    }
}