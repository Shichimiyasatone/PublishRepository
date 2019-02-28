using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RoomInfoWindow : Window {

    public Room room;
    private float roomListX;
    private float roomListHeight;
    private float scrollPaneContentHeight;
    private float buttonY;
    private float percY;
    private float roomListY;

    public RoomInfoWindow(Room room,float roomListX,float roomListY,float roomListHeight,float scrollPaneContentHeight,float buttonY,float percY)
    {
        this.room = room;
        //获得必要坐标，使得window平行显示在button左侧
        this.roomListX = roomListX;
        this.roomListHeight = roomListHeight;
        this.scrollPaneContentHeight = scrollPaneContentHeight;
        this.buttonY = buttonY;
        this.percY = percY;
        this.roomListY = roomListY;
    }

    protected override void OnInit()
    {
        this.contentPane = UIPackage.CreateObject("BattleCity8102", "RoomInfo_Window").asCom;
        /***设置Window位置
        *可显示的List高为roomListHeight，实际List总长为scrollPaneContentPane，纵向滑动率为percY，
        *屏幕宽为screenX，List相对父组件的位置为（roomListX，roomListY），
        *设button在List内的Y轴距离为Y1，已知button相对List的Y轴距离为buttonY，
        *求button显示时相对List父组件的位置的Y值y：
        *因为，（Y1-roomListHeight）/（scrollPaneContentHeight - roomListHeight）=percY
        *通过上式子可求得button在List内位置的Y值Y1，又设deltaY=Y1-buttonY
        *所以，y=roomListY+detalY
        */
        float x = roomListX + MainUI.screenX / 2 -contentPane.width;
        float y = roomListY + roomListHeight - (percY * (scrollPaneContentHeight - roomListHeight)+roomListHeight - buttonY);

        //防止window出屏幕
        if (y>MainUI.screenY- contentPane.height)
        {
            y = MainUI.screenY - contentPane.height;
        }

        contentPane.SetXY(x,y);
        //对房主、地图、模式进行赋值
        //地图图片尚未赋值！
        contentPane.GetChild("frame").asCom.GetChild("hostTextField").asTextField.text = room.roomHost;
        contentPane.GetChild("frame").asCom.GetChild("mapTextField").asTextField.text = room.roomMap;
        contentPane.GetChild("frame").asCom.GetChild("modeTextField").asTextField.text = room.roomMode;
        contentPane.GetChild("frame").asCom.GetChild("playerNumTextField").asTextField.text = room.currentPlayer.Count + "/" + room.limitNum;
        this.onClick.Add(()=> { this.Dispose(); });
    }
}
