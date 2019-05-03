using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    // 场景载入标志
    public static bool load = false;
    private int time = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (load&&time==0)
        {
            time++;
            if (MainUI.roomWindow!=null&&MainUI.roomWindow.isShowing)
            {
                MainUI.roomWindow.Dispose();
            }
            load = false;
            // 在loading场景中异步加载
            SceneManager.LoadScene("Loading");
        }
	}
}
