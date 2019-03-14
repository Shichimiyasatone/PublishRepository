using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tank : NetworkBehaviour {

    // 定义物品列表委托
    public delegate void GoodsDelegate(List<Goods> goodsList);
    public delegate void FollowPlyaerDelegate(Transform transform);

    public static FollowPlyaerDelegate followPlyaerDelegate;

    public GoodsDelegate goodsDelegate;

    // 用于统计附近道具
    public List<Goods> goodsList;

    [SyncVar]
    private Vector3 circleScale;
    [SyncVar]
    private Vector3 target;
    [SyncVar]
    private bool isCounting;
    [SyncVar]
    private Vector3 xzr;

    private GameObject circle;

    public static bool IsServer;

    // Use this for initialization
    void Start () {

        goodsList = new List<Goods>();

        if (isLocalPlayer)
        {
            GameObject.Find("CameraAxis").GetComponent<CameraFollow>().tank = gameObject;

            // 在战斗场景UI加载完时注册相机旋转委托
            GameObject.Find("BattleUIPanel").GetComponent<BattleUI>().turnDelegate +=
                GameObject.Find("CameraAxis").GetComponent<CameraFollow>().Rotate;

            goodsDelegate += GameObject.Find("BattleUIPanel").GetComponent<GoodsUI>().initGoodsListWindow;
        }
        IsServer = isServer;
    }
    [Command]
    public void CmdTest(GameObject circle)
    {
        Debug.Log(circle.transform.localScale);
        NetworkServer.Spawn(circle);
    }

    private bool moveCircle = true;
    private bool flag = false;
    private float timmer = 0;
    // Update is called once per frame
    void Update () {
        if (timmer < 10)
        {
        timmer += Time.deltaTime;
            
        }
        
            if (timmer > 5)
        {
            
            if (isServer&&CircleController.circle ==null)
            {
                circle = GameObject.Find("BattleUIPanel").GetComponent<CircleController>().startCircle();
                CmdTest(circle);
            }
            // 发送同步消息
            if (isServer && CircleController.circle != null)
            {
                circleScale = CircleController.circle.transform.localScale;
                target = new Vector3(CircleController.targetX, CircleController.targetZ, CircleController.targetR);
                isCounting = CircleController.counting;
                xzr = new Vector3(CircleController.x, CircleController.z, CircleController.r);
            }
        }

        if (!isServer)
        {
            if (GameObject.Find("Circle(Clone)") !=null&&!flag)
            {
                flag = true;

                circle = GameObject.Find("Circle(Clone)");
                CircleController.circle = circle;
                //Debug.Log("Client:" + GameObject.Find("Circle(Clone)").transform.localScale);
            }
            // 处理同步消息
            if (circle!=null)
            {
                //GameObject.Find("Circle(Clone)").transform.localScale = circleScale;
                circle.transform.localScale = circleScale;
                CircleController.x = xzr.x;
                CircleController.z = xzr.y;
                CircleController.r = xzr.z;

                CircleController.targetR = target.z;
                CircleController.targetX = target.x;
                CircleController.targetZ = target.y;

                CircleController.counting = isCounting;
                if (CircleController.targetCircleDelegate!=null)
                {
                    CircleController.targetCircleDelegate();
                }
                // TODO
                // 缩圈无法同步至地图
                //if (!isCounting && moveCircle)
                //{
                //    if (CircleController.currentCircleDelegate != null)
                //    {
                //        CircleController.currentCircleDelegate();
                //    }
                //}
                //moveCircle = isCounting;
            }
        }

        if (isLocalPlayer)
        {
            if (followPlyaerDelegate!=null)
            {
                followPlyaerDelegate(transform);
            }
        }
	}

    void addGoods(Goods goods)
    {
        if (goods == null)
        {
            return;
        }
        if (!goodsList.Contains(goods))
        {
            goodsList.Add(goods);
        }
        // 调用委托，显示物品列表
        goodsDelegate(goodsList);
    }

    void removeGoods(Goods goods)
    {
        // 防止空指针
        if (goods == null)
        {
            return;
        }
        if (goodsList.Contains(goods))
        {
            goodsList.Remove(goods);
        }
        // 更新物品列表
        goodsDelegate(goodsList);
    }

}
