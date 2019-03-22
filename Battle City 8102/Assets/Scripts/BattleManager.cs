using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 控制战斗时的网络连接
 */
public class BattleManager : MonoBehaviour {

    NetworkManager manager;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();

        Debug.Log("开启服务器");
        manager.StartHost();

        //Debug.Log("开启客户端");
        //manager.StartClient();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
