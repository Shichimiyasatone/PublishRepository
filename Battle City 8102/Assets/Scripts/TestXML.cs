using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestXML : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XMLManager xml = new XMLManager();
        Player player = xml.ReadXML()[0];
        Debug.Log(""+player.name+" "+player.rank+" "+player.money);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
