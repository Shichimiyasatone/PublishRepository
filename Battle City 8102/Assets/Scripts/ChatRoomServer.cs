using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ChatRoomServer {
    public static int GAME_PORT = 7747;
    static List<ChatRoomClient> clientList = new List<ChatRoomClient>();

    public static Thread chatServerThread;
    public static bool close = false;
    public static Socket tcpServer;

    public static void BroadcastMessage(byte[]data)
    {
        var NotConnectClient = new List<ChatRoomClient>();
        foreach (var client in clientList)
        {
            // Connected不应该作为超时判断条件，使用发送0字节来判断
            if (client.Connected())
            {
                client.SendMessage(data);
            }
            else
            {
                NotConnectClient.Add(client);
            }
        }
        //将掉线的客户端从集合里移除
        foreach (var client in NotConnectClient)
        {
            clientList.Remove(client);
        }
    }

    public static void StartChatServer()
    {
        tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, GAME_PORT);
        tcpServer.Bind(ipep);
        Debug.Log("服务器开启...");
        tcpServer.Listen(100);
        //循环，每连接一个客户端建立一个CharRoomClient对象
        chatServerThread = new Thread(() => {
            while (true)
            {
                Socket clientSocket = tcpServer.Accept();//暂停等待客户端连接，连接后执行后面的代码
                ChatRoomClient client = new ChatRoomClient(clientSocket);//连接后，客户端与服务器的操作封装到ChatRoomClient类中
                clientList.Add(client);
            }
        });
        MainUI.destroyDelegate += AbortChatServer;
        RoomWindow.roomDestroyDelegate += AbortChatServer;
        chatServerThread.Start();
    }

    public static void AbortChatServer()
    {
        if (chatServerThread!=null)
        {
            if (tcpServer!=null)
            {
                try
                {
                    tcpServer.Shutdown(SocketShutdown.Both);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }       
                tcpServer.Close();
            }
            chatServerThread.Abort();
        }
        Debug.Log("关闭服务器！");
    }
}
