using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsWindow : Window {

    GComponent goodsComponent;

    GList goodsGList;

    List<Goods> goodsList;

    public GoodsWindow(List<Goods> goodsList)
    {
        this.goodsList = goodsList;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Goods_Window").asCom;
        contentPane.SetPosition(150, 85, 0);
        goodsComponent = contentPane.GetChild("frame").asCom;

        goodsGList = goodsComponent.GetChild("goodsList").asList;
        goodsGList.SetVirtual();
        goodsGList.itemRenderer = RenderListItem;
        goodsGList.numItems = goodsList.Count;
    }

    private void RenderListItem(int index, GObject obj)
    {
        GButton btn = obj.asButton;
        btn.title = "";
        btn.icon = "";
        if (index >= this.goodsList.Count)
        {
            return;
        }
        Goods goods = this.goodsList[index];
        obj.onClick.Clear();
        obj.onClick.Add(() => {
            // 判断包满
            if (!GoodsUI.getGoodsDelegate(goods))
            {
                return;
            }
            // 移除物品
            goodsList.RemoveAt(index);
            // 通过名字查找实体，调用方法将实体从世界移除
            GameObject.Find(goods.name).GetComponent<Goods>().destroy();
            // 重置虚拟列表显示数量
            goodsGList.numItems = goodsList.Count;
            if (goodsGList.numItems == 0)
            {
                Hide();
            }
            // 刷新列表，物品将向前位移
            goodsGList.RefreshVirtualList();
        });
        btn.title = goods.type;
        // 判断icon是否为具体路径
        string url = goods.icon;
        if (!url.StartsWith("ui"))
        {
            url = UIPackage.GetItemURL("BattleCity8102", "goods-" + goods.icon);
        }
        btn.icon = url;
        // 背景#EFC324表示重要物资，#FFFFFF为普通物资
        //btn.GetChild("rightBg").asGraph.color = new Color();
        //btn.GetChild("leftBg").asGraph.color = new Color();
        btn.GetChild("nameTextField").asTextField.text = goods.goodsName;
        btn.GetChild("descriptionTextField").asTextField.text = goods.description;
        btn.GetChild("countTextField").asTextField.text = goods.count + "";
        btn.GetChild("coldDownTextField").asTextField.text = goods.coldDownTime + "";
    }
}
