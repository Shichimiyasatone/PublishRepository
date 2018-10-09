using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody rigidbody;
    public float speed = 0.3f;

    private float rotation = 0f;
    private float percZ = 0;

    void Awake()
    {
        BattleUI.moveDelegate += Move;
        BattleUI.turnDelegate += Turn;
    }

    // Use this for initialization
    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Is Kinematic = false，不移物理控制
        //rigidbody.rotation = Quaternion.Euler(new Vector3(0,rotation,0));
        //rigidbody.velocity = transform.forward * speed*percZ;

        //Is  Kinematic = ture,移除物理控制
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        transform.Translate(Vector3.forward * speed * percZ);

        //OnTouchEnd速度应该为0；
    }

    //player朝着给定方向进行移动
    public void Move(float rotation, float percZ)
    { 
        this.percZ = percZ;
        if (percZ>=361)
        {
            //代表OnTouchEnd
            return;
        }
        this.rotation = rotation;
    }

    public void Turn(float turnX, float turnY)
    {

    }
}
