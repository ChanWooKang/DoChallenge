using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Define
{
    #region [ System ]
    public enum eScene
    {
        Unknown,
        MainScene = 0,
        GameScene = 1
    }

    public enum eLayer
    {
        UI = 5,
        Ground = 6,
        Player = 7,
        Block = 8,
        Disable = 9,
        Monster = 10,
        Item = 11,
        TransBlock = 12,
    }

    public enum eTag
    {
        Scene,
        Ground,
        Player,
        Monster,
        Interact,
        Weapon,
        Skill,
        Cry,
        Slash,
        Block,
        Boss,
        Fire
    }
    public enum eSound
    {
        BGM = 0,
        SFX,
        Max_Cnt
    }

    public enum PoolType
    {
        Monster,
        RootItem,
        Effect,
        UI,
    }

    public enum eCursor
    {
        Unknwon = 0,
        Default,
        Attack,
    }

    public enum UIEvent
    {
        Click
    }

    public enum MouseEvent
    {
        Click,
        Press,
        PointerDown,
        PointerUp,
    }

    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> Make();
    }
    #endregion [ System ]
}

namespace Define
{
    #region [Enum]    

    #region [ Game ]
    public enum CameraMode
    {
        QuaterView
    }
    #endregion [ Game ]

    #region [ Sound ]


    public enum eSoundList
    {
        BGM_GameScene,
        BGM_MainScene,
        Boss_Flame,
        Boss_Hand,
        Player_Cry,
        Player_Dodge,
        Player_Hit,
        Player_Move,
        Player_Pickup,
        Player_Spin,
        UI_Open,
        UI_Touch,
        Player_LevelUp,
        Shop_BuySell,
        GetHit,
        Inven_ChangeEquip,
        Boss_Growl,
        GameClear,
        GameOver,
        Player_Slash,
        Player_Heal,
        Boss_Bite
    }
    #endregion [ Sound ]   

    #region [ Player ]
    public enum PlayerState
    {
        Idle,
        Move,
        Attack,
        Skill,
        Die
    }

    public enum PlayerBools
    {
        Dead = 0,
        ContinueAttack,
        ActDodge,
        ActSkill,
        ClickMonster,
        InBossField,
        Max_Cnt,
    }

    public enum PlayerSkill
    {
        Unknown = 0,
        Slash,
        Heal,
        Spin,
        Cry,
        Dodge,
    }

    #endregion [ Player ]

    #region [ Monster ]
    public enum eMonster
    {
        Unknown = 0,
        Cactus,
        Mushroom,
        Slime,
        TurtleShell,



        Boss,
        Max_Cnt
    }

    public enum TranslateMonsterName
    {
        Unknown = 0,
        선인장몬,
        버섯몬,
        슬라임,
        가시거북,


        드래곤,
        Max_Cnt,
    }

    public enum MonsterState
    {
        Die,
        Idle,
        Patrol,
        Sense,
        Trace,
        Attack,
        Disable
    }

    #endregion

    #region [ Boss ]
    public enum BossState
    {
        Die,
        Sleep,
        Idle,
        Scream,
        Trace,
        Return,
        Attack,
        HandAttack,
        FlameAttack,
        Disable
    }

    public enum BossPattern
    {
        Basic,
        Hand,
        Flame
    }
    #endregion [ Boss ]

    #region [ Stat ]

    public enum eStat
    {
        HP = 0,
        MP,
        Damage,
        Defense,
        Max_Cnt
    }

    #endregion [ Stat ]

    #region [ Item ]

    public enum eInteract
    {
        Unknwon = 0,
        Item,
        Shop
    }

    public enum eItem
    {
        Unknown = 0,
        Gold,
        Equipment,
        Potion,        
    }

    public enum eEquipment
    {
        Unknown = -1,
        Helm = 0,
        Chest,
        Arm,
        Leg,
        Sheild,
        Weapon,
        Max_Cnt
    }

    public enum ePotion
    {
        Unknown = 0,
        HP,
        MP,
        Double,
    }

    #endregion [ Item ]
   
    #endregion [Enum]

    #region [Class]

    [System.Serializable]
    public class PoolUnit
    {
        public string name;
        public PoolType type;
        public GameObject prefab;
        public int amount;
        int curAmount;
        public int CurAmount { get { return curAmount; } set { curAmount = value; } }
    }


    [System.Serializable]
    public class CursorUnit
    {
        public eCursor type;
        public Texture2D tex;
    }

    [System.Serializable]
    public class ItemWithWeight
    {
        public SOItem item;
        public int weight;
    }

    #endregion [Class]

    #region [ Interface ]

    public interface IHitAble
    {
        bool OnDamage(BaseStat attack);
        bool OnDamage(float damage);
    }
   
    public interface IMonsterSpawn
    {
        void Spawn(Vector3 pos, BossField bf);

        void Respawn();
    }

    public interface IItemSpawn
    {
        void Spawn(SOItem item, int gold = 0);
    }

    public interface IMonster
    {
        void Attack(Animator anim);

        void Sense(Animator anim);
    }

    public interface IInteractive
    {
        void Interact();
        void Out();
    }

    public interface IFSMState<T>
    {
        void Enter(T m);
        void Execute(T m);
        void Exit(T m);
    }

    #endregion [ Interface ]

    #region [ Struct ]
    [System.Serializable]
    public struct STAT
    {
        public eStat statType;
        public string statName;
        public string descName;
        public float sValue;
    }

    [System.Serializable]
    public struct SpawnPoint
    {
        public Transform target;
        public eMonster targetType;
    }
    #endregion [ Struct ]    
}

namespace Define
{
    public static class ConstData
    {
        public const string Tag_Interact = "Interact";
        public const string Tag_Ground = "Ground";
        public const string Tag_Player = "Player";
        public const string Tag_Boss = "Boss";
    }
}



