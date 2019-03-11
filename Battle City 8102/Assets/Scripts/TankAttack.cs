using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/**
 * 坦克脚本，通过UI按钮发射
 * 此物体应为网络物体，由Client发起Command，服务器执行开火并同步
 */
public class TankAttack : NetworkBehaviour
{
    public GameObject shellPrefab;
    public float shellSpeed = 50;
    public AudioClip shotAudio;
    private Transform firePosition;

    private GameObject battleUIPanel;
    private GButton fireButton;

    private bool flag= false;
    // Use this for initialization
    void Start()
    {
        

        firePosition = transform.Find("Main_Turre").Find("FirePositon");
    }

    // Update is called once per frame
    void Update()
    {
        if (!flag&&battleUIPanel == null)
        {
            battleUIPanel = GameObject.Find("BattleUIPanel");

            GComponent battleComponent = battleUIPanel.GetComponent<UIPanel>().ui;
            fireButton = battleComponent.GetChild("fireButton").asButton;
            fireButton.onClick.Add(() => {
                Debug.Log("开火");
                if (!isLocalPlayer)
                {
                    Debug.Log("我是server?" + isServer);
                }
                CmdLanPlayerFire();
            });

            flag = true;
        }
    }

    //void SinglePlayerFire()
    //{
    //    GameObject go = GameObject.Instantiate(shellPrefab, firePosition.position, firePosition.rotation) as GameObject;
    //    go.GetComponent<Rigidbody>().velocity = go.transform.forward * shellSpeed;
    //    Debug.Log(go.GetComponent<Rigidbody>().velocity);
    //    AudioSource.PlayClipAtPoint(shotAudio, transform.position);
    //}

    //TODO
    // 客户端开炮角度有误
    [Command]
    void CmdLanPlayerFire()
    {
        GameObject go = Instantiate(shellPrefab, firePosition.position, firePosition.rotation) as GameObject;
        go.GetComponent<Rigidbody>().velocity = go.transform.forward * shellSpeed;
        AudioSource.PlayClipAtPoint(shotAudio, transform.position);
        NetworkServer.Spawn(go);
    }
}
