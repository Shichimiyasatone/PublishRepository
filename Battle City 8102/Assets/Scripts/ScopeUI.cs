using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ScopeUI : MonoBehaviour {

    public bool isScoping = false;
    public GameObject scopeCanvas;
    public float defFOV = 60;

    // 默认四倍镜
    public static float scopeMultiple = 4;

    private GComponent battleComponent;
    private GButton aimButton;
    private GImage scopeImage;
    private GGroup goodsGroup;
    private GButton mapButton;

	// Use this for initialization
	void Start () {
        //scopeCanvas.SetActive(false);

        battleComponent = GetComponent<UIPanel>().ui;
        aimButton = battleComponent.GetChild("aimButton").asButton;
        scopeImage = battleComponent.GetChild("scopeImage").asImage;
        goodsGroup = battleComponent.GetChild("footer").asGroup;
        mapButton = battleComponent.GetChild("mapButton").asButton;
        // 打开或关闭瞄准镜
        aimButton.onClick.Add(()=> {
            Scope();
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Scope()
    {
        isScoping = !isScoping;
        // 显示/隐藏贴图
        scopeImage.visible = isScoping;

        // 显示瞄准镜时，隐藏多余UI
        goodsGroup.visible = !isScoping;
        mapButton.visible = !isScoping;
        //scopeCanvas.SetActive(isScoping);
        if (isScoping)
        {
            // 根据倍数拉近视距
            Camera.main.fieldOfView = defFOV * (1.0f/scopeMultiple);
        }
        else
        {
            Camera.main.fieldOfView = defFOV ;
        }
        
    }
}
