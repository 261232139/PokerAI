using System;
using System.Collections.Generic;

public class EventManager
{
    // 所有事件的委托缓存
    private static EventManager _instance;
    public static EventManager Instance => _instance ?? (_instance = new EventManager());

    private Dictionary<EventName, Delegate> eventTable = new Dictionary<EventName, Delegate>();

    /// <summary>
    /// 添加监听
    /// </summary>
    public void AddListener(EventName eventName, Action handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action)existing + handler;
        }
        else
        {
            eventTable[eventName] = handler;
        }
    }

    /// <summary>
    /// 添加带参数监听
    /// </summary>
    public void AddListener<T>(EventName eventName, Action<T> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T>)existing + handler;
        }
        else
        {
            eventTable[eventName] = handler;
        }
    }

    /// <summary>
    /// 添加带两个参数监听
    /// </summary>
    public void AddListener<T1, T2>(EventName eventName, Action<T1, T2> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T1, T2>)existing + handler;
        }
        else
        {
            eventTable[eventName] = handler;
        }
    }
    public void AddListener<T1, T2, T3>(EventName eventName, Action<T1, T2, T3> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T1, T2, T3>)existing + handler;
        }
        else
        {
            eventTable[eventName] = handler;
        }
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    public void RemoveListener(EventName eventName, Action handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action)existing - handler;
        }
    }

    /// <summary>
    /// 移除带参数监听
    /// </summary>
    public void RemoveListener<T>(EventName eventName, Action<T> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T>)existing - handler;
        }
    }
    /// <summary>
    /// 移除带两个参数监听
    /// </summary>
    public void RemoveListener<T1, T2>(EventName eventName, Action<T1, T2> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T1, T2>)existing - handler;
        }
    }
    public void RemoveListener<T1, T2, T3>(EventName eventName, Action<T1, T2, T3> handler)
    {
        if (eventTable.TryGetValue(eventName, out var existing))
        {
            eventTable[eventName] = (Action<T1, T2, T3>)existing - handler;
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void TriggerEvent(EventName eventName)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var callback = del as Action;
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 触发带参数事件
    /// </summary>
    public void TriggerEvent<T>(EventName eventName, T arg)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var callback = del as Action<T>;
            callback?.Invoke(arg);
        }
    }

    /// <summary>
    /// 触发带两个参数事件
    /// </summary>
    public void TriggerEvent<T1, T2>(EventName eventName, T1 arg1, T2 arg2)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var callback = del as Action<T1, T2>;
            callback?.Invoke(arg1, arg2);
        }
    }
    public void TriggerEvent<T1, T2, T3>(EventName eventName, T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var callback = del as Action<T1, T2, T3>;
            callback?.Invoke(arg1, arg2, arg3);
        }
    }

    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void Clear()
    {
        eventTable.Clear();
    }
}


public interface IEventSubscription : IDisposable { }

public sealed class EventSubscription<TDelegate> : IEventSubscription where TDelegate : Delegate
{
    private readonly EventName eventName;
    private readonly TDelegate handler;
    private readonly Action<EventName, TDelegate> remover;
    private bool disposed;

    public EventSubscription(EventName eventName, TDelegate handler, Action<EventName, TDelegate> remover)
    {
        this.eventName = eventName;
        this.handler = handler;
        this.remover = remover;
    }

    public void Dispose()
    {
        if (disposed) return;
        remover.Invoke(eventName, handler);
        disposed = true;
    }
}