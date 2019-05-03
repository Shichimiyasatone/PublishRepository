using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Player  : IEquatable<Player>
{

    public string name="";
    public int rank=1;
    public bool isHost = false;
    public bool isReady = false;
    public string ip="";

    [NonSerialized]
    public int money=0;
    [NonSerialized]
    public int experience = 0;

    public Player()
    {

    }

    public Player(string name ,int rank)
    {
        this.name = name;
        this.rank = rank;
        this.ip = IPv4.GetLocalIP();
    }

    public bool Equals(Player player)
    {
        if (ip==player.ip)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
