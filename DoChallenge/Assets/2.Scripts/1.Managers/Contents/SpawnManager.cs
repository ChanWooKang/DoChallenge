using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class SpawnManager : MonoBehaviour
{
    #region [ Singleton ]
    static SpawnManager _uniqueInstance;
    public static SpawnManager Instance { get { return _uniqueInstance; } }
    #endregion [ Singleton ]

    public Action<eMonster, int> OnSpawnEvent;
    public List<SpawnPoint> points;

    [SerializeField] BossField bf;
    PoolingManager pool;

    void Awake() { _uniqueInstance = this; }
    
    eMonster GetMonsterType(GameObject go)
    {
        eMonster type = eMonster.Unknown;
        if (go.TryGetComponent(out MonsterCtrl mc))
            type = mc.MonsterType;
        return type;
    }

    Transform ChooseSpawnPoint(eMonster type)
    {
        Transform tr = null;
        for (int i = 0; i < points.Count; i++)
        {
            if(type == points[i].targetType)
            {
                tr = points[i].target;
                break;
            }
        }
        return tr;
    }

    GameObject SpawnObject(SOItem item, Transform parent)
    {
        Vector3 pos = parent.position;
        pos.y = 0.5f;
        Quaternion rot = Quaternion.Euler(Vector3.zero);
        GameObject go = pool.InstantiateAPS("", pos, rot, Vector3.one);
        return go;
    }

    public GameObject Spawn(eMonster type)
    {
        if (pool == null)
            pool = PoolingManager.Pool;

        Transform tr = ChooseSpawnPoint(type);
        GameObject go = pool.InstantiateAPS(Utility.ConvertEnum(type), tr.position, tr.rotation, Vector3.one);
        if (go.TryGetComponent(out IMonsterSpawn mSpawn))
            mSpawn.Spawn(tr.position,bf);
        else
        {
            Destroy(go);
            return null;
        }

        OnSpawnEvent?.Invoke(type, 1);
        return go;        
    }

    public void Spawn(SOItem item , Transform parent, int gold = 0)
    {
        GameObject go = SpawnObject(item, parent);
        if(go != null)
        {
            if (go.TryGetComponent(out IItemSpawn iSpawn))
                iSpawn.Spawn(item, gold);
        }
    }

    public void MonsterDespawn(GameObject go)
    {
        eMonster type = GetMonsterType(go);
        if (type == eMonster.Unknown || type == eMonster.Max_Cnt)
            return;
        OnSpawnEvent?.Invoke(type, -1);
        go.DestroyAPS();
    }
}
