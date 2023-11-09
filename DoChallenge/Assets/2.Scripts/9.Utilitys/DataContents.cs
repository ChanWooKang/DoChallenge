using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;


namespace DataContents
{

    [System.Serializable]
    public class DataByLevel
    {
        public int level;
        public float hp;
        public float mp;
        public float damage;
        public float defense;
        public float exp;
    }

    [System.Serializable]
    public class PlayerData
    {
        public int level;
        public float nowhp;
        public float nowmp;
        public float nowexp;
        public int gold;
        public float plushp;
        public float plusmp;
        public float plusdamage;
        public float plusdefense;
    }

    [System.Serializable]
    public class DataByMonster
    {
        public eMonster index;
        public float hp;
        public float damage;
        public float defense;
        public float movespeed;
        public float tracespeed;
        public float tracerange;
        public float attackrange;
        public float attackdelay;
        public int mingold;
        public int maxgold;
        public float exp;
    }

    [System.Serializable]
    public class Inventorydata
    {
        public List<int> InvenArrayNumber = new List<int>();
        public List<string> InvenItemName = new List<string>();
        public List<int> InvenItemCount = new List<int>();
        public List<int> EquipArrayNumber = new List<int>();
        public List<string> EquipItemName = new List<string>();
        public string HotKeyItemName;
        public int HotKeyItemCount;
    }

    [System.Serializable]
    public class KillData
    {
        public List<int> MonsterType = new List<int>();
        public List<int> KillCount = new List<int>();
        public float InGameTime;
    }

    [System.Serializable]
    public class StatData : ILoader<int, DataByLevel>
    {
        public List<DataByLevel> stats = new List<DataByLevel>();
        public Dictionary<int, DataByLevel> Make()
        {
            Dictionary<int, DataByLevel> dict = new Dictionary<int, DataByLevel>();
            foreach (DataByLevel stat in stats)
            {
                dict.Add(stat.level, stat);
            }
            return dict;
        }
        public StatData() { }
    }

    [System.Serializable]
    public class MonsterData : ILoader<eMonster, DataByMonster>
    {
        public List<DataByMonster> stats = new List<DataByMonster>();
        public Dictionary<eMonster, DataByMonster> Make()
        {
            Dictionary<eMonster, DataByMonster> dict = new Dictionary<eMonster, DataByMonster>();
            foreach (DataByMonster stat in stats)
            {
                dict.Add(stat.index, stat);
            }
            return dict;
        }
        public MonsterData() { }
    }
}
