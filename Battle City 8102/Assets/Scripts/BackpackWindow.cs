using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
/**
 * 背包窗口，初始化时需要一个表示已装备的table
 * */
public class BackpackWindow : Window {

    private GComponent component;
    // 存放配件按钮的组
    private GGroup buttonGroup;
    // 玩家自身渲染图
    private GGraph playerGraph;

    // 拖拽替身
    private GLoader dragIcon;
    private int touchID;
    // 记录初始位置，计算拖动
    private float startX;
    private float startY;
    // 使用物品的拖拽范围
    private float useRedio = 75;

    // 存放配件，种类为键，具体物体对象为值;
    // 暂时没有赋值！可能出现空指针！
    private Hashtable attachmentTable;

    public BackpackWindow()
    {
        // TODO
        // 在构建时获取一个table，为attachmentTable赋值
    }

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Backpack_Window").asCom;
        contentPane.SetPosition(660, 70, 0);
        component = contentPane.GetChild("frame").asCom;

        playerGraph = component.GetChild("playerImage").asGraph;
        RenderTexture renderTexture = Resources.Load<RenderTexture>("FGUI/PlayerRT");
        Material material = Resources.Load<Material>("FGUI/PlayerMat");
        Image img = new Image();
        img.texture = new NTexture(renderTexture);
        img.material = material;
        playerGraph.SetNativeObject(img);

        dragIcon = component.GetChild("dragIcon").asLoader;
        buttonGroup = component.GetChild("buttonGroup").asGroup;

        // 遍历获取组中按钮，注册监听
        int count = component.numChildren;
        for (int i=0;i<count;i++)
        {
            if (component.GetChildAt(i).group == buttonGroup)
            {
                GButton btn = component.GetChildAt(i).asButton;
                addDragAndDrop(btn);
            }
        }
    }

    // 注册按钮的拖拽监听，拖拽结束时丢弃物品
    private void addDragAndDrop(GButton btn)
    {
        btn.onTouchBegin.Add((EventContext evenContext) => {
            if (touchID == -1)
            {
                InputEvent inputEvent = (InputEvent)evenContext.data;
                touchID = inputEvent.touchId;
                Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
                float posX = localPos.x;
                float posY = localPos.y;
                startX = posX;
                startY = posY;
                dragIcon.url = btn.icon;
                dragIcon.SetXY(posX - dragIcon.width / 2, posY - dragIcon.height / 2);
                dragIcon.visible = true;
                evenContext.CaptureTouch();
            }
        });
        btn.onTouchMove.Add((EventContext evenContext) =>
        {
            InputEvent inputEven = (InputEvent)evenContext.data;
            if (touchID != -1 && inputEven.touchId == touchID)
            {
                Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEven.x, inputEven.y));
                float posX = localPos.x;
                float posY = localPos.y;
                dragIcon.SetXY(posX - dragIcon.width / 2, posY - dragIcon.height / 2);
            }
        });
        btn.onTouchEnd.Add((EventContext evenContext) => {
            InputEvent inputEvent = (InputEvent)evenContext.data;
            if (touchID != -1 && touchID == inputEvent.touchId)
            {
                Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
                // 计算偏移量，判断为点击还是丢弃
                float deltaX = Mathf.Abs(startX - localPos.x);
                float deltaY = Mathf.Abs(startY - localPos.y);
                float redio = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
                // 圆弧外为丢弃
                if (redio > useRedio)
                {
                    // 配件丢弃，调用委托
                    GoodsUI.undoGoodsDelegate(btn.GetChild("nameTextField").asTextField.text);
                    btn.title = "";
                    Debug.Log("丢弃");
                }
                touchID = -1;
            }
        });
    }

    // 期望UI命名与Goods.type一致，方便扩展与对应
    // 例如：Goods.type = track，UI中Button Name = trackButton
    public void getAttachment(Goods goods)
    {
        if (component.GetChild(goods.type + "Button")==null)
        {
            return;
        }
        GButton btn = component.GetChild(goods.type+"Button").asButton;
        btn.title = goods.type;
        btn.GetChild("nameTextField").asTextField.text = goods.name;
        btn.GetChild("descriptionTextField").asTextField.text = goods.description;
        btn.icon = IconUtil.checkURL(goods.icon);
    }
}
