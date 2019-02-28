using System.Collections;
using System.Collections.Generic;
using System;

//本类可序列化，方便传输
[Serializable]
public class Room : IEquatable<Room>{

    //暂定房名、房主、地图为字符串
    public string roomName;
    public string roomHost;//房主后期可能为类
    public string roomMap;
    public string roomMode;
    public List<Player> currentPlayer;
    public int limitNum;
    public bool isAlive;
    public string hostIP;

    public Room(string name,string host,string map,string mode,List<Player> player,int limitNum)
    {
        this.roomName = name;
        this.roomHost = host;
        this.roomMap = map;
        this.roomMode = mode;
        this.currentPlayer = player;
        this.limitNum = limitNum;
    }

    //实现接口的比较方法，方便list使用Contains进行比较
    public bool Equals(Room room)
    {
        if (this.hostIP.Equals(room.hostIP))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
