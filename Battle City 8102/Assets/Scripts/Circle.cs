using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 游戏开始时坦克在圈内，将触发一次
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            collider.SendMessage("OnChangeBleeding");
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Tank")
        {
            collider.SendMessage("OnChangeBleeding");
        }
    }
}
