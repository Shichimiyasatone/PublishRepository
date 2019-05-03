using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

/*
 * xml保存在streamingAssetsPath文件夹内
 * xml结构：
 * <?xml version="1.0" encoding="gb2312"?>
 * <Players>
 *      <Player name="王家龙">
 *          <rank>777</rank>
 *          <money>666</money>
 *          <modifyTime>2019/3/18 13:49</modifyTime>
 *      </Plyaer>
 * </Players>
 */
public class XMLManager{

    XmlDocument doc;
    XmlDeclaration decl;
    // element是特殊的node，可以在标签内写属性键值对
    // element是node的具现类（实现类），node为虚类（抽象类）
    XmlNode node;
    XmlElement elem;

    private string docName = "Player.xml";
    public bool isEmpty=true;

    private string path;

    public XMLManager()
    {
        //InitDocument();

        // C:/Users/Administrator/AppData/LocalLow/ALLGayCompany/Battle City 9102/Player.xml
        path = Application.persistentDataPath +"/"+ docName;
        SetFileToPersistent();
        LoadXml();
    }

    private void LoadXml()
    {
        doc = new XmlDocument();
        doc.Load(path);
    }

    private void SetFileToPersistent()
    {
        //StreamWriter一定要记得关闭，保存修改
        FileInfo info = new FileInfo(path);
        if (!info.Exists)
        {
            // 加载Assets/Resources/XML中的空文件
            TextAsset ts = Resources.Load("XML/Player") as TextAsset;
            string content = ts.text;
            StreamWriter sw = info.CreateText();
            // 复制内容至Persistent，之后的读写操作针对path路径下的xml文件
            // 与Resources中的xml无关
            sw.Write(content);
            sw.Close();
            sw.Dispose();
            Debug.Log("添加Xml文件成功");
        }
        else
        {
            Debug.Log("已存在Xml文件");
        }
    }

    public void InitDocument()
    {
        doc = new XmlDocument();
        // 安卓无法以此方式进行修改
        if (!File.Exists(Application.streamingAssetsPath + "/" + this.docName))
        {
            // 加入XML的声明段落
            decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            elem = doc.CreateElement("", "Players", "");
            doc.AppendChild(elem);
        }
        else
        {
            // 存在，读取文件
            doc.Load(Application.streamingAssetsPath + "/" + this.docName);
            isEmpty = false;
        }
    }
    public List<Player> ReadXML() {
        XmlNodeList nodeList = doc.SelectSingleNode("Players").ChildNodes;
        List<Player> playerList = new List<Player>();
        foreach(XmlNode node in nodeList)
        {
            // 由于存的时候是子类，此处可以强转成子类
            XmlElement elem = (XmlElement)node;
            Player player = new Player();
            player.name = elem.GetAttribute("name");
            foreach (XmlNode subNode in node.ChildNodes)
            {
                // 目前只考虑金币和等级的读取
                switch (subNode.Name)
                {
                    case "rank":
                        player.rank = int.Parse(subNode.InnerText);
                        break;
                    case "money":
                        player.money = int.Parse(subNode.InnerText);
                        break;
                    case "experience":
                        player.experience= int.Parse(subNode.InnerText);
                        break;
                }
            }
            playerList.Add(player);
        }
        return playerList;
    }

    public void AppendPlayer(Player player)
    {
        // 获取根结点
        XmlNode root = doc.SelectSingleNode("Players");
        // 创建Player二级结点
        XmlElement xe = doc.CreateElement("Plyaer");
        // 设置属性键值对
        xe.SetAttribute("name",player.name);

        // 创建代表等级的rank三级节点
        XmlElement xesub = doc.CreateElement("rank");
        // 设置内文本
        xesub.InnerText = player.rank.ToString();
        // 追加至二级节点
        xe.AppendChild(xesub);

        // 创建代表经验的rank三级节点
        xesub = doc.CreateElement("experience");
        xesub.InnerText = player.experience.ToString();
        xe.AppendChild(xesub);

        // 创建代表金钱的money三级节点
        xesub = doc.CreateElement("money");
        xesub.InnerText = player.money.ToString();
        xe.AppendChild(xesub);

        // 创建代表修改时间的modifyTime三级节点
        xesub = doc.CreateElement("modifyTime");
        xesub.InnerText = DateTime.Now.ToLocalTime().ToString();
        xe.AppendChild(xesub);

        // 追加至一级节点
        root.AppendChild(xe);
        // 保存xml
        //doc.Save(Application.streamingAssetsPath + "/" + this.docName);
        doc.Save(path);
    }
}
