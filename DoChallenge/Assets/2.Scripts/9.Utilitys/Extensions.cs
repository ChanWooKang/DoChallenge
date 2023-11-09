using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Define;

public static class Extensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Utility.GetOrAddComponent<T>(go);
    }
    public static void DestroyAPS(this GameObject go)
    {
        PoolingManager.DestroyAPS(go);
    }
}
