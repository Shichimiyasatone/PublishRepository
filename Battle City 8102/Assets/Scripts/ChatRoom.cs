using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class ChatRoom {

    public delegate void MessageDelegate(byte[]recv,int length);
    public static MessageDelegate messageDelegate;

    public static int GAME_PORT = 7747;
    public static Socket clientSocket;
    public static string message = "";

    public static Thread chatThread;

    public static void ConnectToServer(string IP)
    {
        
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //如果房间消失将无法连接，报错SocketException
        try
        {
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), GAME_PORT));
            chatThread = new Thread(new ThreadStart(ReceiveMessage));
            RoomWindow.roomDestroyDelegate += OnDestroy;
            MainUI.destroyDelegate += OnDestroy;
            chatThread.Start();
        }
        catch (SocketException)
        {
            Debug.Log("房间不存在");
        }

    }

    public static void ReceiveMessage()
    {
        if (clientSocket == null)
        {
            return;
        }
        byte[] data = new byte[2048];
        while (true)
        {
            if (!clientSocket.Connected)
            {
                break;
            }
            int length = clientSocket.Receive(data);
            messageDelegate(data,length);
            message = Encoding.UTF8.GetString(data, 0, length);
        }
    }

    public static void SendMessage(byte[] data)
    {
        if (clientSocket == null)
        {
            return;
        }
        clientSocket.Send(data);
    }

    /**
     * unity自带方法
     * 停止运行时执行
     * */
    public static void OnDestroy()
    {
        if (chatThread != null)
        {
            if (clientSocket != null)
            {
                //关闭连接，接受与发送同时关闭
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            chatThread.Abort();
        }
        
    }
}
