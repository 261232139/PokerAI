using System;
using System.Collections.Generic;
using UnityEngine;

//常用的协程缓存起来，避免频繁创建产生大量GC
public class CoroutineManager : Singleton<CoroutineManager>
{
    public static readonly WaitForFixedUpdate WAIT_FOR_FIXED_UPDATE = new WaitForFixedUpdate();
    public static readonly WaitForEndOfFrame WAIT_FOR_END_OF_FRAME = new WaitForEndOfFrame();
    public static readonly WaitForSeconds WAIT_SECONDS_1 = new WaitForSeconds(1f);

    private Dictionary<float, WaitForSeconds> dicWaitForSeconds = new Dictionary<float, WaitForSeconds>();
    private Dictionary<float, WaitForSecondsRealtime> dicWaitForSecondsRealtime = new Dictionary<float, WaitForSecondsRealtime>();
    private Dictionary<Func<bool>, WaitUntil> dicWaitUntil = new Dictionary<Func<bool>, WaitUntil>();
    private Dictionary<Func<bool>, WaitWhile> dicWaitWhile = new Dictionary<Func<bool>, WaitWhile>();


    public WaitForSeconds AcquireWaitForSeconds(float seconds)
    {
        if (!dicWaitForSeconds.TryGetValue(seconds, out WaitForSeconds v))
        {
            v = new WaitForSeconds(seconds);
            dicWaitForSeconds.Add(seconds, v);
        }
        return v;
    }

    public WaitForSecondsRealtime AcquireWaitForSecondsRealtime(float seconds)
    {
        if (!dicWaitForSecondsRealtime.TryGetValue(seconds, out WaitForSecondsRealtime v))
        {
            v = new WaitForSecondsRealtime(seconds);
            dicWaitForSecondsRealtime.Add(seconds, v);
        }
        return v;
    }

    public WaitUntil AcquireWaitUntil(Func<bool> func)
    {
        if (!dicWaitUntil.TryGetValue(func, out WaitUntil v))
        {
            v = new WaitUntil(func);
            dicWaitUntil.Add(func, v);
        }
        return v;
    }

    public WaitWhile AcquireWaitWhile(Func<bool> func)
    {
        if (!dicWaitWhile.TryGetValue(func, out WaitWhile v))
        {
            v = new WaitWhile(func);
            dicWaitWhile.Add(func, v);
        }
        return v;
    }


    public void Clear()
    {
        dicWaitForSeconds.Clear();
        dicWaitForSecondsRealtime.Clear();
        dicWaitUntil.Clear();
        dicWaitWhile.Clear();
    }

}
