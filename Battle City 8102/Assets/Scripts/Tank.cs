using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    // 定义物品列表委托
    public delegate void GoodsDelegate(List<Goods> goodsList);

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

    void addGoods(Goods goods)
    {
        if (goods == null)
        {
            return;
        }
        if (!goodsList.Contains(goods))
        {
            goodsList.Add(goods);
        }
        // 调用委托，显示物品列表
        goodsDelegate(goodsList);
    }

    void removeGoods(Goods goods)
    {
        // 防止空指针
        if (goods == null)
        {
            return;
        }
        if (goodsList.Contains(goods))
        {
            goodsList.Remove(goods);
        }
        // 更新物品列表
        goodsDelegate(goodsList);
    }

}
