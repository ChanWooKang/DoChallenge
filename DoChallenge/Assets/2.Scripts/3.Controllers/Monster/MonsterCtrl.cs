using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class MonsterCtrl : FSM<MonsterCtrl>
{
    #region [ Component ]
    MonsterStat _stat;
    Animator _anim;
    Rigidbody _rigid;
    Collider _colider;
    Renderer[] _meshs;
    NavMeshAgent _agent;
    public SODropTable _dropTable;
    PlayerCtrl _player;
    #endregion

    #region [ Field Data ]
    public Transform target = null;
    eMonster _mType = eMonster.Unknown;
    MonsterState _nowState = MonsterState.Idle;
    MonsterCombo _nowCombo = MonsterCombo.Hit1;

    [SerializeField, Range(8, 15)] float _rSpeed = 8.0f;
    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public Vector3 targetPos = Vector3.zero;
    [HideInInspector] public float lastCallTime;
    [HideInInspector] public float delayTime = 2.0f;
    [HideInInspector] public float cntTime;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool isReturnHome;

    Coroutine DamagedCoroutine;
    #endregion [ Field Data ]

    #region [ Property ]
    public MonsterStat Stat { get { return _stat; } }

    public NavMeshAgent Agent
    {
        get
        {
            if (_agent == null)
                _agent = gameObject.GetOrAddComponent<NavMeshAgent>();
            return _agent;
        }
    }

    public eMonster MonsterType { get { return _mType; } set { _mType = value; } }

    public MonsterState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;
            ChangeAnim(_nowState);
        }
    }

    public MonsterCombo NowCombo { get { return _nowCombo; } set { _nowCombo = value; } }

    public PlayerCtrl player
    {
        get
        {
            if(_player == null)
            {
                if (GameManagerEx.Game.player != null)
                    _player = GameManagerEx.Game.player;
            }
            
            return _player;
        }
    }
    #endregion [ Property ]

    void Start()
    {
        InitComponent();
        InitState(this, MonsterStateInitial._inst);
    }

    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    #region [ Initialize ]
    void InitComponent()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _colider = GetComponent<Collider>();
        _meshs = GetComponentsInChildren<Renderer>();
        _agent = GetComponent<NavMeshAgent>();
        _stat = new MonsterStat();

        _agent.updateRotation = false;
        //HPBAR 생성
    }

    public void InitData()
    {
        targetPos = Vector3.zero;
        _stat.HP = _stat.MaxHP;
        lastCallTime = 0f;
        delayTime = 2.0f;
        isDead = false;
        isAttack = false;
        isReturnHome = false;

        //HPBAR 초기화
    }
    #endregion [ Initialize ]

    #region [ NavMeshAgent Control ]
    public void BaseNavSetting()
    {
        _agent.ResetPath();
        _agent.isStopped = false;
        _agent.updatePosition = true;
        _agent.updateRotation = false;
    }

    public void AttackNavSetting()
    {
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.velocity = Vector3.zero;
    }
    #endregion [ NavMeshAgent Control ]

    #region [ Sub Func ]
    void FreezeRotation()
    {
        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;
    }

    public void ChangeAnim(MonsterState type)
    {
        switch (type)
        {
            case MonsterState.Die:
                _anim.CrossFade("Die", 0.1f);
                break;
            case MonsterState.Idle:
                _anim.CrossFade("Idle", 0.1f);
                break;
            case MonsterState.Sense:
                _anim.CrossFade("Sense", 0.1f, -1, 0);
                break;
            case MonsterState.Patrol:
                _anim.CrossFade("Patrol", 0.1f);
                break;
            case MonsterState.Trace:
                _anim.CrossFade("Trace", 0.1f);
                break;
            case MonsterState.Attack:
                switch (_mType)
                {
                    default:
                        {
                            switch (_nowCombo)
                            {
                                case MonsterCombo.Hit1:
                                    _anim.CrossFade("Hit1", 0.1f, -1, 0);
                                    _nowCombo = MonsterCombo.Hit2;
                                    break;
                                case MonsterCombo.Hit2:
                                    _anim.CrossFade("Hit2", 0.1f, -1, 0);
                                    _nowCombo = MonsterCombo.Hit1;
                                    break;
                            }
                        }
                        break;
                }
                break;

        }
    }

    void ChangeColor(Color color)
    {
        if (_meshs.Length > 0)
            foreach (Renderer mesh in _meshs)
                mesh.material.color = color;
    }

    public void ChangeLayer(eLayer layer)
    {
        int i = (int)layer;
        if (gameObject.layer != i)
            gameObject.layer = i;
    }
    
    public void SetTarget()
    {
        if (player != null && player.State != PlayerState.Die)
            target = player.transform;
        else
            target = null;
    }

    public bool CheckFarFromOffSet(float range = 20.0f)
    {
        float dist = Vector3.SqrMagnitude(transform.position - _offSet);
        if (dist > range * range)
            return true;
        return false;
    }

    public bool CheckCloseTarget(Vector3 pos, float range)
    {
        float dist = Vector3.SqrMagnitude(transform.position - pos);
        if (dist < range * range)
            return true;
        return false;
    }

    public Vector3 GetRandomPos(float range = 5.0f)
    {
        Vector3 pos = Random.onUnitSphere;
        pos.y = 0;
        float r;
        NavMeshPath  path = new NavMeshPath();

        while (true)
        {
            r = Random.Range(0, range);
            pos = _defPos + (pos * r);

            if (_agent.CalculatePath(pos, path))
                break;
        }
        return pos;
    }

    public void OnResurrectEvent()
    {
        if (_colider.enabled == false)
            _colider.enabled = true;
        ChangeLayer(eLayer.Monster);
        ChangeState(MonsterStateInitial._inst);
    }
    #endregion [ Sub Func ]

    #region [ Move & Turn ]
    public void Move(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
    }

    public void TurnTowardPlayer()
    {
        if(player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
        }
    }
    #endregion [ Move & Turn ]

    #region [ Attack ]
    public void Attack()
    {
        if(target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;
            return;
        }

        if (player.Dict_Bool[PlayerBools.ActDodge])
            return;

        _agent.avoidancePriority = 51;
        State = MonsterState.Attack;
        isAttack = true;
    }

    public void OnAttackEvent()
    {
        if (target == null || player.State == PlayerState.Die)
        {
            if (CheckCloseTarget(target.position, _stat.AttackRange))
                player.OnDamage(_stat);
        }
    }

    public void OffAttackEvent()
    {
        _agent.avoidancePriority = 50;
        cntTime = 0;
        isAttack = false;
    }
    #endregion [ Attack ]

    #region [ Damaged ]
    public void OnDamage(BaseStat stat)
    {
        if (isDead)
            return;
        isDead = _stat.GetHit(stat);
        
        if(DamagedCoroutine != null)
            StopCoroutine(DamagedCoroutine);
        DamagedCoroutine = StartCoroutine(OnDamageEvent());
    }

    IEnumerator OnDamageEvent()
    {
        //피격 사운드 && 데미지 텍스트

        if(isDead)
        {
            Die();
            yield break;
        }
        ChangeColor(Color.red);       
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
    }

    public void Die()
    {
        _colider.enabled = false;
        _agent.SetDestination(transform.position);
        _stat.DeadFunc(player.Stat);
        ChangeColor(Color.gray);
        ChangeState(MonsterStateDie._inst);
    }

    public void OnDeadEvent()
    {
        SpawnManager.Inst.MonsterDespawn(gameObject);
        ChangeColor(Color.white);
        ChangeState(MonsterStateDisable._inst);
        _dropTable.ItemDrop(transform, _stat.Gold);
        _dropTable.ItemDrop(transform);

        //GameManger KillCount
    }
    #endregion [ Damaged ]

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(ConstData.Weapon) || other.CompareTag(ConstData.Weapon) || other.CompareTag(ConstData.Weapon)) 
        {
            float damage = 0;
            if (other.CompareTag(ConstData.Weapon))
            {

            }
            else if (other.CompareTag(ConstData.Weapon))
            {

            }
            else
            {

            }

            if (damage > 0)
                isDead = _stat.GetHit(damage);
            else
                return;

            if(DamagedCoroutine != null)
                StopCoroutine(DamagedCoroutine);
            DamagedCoroutine = StartCoroutine(OnDamageEvent());
        }
    }
}
