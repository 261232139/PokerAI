using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnRedDotChangedCallback(RedDotNode node);

public class RedDotNode
{
    //是否本地存储
    private bool isSave;

    //节点名称
    public string nodeName;

    //总的红点数量
    public int dotNum = 0;

    //父节点
    public RedDotNode parent = null;

    //叶子节点   （结点的const名称 结点数据）
    public Dictionary<string, RedDotNode> dicChilds = new Dictionary<string, RedDotNode>();

    //发生变化的回调函数
    public OnRedDotChangedCallback numChangeFunc;

    public RedDotNode(string name, RedDotNode parentNode, bool bSave = false)
    {
        this.nodeName = name;
        this.parent = parentNode;
        this.isSave = bSave;
        if (this.parent != null)
        {
            this.parent.dicChilds[this.nodeName] = this;
        }
    }

    private string GetLocalSaveKey()
    {
        // string key = $"{CrusherGameClient.User.UserID}_RedDot_{this.nodeName}";
        // return key;
        return "";
    }

    /// <summary>
    /// 设置当前节点的红点数量
    /// </summary>
    /// <param name="rdNum"></param>
    public void SetRedDotNum(int rdNum)
    {
        if (this.dicChilds.Count > 0)
        {
            Console.Error.WriteLine("Only Can Set Leaf Node!");
            return;
        }
        this.dotNum = rdNum;
        if (this.isSave)
        {
            PlayerPrefs.SetInt(this.GetLocalSaveKey(), rdNum);
        }
        this.NotifyDotNumChange();
        this.parent?.RefreshDotNum();
    }

    /// <summary>
    /// 计算当前红点数量
    /// </summary>
    public void RefreshDotNum()
    {
        int num = 0;

        foreach (var kv in this.dicChilds)
        {
            num += kv.Value.dotNum;
        }

        //红点有变化
        if (num != this.dotNum)
        {
            this.dotNum = num;
            this.NotifyDotNumChange();
        }

        this.parent?.RefreshDotNum();
    }
    /// <summary>
    /// 通知红点数量变化
    /// </summary>
    public void NotifyDotNumChange()
    {
        // invoke(参数delegate)方法:在拥有此控件的基础窗口句柄的线程上执行指定的委托。
        //如果你的后台线程需要操作UI控件，并且需要等到该操作执行完毕才能继续执行，那么你就应该使用Invoke。
        this.numChangeFunc?.Invoke(this);
    }

    //加载本地红点数据
    public void LoadLocalData()
    {
        if (!isSave)
        {
            return;
        }
        int count = PlayerPrefs.GetInt(nodeName);
        SetRedDotNum(count);
    }

    //删除子节点
    public void RemoveChildNode(string name)
    {
        if (dicChilds.ContainsKey(name))
        {
            dicChilds.Remove(name);
            RefreshDotNum();
        }
    }

    public void Destroy()
    {
        if (dicChilds.Count > 0)
        {
            Debug.LogError("Can not destroy root RedDot node, pls remove child node first");
            return;
        }
        numChangeFunc = null;
        parent?.RemoveChildNode(nodeName);
        parent = null;
        dicChilds.Clear();
    }

}
