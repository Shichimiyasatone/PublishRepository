using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

/*
 * 房间内的玩家退出与进入通过message传输
 * 每当玩家变化时重新渲染玩家列表
 **/

public class RoomWindow :Window {

    public delegate void ChangeRoomDelegate(Room room);
    public static ChangeRoomDelegate changeRoomDelegate;
    public delegate void RoomDestroyDelegate();
    public static RoomDestroyDelegate roomDestroyDelegate;

    private GComponent roomInfoComponent;
    private GTextInput inputTextField;
    private GList messageList;
    private GButton sendMessageButton;
    private GButton startButton;
    private GButton readyButton;
    private GList playerList;

    private List<string> message;

    private bool load = false;
    public RoomWindow()
    {
        
    }

    //void Update()
    //{
    //    Debug.Log("Update");
    //    if (load)
    //    {
    //        Debug.Log("给我加载！");
    //        LoadNewScene();
    //        load = false;
    //    }
    //}

    /*
     * 进入房间房主建立tcp服务器，成员连接服务器
     * 成员将自身player加入playerList发送给服务器
     * 各成员本地修正玩家数目，房主通过maneger更新房间人数
     * 成员修改准备状态/离开房间时发送playList
     * */
    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Room_Window").asCom;
        roomInfoComponent = contentPane.GetChild("frame").asCom.GetChild("roomInfoFrame").asCom;
        roomInfoComponent.GetChild("hostTextField").asTextField.text = MainUI.currentRoom.roomHost;
        roomInfoComponent.GetChild("mapTextField").asTextField.text = MainUI.currentRoom.roomMap;
        roomInfoComponent.GetChild("modeTextField").asTextField.text = MainUI.currentRoom.roomMode;

        inputTextField = contentPane.GetChild("frame").asCom.GetChild("inputTextField").asTextInput;
        message = new List<string>();
        messageList = contentPane.GetChild("frame").asCom.GetChild("messageList").asList;
        messageList.SetVirtual();
        messageList.itemRenderer = RenderMessageList;
        messageList.lineCount = 50;

        //房主创建聊天服务器
        Debug.Log("isHost：" + MainUI.player.isHost);
        if (MainUI.player.isHost)
        {
            ChatRoomServer.StartChatServer();
        }
        try
        {
            ChatRoom.ConnectToServer(MainUI.currentRoom.hostIP);
            Debug.Log(MainUI.currentRoom.hostIP);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            Dispose();
            return;
        }

        //在此处将本地玩家添加进MainUI.currentRoom.currentPlayer
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            MainUI.currentRoom.currentPlayer.Add(MainUI.player);
            bf.Serialize(ms, MainUI.currentRoom.currentPlayer);
            byte[] data = new byte[ms.Length];
            System.Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
            ChatRoom.SendMessage(data);
        }
        roomInfoComponent.GetChild("playerNumTextField").asTextField.text = MainUI.currentRoom.currentPlayer.Count + "/" + MainUI.currentRoom.limitNum;
        ChatRoom.messageDelegate += MessageHandler;

        playerList = contentPane.GetChild("frame").asCom.GetChild("playerList").asList;
        playerList.scrollPane.touchEffect = false;
        playerList.SetVirtual();
        playerList.itemRenderer = RenderListItem;
        playerList.numItems = MainUI.currentRoom.limitNum;

        //发送按钮按下，发送玩家名+inputTextField.text
        sendMessageButton = contentPane.GetChild("frame").asCom.GetChild("sendMessageButton").asButton;
        sendMessageButton.onClick.Add(() =>
        {
            if (inputTextField.text.Equals(""))
            {
                return;
            }
            byte[] data = Encoding.UTF8.GetBytes((MainUI.player.name + ":" + inputTextField.text));
            ChatRoom.SendMessage(data);
            inputTextField.text = "";
        });

        //准备按钮按下，发送playerList
        readyButton = contentPane.GetChild("frame").asCom.GetChild("readyButton").asButton;
        readyButton.onClick.Add(() =>
        {
            if (contentPane.GetChild("frame").asCom.GetChild("readyButton").asButton.selected)
            {
                MainUI.player.isReady = true;
            }
            else
            {
                MainUI.player.isReady = false;
            }
            foreach (var player in MainUI.currentRoom.currentPlayer)
            {
                // 分支未执行
                if (player.ip == MainUI.player.ip)
                {
                    Debug.Log("修改了！");
                    player.isReady = MainUI.player.isReady;
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, MainUI.currentRoom.currentPlayer);
                byte[] data = new byte[ms.Length];
                System.Array.Copy(ms.GetBuffer(), 0, data, 0, ms.Length);
                ChatRoom.SendMessage(data);
            }

        });

        //开始按钮按下，游戏开始
        startButton = contentPane.GetChild("frame").asCom.GetChild("startButton").asButton;
        startButton.onClick.Add(() =>
        {
            //判断准备状态，载入游戏
            foreach (var player in MainUI.currentRoom.currentPlayer)
            {
                // 跳过对房主的判断
                if (player.isHost)
                {
                    continue;
                }
                // 有玩家未准备，退出方法
                if (!player.isReady)
                {
                    startButton.selected = false;
                    return;
                }
            }
            // 随机场景对战主机
            SelectHolder();
            // 异步加载战斗场景
            //LoadNewScene();
        });

        if (MainUI.player.isHost)
        {
            readyButton.visible = false;

        }
        else
        {
            startButton.visible = false;
        }
    }

    private void RenderListItem(int index, GObject obj)
    {
        GButton btn = obj.asButton;
        btn.title = "";
        btn.icon = "";
        btn.GetChild("readyTag").visible = false;
        btn.GetChild("hostTag").visible = false;
        if (index >= MainUI.currentRoom.currentPlayer.Count)
        {
            //btn.visible = false;
            return;
        }
        Debug.Log("渲染第"+index+"个玩家，" + MainUI.currentRoom.currentPlayer[index].isReady);
        obj.onClick.Clear();
        obj.onClick.Add(() => { });//点击玩家显示添加好友、玩家信息列表
        btn.title = MainUI.currentRoom.currentPlayer[index].name;
        btn.icon = UIPackage.GetItemURL("BattleCity8102", "rank" + MainUI.currentRoom.currentPlayer[index].rank);
        btn.GetChild("readyTag").visible = MainUI.currentRoom.currentPlayer[index].isReady;
        btn.GetChild("hostTag").visible = MainUI.currentRoom.currentPlayer[index].isHost;
    }

    private void RenderMessageList(int index ,GObject obj)
    {
        GButton btn = (GButton)obj;
        if (index>=message.Count)
        {
            return;
        }
        btn.title = message[index];
    }

    //数据转换为playerList或string
    private void MessageHandler(byte[] data, int length)
    {
        // 序列化房间列表
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(data, 0, length);
                ms.Flush();
                ms.Position = 0;
                List<Player> pl = bf.Deserialize(ms) as List<Player>;
                MainUI.currentRoom.currentPlayer = pl;
                playerList.RefreshVirtualList();
                roomInfoComponent.GetChild("playerNumTextField").asTextField.text = MainUI.currentRoom.currentPlayer.Count + "/" + MainUI.currentRoom.limitNum;
                Debug.Log("接收到玩家列表，更新！");
                if (MainUI.player.isHost)
                {    
                    changeRoomDelegate(MainUI.currentRoom);
                }
            }
        }
        catch (System.Exception)
        {
            // 序列化主机信息
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    ms.Write(data, 0, length);
                    ms.Flush();
                    ms.Position = 0;
                    HolderInfo holderInfo = bf.Deserialize(ms) as HolderInfo;
                    Debug.Log(IPv4.GetLocalIP() + "接收到主机："+holderInfo.ip);
                    if (holderInfo.ip == IPv4.GetLocalIP())
                    {
                        holderInfo.isHolder = true;
                    }
                    HolderInfoManager.holderInfo = holderInfo;
                }
                // 触发页面加载
                Debug.Log(" 触发页面加载");
                LoadNewScene();
                //load = true;
            }
            catch (System.Exception)
            {
                // 接收字符串消息
                string msg = Encoding.UTF8.GetString(data, 0, length);
                message.Add(msg);
                Debug.Log(msg);
                if (message.Count > 50)
                {
                    message.RemoveRange(0, message.Count - 50);
                }
                messageList.RefreshVirtualList();
            }
        }

    }

    //房主发送房间失活udp消息，成员发送玩家变动tcp消息
    protected override void OnHide()
    {
        Debug.Log("OnHide");
        base.Dispose();
        if (MainUI.player.isHost)
        {
            MainUI.currentRoom.isAlive = false;
            RoomManager.CreateRoom(MainUI.currentRoom);
        }
        else
        {
            if (!SceneController.load)
            {
            MainUI.currentRoom.currentPlayer.Remove(MainUI.player);
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, MainUI.currentRoom.currentPlayer);
                byte[] data = new byte[ms.Length];
                System.Array.Copy(ms.GetBuffer(), 0, data, 0, ms.Length);
                ChatRoom.SendMessage(data);
            }
            }
        }
        MainUI.player.isHost = false;
        if (roomDestroyDelegate!=null)
        {
            roomDestroyDelegate();
        }
        // 清空房间列表
        MainUI.rooms.Clear();
        MainUI.currentRoom = null;
    }

    // 随机挑选主机，在房间内序列化传输主机IP
    private void SelectHolder()
    {
        // 随机潜伏者
        int random = new System.Random().Next(0, MainUI.currentRoom.currentPlayer.Count);
        string ip = MainUI.currentRoom.currentPlayer[random].ip;
        HolderInfo holderInfo = new HolderInfo();
        holderInfo.ip = ip;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, holderInfo);
            byte[] data = new byte[ms.Length];
            System.Array.Copy(ms.GetBuffer(), 0, data, 0, ms.Length);
            ChatRoom.SendMessage(data);
        }
    }

    // 异步加载新场景
    private void LoadNewScene()
    {
        // 保存需要加载的目标场景
        Globe.nextSceneName = "Battle";
        SceneController.load = true;
    }

}
