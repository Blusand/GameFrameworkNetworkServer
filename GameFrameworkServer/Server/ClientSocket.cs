using GameFrameworkServer.Msg;

namespace GameFramewrokServer.Server;

using System.Net.Sockets;

public class ClientSocket
{
    public Socket Socket;
    public int ClientID;
    private static int CLIENT_BEGIN_ID = 1;

    private byte[] m_ReceiveCacheBytes = new byte[1024 * 4];
    private byte[] m_SendCacheBytes = new byte[1024 * 4];
    private int m_CacheNum = 0;

    // 上一次接收到消息的时间
    private long m_FrontTime = -1;

    /// <summary>
    /// 超时时间
    /// </summary>
    private const int TIME_OUT_TIME = 30;

    /// <summary>
    /// 最大超时次数
    /// </summary>
    private const int MAX_OUT_TIME = 3;

    /// <summary>
    /// 超时次数
    /// </summary>
    private int m_OutTime = 0;

    private static readonly HeartBeatPacket m_HeartBeatPacket = new HeartBeatPacket();

    public ClientSocket(Socket socket)
    {
        ClientID = CLIENT_BEGIN_ID++;
        Socket = socket;

        // 开始收消息
        Socket.BeginReceive(m_ReceiveCacheBytes, m_CacheNum, m_ReceiveCacheBytes.Length, SocketFlags.None,
            ReceiveCallBack, null);
        ThreadPool.QueueUserWorkItem(CheckTimeOut);
    }

    /// <summary>
    /// 间隔一段时间检测一次超时，如果超时就会主动断开该客户端的连接
    /// </summary>
    /// <param name="obj"></param>
    private void CheckTimeOut(object obj)
    {
        while (Socket != null && Socket.Connected)
        {
            if (m_FrontTime != -1 && DateTime.Now.Ticks / TimeSpan.TicksPerSecond - m_FrontTime >= TIME_OUT_TIME)
            {
                if (++m_OutTime >= MAX_OUT_TIME)
                {
                    Program.serverSocket.CloseClientSocket(this);
                    break;
                }
            }

            Thread.Sleep(5000);
        }
    }

    private void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            if (Socket != null && Socket.Connected)
            {
                // 消息成功
                int num = Socket.EndReceive(result);
                if (num != 0)
                {
                    // 处理分包黏包
                    HandleReceiveMsg(num);
                }

                Socket.BeginReceive(m_ReceiveCacheBytes, m_CacheNum, m_ReceiveCacheBytes.Length - m_CacheNum,
                    SocketFlags.None,
                    ReceiveCallBack, Socket);
            }
            else
            {
                Console.WriteLine("没有连接，不用再收消息了");
                Program.serverSocket.CloseClientSocket(this);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("接受消息错误" + e.SocketErrorCode + e.Message);
            Program.serverSocket.CloseClientSocket(this);
        }
    }

    /// <summary>
    /// 处理分包黏包
    /// </summary>
    /// <param name="receiveNum"></param>
    private void HandleReceiveMsg(int receiveNum)
    {
        int msgID = 0, packetLength = 0, nowIndex = 0;

        // 由于消息接收后是直接存储在cacheBytes中的，所以不需要进行什么拷贝操作
        // 收到消息的字节数量
        m_CacheNum += receiveNum;

        while (true)
        {
            // 每次将长度设置为-1 是避免上一次解析的数据 影响这一次的判断
            packetLength = -1;
            // 处理解析一条消息
            if (m_CacheNum - nowIndex >= 8)
            {
                // 解析ID
                msgID = BitConverter.ToInt32(m_ReceiveCacheBytes, nowIndex);
                nowIndex += sizeof(int);

                // 解析长度
                packetLength = BitConverter.ToInt32(m_ReceiveCacheBytes, nowIndex);
                nowIndex += sizeof(int);
            }

            if (m_CacheNum - nowIndex >= packetLength && packetLength != -1)
            {
                // 解析消息体
                PacketBase baseMsg = null;
                switch (msgID)
                {
                    case (int)PacketType.CS_HeartBeat:
                        baseMsg = m_HeartBeatPacket;
                        break;
                    case (int)PacketType.Message:
                        baseMsg = new MessagePacket();
                        break;
                    case (int)PacketType.CSPlayerInfo:
                        baseMsg = new PlayerInfoPacket();
                        break;
                    case (int)PacketType.QuitMsg:
                        baseMsg = new QuitPacket();
                        break;
                }

                if (baseMsg != null)
                {
                    baseMsg.Deserialize(m_ReceiveCacheBytes, nowIndex, packetLength);
                    ThreadPool.QueueUserWorkItem(PacketHandle, baseMsg);
                }

                nowIndex += packetLength;
                if (nowIndex == m_CacheNum)
                {
                    m_CacheNum = 0;
                    break;
                }
            }
            else
            {
                // 如果不满足那就证明有分包，那么我们需要把当前收到的内容记录下来，
                // 有待下次接受到消息后再做处理
                // receiveBytes.CopyTo(cacheBytes, 0);
                // cacheNum = receiveNum;
                // 如果进行了 id和长度的解析 但是 没有成功解析消息体 那么我们需要减去nowIndex移动的位置
                if (packetLength != -1)
                {
                    nowIndex -= 8;
                }

                // 就是把剩余没有解析的字节数组内容移到前面来，用于缓存下次继续解析
                Array.Copy(m_ReceiveCacheBytes, nowIndex, m_ReceiveCacheBytes, 0, m_CacheNum - nowIndex);
                m_CacheNum = m_CacheNum - nowIndex;
                break;
            }
        }
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="obj"></param>
    private void PacketHandle(object obj)
    {
        switch (obj)
        {
            case HeartBeatPacket heartBeatPacket:
                // 收到心跳消息 记录收到消息的时间
                m_FrontTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                m_OutTime = 0;
                Console.WriteLine($"{heartBeatPacket.Id}: 收到心跳消息");
                Send(m_HeartBeatPacket);
                break;
            case MessagePacket messagePacket:
                Console.WriteLine($"{messagePacket.Id}: {messagePacket.GetMsg()}");
                break;
            case PlayerInfoPacket playerInfoPacket:
                Console.WriteLine($"{playerInfoPacket.Id}: {playerInfoPacket.GetMsg()}");
                // 客户端请求玩家数据，直接将玩家数据返回
                playerInfoPacket.SetMsg(25, "玩家2", 21, 591, 48100.1f, "这个玩家很懒，什么都没有写");
                Send(playerInfoPacket);
                break;
            case QuitPacket quitPacket:
                // 收到断开连接消息，把自己添加到待移除的列表当中
                Program.serverSocket.CloseClientSocket(this);
                Console.WriteLine($"{quitPacket.Id}: 客户端{ClientID}断开链接");
                break;
        }
    }

    public void Send(PacketBase packet)
    {
        if (Socket.Connected)
        {
            byte[] bytes = packet.Serialize();
            bytes.CopyTo(m_SendCacheBytes, 0);
            Socket.BeginSend(m_SendCacheBytes, 0, bytes.Length, SocketFlags.None, SendCallBack, null);
        }
        else
        {
            Program.serverSocket.CloseClientSocket(this);
        }
    }

    private void SendCallBack(IAsyncResult result)
    {
        try
        {
            if (Socket != null && Socket.Connected)
            {
                Socket.EndSend(result);
            }
            else
            {
                Program.serverSocket.CloseClientSocket(this);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("发送失败" + e.SocketErrorCode + e.Message);
            Program.serverSocket.CloseClientSocket(this);
        }
    }

    public void Close()
    {
        if (Socket != null)
        {
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            Socket = null;
        }
    }
}