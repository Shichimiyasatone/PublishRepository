using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    // Use this for initialization
    void Start() {
        //LANMultiplayerGame();
    }

    // Update is called once per frame
    void Update() {

    }

    //以单机模式启动
    public void SinglePlayerGame()
    {
        new MapManager().InitMap();
        Debug.Log("启动单机模式！");
    }

    public void LANMultiplayerGame()
    {
        new MapManager().InitLanMap();
    }

}
