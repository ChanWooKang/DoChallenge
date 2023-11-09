using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[AddComponentMenu("Custom/PoolingManager")]
public class PoolingManager : MonoBehaviour
{
    #region [ SingleTon ]

    static PoolingManager _pool;
    public static PoolingManager Pool { get { return _pool; } }

    #endregion [ SingleTon ]

    #region [ Field Data ]
    public PoolUnit[] _poolingUnits;
    public List<GameObject>[] _pooledUnitList;
    public int _defPoolAmount = 5;
    public bool _canPoolExpand = true;
    #endregion [ Field Data ]

    void Awake() { _pool = this; LoadObjectPool(); }

    public void Clear() { _pool = null; }

    
    void LoadObjectPool()
    {
        _pooledUnitList = new List<GameObject>[_poolingUnits.Length];
        for (int i = 0; i < _poolingUnits.Length; i++)
        {
            _pooledUnitList[i] = new List<GameObject>();
            if (_poolingUnits[i].amount > 0)
                _poolingUnits[i].CurAmount = _poolingUnits[i].amount;
            else
                _poolingUnits[i].CurAmount = _defPoolAmount;

            int index = 0;
            for (int j = 0; j < _poolingUnits[i].CurAmount; j++)
            {
                GameObject newItem = (GameObject)Instantiate(_poolingUnits[i].prefab);
                string suffix = "_" + index;
                AddToPooledUnitList(i, newItem, suffix);
                ++index;
            }
        }
    }

    void AddToPooledUnitList(int index, GameObject newItem, string suffix, Transform parent = null)
    {
        newItem.name += suffix;
        newItem.SetActive(false);
        if (parent == null)
            newItem.transform.SetParent(transform);
        else
            newItem.transform.SetParent(parent);
        _pooledUnitList[index].Add(newItem);
    }

    GameObject GetPooledItem(string value)
    {
        for (int unitIdx = 0; unitIdx < _pooledUnitList.Length; unitIdx++)
        {
            if (_poolingUnits[unitIdx].prefab.name == value)
            {
                int listIdx = 0;
                for (; listIdx < _pooledUnitList[unitIdx].Count; listIdx++)
                {
                    if (_pooledUnitList[unitIdx][listIdx] == null)
                        return null;
                    if (_pooledUnitList[unitIdx][listIdx].activeInHierarchy == false)
                        return _pooledUnitList[unitIdx][listIdx];
                }

                if (_canPoolExpand)
                {
                    GameObject tmpObj = (GameObject)Instantiate(_poolingUnits[unitIdx].prefab);
                    string suffix = $"_{listIdx}({(listIdx - _poolingUnits[unitIdx].CurAmount + 1)})";
                    AddToPooledUnitList(unitIdx, tmpObj, suffix);
                    return tmpObj;
                }
                break;
            }
        }
        return null;
    }

    public static void DestroyAPS(GameObject go)
    {
        go.SetActive(false);
        if (go.transform.parent != _pool.transform)
            go.transform.SetParent(_pool.transform);
    }

    #region [ Instatiate ]
    public GameObject InstantiateAPS(int idx, Transform parent = null)
    {
        string pooledUnitName = _poolingUnits[idx].name;
        Transform prefabTransform = _poolingUnits[idx].prefab.transform;
        GameObject go = InstantiateAPS(pooledUnitName, Vector3.zero, prefabTransform.rotation, Vector3.one, parent);
        return go;
    }

    public GameObject InstantiateAPS(
        int idx, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null
        )
    {
        string pooledUnitName = _poolingUnits[idx].name;
        GameObject go = InstantiateAPS(pooledUnitName, pos, rot, scale, parent);
        return go;
    }

    public GameObject InstantiateAPS(string pooledUnitName, Transform parent = null)
    {
        GameObject go = GetPooledItem(pooledUnitName);
        if (parent != null)
            go.transform.SetParent(parent);
        go.SetActive(true);
        return go;
    }

    public GameObject InstantiateAPS(
        string pooledUnitName, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null
        )
    {
        GameObject go = GetPooledItem(pooledUnitName);
        if (go != null)
        {
            if (parent != null)
                go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.transform.localScale = scale;
            go.SetActive(true);
        }
        return go;
    }
    #endregion [ Instatiate ]

    #region [ UI_WorldSpace ]
    public T MakeUIWorldSpace<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        GameObject go = InstantiateAPS(name, parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Utility.GetOrAddComponent<T>(go);

    }
    #endregion [ UI_WorldSpace ]
}
