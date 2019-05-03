using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class BattleUI : MonoBehaviour {

    //前进率与前进角度
    public delegate void MoveDelegate(float rotation, float percZ);
    public delegate void TurnDelegate(float turnUp, float turnDown);

    public MoveDelegate moveDelegate;
    public TurnDelegate turnDelegate;

    private GComponent battleComponent;

    //移动、转向数值显示文本
    private GTextField degreeTextField;
    private GTextField permTextField;
    private GTextField turnXTextField;
    private GTextField turnYTextField;

    private Joystick joystick;
    private TurnAround turnAround;
    private GList compassList;//循环列表替代指南针

    //用于计算坐标
    private float itemWidth = 0f;
    private float lastDegree = 0f;
    private float lastPercX = 0f;
    private float initRightPercX = 0f;
    private float initLeftPercX = 0f;

    public static SettlementWindow settlementWindow;

    // Use this for initialization
    void Start () {
        battleComponent = GetComponent<UIPanel>().ui;

        degreeTextField = battleComponent.GetChild("degreeTextField").asTextField;
        permTextField = battleComponent.GetChild("permTextField").asTextField;//前进率，范围0-1
        turnXTextField = battleComponent.GetChild("turnXTextField").asTextField;
        turnYTextField = battleComponent.GetChild("turnYTextField").asTextField;

        //控制移动
        joystick = new Joystick(battleComponent);
        joystick.onMove.Add(JoystickOnMove);
        joystick.onEnd.Add(JoystickOnEnd);
        //控制转向
        turnAround = new TurnAround(battleComponent);
        turnAround.onEnd.Add(TurnAroundOnEnd);
        turnAround.onMove.Add(TurnAroundOnMove);
        //指南针
        compassList = battleComponent.GetChild("compassList").asList;
        compassList.SetVirtualAndLoop();

        compassList.itemRenderer = (int index,GObject obj)=> { itemWidth = obj.asButton.width; };
        compassList.numItems = 1;
        compassList.scrollPane.bouncebackEffect = false;
        /*  循环列表实现时会产生2个顺序排列的内容物，每个内容物里装有3个item
         *  每当视窗滑动至左边界时，将移动到第二个内容物首部继续滑动
         *  每当视窗滑动至右边界时，将移动到第一个内容物尾部继续滑动
         *  因为设计时可视宽为X，一个item宽为4X
         *  所以scrollPane.contentWidth=2*3*4X=24X
         *  设视窗底当前滑动位置X'
         *  滚动率perX=(X'-X)/23X
         *  因为每4个X为同一个item，将X'转换至一个item内
         *  Xi = X'%4
         *  设一个item头尾代表0°-360°
         *  则，degree = Xi * 360°/4
         */

        //compassList.scrollPane.onScroll.Add(()=> {
        //    battleComponent.GetChild("perXTextField").asTextField.text = "perX：" + compassList.scrollPane.percX;

        //});
       
        //将窗体滑至中部，避免从0开始向左滑动percX突变为0.52
        initRightPercX = ((compassList.scrollPane.contentWidth / 2 + compassList.width) - compassList.width) / (compassList.scrollPane.contentWidth - compassList.width);
        initLeftPercX = ((compassList.scrollPane.contentWidth / 2 ) - compassList.width) / (compassList.scrollPane.contentWidth - compassList.width);
        lastPercX = initRightPercX;
        compassList.scrollPane.SetPercX(initRightPercX, false);

        // 测试
        GTextField holderText = battleComponent.GetChild("holderTextField").asTextField;
        holderText.text = "isHolder："+HolderInfoManager.holderInfo.isHolder.ToString()+" "+HolderInfoManager.holderInfo.ip;
        GTextField playerText = battleComponent.GetChild("playerTextField").asTextField;
        playerText.text = "player：" + IPv4.GetLocalIP();
    }

    // Update is called once per frame
    void Update () {

	}

    //数组第一个值为弧度，第二值为底边长，第三个值为直角边长
    //摇杆的坐标轴右侧为X轴正方向，下侧为Y轴正方向
    private void JoystickOnMove(EventContext eventContext)
    {
        float[] data = (float[])eventContext.data;
        float degree = data[0] * 180 / Mathf.PI;
        float percZ;
        //避免除零
        if (degree==90||degree ==0||degree == 180)
        {
            percZ = data[2]/joystick.radius;
        }
        else if (degree == 270)
        {
            percZ = -data[2] / joystick.radius;
        }
        else
        {
            percZ = data[1]/Mathf.Sin(degree/180*Mathf.PI) / joystick.radius;
        }  
        degreeTextField.text = "角度：" + (degree+90);//与transform显示一致
        permTextField.text = "前进率：" +percZ;
        //调整角度使得X轴正方向在又，Y正方向在上
        moveDelegate(-degree,percZ);
    }

    private void JoystickOnEnd()
    {
        degreeTextField.text = "角度：";
        permTextField.text = "前进率：";
        //结束时速度为0
        moveDelegate(361, 0);
    }

    //第一个值为横轴转角，第二个值为纵轴转角
    private void TurnAroundOnMove(EventContext eventContext)
    {

        float[] data = (float[])eventContext.data;
        turnXTextField.text = "横轴转角："+data[0];
        turnYTextField.text = "纵轴转角："+data[1];
        turnDelegate(data[0],data[1]);
        float deltaPercX = DegreeToPercX(compassList, itemWidth, data[0] - lastDegree);
        lastPercX =lastPercX+deltaPercX;
        if (lastPercX<=0)
        {
            lastPercX = initRightPercX;
        }
        else if (lastPercX >=1)
        {
            lastPercX = initLeftPercX;
        }
        lastDegree = data[0];
        battleComponent.GetChild("compassTextField").asTextField.text = "compass:" + lastPercX;
        compassList.scrollPane.SetPercX(lastPercX, false);
        turnDelegate(data[0],data[1]);
    }

    private void TurnAroundOnEnd()
    {
        turnXTextField.text = "横轴转角：";
        turnYTextField.text = "纵轴转角：";
        lastDegree = 0;
        //结束
        turnDelegate(361, 361);
    }

    private float perXToDegree(GList list,float itemWidth)
    {
        float contentWidth = list.scrollPane.contentWidth;
        float viewWidth = list.width;
        float deltaX = contentWidth - viewWidth;
        float percX = list.scrollPane.percX;
        float x = percX * deltaX + viewWidth;
        //让连接处为0°
        float degree = (x%itemWidth)/itemWidth *360-45;
        return degree;
    }

    //相机Y旋转值传入，计算出percX
    private float DegreeToPercX(GList list, float itemWidth,float degree)
    {
        /*将一个itemWidth看做圆的周长，根据角度求弧长
         * 设前进到x
         * deltax/π*r = degree/180
         * 2*π*r = itemWidth
         * 用deltaX求deltaPrecX
         */
        float percX = ((list.width * 2 * degree) / 180) / (list.scrollPane.contentWidth - list.width);
        return percX;
    }
}
