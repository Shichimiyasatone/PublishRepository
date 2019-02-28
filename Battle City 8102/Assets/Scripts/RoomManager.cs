using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/*
 * 创建房间后持续广播房间信息
 * 每次刷新房间时将新房间填入list
 * 删除失效房间，退出房间时设置房间失效并发送房间
 * */
public class RoomManager {

    public static int GAME_PORT = 7777;
    public static Room room;
    public static Thread createThread ;
    public static Thread searchThread;

    public static UdpClient createClient;
    public static UdpClient searchClient;

    public RoomManager()
    {
        
    }

    public static void CreateRoom(Room r)
    {
        //终止上一次的发送
        if (createThread != null)
        {
            createThread.Abort();
        }
        room = r;
        RoomWindow.changeRoomDelegate += changeRoom;
        MainUI.player.isHost = true;
        IPEndPoint udpPoint = new IPEndPoint(IPAddress.Any, 0);
        createClient = new UdpClient(udpPoint);
        IPEndPoint targetPoint = new IPEndPoint(IPAddress.Broadcast, GAME_PORT);
        BinaryFormatter bf = new BinaryFormatter();
        MainUI.destroyDelegate += AbortCreateThread;
        int count = 3;
        if (room.isAlive)
        {
            createThread = new Thread(() =>
            {
                while (true)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, room);//将room类序列化至内存流
                        createClient.Send(ms.GetBuffer(), (int)ms.Length, targetPoint);
                    }
                    Thread.Sleep(2000);
                }
            }); 
            createThread.Start();
        }
        else
        {
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, room);//将room类序列化至内存流
            for (int i = 0; i < count; i++)
            {

                createClient.Send(ms.GetBuffer(), (int)ms.Length, targetPoint);
            }
            ms.Close();
        }
    }

    //3次加入无用房间返回
    public static List<Room> SearchRoom(List<Room> roomList)
    {
       
        int maxCount = 3;//3次无用接受
        //MainUI.player.isHost = false;
        BinaryFormatter bf = new BinaryFormatter();
        searchThread = new Thread(() =>
        {
            IPEndPoint udpPoint = new IPEndPoint(IPAddress.Any, GAME_PORT);
            searchClient = new UdpClient(udpPoint);
            //searchClient.Client.ReceiveTimeout = 2000;//设置超时，超时将报错
            IPEndPoint senderPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] recvData = searchClient.Receive(ref senderPoint);
                int count = 0;
                while (count <= maxCount)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(recvData, 0, recvData.Length);//将数据写入内存流
                        ms.Flush();
                        ms.Position = 0;
                        Room rm = bf.Deserialize(ms) as Room;//将内存流中数据作为Room读出
                        rm.hostIP = senderPoint.Address.ToString();
                        rm.roomName = senderPoint.Address.ToString();

                        //更新房间列表，保留有效房间
                        if (roomList.Contains(rm))
                        {
                            roomList.Remove(rm);
                            count = 0; 
                        }
                        if (rm.isAlive)
                        {
                            roomList.Add(rm);
                            count = 0;
                        }

                        count++;
                    recvData = searchClient.Receive(ref senderPoint);
                    }  
                }
                if (searchClient != null)
                {
                    searchClient.Close();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        });
        MainUI.destroyDelegate += AbortSearchThread;
        searchThread.Start();
        return roomList;
    }

    public static void changeRoom(Room room)
    {
        RoomManager.room = room;
    }

    public static void AbortCreateThread()
    {
        if (createClient != null)
        {
            createClient.Close();
        }
        if (createThread!=null)
        {
            createThread.Abort();
        }
    }
    public static void AbortSearchThread()
    {
        if (searchClient != null)
        {
            searchClient.Close();
        }
        if (searchThread != null)
        {
            searchThread.Abort();
        }
    }
    
}
