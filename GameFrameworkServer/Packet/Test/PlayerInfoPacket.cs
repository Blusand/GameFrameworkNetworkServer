using Test;

namespace GameFrameworkServer.Msg;

public class PlayerInfoPacket : PacketBase<SCPlayerInfo>
{
    public override int Id => (int)PacketType.SCPlayerInfo;

    public void SetMsg(int id, string name, int lv, int exp, float gold, string desc)
    {
       Msg.Id = id;
       Msg.Name = name;
       Msg.Lv = lv;
       Msg.Exp = exp;
       Msg.Gold = gold;
       Msg.Desc = desc;
    }
}