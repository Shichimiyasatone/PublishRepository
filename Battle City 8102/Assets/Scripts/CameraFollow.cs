using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject tank;

    private float initRotate;
    private float deltaRotate;

    public delegate void CameraRotate(float rotate);
    public CameraRotate cameraRotate;

    private bool flag = false;

    void Start()
    {
        initRotate = transform.localEulerAngles.y;
    }

    void Update()
    {
        if (tank == null)
        {
            return;
        }
        transform.position = tank.transform.position + new Vector3(0,5,0);
        //展示仅处理Y轴转动
        transform.localEulerAngles = Vector3.up * (deltaRotate + initRotate);
    }

    public void Rotate(float rotateY,float rotateX)
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
