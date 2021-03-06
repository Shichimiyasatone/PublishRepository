﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 描述道具的类，记录道具的种类、名字、描述、数量
 * 用于挂载在游戏物体上，道具生成时应有一个随机唯一的名字
 */
public class Goods  :MonoBehaviour{

    public string type;
    [Tooltip("道具url")]
    public string icon;
    public int count;
    public string goodsName;
    public string description;
    public float coldDownTime;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void destroy()
    {
        // 移除自身
        Destroy(this.gameObject);
    }

    // 添加可拾取物体
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            collider.SendMessage("addGoods",this);
            Debug.Log("碰撞物体！");
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            collider.SendMessage("removeGoods", this);
            Debug.Log("远离物体！");
        }
    }
}
