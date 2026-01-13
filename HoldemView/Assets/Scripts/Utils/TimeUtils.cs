using System;
using UnityEngine;

public static class TimeUtils
{
    public const int SecondsOneDay = 86400;
    public const int DAYS_IN_WEEK = 7;

    private static DateTime? _networkUtcTime = null;
    private static DateTime? _networkLocalTime = null;
    private static double _lastUpdateRealtime = 0f;
    private static TimeSpan _timeOffset;

    public static void RefreshNetworkTime(DateTime networkTime)
    {
        _networkUtcTime = networkTime.ToUniversalTime();
        _networkLocalTime = networkTime;
        _lastUpdateRealtime = Time.realtimeSinceStartupAsDouble;
        _timeOffset = networkTime - DateTime.Now;

        Debug.Log($"TimeUtils刷新网络时间: UTC={_networkUtcTime}, Local={_networkLocalTime}");
    }

    /// <summary>
    /// 获取UTC时间（优先使用网络时间）
    /// </summary>
    public static DateTime UtcNow()
    {
        if (_networkUtcTime.HasValue)
        {
            // 使用缓存的网络时间 + 基于游戏运行时间的偏移
            double elapsedRealtime = Time.realtimeSinceStartupAsDouble - _lastUpdateRealtime;
            return _networkUtcTime.Value.AddSeconds(elapsedRealtime);
        }
        return DateTime.UtcNow;
    }

    /// <summary>
    /// 获取本地时间（优先使用网络时间）
    /// </summary>
    public static DateTime Now()
    {
        if (_networkLocalTime.HasValue)
        {
            // 使用缓存的网络时间 + 本地时间偏移
            double elapsedRealtime = Time.realtimeSinceStartupAsDouble - _lastUpdateRealtime;
            return _networkLocalTime.Value.AddSeconds(elapsedRealtime);
        }
        return DateTime.Now;
    }

    public static DateTime GetLocalTimeFromUtc(DateTime utcTime)
    {
        DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
        return localDateTime;
    }

    public static int TotalSeconds()
    {
        TimeSpan ts = (UtcNow() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return Convert.ToInt32(ts.TotalSeconds);
    }

    public static long TotalMilliseconds()
    {
        TimeSpan ts = (UtcNow() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

    public static int DateTimeToSeconds(DateTime date)
    {
        return (int)(date - DateTime.Parse("1970-01-01")).TotalSeconds;
    }

    public static double Countdown(DateTime dt)
    {
        TimeSpan ts = (dt - UtcNow());
        return ts.TotalSeconds;
    }

    public static double Countdown(long seconds)
    {
        TimeSpan ts = (UtcNow() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return seconds - ts.TotalSeconds;
    }

    public static DateTime ParseTimestampToDate(long seconds)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return dt.AddSeconds(seconds);
    }

    public static bool IsSameDayByLocalTime(long timestamp1, long timestamp2)
    {
        DateTime dt1 = ParseTimestampToDate(timestamp1);
        DateTime dt2 = ParseTimestampToDate(timestamp2);
        return dt1.Year == dt2.Year
            && dt1.Month == dt2.Month
            && dt1.Day == dt2.Day;
    }

    public static DateTime ParseTimeMilliSecondToDate(long milliseconds)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return dt.AddMilliseconds(milliseconds);
    }

    public static DateTime ParseTimeFromNow(long countdown)
    {
        return UtcNow().AddSeconds(countdown);
    }

    public static double PastTime(DateTime pastTime)
    {
        TimeSpan ts = (UtcNow() - pastTime);
        return ts.TotalSeconds;
    }

    //获取第二天凌晨时间戳
    public static long GetTomorrowTimestamp()
    {
        long now = TotalSeconds();
        DateTime dt1 = DateTime.Parse(Now().ToShortDateString() + " 23:59:59");
        TimeSpan ts = dt1 - Now();
        return now + Convert.ToInt64(ts.TotalSeconds);
    }

    public static bool IsNewDayAfterUtc22(long lastUtcTime)
    {
        DateTime lastTime = ParseTimestampToDate(lastUtcTime).ToLocalTime();
        DateTime nowTime = Now();

        DateTime lastUtc22 = new DateTime(lastTime.Year, lastTime.Month, lastTime.Day, 22, 0, 0);
        if (lastTime < lastUtc22)
        {
            lastUtc22 = lastUtc22.AddDays(-1);
        }

        DateTime nowUtc22 = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 0, 0);
        if (nowTime < nowUtc22)
        {
            nowUtc22 = nowUtc22.AddDays(-1);
        }

        return nowUtc22 > lastUtc22;
    }
    public static string GetUtcDateString()
    {
        return UtcNow().ToString();
    }

    public static string GetTimeString(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds >= 3600f)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        else if (timeSpan.TotalSeconds < 0)
        {
            return "0:00";
        }
        else
        {
            return string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    /// <summary>
    /// 得到倒计时字符串 带天
    /// </summary>
    public static string GetTimeCountDownStringWithDay(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays > 1f)
        {
            if (timeSpan.Hours == 0)
            {
                return $"{timeSpan.Days}d";
            }
            return $"{timeSpan.Days}d {timeSpan.Hours}h";
        }
        else
        {
            return GetTimeString(timeSpan);
        }
    }

    public static string GetTimeString(string format, int seconds)
    {
        string label = format;
        int ms = seconds * 1000;
        int s = seconds;
        int m = s / 60;
        int h = m / 60;
        int d = h / 24;

        string t = "";
        //处理天
        if (label.Contains("%dd"))
        {
            t = FillFormat(d);
            label = label.Replace("%dd", t);
            h = h % 24;
        }
        else if (label.Contains("%d"))
        {
            label = label.Replace("%d", d.ToString());
            h = h % 24;
        }

        //处理小时
        if (label.Contains("%hh"))
        {
            t = FillFormat(h);
            label = label.Replace("%hh", t);
            m = m % 60;
        }
        else if (label.Contains("%h"))
        {
            label = label.Replace("%h", h.ToString());
            m = m % 60;
        }

        //处理分
        if (label.Contains("%mm"))
        {
            t = FillFormat(m);
            label = label.Replace("%mm", t);
            s = s % 60;
        }
        else if (label.Contains("%m"))
        {
            label = label.Replace("%m", m.ToString());
            s = s % 60;
        }

        //处理秒
        if (label.Contains("%ss"))
        {
            t = FillFormat(s);
            label = label.Replace("%ss", t);
            ms = ms % 1000;
        }
        else if (label.Contains("%s"))
        {
            label = label.Replace("%s", s.ToString());
            ms = ms % 1000;
        }

        //处理毫秒
        if (label.Contains("ms"))
        {
            t = ms.ToString();
            label = label.Replace("%ms", t);
        }

        return label;
    }

    public static string SecondToTimeFormat24(int second)
    {
        if (second <= 0)
        {
            return "00:00:00";
        }

        int h = (int)(second / 3600);
        int m = (int)((second - 3600 * h) / 60);
        int s = second - h * 3600 - m * 60;

        string time = FillFormat(h) + ":" + FillFormat(m) + ":" + FillFormat(s);
        return time;
    }

    public static string SecondToTimeFormat(int second)
    {
        if (second <= 0)
        {
            return "00:00";
        }

        int h = (int)(second / 3600);
        int m = (int)((second - 3600 * h) / 60);
        int s = second - h * 3600 - m * 60;

        if (h == 0)
        {
            return FillFormat(m) + ":" + FillFormat(s);
        }
        else
        {
            return FillFormat(h) + ":" + FillFormat(m);
        }
    }

    public static string FillFormat(int value)
    {
        if (value < 10)
        {
            return "0" + value;
        }
        else
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// 检查是否正在使用网络时间
    /// </summary>
    public static bool IsUsingNetworkTime()
    {
        return _networkUtcTime.HasValue;
    }

    /// <summary>
    /// 清除网络时间缓存（强制下次使用本地时间）
    /// </summary>
    public static void ClearNetworkTime()
    {
        _networkUtcTime = null;
        _networkLocalTime = null;
        Debug.Log("TimeUtils已清除网络时间缓存");
    }
}