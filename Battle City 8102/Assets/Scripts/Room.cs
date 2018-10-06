using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    //暂定房名、房主、地图为字符串
    public string roomName;
    public string roomHost;//房主后期可能为类
    public string roomMap;
    public string roomMode;

    public Room(string name,string host,string map,string mode)
    {
        this.roomName = name;
        this.roomHost = host;
        this.roomMap = map;
        this.roomMode = mode;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
