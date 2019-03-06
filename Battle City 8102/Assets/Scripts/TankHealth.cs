using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 控制血量的脚本，hp与进度条关联
 * 此物体应为网络物体
 */
// NetworkBehaviour
public class TankHealth : MonoBehaviour
{
    //[SyncVar(hook = "OnChangeHealth")]
    public int hp = 100;
    public GameObject tankExplotion;
    public AudioClip explosionAudio;
    // 血量进度条
    private GProgressBar hpProgressBar;

    private int hpTotal;

    // Use this for initialization
    void Start()
    {
        //Debug.Log(GRoot.inst.GetChild("Battle_Component").name);
        //GGroup footerGroup = GRoot.inst.GetChild("footer").asGroup;
        //hpProgressBar = GRoot.inst.GetChildInGroup(footerGroup, "hpProgressBar").asProgress;

        hpTotal = hp;
    }

    // Update is called once per frame
    void Update()
    {

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
        hpProgressBar.value = value;
    }
}
