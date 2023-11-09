using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class MonsterStat : BaseStat
{
    #region [ Field Data ]
    protected eMonster _type;
    protected float _traceRange;
    protected float _traceSpeed;
    protected float _attackRange;
    protected float _attackDelay;
    protected int _minGold;
    protected int _maxGold;
    protected float _exp;
    #endregion [ Field Data ]

    #region [ Property ]
    public eMonster Type { get { return _type; } }
    public float TraceRange { get { return _traceRange; } }
    public float TraceSpeed { get { return _traceSpeed; } } 
    public float AttackRange { get { return _attackRange; } }
    public float AttackDelay { get { return _attackDelay; } }
    public int Gold { get { return Random.Range(_minGold,_maxGold); } }
    public float EXP { get { return _exp; } }
    #endregion [ Property ]

    void Init()
    {
        _type = eMonster.Unknown;
        _level = 0;
        _hp = 0;
        _maxhp = 0;
        _damage = 0;
        _defense = 0;
        _moveSpeed = 0;
        _traceRange = 0;
        _traceSpeed = 0;
        _attackRange = 0;
        _attackDelay = 0;
        _minGold = 0;
        _maxGold = 0;
        _exp = 0;
    }

    public void SetStat(eMonster type)
    {
        if (Managers.Data.Dict_Monster.TryGetValue(type, out DataByMonster data))
        {
            _type = data.index;
            _level = 0;
            _hp = data.hp;
            _maxhp = data.hp;
            _damage = data.damage;
            _defense = data.defense;
            _moveSpeed = data.movespeed;
            _traceRange = data.tracerange;
            _traceSpeed = data.tracespeed;
            _attackRange = data.attackrange;
            _attackDelay = data.attackdelay;
            _minGold = data.mingold;
            _maxGold = data.maxgold;
            _exp = data.exp;
        }
        else
            Init();
    }

    public void DeadFunc(PlayerStat stat)
    {
        if (stat != null)
            stat.EXP += _exp;
    }
}
