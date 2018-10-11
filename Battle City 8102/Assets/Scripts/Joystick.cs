using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
                        //事件收发类
public class Joystick : EventDispatcher {
    //事件监听者
    public EventListener onMove { get; private set; }
    public EventListener onEnd { get; private set; }

    //battleUI里的对象
    private GButton joystickButton;
    private GObject thumb;
    private GObject touchArea;
    private GObject joystickCenter;

    //摇杆的属性
    private float initX;
    private float initY;
    private float startStageX;
    private float startStageY;
    private float lastStageX;
    private float lastStageY;
    private int touchID;
    public int radius { get; set; }//可拖动的范围
    private GTweener tweener;

    private GComponent battleComponent;

    public Joystick(GComponent battleComponent)
    {
        onMove = new EventListener(this, "onMove");
        onEnd = new EventListener(this, "onEnd");

        this.battleComponent = battleComponent;
        joystickButton = battleComponent.GetChild("joystickButton").asButton;
        joystickButton.changeStateOnClick = false;
        thumb = joystickButton.GetChild("thumb");
        touchArea = battleComponent.GetChild("stickTouchArea");
        joystickCenter = battleComponent.GetChild("joystickCenter");

        initX = joystickCenter.x + joystickCenter.width / 2;
        initY = joystickCenter.y + joystickCenter.height / 2;
        touchID = -1;
        radius = 150;

        touchArea.onTouchBegin.Add(OnTouchBegin);
        touchArea.onTouchMove.Add(OnTouchMove);
        touchArea.onTouchEnd.Add(OnTouchEnd);
    }
    
    //开始触摸
    private void OnTouchBegin(EventContext evenContext)
    {
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)evenContext.data;
            touchID = inputEvent.touchId;
            if (tweener != null)
            {
                tweener.Kill();//终止上一个动效
                tweener = null;
            }
            Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            float posX = localPos.x;
            float posY = localPos.y;
            joystickButton.selected = true;

            lastStageX = posX;
            lastStageY = posY;
            startStageX = posX;
            startStageY = posY;

            joystickCenter.visible = true;
            joystickCenter.SetXY(posX - joystickCenter.width / 2, posY - joystickCenter.height / 2);
            joystickButton.SetXY(posX - joystickButton.width / 2, posY - joystickButton.height / 2);

            evenContext.CaptureTouch();
        }
       
    }

    //移动触摸，joystickButton图标以joystickCenter为圆心跟随触摸且指向移动方向
    private void OnTouchMove(EventContext evenContext)
    {
        InputEvent inputEven = (InputEvent)evenContext.data;
        if (touchID != -1 && inputEven.touchId == touchID)
        {
            Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEven.x, inputEven.y));
            float posX = localPos.x;
            float posY = localPos.y;
            float moveX = posX - lastStageX;
            float moveY = posY - lastStageY;
            lastStageX = posX;
            lastStageY = posY;
            float buttonX = joystickButton.x + moveX;
            float buttonY = joystickButton.y + moveY;

            //button锚点在左上角，计算视觉中心位置需考虑宽高
            float deltaX = buttonX + joystickButton.width / 2 - startStageX;
            float deltaY = buttonY + joystickButton.height / 2 - startStageY;
            float rad = Mathf.Atan2(deltaY, deltaX);
            float degree = rad * 180 / Mathf.PI;
            thumb.rotation = degree +90;

            //设置范围
            float maxX = radius * Mathf.Cos(rad);
            float maxY = radius * Mathf.Sin(rad);
            if (Mathf.Abs(deltaX) > Mathf.Abs(maxX))
            {
                deltaX = maxX;
            }
            if (Mathf.Abs(deltaY) > Mathf.Abs(maxY))
            {
                deltaY = maxY;
            }

            buttonX = startStageX + deltaX;
            buttonY = startStageY + deltaY;

            //锚点为左上角，将视觉中点转换成坐标时减去宽高一半
            joystickButton.SetXY(buttonX - joystickButton.width / 2, buttonY - joystickButton.height / 2);

            onMove.Call(new float[]{rad,deltaY,deltaX});
        }
    }

    //结束触摸，joystickButton返回joystickCenter
    private void OnTouchEnd(EventContext evenContext)
    {
        InputEvent inputEvent = (InputEvent)evenContext.data;
        if (touchID != -1 && touchID == inputEvent.touchId)
        {
            touchID = -1;
            thumb.rotation = thumb.rotation + 180;//指向圆心
            joystickCenter.visible = false;
            tweener = joystickButton.TweenMove(new Vector2(initX - joystickButton.width / 2, initY - joystickButton.height / 2), 0.3f).OnComplete(()=> {
                tweener = null;
                joystickButton.selected = false;
                thumb.rotation = 0;
                joystickCenter.visible = true;
                joystickCenter.SetXY(initX - joystickCenter.width / 2, initY - joystickCenter.height);
            });
        }
        onEnd.Call();
    }
}
