using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/**
 * 管理物品的拾取、展示、使用
 */
public class GoodsUI : MonoBehaviour {

    // 拾取事件代理
    public delegate bool GetGoodsDelegate(Goods goods);
    public delegate void DropGoodsDelegate(Goods goods);
    // 使用、卸载物体的代理
    public delegate bool DoGoodsDelegate(string goodsName);
    public delegate bool UndoGoodsDelegate(string goodsName);

    public static GetGoodsDelegate getGoodsDelegate;
    public static DropGoodsDelegate dropGoodsDelegate;
    public static DoGoodsDelegate doGoodsDelegate;
    public static UndoGoodsDelegate undoGoodsDelegate;

    private GComponent battleComponent;

    // 展开他人盒子的按钮
    private GButton boxButton;

    private GList mulGoodsList;
    private GList sinGoodsList;

    public BoxWindow boxWindow;
    private GoodsWindow goodsWindow;
    private BackpackWindow backpackWindow;

    // 拖拽替身
    private GLoader dragIcon;
    private int touchID;

    // 保存碰撞到的盒子
    private Box box;

    // 记录初始位置，计算拖动
    float startX;
    float startY;
    // 使用物品的拖拽范围
    float useRedio = 75;

    // 需要冷却的列表
    private Hashtable coldDownTable;
    private float bulletColdDownTime;
    private float medecineColdDownTime;

    // 表示丢弃物品后，产生的空位
    public List<GButton> bulletEmptyList;
    public List<GButton> medecineEmptyList;

    // 提示动效
    Transition alertTransition;

    // Use this for initialization
    void Start () {
        battleComponent = GetComponent<UIPanel>().ui;
        boxButton = battleComponent.GetChild("boxButton").asButton;
        boxButton.visible = false;

        boxButton.onClick.Add(()=> {
            boxWindow = new BoxWindow(box);
            boxWindow.Show();
        });

        // 坦克将碰撞到的盒子传递过来，通过判断盒子是否为空表示远离或接近
        Box.boxDelegate += (Box box) => {
            boxButton.visible = box == null ? false : true;
            // 远离盒子时关闭窗口
            if (!boxButton.visible && boxWindow != null)
            {
                boxWindow.Hide();
                boxWindow.Dispose();
                boxWindow = null;
                boxButton.selected = false;
            }
            this.box = box;
        };

        Tank.goodsDelegate += (List<Goods> goodsList) => {
            // 清除旧窗口
            if (goodsWindow != null)
            {
                goodsWindow.Hide();
                goodsWindow.Dispose();
                goodsWindow = null;
            }
            // 新建新窗口
            createGoodsWindow(goodsList);
        };

        // 注册拖拽事件
        GGroup footer = battleComponent.GetChild("footer").asGroup;
        mulGoodsList = battleComponent.GetChildInGroup(footer, "bulletList").asList;
        sinGoodsList = battleComponent.GetChildInGroup(footer, "medecineList").asList;
        GObject[] mulGoodsButtons = mulGoodsList.GetChildren();
        GObject[] sinGoodsButtons = sinGoodsList.GetChildren();
        getGoodsDelegate += getGoods;

        dragIcon = battleComponent.GetChild("dragIcon").asLoader;
        touchID = -1;

        bulletEmptyList = new List<GButton>();
        medecineEmptyList = new List<GButton>();

        for (int i=0;i<mulGoodsButtons.Length;i++)
        {
            GButton btn = (GButton)mulGoodsButtons[i];
            btn.visible = false;
            addDragAndDrop(btn);

            bulletEmptyList.Add(btn);
        }
        for (int i = 0; i < sinGoodsButtons.Length; i++)
        {
            GButton btn = (GButton)sinGoodsButtons[i];
            btn.visible = false;
            addDragAndDrop(btn);

            // 包含药品、道具
            medecineEmptyList.Add(btn);
        }

        coldDownTable = new Hashtable();
        bulletColdDownTime = 0;
        medecineColdDownTime = 0;

        GButton backpackButton = battleComponent.GetChild("backpackButton").asButton;
        backpackButton.onClick.Add(()=> {
            backpackWindow = new BackpackWindow();
            backpackWindow.Show();
        });
    }
	
	void Update () {
        if (boxWindow!=null)
        {
            boxButton.selected = boxWindow.isShowing;
        }
        // 遍历，产生冷却、消耗效果
        if (coldDownTable.Count!=0)
        {
            // 冷却完毕，等待移除的key
            string keys = "";
            foreach(System.Object obj in coldDownTable.Values) {
                GButton btn = (GButton)obj;
                float coldDownTime = 0;
                // 判断是子弹冷却、药物冷却、道具冷却
                switch (btn.title)
                {
                    case "bullet":
                        bulletColdDownTime += Time.deltaTime;
                        coldDownTime = bulletColdDownTime;
                        break;
                    case "medecine":
                        medecineColdDownTime += Time.deltaTime;
                        coldDownTime = medecineColdDownTime;
                        break;
                }
                keys += coldDownTweener(coldDownTime,btn) + ",";
            }
            // 处理字符串，获取非空冷却完毕的key
            string[] keyNames = keys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string key in keyNames)
            {
                coldDownTable.Remove(key);
            }
        }
	}

    private void createGoodsWindow(List<Goods> goodsList)
    {
        // 判断是否需要显示
        if (goodsList.Count<=0)
        {
            return;
        }
        goodsWindow = new GoodsWindow(goodsList);
        goodsWindow.Show();

        // 在查看盒子时，将物品列表显示在后方
        if (boxWindow != null && boxWindow.isShowing)
        {
            boxWindow.BringToFront();
        }
    }

    private bool getGoods(Goods goods)
    {
        // 将Backpack.getAttachment()在此调用
        // 变更footer中UI名字，bulletList、medecineList
        // 效仿配件的获取，将type与UI的名字统一，减少不必要的判断
        // 投掷物一同放入medecine
        string type = goods.type == "throws" ? "medecine" : goods.type;
        if (type!= "medecine"&&type!="bullet")
        {
            // TODO
            // 调用另一个委托，将物品拾取到BackpackWindow实例化时读取的table中
            // table在add物品时需要判断是否存在该类配件，如果为覆盖需要undo
            doGoodsDelegate(goods.name);
            // 如果窗口为打开状态，更新UI
            if (backpackWindow!=null)
            {
                backpackWindow.getAttachment(goods);
            }
            return true;
        }
        Debug.Log("goods.type = "+type);
        GList goodsList = battleComponent.GetChild(type + "List").asList;
        GObject[] goodsButton = goodsList.GetChildren();
        foreach (GObject obj in goodsButton)
        {
            GButton button = (GButton)obj;
            string goodsUrl = goods.icon;
            if (!goods.icon.StartsWith("ui"))
            {
                goodsUrl = UIPackage.GetItemURL("BattleCity8102", "goods-" + goods.icon);
            }
            if (button.icon == goodsUrl)
            {
                int count = int.Parse(button.GetChild("countTextField").asTextField.text);
                count += goods.count;
                button.GetChild("countTextField").asTextField.text = count + "";
                return true;
            }
        }
        GButton btn = chooseEmptyIndex(goods);
        if (btn == null)
        {
            Debug.Log("背包满");
            // 播放提示动效
            playAlert("背包已满");
            return false;
        }
        btn.visible = true;
        btn.title = goods.type;
        // 判断icon是否为具体路径
        string url = goods.icon;
        if (!url.StartsWith("ui"))
        {
            url = UIPackage.GetItemURL("BattleCity8102", "goods-" + goods.icon);
        }
        btn.icon = url;
        btn.GetChild("descriptionTextField").asTextField.text = goods.description;
        btn.GetChild("countTextField").asTextField.text = goods.count + "";
        btn.GetChild("nameTextField").asTextField.text = goods.goodsName;
        btn.GetChild("coldDownTextField").asTextField.text = goods.coldDownTime + "";
        return true;
    }

    private GButton chooseEmptyIndex(Goods goods)
    {
        List <GButton> emptyList = goods.type == "bullet" ? bulletEmptyList : medecineEmptyList;
        if (emptyList.Count <= 0)
        {
            return null;
        }
        GButton btn = emptyList[0];
        emptyList.Remove(btn);
        // 默认放在最开始丢弃的位置
        Debug.Log(goods.type + "空位个数 = "+emptyList.Count);
        return btn;
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
                // 在一个固定圆弧内
                if (redio < useRedio)
                {
                    Debug.Log("使用");
                    useGoods(btn);
                }
                else
                {
                    Debug.Log("丢弃");
                    dropGoods(btn);
                }
                touchID = -1;
            }
        });
    }

    private void useGoods(GButton btn)
    {
        playAlert("正在使用"+btn.GetChild("nameTextField").asTextField.text);

        btn.selected = true;
        dragIcon.visible = false;
        btn.GetChild("mask").asImage.visible = true;
        // 使用物品，复选为选中，单选为消耗数量
        // 重置冷却时间
        if (btn.title=="bullet")
        {
            bulletColdDownTime = 0;
        }else if(btn.title == "medecine")
        {
            medecineColdDownTime = 0;
        }
        if (coldDownTable.Contains(btn.title))
        {
            coldDownTable.Remove(btn.title);
        }
        // 使用table覆盖同类型冷却
        coldDownTable.Add(btn.title,btn);
    }

    private void dropGoods(GButton btn)
    {
        GameObject gb = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Goods.prefab", typeof(GameObject)) as GameObject;
        Goods goods = gb.GetComponent<Goods>();
        goods.type = btn.title;
        goods.icon = btn.icon;
        goods.count = int.Parse(btn.GetChild("countTextField").asTextField.text);
        goods.goodsName = btn.GetChild("nameTextField").asTextField.text;
        goods.description = btn.GetChild("descriptionTextField").asTextField.text;
        Debug.Log(btn.GetChild("coldDownTextField").asTextField.text);
        goods.coldDownTime = float.Parse(btn.GetChild("coldDownTextField").asTextField.text);
        // 使用UUID随机命名
        gb.name = System.Guid.NewGuid().ToString();
        Transform transform = GameObject.Find("m1").transform;
        Instantiate(gb, transform.position, transform.rotation);
        //dropGoodsDelegate();

        switch (btn.title)
        {
            case "bullet":
                Debug.Log("添加子弹空槽");
                bulletEmptyList.Add(btn);
                break;
            case "medecine":
                Debug.Log("添加药品空槽");
                medecineEmptyList.Add(btn);
                break;
        }

        btn.title = "";
        btn.icon = "";
        btn.GetChild("countTextField").asTextField.text = "";
        btn.selected = false;
        btn.GetChild("mask").asImage.visible = false;
        dragIcon.visible = false;

    }

    private string coldDownTweener(float coldDownTime,GButton btn)
    {
        float totleTime = float.Parse(btn.GetChild("coldDownTextField").asTextField.text);
        // 冷却完毕，数量减1
        if (coldDownTime > totleTime)
        {
            Debug.Log("冷却完毕");
            // 子弹不在冷却后消耗，冷却为切换弹种
            // 消耗品在冷却后数量减少
            if (btn.title == "medecine")
            {
                int count = int.Parse(btn.GetChild("countTextField").asTextField.text);
                btn.GetChild("countTextField").asTextField.text = --count + "";
                // 消耗完毕，清除物品
                if (count == 0)
                {
                    List<GButton> emptyList = btn.title == "bullet" ? bulletEmptyList : medecineEmptyList;
                    emptyList.Add(btn);
                    btn.visible = false;
                }
            }
            btn.GetChild("mask").asImage.visible = false;
            // 调用委托，传入完成冷却的物品
            // 无监听注册时产生空指针，text有值
            if (doGoodsDelegate!=null)
            {
                doGoodsDelegate(btn.GetChild("nameTextField").asTextField.text);
            }
            // 无法在遍历时删除table元素，会发生快速失败
            return btn.title;
        }
        else
        {
            btn.GetChild("mask").asImage.fillAmount = (totleTime - coldDownTime) / totleTime;
            return "";
        }
    }

    public void playAlert(string alert)
    {
        GComponent alertComponent = UIPackage.CreateObject("BattleCity8102", "Alert_Component").asCom;
        // 停止正在播放的动效
        if (alertTransition!=null&&alertTransition.playing)
        {
            alertTransition.Stop();
            GRoot.inst.RemoveChild(alertComponent);
        }
        GRoot.inst.AddChild(alertComponent);
        // 显示在中心偏上的位置
        alertComponent.Center();
        Vector3 vector = alertComponent.position;
        vector.y -= vector.y / 2;
        alertComponent.position = vector;
        alertComponent.GetChild("alertTextField").asTextField.text = alert;
        alertTransition = alertComponent.GetTransition("t0");
        alertTransition.Play(() => {
            GRoot.inst.RemoveChild(alertComponent);
        });
    }
}
