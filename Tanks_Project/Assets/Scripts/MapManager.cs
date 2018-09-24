using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class MyEvent:UnityEvent<GameObject> { }

public class MapManager : NetworkBehaviour {

    public GameObject floor;
    public GameObject tank1;
    public GameObject tank2;
    public Material red;
    public Material green;

    //public delegate void InitGO(GameObject go);
    //public static event InitGO InitEent;

    public static UnityAction<GameObject> action;
    public MyEvent myEvent = new MyEvent();

    void Awake()
    {
        action = new UnityAction<GameObject>(MyFunction);
    }

    // Use this for initialization
    void Start() {
        //InitMap();
    }

    // Update is called once per frame
    void Update() {

    }

    public void InitLanMap()
    {
        GameObject.Instantiate(floor);
    }

    public void InitMap()
    {

        myEvent.AddListener(action);
        //实例化地图
        GameObject.Instantiate(floor);
        //实例化tank
        GameObject go = GameObject.Instantiate(tank1, new Vector3(1, 0, 1), new Quaternion()) as GameObject;
        //tank1.GetComponent<Tank>().SetColor(red);
        go.GetComponent<TankMovement>().playerNum = 1;
        go.GetComponent<TankAttack>().fireKey = KeyCode.Space;
        go.name = "tank1";
        if (myEvent != null)
        {
            myEvent.Invoke(go);
        }
        //if (InitEent != null)
        //{
        //    InitEent(go);
        //}
        //FollowTank.SetTank1(go);
        go = GameObject.Instantiate(tank2, new Vector3(-20, 0, 10), new Quaternion()) as GameObject;
        go.GetComponent<TankMovement>().playerNum = 2;
        go.name = "tank2";
        if (myEvent != null)
        {
            myEvent.Invoke(go);
        }
        //if (InitEent != null)
        //{
        //    InitEent(go);
        //}
        //FollowTank.SetTank2(go);
        //


    }

    public void MyFunction(GameObject go)
    {

    }

}
