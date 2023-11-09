using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    #region [ Const Strings ]
    const string PathFormat = "Prefabs/{0}";
    const string CloneSuffix = "(Clone)";
    #endregion [ Const Strings ]

    
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        string realPath = string.Format(PathFormat, path);
        GameObject prefab = Load<GameObject>(string.Format(PathFormat, path));
        if (prefab == null)
            return null;
        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf(CloneSuffix);
        if (index > 0)
            go.name = go.name.Substring(0, index);
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;
        Object.Destroy(go);
    }
   
}
