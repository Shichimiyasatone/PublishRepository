using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 描述盒子的类，包含一个List<Goods>
 * 暂时存放了一些用于测试的数据
 */
public class Box : MonoBehaviour {

    // 盒子内的物品
    public List<Goods> goodsList;
    // 谁的盒子
    public string holderName;

	// 用于测试，赋初始值
	void Start () {
        goodsList = new List<Goods>();
        Goods goods = new Goods();
        goods.type = "bullet";
        goods.icon = "762";
        goods.count = 160;
        goods.goodsName = "7.62毫米子弹";
        goods.description = "步枪、狙击枪使用";
        goods.coldDownTime = 2;
        goodsList.Add(goods);

        goods = new Goods();
        goods.type = "medecine";
        goods.icon = "painKiller";
        goods.count = 3;
        goods.goodsName = "止痛药";
        goods.description = "提升70%能量";
        goods.coldDownTime = 8;
        goodsList.Add(goods);

        goods = new Goods();
        goods.type = "bullet";
        goods.icon = "762";
        goods.count = 160;
        goods.goodsName = "7.62毫米子弹";
        goods.description = "步枪、狙击枪使用";
        goods.coldDownTime = 2;
        goodsList.Add(goods);

        goods = new Goods();
        goods.type = "bullet";
        goods.icon = "762";
        goods.count = 160;
        goods.goodsName = "7.62毫米子弹";
        goods.description = "步枪、狙击枪使用";
        goods.coldDownTime = 2;
        goodsList.Add(goods);

        goods = new Goods();
        goods.type = "bullet";
        goods.icon = "762";
        goods.count = 160;
        goods.goodsName = "7.62毫米子弹";
        goods.description = "步枪、狙击枪使用";
        goods.coldDownTime = 2;
        goodsList.Add(goods);

        holderName = "卢姥爷";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
