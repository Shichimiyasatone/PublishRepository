using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class Room : MonoBehaviour
{

    public Button createButton;
    public Button joinButton;

    public bool isHost = false;
    public static int GAME_PORT = 7777;
    public static string ROOM_START = "DO YOU LIKE VAN 游戏？";
    public static string ROOM_CONNECT = "BOY,NEXT♂DOOR!";


    // Use this for initialization
    void Start()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateRoom()
    {
        isHost = true;

        IPEndPoint udpPoint = new IPEndPoint(IPAddress.Any, 0);
        UdpClient udpClient = new UdpClient(udpPoint);
        //UdpClient udpClient = new UdpClient();
        string sendMsg = "Hello UDP Server.";
        byte[] sendData = Encoding.Default.GetBytes(sendMsg);
        IPEndPoint targetPoint = new IPEndPoint(IPAddress.Broadcast, GAME_PORT);
        udpClient.Send(sendData, sendData.Length, targetPoint);
        Debug.Log("Client:" + sendMsg);


    }

    public void JoinRoom()
    {
        isHost = false;

        //使用多线程避免unity未响应
        new Thread(() =>
        {
            IPEndPoint udpPoint = new IPEndPoint(IPAddress.Any,GAME_PORT);
            UdpClient udpClient = new UdpClient(udpPoint);
            IPEndPoint senderPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData = udpClient.Receive(ref senderPoint);
            Debug.Log(Encoding.Default.GetString(recvData));
        }).Start();
    
    }

}
