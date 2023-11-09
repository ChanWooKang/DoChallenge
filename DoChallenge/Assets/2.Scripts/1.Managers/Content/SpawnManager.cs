using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class SpawnManager : MonoBehaviour
{
    #region [ Singleton ]
    static SpawnManager _spawn;
    public static SpawnManager Inst { get { return _spawn; } }
    #endregion [ Singleton ]

    #region [ Field Data ]
    public Action<eMonster, int> OnSpawnAction;
    public List<SpawnPoint> _spawnPoints;


    PoolingManager pool;
    #endregion [ Field Data]

    #region [ Property ]
    public PoolingManager Pool
    {
        get
        {
            if (pool == null)
                pool = PoolingManager.Pool;
            return pool;
        }
    }
    #endregion [ Property ]

    void Awake()
    {
        _spawn = this;
    }
    
    eMonster GetMonsterType(GameObject go)
    {
        eMonster type = eMonster.Unknown;
        if(go.TryGetComponent<MonsterCtrl>(out MonsterCtrl mc))
        {
            type = mc.MonsterType;
        }
        return type;
    }
    
    //몬스터 생성 수정 보스 생성시 수정
    public GameObject Spawn(eMonster type)
    {
        Transform tr = null;
        for(int i = 0; i < _spawnPoints.Count;i++)
        {
            if(type == _spawnPoints[i].targetType)
            {
                tr = _spawnPoints[i].target; 
                break;
            }
        }

        if(tr != null)
        {
            GameObject go = Pool.InstantiateAPS(Utility.ConvertEnum(type), tr.position, tr.rotation, Vector3.one);

            if(type != eMonster.Boss)
            {
                if(go.TryGetComponent(out MonsterCtrl mc) == false)
                {
                    Destroy(go);
                    return null;
                }
                mc._defPos = tr.position;
            }
            else
            {

            }

            OnSpawnAction?.Invoke(type, 1);
            return go;
        }        

        return null;
    }

    //아이템 생성
    public Item Spawn(SOItem item, Transform parent)
    {
        GameObject go = SpawnObject(item, parent);
        if (go != null)
        {
            if (go.TryGetComponent(out Item _item) == false)
            {
                _item = go.AddComponent<Item>();
                _item.itemSO = item;
            }
            _item.Spawn();
            return _item;
        }
        else
            return null;
    }

    //골드 생성
    public void Spawn(SOItem item, Transform parent, int gold)
    {
        Item itemScript = Spawn(item, parent);
        if(itemScript != null)
            itemScript.Gold = gold;
    }

    GameObject SpawnObject(SOItem item, Transform parent)
    {
        Vector3 pos = parent.position;
        pos.y = 0.5f;
        Quaternion rot = Quaternion.Euler(Vector3.zero);
        GameObject go = Pool.InstantiateAPS(item.Nmae,pos,rot, Vector3.one);
        return go;
    }

    public void MonsterDespawn(GameObject go)
    {
        eMonster type = GetMonsterType(go);
        if (type == eMonster.Unknown || type == eMonster.Max_Cnt)
            return;
        OnSpawnAction?.Invoke(type, -1);
        go.DestroyAPS();
    }
}
