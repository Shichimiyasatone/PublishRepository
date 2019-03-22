using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DropGoods : NetworkBehaviour
{

    // 丢弃物品的委托
    public delegate void DropGoodsDelegate(GameObject go);
    public static DropGoodsDelegate dropGoodsDelegate;

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
        dropGoodsDelegate += InstantiateGoods;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // TODO
    // 其他客户端接收到的go为null！
    // 请求服务器执行
    [Command]
    private void CmdDropGoods(GameObject go)
    {
        // 孵化丢弃的物品
        Debug.Log(go);
        NetworkServer.Spawn(go);
    }

    private void InstantiateGoods(GameObject go){
        //GameObject gb = Instantiate(go, transform.position, transform.rotation);
        go.transform.position = transform.position;
        CmdDropGoods(go);
}
}
