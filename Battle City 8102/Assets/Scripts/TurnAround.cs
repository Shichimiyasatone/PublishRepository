using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class TurnAround : EventDispatcher {

    public EventListener onMove { get; private set; }
    public EventListener onEnd { get; private set; }

    private GComponent battleComponent;

    private float startStageX;
    private float startStageY;
    private int touchID ;
    private GObject turnAroundTouchArea;

    //转向灵敏度
    private float sensitivity = 0.1f;


    public TurnAround(GComponent battleComponent)
    {
        onMove = new EventListener(this, "onMove");
        onEnd = new EventListener(this, "onEnd");
        this.battleComponent = battleComponent;
        turnAroundTouchArea = battleComponent.GetChild("turnAroundTouchArea");
        turnAroundTouchArea.onTouchBegin.Add(onTouchBegin);
        turnAroundTouchArea.onTouchMove.Add(onTouchMove);
        turnAroundTouchArea.onTouchEnd.Add(onTouchEnd);
        touchID = -1;
    }

    private void onTouchBegin(EventContext eventCountext)
    {
        InputEvent inputEvent = (InputEvent)eventCountext.data;
        if (touchID == -1)
        {
            //记录touchID和初始位置
            touchID = inputEvent.touchId;
            Vector2 localPos= GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            startStageX = localPos.x;
            startStageY = localPos.y;
            //捕获触摸
            eventCountext.CaptureTouch();
        }
    }

    private void onTouchMove(EventContext eventContext)
    {
        InputEvent inputEvent = (InputEvent)eventContext.data;
        if (touchID != -1 && touchID == inputEvent.touchId)
        {
            Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            //deltaX * sensitivity为横向转角，deltaY * sensitivity为纵向转角
            float deltaX = localPos.x - startStageX;
            float deltaY = localPos.y - startStageY;
            //注意二维坐标与三维坐标XY轴对应关系
            onMove.Call(new float []{ deltaX*sensitivity,-deltaY*sensitivity});
        }
    }

    private void onTouchEnd(EventContext eventContext)
    {
        InputEvent inputEvent = (InputEvent)eventContext.data;
        if (touchID !=-1 && touchID == inputEvent.touchId)
        {
            touchID = -1;
            onEnd.Call();
        }
    }


}
