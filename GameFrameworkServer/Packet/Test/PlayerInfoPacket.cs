using Google.Protobuf;
using Test;

namespace GameFrameworkServer.Msg;

public class PlayerInfoPacket : PacketBase
{
    public override int Id => (int)PacketType.PlayerInfo;
    protected override IMessage Msg { get; set; } = new PlayerInfo();

    public void SetMsg(int id, string name, int lv, int exp, float gold, string desc)
    {
        var playerInfo = Msg as PlayerInfo;
        playerInfo.Id = id;
        playerInfo.Name = name;
        playerInfo.Lv = lv;
        playerInfo.Exp = exp;
        playerInfo.Gold = gold;
        playerInfo.Desc = desc;
    }

    public PlayerInfo GetMsg()
    {
        return Msg as PlayerInfo;
    }
}