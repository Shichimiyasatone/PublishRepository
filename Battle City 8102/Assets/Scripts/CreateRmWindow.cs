using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CreateRmWindow : Window {

    private GComponent createRoomFrame;
    private GTextField roomNameTextField;
    private GComboBox mapComboBox;
    private GComboBox modeComboBox;
    private GComboBox numComboBox;
    private GButton tickButton;

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "CreateRoom_Window").asCom;
        createRoomFrame = contentPane.GetChild("frame").asCom;
        roomNameTextField = createRoomFrame.GetChild("roomNameTextField").asTextField;
        mapComboBox = createRoomFrame.GetChild("mapComboBox").asComboBox;
        modeComboBox = createRoomFrame.GetChild("modeComboBox").asComboBox; ;
        numComboBox = createRoomFrame.GetChild("numComboBox").asComboBox;
        tickButton = createRoomFrame.GetChild("tickButton").asButton;
        
        tickButton.onClick.Add(() => {
            if (roomNameTextField.text.Equals(""))
            {
                return;
            }
            Room rm = new Room(roomNameTextField.text,MainUI.player.name,mapComboBox.value,modeComboBox.value,new List<Player> {},System.Convert.ToInt32(numComboBox.value));
            rm.isAlive = true;
            rm.hostIP = System.Net.IPAddress.Loopback.ToString();
            MainUI.currentRoom = rm;
            RoomManager.CreateRoom(rm);
            // 显示房间窗口
            MainUI.roomWindow = new RoomWindow();
            MainUI.roomWindow.Show();
            this.Dispose();
        });

    }

}
