using FairyGUI;
using System.Collections;
using System.Collections.Generic;

/**
 * 用于按钮图片处理URL
 */
public class IconUtil{

    public static string checkURL(string iconUrl)
    {
        if (!iconUrl.StartsWith("ui"))
        {
            iconUrl = UIPackage.GetItemURL("BattleCity8102", "goods-" + iconUrl);
        }
        return iconUrl;
    }
}
