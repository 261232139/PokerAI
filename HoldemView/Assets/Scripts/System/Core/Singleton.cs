public class Singleton<T> where T : new()
{
    /// <summary>
    /// 实例
    /// </summary>
    private static T instance;

    /// <summary>
    /// 获取单例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null) instance = new T();
            return instance;
        }
    }
}