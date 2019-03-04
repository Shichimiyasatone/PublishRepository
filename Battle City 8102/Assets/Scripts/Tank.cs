using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    // 定义盒子、物品列表委托
    public delegate void BoxDelegate(Box box);
    public delegate void GoodsDelegate(List<Goods> goodsList);

    public static BoxDelegate boxDelegate;
    public static GoodsDelegate goodsDelegate;

    // 用于统计附近道具
    public List<Goods> goodsList;

    // Use this for initialization
    void Start () {
        goodsList = new List<Goods>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    // 添加可拾取物体
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Goods")
        {
            Goods goods = collider.gameObject.GetComponent<Goods>();
            // 防止空指针
            if (goods == null)
            {
                return;
            }
            if (!goodsList.Contains(goods))
            {
                Debug.Log("添加"+goods.goodsName + "X"+goods.count);
                goodsList.Add(goods);
            }
            // 调用委托，显示物品列表
            goodsDelegate(goodsList);
        }else if (collider.tag == "Box")
        {
            Box box = collider.gameObject.GetComponent<Box>();
            if (box == null)
            {
                return;
            }
            // 调用委托，显示盒子列表
            boxDelegate(box);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Goods")
        {
            Goods goods = collider.gameObject.GetComponent<Goods>();
            // 防止空指针
            if (goods == null)
            {
                return;
            }
            if (goodsList.Contains(goods))
            {
                Debug.Log("移除" + goods.goodsName + "X" + goods.count);
                goodsList.Remove(goods);
            }
            // 更新物品列表
            goodsDelegate(goodsList);
        }
        else if (collider.tag =="Box")
        {
            boxDelegate(null);
        }
    }
}
