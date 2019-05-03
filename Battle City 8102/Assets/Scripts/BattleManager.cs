using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 控制战斗时的网络连接
 */
public class BattleManager : MonoBehaviour {

    NetworkManager manager;
    private HolderInfo holderInfo;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();

        holderInfo = HolderInfoManager.holderInfo;
        Debug.Log("isHolder = "+holderInfo.isHolder);
        Debug.Log("ip = " + holderInfo.ip);
        if (holderInfo.isHolder)
        {
            Debug.Log("开启服务器");
            manager.StartHost();
        }
        else
        {
            Debug.Log("开启客户端");
            manager.networkAddress = holderInfo.ip;
            manager.StartClient();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
