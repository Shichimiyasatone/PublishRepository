using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class ChatRoomClient {

    private Socket clientSocket;
    private byte[] data = new byte[2048];//存储数据

    public static Thread chatClientThread;

    public ChatRoomClient(Socket s)
    {
        clientSocket = s;
        //开启一个线程 处理客户端的数据接收
        chatClientThread = new Thread(new ThreadStart(ReceiveMessage));
        //MainUI.destroyDelegate += AbortChatClient;
        //RoomWindow.roomDestroyDelegate += AbortChatClient;
        chatClientThread.Start();
    }

    private void ReceiveMessage()
    {
        //服务器一直接收客户端数据
        while (true)
        {
            //如果客户端掉线，直接出循环
            if (clientSocket.Poll(10, SelectMode.SelectRead))
            {
                clientSocket.Close();
                break;
            }
            //接受信息
            int length = clientSocket.Receive(data);
            byte[] msg = data.Skip(0).Take(length).ToArray();
            //广播信息
            ChatRoomServer.BroadcastMessage(msg);
        }
    }

    public void SendMessage(byte[] data)
    {
        clientSocket.Send(data);
    }

    public bool Connected()
    {
        return clientSocket.Connected;
    }

    //public static void AbortChatClient()
    //{
    //    if (chatClientThread!=null)
    //    {
    //        if (clientSocket!=null)
    //        {
    //            try
    //            {
    //                clientSocket.Shutdown(SocketShutdown.Both);
    //            }
    //            catch (System.Exception e)
    //            {
    //                UnityEngine.Debug.Log(e.Message);
    //            }
                
    //            clientSocket.Close();
    //        }
    //        chatClientThread.Abort();
    //    }
    //}

}
