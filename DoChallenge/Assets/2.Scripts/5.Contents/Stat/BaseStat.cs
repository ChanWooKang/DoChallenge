using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat
{
    #region [ Field Data ]
    protected int _level;
    protected float _hp;
    protected float _maxhp;
    protected float _damage;
    protected float _defense;
    protected float _moveSpeed;
    protected float _attackedDamage;        
    #endregion [ Field Data ]

    #region [ Property Data ]
    public int Level { get { return _level; } set { _level = value; } }
    public float HP { get { return _hp; } set { _hp = value; } }
    public float MaxHP { get { return _maxhp; } set { _maxhp = value; } }
    public float Damage { get { return _damage; } set { _damage = value; } }
    public float Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackedDamage { get { return _attackedDamage; } }
    #endregion [ Property Data ]

    #region [ Sub Func ]
    public virtual bool GetHit(BaseStat attacker)
    {
        float damage = Mathf.Max(1, attacker.Damage - _defense);
        _attackedDamage = damage;
        if(_hp > damage)
        {
            _hp -= damage;
            return false;
        }
        else
        {
            _hp = 0;
            return true;
        }
    }

    public virtual bool GetHit(float damage)
    {
        _attackedDamage = damage;
        if (_hp > damage)
        {
            _hp -= damage;
            return false;
        }
        else
        {
            _hp = 0;
            return true;
        }
    }
    #endregion [ Sub Func ]
}
