using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using DG.Tweening;

/***
 * 房主名长限制在13数字大小内
 * 限制来源于RoomInfoFrame中hostNameTextField的长度
 * 
 */

public class MainUI : MonoBehaviour {
    public static Player player = new Player("wasdTest",4);

    public static int screenX = 1280;
    public static int screenY = 720;

    public delegate void DestroyDelegate();
    public static DestroyDelegate destroyDelegate;

    //对应FGUI中的Main,SelectModeComponent,SelectTankComponent
    private GComponent mainComponent;
    private GComponent tankViewComponent;
    private GComponent selectModeComponent;
    private GComponent roomListComponent;

    //窗口
    private ConfigWindow configWindow;
    private RoomInfoWindow roomInfoWindow;
    private TankListWindow tankListWindow;
    private CreateRmWindow createRmWindow;

    //列表
    GList roomList;

    //选中房间
    public static Room currentRoom;

    //经验条
    public GProgressBar experienceBar;
    //金钱
    public GTextField moneyTextField;
    //房名滚动动效
    private GTweener tweener;


    //测试经验条、金币
    public int money = 100;
    public int experience = 10;
    //测试房间信息
    public List<Room> rooms = new List<Room>();
    //测试坦克总数
    public int tankNum = 10;

    // Use this for initialization
    void Start () {
        //赋值组件
        mainComponent = GetComponent<UIPanel>().ui;
        tankViewComponent = mainComponent.GetChild("tankViewComponent").asCom;
        selectModeComponent = mainComponent.GetChild("selectModeComponent").asCom;
        roomListComponent = UIPackage.CreateObject("BattleCity8102","RoomList_Component").asCom;
        //设置钱
        moneyTextField = mainComponent.GetChild("moneyTextField").asTextField;
        mainComponent.GetChild("configButton").asButton.displayObject.layer = 0;
        mainComponent.GetChildIndex(mainComponent.GetChild("configButton").asButton);

        moneyTextField.text = money.ToString();
        //设置经验
        experienceBar = mainComponent.GetChild("experienceBar").asProgress;
        experienceBar.value = experience;

        //给按钮注册监听
        //设置按钮按下，显示设置窗口
        configWindow = new ConfigWindow(50);//初始音量设置为50
        mainComponent.GetChild("configButton").asButton.onClick.Add(() => {
            configWindow.Show();
        });

        //返回按钮按下，返回主页面
        mainComponent.GetChild("returnButton").asButton.onClick.Add(() => {
            //关闭窗口，移除房间列表组件，将模式选择组件渲染先于经验条
            if (roomInfoWindow!=null)
            {
                roomInfoWindow.Dispose();
            }
            roomListComponent.GetChild("entryRoomButton").asButton.enabled = false;
            currentRoom = null;//当前选中房间为空
            mainComponent.RemoveChild(roomListComponent);
            mainComponent.AddChild(selectModeComponent);
            mainComponent.SetChildIndexBefore(selectModeComponent, mainComponent.GetChildIndex(experienceBar));

            mainComponent.GetChild("configButton").asButton.visible = true;
            mainComponent.GetChild("returnButton").asButton.visible = false;
        });

        //对战按钮按下，加载roomConponent
        selectModeComponent.GetChild("LANBattleButton").onClick.Add(()=> {
            //隐藏设置按钮，显示返回按钮
            mainComponent.GetChild("configButton").asButton.visible = false;
            mainComponent.GetChild("returnButton").asButton.visible = true;

            BattleButtonOnClick(roomListComponent);
        });

        //进入房间按钮按下，显示TankListWindow
        roomListComponent.GetChild("entryRoomButton").asButton.enabled = false;//防止不选房间进行点击
        roomListComponent.GetChild("entryRoomButton").asButton.onClick.Add(()=> {
            if (roomInfoWindow != null)
            {
                roomInfoWindow.Dispose();//关闭房间信息窗口
            }
            tankListWindow = new TankListWindow(tankNum);
            tankListWindow.Show();
        });

        //创建房间时弹窗，创建完成循环发送房间信息
        roomListComponent.GetChild("createRoomButton").asButton.onClick.Add(( )=> {
            MainUI.player.isHost = true;
            createRmWindow = new CreateRmWindow();
            createRmWindow.Show();  
        });

        //刷新时清空当前房间，播放动效，查询房间，重新渲染列表
        Transition t = roomListComponent.GetTransition("t0");
        roomListComponent.GetChild("refreshButton").asButton.onClick.Add(() => {
            currentRoom = null;
            roomListComponent.GetChild("entryRoomButton").asButton.enabled = false;
            t.ChangePlayTimes(2);
            t.Play(()=>{
                roomListComponent.GetChild("refreshMask").visible = false;
                roomListComponent.GetChild("refreshTextField").visible = false;
            });
            rooms = RoomManager.SearchRoom(rooms);
            roomList.itemRenderer = RenderListItem;
            roomList.numItems = rooms.Count;
        });
        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //加载房间列表
    private void BattleButtonOnClick(GComponent targetComponent)
    {
        rooms = RoomManager.SearchRoom(rooms);
        //设置组件位置
        targetComponent.SetXY(screenX / 2, 0);
        //移除模式选择组件，添加房间列表组件
        mainComponent.RemoveChild(selectModeComponent);
        mainComponent.AddChild(targetComponent);
        //注册监听房间按钮，显示房间信息
        roomList = targetComponent.GetChild("roomList").asList;
        roomList.SetVirtual();
        roomList.itemRenderer = RenderListItem;
        //设置房间数目
        roomList.numItems = rooms.Count;
    }

    //渲染roomList
    private void RenderListItem(int index,GObject obj)
    {
        GButton roomButton = obj.asButton;
        roomButton.title = rooms[index].roomName;
        if (rooms[index].currentPlayer.Count>= rooms[index].limitNum)
        {
            roomButton.enabled = false;
        }
        GTextField textField = roomButton.GetChild("title").asTextField;
        //点击房间，显示详细信息
        roomButton.onClick.Add(()=> {
            currentRoom = rooms[index];
            roomListComponent.GetChild("entryRoomButton").asButton.enabled = true;
            if (roomInfoWindow != null)
            {
                roomInfoWindow.Dispose();
            }
            //窗口平行显示在roomButton左侧,需要list位置和scrollpane的滚动比例
            roomInfoWindow = new RoomInfoWindow(rooms[index],roomList.x,roomList.y,roomList.height,roomList.scrollPane.contentHeight,roomButton.y,roomList.scrollPane.percY);
            //Debug.Log("percY:"+roomList.scrollPane.percY);
            roomInfoWindow.Show();
            //文本域超出显示范围，滚动
            if (textField.width > roomButton.width)
            {//调整文本域的位置，使文本域向左移动
                tweener = textField.TweenMove(new Vector2(roomList.width - textField.width, 0), 2.0f).OnComplete(() =>
                {
                    //播放完成后清空tweener，重置位置
                    tweener = null;
                    textField.SetXY(0, 0);
                });
            }
        });
    }

    //程序结束时释放线程、socket
    void OnDestroy()
    {
        try
        {
            destroyDelegate();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

}
