namespace GameFrameworkServer.Msg;

public enum PacketType
{
    CS_HeartBeat = 1,
    SC_HeartBeat = 2,
    Message = 1001,
    PlayerInfo = 1002,
    QuitMsg = 10000,
}