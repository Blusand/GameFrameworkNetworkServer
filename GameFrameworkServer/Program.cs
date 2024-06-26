﻿using GameFrameworkServer.Msg;
using GameFramewrokServer.Server;

class Program
{
    public static ServerSocket serverSocket;

    static void Main(string[] args)
    {
        serverSocket = new ServerSocket();
        serverSocket.Start("127.0.0.1", 9000, 1024);
        Console.WriteLine("开启服务器成功");

        while (true)
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1001":
                    MessagePacket messagePacket = new MessagePacket();
                    messagePacket.SetMsg(1001, "这是服务器发来的消息");
                    serverSocket.Broadcast(messagePacket);
                    break;
                case "Quit":
                case "q":
                case "Q":
                    return;
            }
        }
    }
}