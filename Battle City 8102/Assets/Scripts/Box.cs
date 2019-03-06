using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 描述盒子的类，包含一个List<Goods>
 * 暂时存放了一些用于测试的数据
 */
public class Box : MonoBehaviour {

    // 定义盒子委托
    public delegate void BoxDelegate(Box box);
    public static BoxDelegate boxDelegate;

    // 盒子内的物品
    public List<Goods> goodsList;
    // 谁的盒子
    public string holderName;

	// 用于测试，赋初始值
	void Start () {
        goodsList = new List<Goods>();
        Goods goods = new Goods("bullet", "762", 160, "7.62毫米子弹", "步枪、狙击枪使用", 2, TemporarEffect.TestEffect);
        goods = new Goods("medecine", "painKiller", 3, "止痛药", "提升70%能量", 8, null);
        goods = new Goods("bullet", "762", 160, "7.62毫米子弹", "步枪、狙击枪使用", 2, null);
        goods = new Goods("bullet", "762", 160, "7.62毫米子弹", "步枪、狙击枪使用", 2, null);
        goods = new Goods("bullet", "762", 160, "7.62毫米子弹", "步枪、狙击枪使用", 2, null);

        holderName = "卢姥爷";
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // 添加可拾取物体
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            // 调用委托，显示盒子列表
            boxDelegate(this);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            boxDelegate(null);
        }

    }
}
