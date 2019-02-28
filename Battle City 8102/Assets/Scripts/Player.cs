using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Player  : IEquatable<Player>
{

    public string name;
    public int rank;
    public bool isHost = false;
    public bool isReady = false;
    public string ip;

    public Player(string name ,int rank)
    {
        this.name = name;
        this.rank = rank;
        this.ip = IPv4.GetLocalIP();
    }

    public bool Equals(Player player)
    {
        if (this.ip.Equals(player.ip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
