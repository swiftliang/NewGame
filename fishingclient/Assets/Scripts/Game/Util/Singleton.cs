using UnityEngine;
using System.Collections;
public class Singleton<T> where T : class, new()
{
    private static object _syncobj = new object();
    public static volatile T _Instance = null;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                lock (_syncobj)
                {
                    if (_Instance == null)
                    {
                        _Instance = new T();
                    }
                }
            }
            return _Instance;
        }
    }
    protected Singleton()
    {
        Debug.Assert(_Instance == null, "Only one instance can be created for singleton!!!");
    }
}
