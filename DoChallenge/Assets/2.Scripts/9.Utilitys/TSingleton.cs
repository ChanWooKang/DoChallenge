using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _uniqueInstance;
    static object _lock = new object();

    public static T _inst
    {
        get
        {
            lock (_lock)
            {
                if (_uniqueInstance == null)
                {
                    _uniqueInstance = (T)FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                        return _uniqueInstance;
                    if (_uniqueInstance == null)
                    {
                        GameObject go = new GameObject();
                        _uniqueInstance = go.AddComponent<T>();
                        go.name = typeof(T).ToString();
                        go.hideFlags = HideFlags.HideAndDontSave;
                    }
                }
            }
            return _uniqueInstance;
        }
    }
}
