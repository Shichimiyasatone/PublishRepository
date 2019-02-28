using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody rigidbody;

    private float tankVertical;
    private float horizontal;
    public float moveForce = 150f;
    public float tankRotateSpeed = 60f;
    public float tankMoveSpeed = 60f;
    public float turreRotateSpeed = 60f;
    public float maxSpeed = 10f;
    private float targetDergee;

    private float deltaTurreDegree =0f;
    private float initTurreDegree = 0f;

    private float turreRotateY = 0f;
    private float turreVertical;
    private float deltaTurreRotateY = 0f;


    void Awake()
    {
        BattleUI.moveDelegate += Move;
        CameraFollow.cameraRotate += Turn;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initTurreDegree = transform.localEulerAngles.y;
    }

    void FixedUpdate()
    {                          //返回向量长
        //if (rigidbody.velocity.magnitude > maxSpeed)
        //{                                                   //返回长度为1的向量
        //    rigidbody.velocity = maxSpeed * rigidbody.velocity.normalized;
        //}
        
        if (Mathf.Abs(transform.localEulerAngles.y - targetDergee)<1f)
        {
            tankVertical = 0;
        }    
        rigidbody.angularVelocity = Vector3.up * tankVertical *1;
        rigidbody.velocity = transform.forward * horizontal *10;
        //var angle = transform.localEulerAngles + rotateSpeed * Time.deltaTime * transform.up*vertical;
        //rigidbody.MoveRotation(Quaternion.Euler(angle));

        //rigidbody.AddForce(Vector3.forward * horizontal*moveForce);

        
        if (Mathf.Abs((transform.Find("Main_Turre").transform.localEulerAngles.y + transform.localEulerAngles.y )%360- turreRotateY) < 1f)
        {
            turreVertical = 0;
        }
        else
        {
            deltaTurreRotateY = deltaTurreRotateY + turreVertical;
        }
        //炮塔向父对象的反方向进行旋转，模拟不跟随旋转效果
        transform.Find("Main_Turre").transform.localEulerAngles = Vector3.up * (- transform.localEulerAngles.y+deltaTurreRotateY);
    }

    private void Move(float tankRotateY,float percZ)
    {
        //世界轴
        // 2 1      
        // 3 4     
        //坦克轴
        // 4 1      1、4象限为前进，2、3象限为后退
        // 3 2      4、3象限为左转，1、2象限为右转
        if (tankRotateY >= 361)
        {
            horizontal = 0;
            tankVertical = 0;
            return;
        }
        //将角度转换为正角
        float currentDegree = (transform.localEulerAngles.y + 360) % 360;
        targetDergee = -tankRotateY + 90+turreRotateY;//修正轴
        targetDergee = (targetDergee + 360) % 360;
        percZ = Mathf.Abs(percZ);

        if (currentDegree <= 180 && currentDegree >= 90)
        {
            if (targetDergee >= currentDegree - 90 && targetDergee <= currentDegree + 90)
            {
                horizontal = percZ;
            }
            else
            {

                targetDergee = (targetDergee + 180) % 360;

                horizontal = -percZ;
            }
        }
        else
        {
            if (targetDergee >= (currentDegree + 90) % 360 && targetDergee <= (currentDegree - 90 + 360) % 360)
            {
                horizontal = -percZ;
                targetDergee = (targetDergee + 180) % 360;
            }
            else
            {
                horizontal = percZ;
            }
        }


        if (currentDegree <=180)
        {
            if (targetDergee>=currentDegree&&targetDergee<=currentDegree+180)
            {
                tankVertical = 1;
            }
            else
            {
                tankVertical = -1;
            }
        }
        else
        {
            if (targetDergee>=currentDegree-180&&targetDergee<=currentDegree)
            {
                tankVertical = -1;
            }
            else
            {
                tankVertical = 1;
            }
        }
        
        
    }

    private void Turn(float turreRotateY)
    {
        this.turreRotateY = turreRotateY;
        float currentDegree = (transform.Find("Main_Turre").localEulerAngles.y +transform.localEulerAngles.y+ 360) % 360;
        float targetRotate = (turreRotateY + 360) % 360;

        if (currentDegree <= 180)
        {
            if (targetRotate >= currentDegree && targetRotate <= currentDegree + 180)
            {
                turreVertical = 1;
            }
            else
            {
                turreVertical = -1;
            }
        }
        else
        {
            if (targetRotate >= currentDegree - 180 && targetRotate <= currentDegree)
            {
                turreVertical = -1;
            }
            else
            {
                turreVertical = 1;
            }
        }
    }

}
