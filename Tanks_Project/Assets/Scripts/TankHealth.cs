using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TankHealth : NetworkBehaviour {
    [SyncVar(hook = "OnChangeHealth")]
    public int hp = 100;
    public GameObject tankExplotion;
    public AudioClip explosionAudio;
    public Slider hpSlider;

    private int hpTotal;

    // Use this for initialization
    void Start() {
        hpTotal = hp;
    }

    // Update is called once per frame
    void Update() {

    }

    void TakeDamage()
    {
        //if (!isServer)
        //{
        //    return;
        //}

        //判断受伤、阵亡，播放特效
        if (hp <= 0)
        {
            return;
        }
        hp -= Random.Range(10, 20);
        if (hp <= 0)
        {
            GameObject.Instantiate(tankExplotion, transform.position + Vector3.up, transform.rotation);
            AudioSource.PlayClipAtPoint(explosionAudio, transform.position);
            Destroy(this.gameObject);
        }
    }
    void OnChangeHealth(int hp)
    {
        //滑动条的值跟随hp变化
        float value = (float)hp / hpTotal;
        hpSlider.value = value;
    }
}
