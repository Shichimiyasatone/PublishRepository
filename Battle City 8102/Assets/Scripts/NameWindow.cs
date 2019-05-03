using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class NameWindow :  Window{

    private GTextField nameTextField;

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Name_Window").asCom;
        nameTextField = frame.GetChild("nameTextField").asTextField;
        nameTextField.onClick.Add(()=>{
            if (nameTextField == null || nameTextField.text == ""|| nameTextField.text== "请输入角色昵称...")
            {
                closeButton.visible = false;
            }
            else
            {
                closeButton.visible = true;
            }
        });
    }

    protected override void OnHide()
    {
        XMLManager xmlManager = new XMLManager();
        Player player = new Player();
        player.name = nameTextField.text;
        xmlManager.AppendPlayer(player);

        MainUI.player.name = player.name;
        MainUI.player.money = player.money;
        MainUI.player.rank = player.rank;
    }
}
