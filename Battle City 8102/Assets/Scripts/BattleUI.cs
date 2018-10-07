using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class BattleUI : MonoBehaviour {

    private GComponent battleComponent;

    //移动、转向数值显示文本
    private GTextField degreeTextField;
    private GTextField permTextField;
    private GTextField turnXTextField;
    private GTextField turnYTextField;

    private Joystick joystick;
    private TurnAround turnAround;

	// Use this for initialization
	void Start () {
        battleComponent = GetComponent<UIPanel>().ui;

        degreeTextField = battleComponent.GetChild("degreeTextField").asTextField;
        permTextField = battleComponent.GetChild("permTextField").asTextField;//前进率，范围0-1
        turnXTextField = battleComponent.GetChild("turnXTextField").asTextField;
        turnYTextField = battleComponent.GetChild("turnYTextField").asTextField;

        //控制移动
        joystick = new Joystick(battleComponent);
        joystick.onMove.Add(JoystickOnMove);
        joystick.onEnd.Add(JoystickOnEnd);
        //控制转向
        turnAround = new TurnAround(battleComponent);
        turnAround.onEnd.Add(TurnAroundOnEnd);
        turnAround.onMove.Add(TurnAroundOnMove);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //数组第一个值为弧度，第二值为直角边长
    private void JoystickOnMove(EventContext eventContext)
    {
        float[] data = (float[])eventContext.data;
        degreeTextField.text = "角度：" + data[0]*180/Mathf.PI;
        permTextField.text = "前进率：" +data[1]/ Mathf.Sin(data[0]) / joystick.radius;
    }

    private void JoystickOnEnd()
    {
        degreeTextField.text = "角度：";
        permTextField.text = "前进率：";
    }

    //第一个值为横轴转角，第二个值为纵轴转角
    private void TurnAroundOnMove(EventContext eventContext)
    {
        float[] data = (float[])eventContext.data;
        turnXTextField.text = "横轴转角："+data[0];
        turnYTextField.text = "纵轴转角："+data[1];
    }

    private void TurnAroundOnEnd()
    {
        turnXTextField.text = "横轴转角：";
        turnYTextField.text = "纵轴转角：";
    }
}
