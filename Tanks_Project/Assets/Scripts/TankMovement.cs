using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankMovement : NetworkBehaviour {

    public float speed = 5;
    public float angularSpeed = 5;
    public float playerNum = 1;//设置玩家编号，区分移动控制

    public AudioClip idleAudio;
    public AudioClip moveAudio;
    private AudioSource audio;

    private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isLocalPlayer)
        {
            return;
        }
        float v = Input.GetAxis("VerticalPlayer"+playerNum);
        float h = Input.GetAxis("HorizontalPlayer" + playerNum);
        rigidbody.velocity = transform.forward * v *speed;
        if(v < 0)//倒车时转向调换
        {
            h = -h;
        }
        rigidbody.angularVelocity = transform.up * h * angularSpeed;
        if (v == 0 && h ==0)
        {
            if (audio.isPlaying == false)
            {
                audio.clip = idleAudio;
                audio.Play();
            }  
        }
        else
        {
            if (audio.isPlaying == false)
            {
                audio.clip = moveAudio;
                audio.Play();
            } 
        }
    }
}
