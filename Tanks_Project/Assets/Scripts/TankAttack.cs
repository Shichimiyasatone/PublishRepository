using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankAttack : NetworkBehaviour {

    public GameObject shellPrefab;
    public KeyCode fireKey = KeyCode.Space;
    public float shellSpeed = 5;
    public AudioClip shotAudio;

    private Transform firePosition;

    // Use this for initialization
    void Start() {
        firePosition = transform.Find("FirePositon");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(fireKey))
        {
            //Debug.Log("Fire!");
            CmdLanPlayerFire();
        }
    }

    void SinglePlayerFire()
    {    
            GameObject go = GameObject.Instantiate(shellPrefab, firePosition.position, firePosition.rotation) as GameObject;
            go.GetComponent<Rigidbody>().velocity = go.transform.forward * shellSpeed;
            AudioSource.PlayClipAtPoint(shotAudio, transform.position); 
    }

    [Command]
    void CmdLanPlayerFire()
    {
        GameObject go = GameObject.Instantiate(shellPrefab, firePosition.position, firePosition.rotation) as GameObject;
        go.GetComponent<Rigidbody>().velocity = go.transform.forward * shellSpeed;
        AudioSource.PlayClipAtPoint(shotAudio, transform.position);
        NetworkServer.Spawn(go);
    }
}
