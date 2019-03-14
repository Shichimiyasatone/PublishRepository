using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CmdDelegate : NetworkBehaviour {

    public delegate void CmdSpawnDelegate(GameObject gameObject);
    public CmdSpawnDelegate cmdSpawnDelegate;

    public delegate void RpcSpawnDelegate(GameObject gameObject);
    public RpcSpawnDelegate rpcSpawnDelegate;

    void Awake()
    {
        cmdSpawnDelegate += CmdSpawm;
        rpcSpawnDelegate += RpcSpawm;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [Command]
    private void CmdSpawm(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }

    [ClientRpc]
    private void RpcSpawm(GameObject obj)
    {
        if (isServer)
        {
            NetworkServer.Spawn(obj);
        }
    }
}
