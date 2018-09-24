using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelescopicView : MonoBehaviour
{

    //公有成员

    public float ZoomLevel = 2.0f;

    public float ZoomInSpeed = 100.0f;

    public float ZoomOutSpeed = 100.0f;

    //私有成员

    private float initFOV;

    //脚本初始化时，调用此函数

    void Start()

    {

        //获取当前摄像机的视野范围

        initFOV = Camera.main.fieldOfView;
        //Debug.Log("初始化FOV=" + initFOV);
    }

    //运行游戏时的每一帧，都调用此函数

    void Update()

    {

        //当鼠标左键按下时

        if (Input.GetKey(KeyCode.Mouse0))

        {

            ZoomView();

        }

        else

        {

            ZoomOut();

        }

    }

    //放大摄像机的视野区域

    void ZoomView()


    {
        //Debug.Log("触发ZoomView");

        if (Mathf.Abs(Camera.main.fieldOfView - (initFOV / ZoomLevel)) < 0.5f)

        {

            Camera.main.fieldOfView = initFOV / ZoomLevel;

        }

        else if (Camera.main.fieldOfView - (Time.deltaTime * ZoomInSpeed) >= (initFOV / ZoomLevel))

        {

            Camera.main.fieldOfView -= (Time.deltaTime * ZoomInSpeed);

        }

    }

    //缩小摄像机的视野区域

    void ZoomOut()

    {
        //Debug.Log("触发ZoomOut");
        if (Mathf.Abs(Camera.main.fieldOfView - initFOV) < 0.5f)

        {

            Camera.main.fieldOfView = initFOV;

        }

        else if (Camera.main.fieldOfView + (Time.deltaTime * ZoomOutSpeed) <= initFOV)
        {

            Camera.main.fieldOfView += (Time.deltaTime * ZoomOutSpeed);

        }

    }
}
