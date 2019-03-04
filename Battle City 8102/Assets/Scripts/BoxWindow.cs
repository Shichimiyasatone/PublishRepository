using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWindow : Window
{
    private GComponent boxComponent;
    private GList boxList;

    private Box box;

    public BoxWindow(Box box)
    {
        this.box = box;
    }

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Box_Window").asCom;
        contentPane.SetPosition(75,90,0);
        boxComponent = contentPane.GetChild("frame").asCom;

        boxList = boxComponent.GetChild("boxList").asList;
        boxList.SetVirtual();
        boxList.itemRenderer = RenderListItem;
        boxList.numItems = box.goodsList.Count;

        boxComponent.GetChild("holderNameTextField").asTextField.text = box.holderName;
    }

    private void RenderListItem(int index, GObject obj)
    {
        GButton btn = obj.asButton;
        btn.title = "";
        btn.icon = "";
        if (index >= box.goodsList.Count)
        {
            return;
        }
        Goods goods = box.goodsList[index];
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
        btn.GetChild("countTextField").asTextField.text = goods.count + "";
        btn.GetChild("descriptionTextField").asTextField.text = goods.description;
        btn.GetChild("coldDownTextField").asTextField.text = goods.coldDownTime + "";

        //点击物品拾取，清除上次监听
        obj.onClick.Clear();
        obj.onClick.Add(()=> {
            // 判断包满
            if (!GoodsUI.getGoodsDelegate(goods))
            {
                return;
            }
            box.goodsList.RemoveAt(index);
            boxList.numItems = box.goodsList.Count;
            boxList.RefreshVirtualList();
        });

    }
}
