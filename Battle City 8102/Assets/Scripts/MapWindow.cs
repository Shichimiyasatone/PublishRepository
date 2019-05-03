using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using DG.Tweening;

public class MapWindow : Window {

    private GComponent mapComponent;
    
    // 背景（虚拟地图）
    private GGraph background;
    // 圈外为毒圈，边缘为蓝色
    private GImage currentCircle;
    // 下次缩圈位置，边缘为白色
    private GImage targetCircle;

    // 玩家定位
    private GImage locationImage;

    // 比例尺，icon.width/mapSize
    private float mapScale;

    private float targetX;
    private float targetY;
    private float targetR;

    private float currentX;
    private float currentY;
    private float currentR;

    protected override void OnInit()
    {
        contentPane = UIPackage.CreateObject("BattleCity8102", "Map_Window").asCom;
        mapComponent = contentPane.GetChild("frame").asCom;
        contentPane.SetPosition(800, 0, 0);

        currentCircle = mapComponent.GetChild("currentCircle").asImage;
        targetCircle = mapComponent.GetChild("targetCircle").asImage;

        background = mapComponent.GetChild("icon").asGraph;
        mapScale = background.width / CircleController.mapSize;

        CircleController.targetCircleDelegate += FollowTargetCircle;
        CircleController.currentCircleDelegate += FollowCurrentCircle;

        // 初始化毒圈位置
        currentX = CircleController.circle.transform.position.x * mapScale + background.width / 2;
        currentY = -(CircleController.circle.transform.position.z * mapScale - background.height / 2);
        currentR = CircleController.circle.transform.localScale.x;
        currentCircle.position = new Vector3(currentX, currentY);
        currentCircle.scaleX = currentR / (CircleController.mapSize * 1.5f);
        currentCircle.scaleY = currentCircle.scaleX;
        if (!CircleController.counting)
        {
            CircleController.currentCircleDelegate();
        }
        CircleController.targetCircleDelegate();

        locationImage = mapComponent.GetChild("locationImage").asImage;
        Tank.followPlyaerDelegate += FollowPlayer;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void FollowCurrentCircle()
    {
        
        // 同比例进行刷圈
        // position相对父容器的位置，已将锚点设置为中心
        // (x, y)坐标系与unity(x, 0, z)不同，fairy中左上角为(0，0)，uinity中心为(0, 0)
        // 换算fairyX = unityX + parent.width/2，
        //     fairyY = unityY + parent.height/2

        // 获取实际圈缩放比例，
        // 同步缩小方法为，使用DoTween，从当前

        currentX = CircleController.x * mapScale + background.width / 2;
        currentY = -(CircleController.z * mapScale - background.height / 2);
        currentR = CircleController.r;
        currentCircle.TweenMove(new Vector3(currentX, currentY), CircleController.circleShrinkTime);
        currentCircle.TweenScaleX(currentR / (CircleController.mapSize * 1.5f), CircleController.circleShrinkTime);
        currentCircle.TweenScaleY(currentR / (CircleController.mapSize * 1.5f), CircleController.circleShrinkTime);
        //Debug.Log("("+ targetCircle.position.x+", "+ targetCircle.position.y+")");
    }

    private void FollowTargetCircle()
    {
        targetX = CircleController.targetX * mapScale + background.width / 2;
        targetY = -(CircleController.targetZ * mapScale - background.height / 2);
        targetR = CircleController.targetR;
        targetCircle.position = new Vector3(targetX, targetY);
        targetCircle.scaleX = targetR / (CircleController.mapSize * 1.5f);
        targetCircle.scaleY = targetCircle.scaleX;
    }

    private void FollowPlayer(Transform transform)
    {
        locationImage.rotation = 
            transform.Find("Main_Turre").transform.localEulerAngles.y
            +transform.localEulerAngles.y-45;

        targetX = CircleController.targetX * mapScale + background.width / 2;
        targetY = -(CircleController.targetZ * mapScale - background.height / 2);
        targetR = CircleController.targetR;
        locationImage.position = new Vector3(transform.position.x * mapScale + background.width / 2,
                                                 -(transform.position.z * mapScale - background.height / 2));
    }
}
