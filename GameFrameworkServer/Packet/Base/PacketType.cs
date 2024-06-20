namespace GameFrameworkServer.Msg;

public enum PacketType
{
    CS_HeartBeat = 1,
    SC_HeartBeat = 2,
    Message = 1001,
    CSPlayerInfo = 1002,
    SCPlayerInfo = 1003,
    QuitMsg = 10000,
}