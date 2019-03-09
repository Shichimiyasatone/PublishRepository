using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 控制血量的脚本，hp与进度条关联
 * 此物体应为网络物体
 */
public class TankHealth : NetworkBehaviour
{
    // 钩子函数，当hp变化时调用OnchangeHealth更新血条
    [SyncVar(hook = "OnChangeHealth")]
    // 初始血量100
    public int hp = 100;
    // 坦克爆炸特效
    public GameObject tankExplotion;
    // 爆炸音效
    public AudioClip explosionAudio;
    // 血量进度条
    public GameObject battleUIPanel;
    private GProgressBar hpProgressBar;

    // 用于记录总血量，赋值进度条显示
    private int hpTotal;

    // Use this for initialization
    void Start()
    {
        //GComponent battleComponent = battleUIPanel.GetComponent<UIPanel>().ui;
        //GGroup footerGroup = battleComponent.GetChild("footer").asGroup;
        //hpProgressBar = battleComponent.GetChildInGroup(footerGroup, "hpProgressBar").asProgress;

        //hpTotal = hp;
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
