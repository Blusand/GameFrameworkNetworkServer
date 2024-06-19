using GameFrameworkServer.Msg;

namespace GameFramewrokServer.Server;

using System.Net;
using System.Net.Sockets;

public class ServerSocket
{
    private Socket _socket;
    private Dictionary<int, ClientSocket> _clientDic = new Dictionary<int, ClientSocket>();

    public void Start(string ip, int port, int num)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        try
        {
            _socket.Bind(ipPoint);
            _socket.Listen(num);

            // 通过异步接受客户端连入
            _socket.BeginAccept(AcceptCallBack, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("启动服务器失败" + e.Message);
        }
    }

    private void AcceptCallBack(IAsyncResult result)
    {
        try
        {
            // 获取连入的客户端
            Socket clientSocket = this._socket.EndAccept(result);
            ClientSocket client = new ClientSocket(clientSocket);
            // 记录客户端对象
            _clientDic.Add(client.ClientID, client);

            // 继续去让别的客户端可以连入
            _socket.BeginAccept(AcceptCallBack, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("客户端连入失败" + e.Message);
        }
    }

    public void Broadcast(PacketBase msg)
    {
        foreach (ClientSocket client in _clientDic.Values)
        {
            client.Send(msg);
        }
    }

    /// <summary>
    /// 关闭客户端连接的，从字典删除
    /// </summary>
    /// <param name="socket"></param>
    public void CloseClientSocket(ClientSocket socket)
    {
        lock (_clientDic)
        {
            socket.Close();
            if (_clientDic.ContainsKey(socket.ClientID))
            {
                _clientDic.Remove(socket.ClientID);
                Console.WriteLine($"客户端{socket.ClientID}主动断开连接了");
            }
        }
    }
}