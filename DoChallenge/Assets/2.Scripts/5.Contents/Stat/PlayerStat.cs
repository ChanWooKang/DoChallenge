using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class PlayerStat : BaseStat
{
    #region [ Field Data ]
    protected float _mp;
    protected float _maxmp;
    protected float _exp;
    protected int _gold;
    protected float _plushp;
    protected float _plusmp;
    protected float _plusdamage;
    protected float _plusdefense;
    PlayerCtrl player;
    #endregion [ Field Data ]

    #region [ Property ]
    public float MP { get { return _mp; } set { _mp = value; } }
    public float MaxMP { get { return _maxmp; } set { _maxmp = value; } }
    public float PlusHP { get { return _plushp; } }
    public float PlusMP { get { return _plusmp; } }
    public float PlusDamage { get { return _plusdamage; } }
    public float PlusDefense { get { return _plusdefense; } }
    public int Gold { get { return _gold; } set { _gold = value; } }

    public float EXP
    {
        get { return _exp; }
        set
        {
            _exp = value;
            CheckLevelUp();
        }
    }
    #endregion [ Property ]

    #region [ Initialize ]
    public void SettingPlayer(PlayerCtrl pc)
    {
        player = pc;
    }

    public void InitData()
    {
        _level = 1;
        _hp = _maxhp = 200;
        _mp = _maxmp = 200;
        _damage = 200;
        _defense = 5;
        _exp = 0;
        _gold = 5000;
        _moveSpeed = 10;
        _plushp = 0;
        _plusmp = 0;
        _plusdamage = 0;
        _plusdefense = 0;
    }

    #endregion [ Initialize ]

    #region [ Sub Func ]
    void CheckLevelUp()
    {
        int level = 1;
        while (true)
        {
            DataByLevel stat = TryGetData(level + 1);
            if (stat == null)
                break;
            if (_exp < stat.exp)
                break;
            level++;
        }

        if(level != _level)
        {
            _level = level;
        }
    }

    DataByLevel TryGetData(int level)
    {
        if (Managers.Data.Dict_Stat.TryGetValue(level, out DataByLevel stat))
            return stat;
        else
            return null;
    }

    public void SetStatByLevelUp(int level)
    {
        if (player == null)
            return;

        player.LevelUpEvent();
        DataByLevel stat = Managers.Data.Dict_Stat[level];
        _maxhp = stat.hp;
        _maxmp = stat.mp;
        _damage = stat.damage;
        _defense = stat.defense;
        SetMaxData();
        _hp = _maxhp;
        _mp = _maxmp;        
    }

    void SetPlayerData(float maxhp, float maxmp, float damage, float defense, float exp, int gold)
    {
        _maxhp = maxhp;
        _maxmp = maxmp;
        _damage = damage;
        _defense = defense;
        _exp = exp;
        _gold = gold;
        _moveSpeed = 10;
    }

    void SetPlusData(float hp, float mp, float dam, float def)
    {
        _plushp = hp;
        _plusmp = mp;
        _plusdamage = dam;
        _plusdefense = def;
        SetMaxData();
    }

    void SetMaxData()
    {
        _maxhp += _plushp;
        _maxmp += _plusmp;
        _damage += _plusdamage;
        _defense += _plusdefense;
    }

    #endregion [ Sub Func ]

    #region [ Stat Func ]
    public void SetPlusStat(eStat type, float value)
    {
        switch (type)
        {
            case eStat.HP:
                _plushp += value;
                _hp = Mathf.Min(_hp, _maxhp);
                _maxhp += value;
                break;
            case eStat.MP:
                _plusmp += value;
                _mp = Mathf.Min(_mp, _maxhp);
                _maxmp += value;
                break;
            case eStat.Damage:
                _plusdamage += value;
                _damage += value;
                break;
            case eStat.Defense:
                _plusdefense += value;
                _defense += value;
                break;
        }
    }

    public void UsePotion(eStat type, float value)
    {
        switch (type)
        {
            case eStat.HP:
                _hp = Mathf.Min(_hp + _maxhp * value, _maxhp);
                break;
            case eStat.MP:
                _mp = Mathf.Min(_mp + _maxmp * value, _maxmp);
                break;            
        }
    }
    
    public bool UseMP(float value)
    {
        if (_mp >= value)
        {
            _mp -= value;
            return true;
        }
        else        
            return false;
        
    }

    public bool UseMoney(int value)
    {
        if (_gold >= value)
        {
            _gold -= value;
            return true;
        }
        else
            return false;
    }

    #endregion [ Stat Func ]

    #region [ Save & Load ]
    public PlayerData SavePlayer()
    {
        if (_hp <= 0)
            InitData();

        PlayerData save = new PlayerData()
        {
            level = _level,
            nowhp = _hp,
            nowmp = _mp,
            nowexp = _exp,
            gold = _gold,
            plushp = _plushp,
            plusmp = _plusmp,
            plusdamage = _plusdamage,
            plusdefense = _plusdefense
        };

        return save;
    }

    public void LoadPlayer()
    {
        PlayerData data = Managers.Data.Data_Player;
        if (data != null)
        {
            if (data.nowhp == 0)
            {
                InitData();
                return;
            }            
            _level = data.level;
            DataByLevel DBL = TryGetData(_level);
            if (DBL != null)
            {
                SetPlayerData(DBL.hp, DBL.mp, DBL.damage, DBL.defense, data.nowexp, data.gold);
                SetPlusData(data.plushp, data.plusmp, data.plusdamage, data.plusdefense);
            }
            else
                InitData();            
        }
        else
            InitData();        
    }

    #endregion [ Save & Load ]
}
