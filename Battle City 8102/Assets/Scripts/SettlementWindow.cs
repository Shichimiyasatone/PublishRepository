using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SettlementWindow :  Window{

    GTextField playerNameTextField;

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Settlement_Window").asCom;
        GGroup left = frame.GetChild("left").asGroup;
        playerNameTextField = frame.GetChildInGroup(left, "playerNameTextField").asTextField;
        XMLManager xmlManager = new XMLManager();
        playerNameTextField.text = xmlManager.ReadXML()[0].name;

        closeButton.onClick.Add(() => {
            LoadNewScene();
        });
    }

    // 异步加载新场景
    private void LoadNewScene()
    {
        // 保存需要加载的目标场景
        Globe.nextSceneName = "Main";
        SceneController.load = true;
    }
}
