using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ConfigWindow : Window {

    //音量
    public int volume = 50;
    private int initVolume;

    private GButton soundButton;//禁音按钮
    private GSlider volumeSlider;//音量滑块

    private bool mute = false;//是否为禁音状态

    //公共的构造方法
    public ConfigWindow(int initVolume)
    {
        this.initVolume = initVolume;
    }

    //窗口初始化时
    protected override void OnInit()
    {
        //创建窗体
        this.contentPane = UIPackage.CreateObject("BattleCity8102","Config_Window").asCom;
        //设置弹窗位于屏幕中心
        contentPane.SetXY((MainUI.screenX-contentPane.width)/2,(MainUI.screenY -contentPane.height)/2);
        //将窗体中的frame的子赋值给属性
        soundButton = contentPane.GetChild("frame").asCom.GetChild("soundButton").asButton;
        volumeSlider = contentPane.GetChild("frame").asCom.GetChild("volumeSlider").asSlider;
        //设置初始音量
        initVolume = volume;
        volumeSlider.value = initVolume;
        //注册监听禁音按钮
        soundButton.onClick.Add(SoundButtonOnClick);
        //注册监听音量滑块
        volumeSlider.onChanged.Add(VolumeSliderOnChanged);

    }

    //禁音按钮按下
    private void SoundButtonOnClick()
    {
        //如果为禁音状态
        if (mute)
        {
            //音量恢复初始值
            volume = initVolume;
            volumeSlider.value = volume;
            mute = false;
        }
        else
        {
            //音量置零
            volume = 0;
            volumeSlider.value = volume;
            mute = true;
        }
    }

    //滑块拖动时
    private void VolumeSliderOnChanged()
    {
        volume = (int)volumeSlider.value;
    }
}
