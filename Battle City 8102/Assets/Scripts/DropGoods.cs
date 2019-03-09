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
        dropGoodsDelegate += CmdDropGoods;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // 请求服务器执行
    [Command]
    private void CmdDropGoods(GameObject go)
    {
        GameObject goods =  GameObject.Instantiate(go, this.transform.position, this.transform.rotation) as GameObject;
        // 孵化丢弃的物品
        NetworkServer.Spawn(goods);
    }
}
