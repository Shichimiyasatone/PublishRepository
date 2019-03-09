using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 子弹脚本，控制子弹爆炸
 */
public class Shell : MonoBehaviour
{

    public GameObject shellExplosion;
    public AudioClip shellExplositionAudio;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        AudioSource.PlayClipAtPoint(shellExplositionAudio, transform.position);
        GameObject.Instantiate(shellExplosion, transform.position, transform.rotation);
        Destroy(this.gameObject);

        if (collider.tag == "Tank")
        {
            collider.SendMessage("TakeDamage");
        }
    }
}

