using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using DataContents;

public class PlayerStat : BaseStat
{
    protected float _mp;
    protected float _maxmp;
    protected float _exp;
    protected int _gold;
    protected float _plushp;
    protected float _plusmp;
    protected float _plusdamage;
    protected float _plusdefense;

    public float MP { get { return _mp; } set { _mp = value; } }
    public float MaxMP { get { return _maxmp; } set { _maxmp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }
    public float PlusHP { get { return _plushp; } }
    public float PlusMP { get { return _plusmp; } }
    public float PlusDamage { get { return _plusdamage; } }
    public float PlusDefense { get { return _plusdefense; } }

    public float EXP
    {
        get { return _exp; }
        set
        {
            _exp = value;
            int level = 1;
            while (true)
            {
                if (Managers.Data.Dict_Stat.TryGetValue(level + 1, out DataByLevel stat) == false) 
                    break;
                if (_exp < stat.exp)
                    break;
                level++;
            }

            if(level != _level)
            {
                _level = level;
                SetStat(_level);
            }
        }
    }

    public float ConvertEXP
    {
        get
        {
            if (_level == 1)
                return _exp;
            else
            {
                if (Managers.Data.Dict_Stat.TryGetValue(_level, out DataByLevel stat))
                    return _exp - stat.exp;
                else
                    return -1;
            }
        }
    }

    public float ConvertTotalEXP
    {
        get
        {
            if (Managers.Data.Dict_Stat.TryGetValue(_level, out DataByLevel stat) == false)
                return -1;
            else
            {
                if (Managers.Data.Dict_Stat.TryGetValue(_level + 1, out DataByLevel Nextstat) == false)
                    return -1;
                else
                    return Nextstat.exp - stat.exp;
            }
        }
    }

    public void Init()
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

    #region [ Setting Stat ]

    public void SetStat(int level)
    {
        if (GameManagerEX.Game.player == null)
            return;
        if (GameManagerEX.Game.player.Bools[PlayerBools.Dead] == true)
            return;
        GameManagerEX.Game.player.LevelUP();
        DataByLevel stat = Managers.Data.Dict_Stat[level];
        SetMaxData(stat);
        SetTotalData();
        _hp = _maxhp;
        _mp = _maxmp;
    }

    void SetMaxData(DataByLevel stat)
    {
        _maxhp = stat.hp;
        _maxmp = stat.mp;
        _damage = stat.damage;
        _defense = stat.defense;
    }

    void SetTotalData()
    {
        _maxhp += _plushp;
        _maxmp += _plusmp;
        _damage += _plusdamage;
        _defense += _plusdefense;
    }

    void SetPlayerData(DataByLevel DBL, PlayerData stat)
    {
        SetMaxData(DBL);
        _exp = stat.nowexp;
        _gold = stat.gold;
    }

    void SetPlusData(PlayerData stat)
    {
        _plushp = stat.plushp;
        _plusmp = stat.plusmp;
        _plusdamage = stat.plusdamage;
        _plusdefense = stat.plusdefense;
    }

    #endregion [ Setting Stat ]

    #region [ Save & Load ]
    public void LoadPlayer()
    {
        PlayerData stat = Managers.Data.Data_Player;
        if (stat != null)
        {
            _level = stat.level;
            if(Managers.Data.Dict_Stat.TryGetValue(_level, out DataByLevel DBL))
            {
                SetPlayerData(DBL, stat);
                SetPlusData(stat);
                SetTotalData();

                _hp = Mathf.Min(stat.nowhp, _maxhp);
                _mp = Mathf.Min(stat.nowmp, _maxmp);
            }
            else
                Init();
        }
        else
            Init();
    }

    public PlayerData SavePlayer()
    {
        if (_hp <= 0)
            Init();

        PlayerData saveData = new PlayerData()
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
        return saveData;
    }

    #endregion [ Save & Load ]

    public void AddPlusStat(eStat type, float value)
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
                _mp = Mathf.Min(_mp + _maxmp * value, _maxhp);
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
        {
            return false;
        }
    }

    public bool TryUseMoney(int value)
    {
        if (_gold >= value)
        {
            _gold -= value;
            return true;
        }
        else
            return false;
    }
}
