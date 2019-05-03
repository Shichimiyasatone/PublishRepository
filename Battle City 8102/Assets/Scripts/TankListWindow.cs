using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class TankListWindow : Window {

    public string currentTank;
    private int tankNum;
    private GList tankList;

	public TankListWindow(int num)
    {
        tankNum = num;
    }

    protected override void OnInit()
    {
        this.contentPane = UIPackage.CreateObject("BattleCity8102", "TankList_Window").asCom;
        GComponent frameComponent = contentPane.GetChild("frame").asCom;
        //渲染TankList
        tankList = frameComponent.GetChild("tankList").asList;
        tankList.SetVirtual();
        tankList.itemRenderer = RenderListItem;
        tankList.numItems = tankNum;
        //添加滚动特效
        tankList.scrollPane.onScroll.Add(DoSpecialEffect);
        DoSpecialEffect();
        //加入房间
        contentPane.GetChild("frame").asCom.GetChild("tickButton").onClick.Add(()=>{
            contentPane.Dispose();
            MainUI.roomWindow = new RoomWindow();
            MainUI.roomWindow.Show();
        });
    }

    private void RenderListItem(int index,GObject obj)
    {
        GButton tankButton = obj.asButton;
        //设置锚点为中心
        tankButton.SetPivot(0.5f, 0.5f);
        //通过URL加载图片
        tankButton.icon = UIPackage.GetItemURL("BattleCity8102","v0");
        //将title赋值给currentTank
        tankButton.title = icon + index;
        tankButton.onClick.Add(()=> {
            currentTank = tankButton.title;
        });
    }

    //处于中部的坦克放大
    private void DoSpecialEffect()
    {
        float listCenter = tankList.scrollPane.posX + tankList.viewWidth / 2;
        for (int i = 0; i < tankList.numChildren; i++)
        {
            GObject item = tankList.GetChildAt(i);
            float itemCenter = item.x + item.width / 2;
            float itemWidth = item.width;
            float distance = Mathf.Abs(listCenter - itemCenter);
            //判断子物体是否进入放大范围（距中心一个item宽）
            if (distance < itemWidth)
            {
                //按照0.2的系数，依据“距离/范围”进行缩放
                float distanceRange = 1 + (1 - distance / itemWidth) * 0.2f;
                item.SetScale(distanceRange, distanceRange);
            }
            //范围外保持原大小
            else
            {
                item.SetScale(1, 1);
            }
        }
    }

}
