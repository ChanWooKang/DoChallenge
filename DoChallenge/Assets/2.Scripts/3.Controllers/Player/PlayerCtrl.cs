using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class PlayerCtrl : MonoBehaviour
{
    #region [ Component ]
    PlayerStat _stat;
    Animator _anim;
    Rigidbody _rigid;
    NavMeshAgent _agent;
    Renderer[] _meshs;


    #endregion [ Component ]

    #region [ Field Data ]     
    int _clickMask;
    int _blockMask;
    bool isClickMonster = false;
    bool isBossField = false;
    float hitcntTime = 0;
    float hitDamage = 0;
    [SerializeField, Range(0.2f, 0.5f)] float hitRate;
    [SerializeField, Range(0.2f, 0.5f)] float hitTime;
    [SerializeField, Range(2.0f, 4.0f)] float attackRange;

    [SerializeField] Vector3 _offSetPoint;
    Vector3 _destPos;
    Vector3 _moseWorldPoint;
    GameObject _lockTarget;

    GameObject _nearObject;
    eInteract _nearType;

    PlayerState _state = PlayerState.Idle;
    eSkill _sType = eSkill.Unknown;

    Dictionary<PlayerBools, bool> dict_bool;
    Coroutine RegerectCoroutine;
    Coroutine SlashCoroutine;
    #endregion [ Field Data ]

    #region [ Property ]
    public PlayerStat Stat { get { return _stat; } }
    public eSkill SkillState { get { return _sType; } }
    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            //사운드 

            switch (_state)
            {
                case PlayerState.Die:
                    _anim.CrossFade("Die", 0.1f);
                    break;
                case PlayerState.Idle:
                    _anim.CrossFade("Idle", 0.1f);
                    break;
                case PlayerState.Move:
                    _anim.CrossFade("Move", 0.1f);
                    break;
                case PlayerState.Attack:
                    _anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case PlayerState.Skill:
                    {
                        switch (_sType)
                        {
                            case eSkill.Dodge:
                                _anim.CrossFade("Dodge", 0.1f, -1, 0);
                                break;
                            case eSkill.Slash:
                                _anim.CrossFade("Slash", 0.1f, -1, 0);
                                break;
                            case eSkill.Spin:
                                _anim.CrossFade("Spin", 0.1f);
                                break;
                            case eSkill.Cry:
                                _anim.CrossFade("Cry", 0.1f, -1, 0);
                                break;
                        }
                    }
                    break;
            }
        }
    }

    public GameObject LockTarget { get { return _lockTarget; } set { _lockTarget = value; } }
    public Dictionary<PlayerBools, bool> Dict_Bool { get { return dict_bool; } }
    public NavMeshAgent Agent { get { return _agent; } }
    #endregion [ Property ]

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    void InitComponent()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _meshs = transform.GetChild(0).GetComponentsInChildren<Renderer>();
        _stat = new PlayerStat();

        //스킬 && 이펙트 && 사운드 컴퍼넌트
    }

    void InitData()
    {
        _offSetPoint = transform.position;
        _clickMask = (1 << (int)eLayer.Ground) | (1 << (int)eLayer.Monster);
        _blockMask = (1 << (int)eLayer.Block) | (1 << (int)eLayer.TransBlock);
        _destPos = transform.position;
        _moseWorldPoint = transform.position;

        _lockTarget = null;
        ClearNearObject();

        dict_bool = new Dictionary<PlayerBools, bool>();
        for(int i = 0; i <(int)PlayerBools.Max_Cnt; i++)
            dict_bool.Add((PlayerBools)i, false);

    }

    void ClearNearObject()
    {
        _nearObject = null;
        _nearType = eInteract.Unknwon;
    }

    public void LevelUpEvent() { }
    public void OnDamage(BaseStat stat) { }
}
