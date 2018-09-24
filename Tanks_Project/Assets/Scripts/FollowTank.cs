using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTank : MonoBehaviour {

    public static Transform tank1;
    public static Transform tank2;

    public Camera camera;

    private Vector3 offset;

    void Awake()
    {
        //MapManager.action += SetTank;

        //MapManager.InitEent += SetTank;
    }

	// Use this for initialization
	void Start () {
        //tank1 = GameObject.Find("GameManager").GetComponent<MapManager>().tank1.transform;
        //tank2 = GameObject.Find("GameManager").GetComponent<MapManager>().tank2.transform;
        
        //Debug.Log("TANK_1 POSITION = "+tank1.position);
        //Debug.Log("TANK_2 POSITION = " + tank2.position);
        //offset = transform.position - (tank2.position + tank1.position) / 2;
        //camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update () {
        //if (tank1 == null || tank2 ==null)
        //{
        //    return;
        //}
        //transform.position = (tank2.position + tank1.position) / 2 + offset;
        //float distance = Vector3.Distance(tank1.position, tank2.position);
        //float size = distance * 0.58f;
        //camera.orthographicSize = size;
    }

    public static void SetTank1(GameObject tank)
    {
        tank1 = tank.transform;
    }

    public static void SetTank2(GameObject tank)
    {
        tank2 = tank.transform;
    }

    public void SetTank(GameObject tank)
    {
        Debug.Log("我拿到"+tank.name);
        if (tank.name.Equals("tank1"))
        {
            tank1 = tank.transform;
        }
        else if (tank.name.Equals("tank2"))
        {
            tank2 = tank.transform;
        }
    }
}
