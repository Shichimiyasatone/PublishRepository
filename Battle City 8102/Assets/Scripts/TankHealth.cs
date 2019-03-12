using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 控制血量的脚本，hp与进度条关联
 * 血量关系游戏是否结束
 */
public class TankHealth : MonoBehaviour
{
    // 不需要更新，界面不显示其他玩家血条
    // 与其他案例中不同的是，每个玩家的血条并不附着在plyaer object上
    // 所以如果同步此变量，将会导致所有玩家血量同步！
    // 钩子函数，当hp变化时调用OnchangeHealth更新血条
    //[SyncVar(hook = "OnChangeHealth")]
    // 初始血量100
    public float hp = 100;
    // 坦克爆炸特效
    public GameObject tankExplotion;
    // 爆炸音效
    public AudioClip explosionAudio;
    // 血量进度条
    private GameObject battleUIPanel;
    private GProgressBar hpProgressBar;

    // 用于记录总血量，赋值进度条显示
    private float hpTotal;

    private bool flag = false;

    // 默认值为true，坦克初始生成在圈内，触发碰撞器的OnEnter置为false
    private bool bleeding = true;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (battleUIPanel==null)
        {
             battleUIPanel=GameObject.Find("BattleUIPanel");
        }
        else if (!flag)
        {
            GComponent battleComponent = battleUIPanel.GetComponent<UIPanel>().ui;
            GGroup footerGroup = battleComponent.GetChild("footer").asGroup;
            hpProgressBar = battleComponent.GetChildInGroup(footerGroup, "hpProgressBar").asProgress;

            hpTotal = hp;

            flag = true;
        }
        // 圈外持续掉血
        if (bleeding)
        {
            Debug.Log("hp = "+hp);
            hp -= CircleController.stage*Time.deltaTime;
            if (hp <= 0)
            {
                GameObject.Instantiate(tankExplotion, transform.position + Vector3.up, transform.rotation);
                AudioSource.PlayClipAtPoint(explosionAudio, transform.position);
                Destroy(this.gameObject);
                // TODO GAME OVER
            }
        }
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
            // TODO GAME OVER
        }
    }
    void OnChangeHealth(int hp)
    {
        //滑动条的值跟随hp变化
        float value = hp / hpTotal;
        hpProgressBar.value = value;
    }

    // 进出圈改变状态
    void OnChangeBleeding()
    {
        bleeding = bleeding ? false : true;
    }
}
