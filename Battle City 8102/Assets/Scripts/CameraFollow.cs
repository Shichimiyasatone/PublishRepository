using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private GameObject tank;

    private float initRotate;
    private float deltaRotate;

    public delegate void CameraRotate(float rotate);
    public static CameraRotate cameraRotate;

    private bool flag = false;

    void Awake()
    {
        
    }

    void Start()
    {
        // TODO
        // 不能通过该种方式绑定坦克，网络中生成的坦克默认都叫XX(Clone)
        initRotate = transform.localEulerAngles.y;

        tank = GameObject.Find("m1(Clone)");
        Debug.Log(tank);
    }

    void Update()
    {
        // 在战斗场景UI加载完时注册相机旋转委托
        if (!flag&&GameObject.Find("BattleUIPanel")!=null)
        {
            BattleUI.turnDelegate += Rotate;

            flag = true;
        }
        // 或者使用委托赋值
        if (tank == null)
        {

            tank = GameObject.Find("m1(Clone)");
            return;
        }
        transform.position = tank.transform.position + new Vector3(0,5,0);
        //展示仅处理Y轴转动
        transform.localEulerAngles = Vector3.up * (deltaRotate + initRotate);
    }

    void Rotate(float rotateY,float rotateX)
    {
        if (rotateY >=361)
        {
            initRotate = transform.localEulerAngles.y;
            deltaRotate = 0;
        }
        deltaRotate = rotateY;

        cameraRotate(transform.localEulerAngles.y);
    }
}
