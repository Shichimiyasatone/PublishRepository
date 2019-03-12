using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class MapUI : MonoBehaviour {

    GComponent battleComponent;

    GButton mapButton;

	// Use this for initialization
	void Start () {
        battleComponent = GetComponent<UIPanel>().ui;
        mapButton = battleComponent.GetChild("mapButton").asButton;

        mapButton.onClick.Add(() =>
        {
            MapWindow mapWindow = new MapWindow();
            mapWindow.Show();
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
