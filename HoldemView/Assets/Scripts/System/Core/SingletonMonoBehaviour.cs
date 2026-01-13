using UnityEngine;

/// <summary>
/// A base class for creating singleton MonoBehaviour classes.
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    /// <summary>
    /// Gets the singleton instance of the MonoSingleton class.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = $"[Singleton] {typeof(T).Name}";

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    void OnApplicationQuit()
    {
        if (instance != null)
        {
            instance = null;
        }
    }
}
