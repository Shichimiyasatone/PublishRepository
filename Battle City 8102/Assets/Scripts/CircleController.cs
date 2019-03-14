using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleController : MonoBehaviour {

    public delegate void TargetCircleDelegate();
    public delegate void CurrentCircleDelegate();

    public static TargetCircleDelegate targetCircleDelegate;
    public static CurrentCircleDelegate currentCircleDelegate;

    public GameObject circlePrefab;
    public static GameObject circle;


    public static float mapSize = 100;

    // 缩圈目标位置(x, 0, z)，目标半径r
    public static float r;
    public static float x;
    public static float z;

    public static float targetX;
    public static float targetZ;
    public static float targetR;

    // 有显示倒计时需求再添加显示文本
    //public Text timeText;

    // 缩圈等待时间
    public float[] timeLimis;
    // 剩余时间
    public float timeLeft;
    // 刷圈时间
    public static float circleShrinkTime = 3;

    // 刷圈状态
    [SerializeField]
    public static bool counting = true;
    // 圈数
    public static int stage = 0;
    // Use this for initialization
    // 将先于连接执行
    void Start()
    {

    }
	
    // TODO
    // 交由服务器生成，理想状态为玩家同步服务器中的circle
    public GameObject startCircle()
    {
        stage = 0;

        // 开始时因为加载原因，看不到第一个提示，调整缩圈时机
        TimeSpan t = TimeSpan.FromSeconds(timeLimis[stage]);
        // 时间分钟限定在个位数（D1）
        GetComponent<GoodsUI>().playAlert(String.Format("{0:D1}分:{1:D2}秒", t.Minutes, t.Seconds) + "后缩圈！");

        timeLimis[stage] += Time.time;
        timeLeft = timeLimis[stage];

        circle = Instantiate(circlePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0));
        circle.transform.localScale = new Vector3(mapSize * 1.5f, 20, mapSize * 1.5f);


        // 缩圈以1/2进行，1.5 / 2 = 0.75
        // 这里的r其实为直径
        r = mapSize * 0.75f;
        x = UnityEngine.Random.Range(-r, r) / 2;
        z = UnityEngine.Random.Range(-r, r) / 2;

        targetX = x;
        targetZ = z;
        targetR = r;

        return circle;
    }

	// Update is called once per frame
	void Update () {
        if (circle==null||!Tank.IsServer)
        {
            return;
        }

        // 等待计时
        if (counting)
        {
            timeLeft = timeLimis[stage] - Time.time;

            //TimeSpan t = TimeSpan.FromSeconds(timeLeft);

            //timeText.text = String.Format("{0:D1}m:{1:D2}s",t.Minutes,t.Seconds);

            if (timeLeft < 0)
            {
                timeLeft = 0;
                counting = false;
                stage++;
                // 刷圈
                CircleMove();
            }
        }      
    }

    // 第一个圈r1 = mapSize*1.5
    // 第二个圈r2 = r1*0.5
    // 使用DoTween渐变大小
    public void CircleMove()
    {
        if (currentCircleDelegate != null)
        {
            currentCircleDelegate();
        }
        // 修改x, z位置
        circle.transform.DOMove(new Vector3(x, 0, z), circleShrinkTime);
        // 修改x，z大小
        circle.transform.DOScaleX(r, circleShrinkTime);
        circle.transform.DOScaleZ(r, circleShrinkTime);

        r = r * 0.5f;
        x = UnityEngine.Random.Range(x - r /2, x + r/2);
        z = UnityEngine.Random.Range(z - r/2, z + r/2);
        
        targetX = x;
        targetZ = z;
        targetR = r;

        if (stage < timeLimis.Length)
        {
            StartCoroutine(ShrinkTime());
        }
    }

    // 启用协程等待缩圈，缩圈结束后开始下一次计时
    IEnumerator ShrinkTime()
    {
        yield return new WaitForSeconds(circleShrinkTime);
        StartNextCircle();
    }

    public void StartNextCircle()
    {
        TimeSpan t = TimeSpan.FromSeconds(timeLimis[stage]);
        // 时间分钟限定在个位数（D1）
        GetComponent<GoodsUI>().playAlert(String.Format("{0:D1}分:{1:D2}秒", t.Minutes, t.Seconds) + "后缩圈！");

        if (targetCircleDelegate != null)
        {
            targetCircleDelegate();
        }

        counting = true;
        timeLimis[stage] += Time.time;
        timeLeft = timeLimis[stage];

    }
}
