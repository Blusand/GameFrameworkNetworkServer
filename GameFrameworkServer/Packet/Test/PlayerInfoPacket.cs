using Google.Protobuf;
using Test;

namespace GameFrameworkServer.Msg;

public class PlayerInfoPacket : PacketBase
{
    public override int Id => (int)PacketType.SCPlayerInfo;
    protected override IMessage Msg { get; set; } = new SCPlayerInfo();

    public void SetMsg(int id, string name, int lv, int exp, float gold, string desc)
    {
        var playerInfo = Msg as SCPlayerInfo;
        playerInfo.Id = id;
        playerInfo.Name = name;
        playerInfo.Lv = lv;
        playerInfo.Exp = exp;
        playerInfo.Gold = gold;
        playerInfo.Desc = desc;
    }

    public SCPlayerInfo GetMsg()
    {
        return Msg as SCPlayerInfo;
    }
}