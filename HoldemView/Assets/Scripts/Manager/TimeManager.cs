using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    // 常用的NTP服务器
    private static readonly string[] ntpServers = {
        "time.windows.com",
        "time.apple.com",
        "pool.ntp.org",
        "time.google.com",
        "ntp.aliyun.com"
    };

    private DateTime? networkTime = null;
    private DateTime lastLocalTime;
    private TimeSpan timeOffset;
    private bool isUpdating = false;
    private NetworkReachability networkStatus = NetworkReachability.NotReachable;


    private async void Update()
    {
        NetworkReachability currentNetStatus = Application.internetReachability;
        if (networkStatus != currentNetStatus)
        {
            networkStatus = currentNetStatus;
            if (networkStatus == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                ForceUpdateNetworkTime();
            }
        }
    }

    /// <summary>
    /// 获取网络时间戳（Unix时间戳，秒）
    /// </summary>
    public async Task<long> GetNetworkTimestampAsync(Action<bool> completeAction = null)
    {
        try
        {
            DateTime networkTime = await GetNetworkTimeAsync(completeAction);
            return ConvertToUnixTimestamp(networkTime);
        }
        catch (Exception ex)
        {
            GameDebugLog.LogWarning($"获取网络时间失败: {ex.Message}");
            completeAction?.Invoke(false);
            completeAction = null;
            return GetLocalTimestamp();
        }
    }

    /// <summary>
    /// 获取网络时间戳（毫秒）
    /// </summary>
    public async Task<long> GetNetworkTimestampMillisecondsAsync()
    {
        try
        {
            DateTime networkTime = await GetNetworkTimeAsync();
            return ConvertToUnixTimestampMilliseconds(networkTime);
        }
        catch (Exception ex)
        {
            GameDebugLog.LogWarning($"获取网络时间失败: {ex.Message}");
            return GetLocalTimestampMilliseconds();
        }
    }

    /// <summary>
    /// 获取网络DateTime
    /// </summary>
    public async Task<DateTime> GetNetworkTimeAsync(Action<bool> completeAction = null)
    {
        if (networkTime.HasValue)
        {
            // 使用缓存的时间 + 本地时间偏移
            TimeSpan elapsed = DateTime.Now - lastLocalTime;
            completeAction?.Invoke(true);
            completeAction = null;
            return networkTime.Value + elapsed;
        }

        // 如果已经在更新中，等待当前更新完成
        if (isUpdating)
        {
            // 等待一段时间，避免重复请求
            await Task.Delay(100);
            if (networkTime.HasValue)
            {
                TimeSpan elapsed = DateTime.Now - lastLocalTime;
                completeAction?.Invoke(true);
                completeAction = null;
                return networkTime.Value + elapsed;
            }
        }

        try
        {
            isUpdating = true;
            DateTime time = await GetNetworkTimeFromAnyServerAsync();
            networkTime = time;
            lastLocalTime = DateTime.Now;
            timeOffset = time - DateTime.Now;
            TimeUtils.RefreshNetworkTime(time);
            completeAction?.Invoke(true);
            completeAction = null;
            return time;
        }
        finally
        {
            isUpdating = false;
            completeAction?.Invoke(false);
            completeAction = null;
        }
    }

    /// <summary>
    /// 并行从多个NTP服务器获取时间，使用最先响应的结果
    /// </summary>
    private async Task<DateTime> GetNetworkTimeFromAnyServerAsync()
    {
        var tasks = new List<Task<DateTime>>();

        // 为每个服务器创建任务
        foreach (string server in ntpServers)
        {
            tasks.Add(TryGetNtpTimeAsync(server));
        }

        // 等待任意一个任务完成
        var completedTask = await Task.WhenAny(tasks);

        try
        {
            DateTime result = await completedTask;
            GameDebugLog.Log($"使用最先响应的NTP服务器结果");

            // 取消其他仍在进行的请求
            foreach (var task in tasks)
            {
                if (!task.IsCompleted)
                {
                    // 这里无法真正取消socket操作，但可以标记为不需要结果
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            GameDebugLog.LogWarning($"最先响应的服务器失败: {ex.Message}");

            // 如果最先完成的也失败了，尝试等待其他任务
            var successfulTasks = tasks.Where(t => t.IsCompletedSuccessfully).ToArray();
            if (successfulTasks.Length > 0)
            {
                return await successfulTasks[0];
            }

            // 如果所有任务都失败了，等待所有任务完成，看看是否有成功的
            try
            {
                await Task.WhenAll(tasks);
                var successfulTask = tasks.FirstOrDefault(t => t.IsCompletedSuccessfully);
                if (successfulTask != null)
                {
                    return await successfulTask;
                }
            }
            catch
            {
                // 忽略异常，继续抛出原始错误
            }

            throw new Exception("所有NTP服务器都连接失败");
        }
    }

    /// <summary>
    /// 尝试从单个NTP服务器获取时间，包含异常处理
    /// </summary>
    private async Task<DateTime> TryGetNtpTimeAsync(string server)
    {
        try
        {
            DateTime time = await GetNtpTimeAsync(server);
            GameDebugLog.Log($"成功从 {server} 获取网络时间");
            return time;
        }
        catch (Exception ex)
        {
            GameDebugLog.LogWarning($"从 {server} 获取时间失败: {ex.Message}");
            throw; // 重新抛出异常，让调用方处理
        }
    }

    private async Task<DateTime> GetNtpTimeAsync(string server)
    {
        return await Task.Run(() =>
        {
            // NTP消息大小（16字节的RFC-2030）
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // LI = 0, VN = 3, Mode = 3

            // 解析服务器地址
            IPAddress[] addresses = Dns.GetHostEntry(server).AddressList;
            if (addresses.Length == 0)
                throw new Exception("无法解析NTP服务器地址");

            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 3000;
                socket.SendTimeout = 3000;

                // 发送NTP请求
                socket.Send(ntpData);

                // 接收NTP响应
                socket.Receive(ntpData);

                // 关闭socket
                socket.Close();
            }

            // 解析NTP时间
            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        });
    }

    /// <summary>
    /// 强制更新网络时间（可用于手动刷新）
    /// </summary>
    public async void ForceUpdateNetworkTime()
    {
        networkTime = null;
        await GetNetworkTimeAsync();
    }

    // 转换方法
    private long ConvertToUnixTimestamp(DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    private long ConvertToUnixTimestampMilliseconds(DateTime dateTime)
    {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    private long GetLocalTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    private long GetLocalTimestampMilliseconds()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }
}