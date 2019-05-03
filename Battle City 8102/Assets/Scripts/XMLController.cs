using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XMLManager xmlManager = new XMLManager();
        List<Player> players = xmlManager.ReadXML();
        if (players==null||players.Count==0)
        {
            // 无player数据，弹出创建窗口
            NameWindow nameWindow = new NameWindow();
            nameWindow.Show();
        }
        else
        {
            // 读取数据，默认读取players[0]
            MainUI.player.name = players[0].name;
            MainUI.player.money = players[0].money;
            MainUI.player.rank = players[0].rank;
            MainUI.player.experience = players[0].experience;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
