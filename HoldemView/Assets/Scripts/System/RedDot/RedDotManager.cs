using System.Collections.Generic;

public class RedDotManager : Singleton<RedDotManager>
{
    private readonly Dictionary<string, RedDotNode> dicNodes = new Dictionary<string, RedDotNode>();


    public RedDotManager()
    {
        InitNodesRelations();
    }


    private void InitNodesRelations()
    {
        this.RegisterNode(RedDotConst.LOBBY_COIN_BAR, null);
    }

    /// <summary>
    /// 注册红点
    /// </summary>
    /// <param name="nodeName">节点名</param>
    /// <param name="parent">父节点</param>
    /// <param name="bSaveLocal">红点数据是否存储本地</param>
    /// <returns></returns>
    private RedDotNode RegisterNode(string nodeName, RedDotNode parent, bool bSaveLocal = false)
    {
        RedDotNode node = new RedDotNode(nodeName, parent, bSaveLocal);
        this.dicNodes.Add(nodeName, node);
        return node;
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="nodeName">节点名</param>
    /// <param name="parentNodeName">父节点</param>
    /// <param name="bSaveLocal">红点数据是否存储本地</param>
    /// <returns></returns>
    public RedDotNode AddNode(string nodeName, string parentNodeName, bool bSaveLocal = false)
    {
        RedDotNode node = this.GetNode(nodeName);
        if (node != null)
        {
            return null;
        }
        RedDotNode parent = this.GetNode(parentNodeName);
        node = new RedDotNode(nodeName, parent, bSaveLocal);
        this.dicNodes.Add(nodeName, node);
        return node;
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    /// <param name="nodeName">节点名</param>
    public void RemoveNode(string nodeName)
    {
        RedDotNode node = this.GetNode(nodeName);
        if (node == null)
        {
            return;
        }
        this.dicNodes.Remove(nodeName);
    }

    /// <summary>
    /// 获取红点节点
    /// </summary>
    /// <param name="name">节点名</param>
    /// <returns></returns>
    public RedDotNode GetNode(string name)
    {
        if (this.dicNodes.ContainsKey(name))
        {
            return this.dicNodes[name];
        }
        return null;
    }

    /// <summary>
    /// 获取某个节点的红点数量
    /// </summary>
    /// <param name="name">节点名</param>
    /// <returns></returns>
    public int GetRedDotCount(string name)
    {
        RedDotNode node = this.GetNode(name);
        if (node != null)
        {
            return node.dotNum;
        }
        return 0;
    }


    public void RefeshRedDot(string name)
    {
        RedDotNode node = this.GetNode(name);
        if (node != null)
        {
            node.NotifyDotNumChange();
        }
    }

    //现在树结构有了，那么我们要对树结构设置事件驱动，其实就是给这棵树绑定一个事件回调：
    //事件回调    参数（节点名字，节点数量改变的回调方法）
    public void SetRedDotNodeCallBack(string nodeName, OnRedDotChangedCallback callBack)
    {
        RedDotNode node = this.GetNode(nodeName);
        if (node != null)
        {
            node.numChangeFunc = callBack;
            callBack?.Invoke(node);
        }
    }

    //驱动层
    public void SetInvoke(string nodeName, int rpNum)
    {
        RedDotNode node = this.GetNode(nodeName);
        if (node != null)
        {
            node.SetRedDotNum(rpNum);
        }
    }

    //加载红点本地数据
    public void LoadLocalRedDotData(string nodeName)
    {
        RedDotNode node = this.GetNode(nodeName);
        if (node != null)
        {
            node.LoadLocalData();
        }
    }

}