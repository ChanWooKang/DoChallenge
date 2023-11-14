using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class MonsterCtrl : FSM<MonsterCtrl>, IHitAble , IMonsterSpawn
{
    //Component
    [SerializeField] MonsterStat _stat;
    [SerializeField] SODropTable _dropTable;
    //HP바
    Animator _anim;
    Rigidbody _rigd;
    Collider _coll;
    Renderer[] _meshs;
    NavMeshAgent _agent;

    //Field Data
    [SerializeField] eMonster mType = eMonster.Unknown;
    MonsterState _nowState = MonsterState.Idle;
    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public Vector3 targetPos = Vector3.zero;
    [HideInInspector] public Transform target = null;
    [SerializeField, Range(8, 15)] float _rSpeed;
    [HideInInspector] public float delayTime = 2.0f;
    [HideInInspector] public float lastCallTime = 0;
    [HideInInspector] public float cntTime = 0;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector] public bool isReturnHome = false;
    Coroutine DamgedCoroutine = null;


    //Property
    public MonsterStat Stat { get { return _stat; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public eMonster MonsterType { get { return mType; } }
    public MonsterState State 
    {
        get { return _nowState; } 
        set 
        {
            _nowState = value;
        } 
    }
    public PlayerCtrl player { get { return GameManagerEX.Game.player; } }

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

    void InitComponent()
    {
        _anim = GetComponent<Animator>();
        _rigd = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _meshs = GetComponentsInChildren<Renderer>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }

    public void InitData()
    {
        targetPos = Vector3.zero;
        lastCallTime = 0;
        delayTime = 2.0f;
        isDead = false;
        isAttack = false;
        isReturnHome = false;
    }

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

    void FreezeRotation()
    {
        _rigd.velocity = Vector3.zero;
        _rigd.angularVelocity = Vector3.zero;
    }

    public void SetTarget()
    {
        if (player != null && player.State != PlayerState.Die)
            target = player.transform;
        else
            target = null;
    }

    public void ChangeAnim()
    {
        switch (_nowState)
        {
            case MonsterState.Die:
                _anim.CrossFade("Die", 0.1f);
                break;
            case MonsterState.Idle:
                _anim.CrossFade("Idle", 0.1f);
                break;
            case MonsterState.Sense:
                _stat.Sense(_anim);
                break;
            case MonsterState.Patrol:
                _anim.CrossFade("Patrol", 0.1f);
                break;
            case MonsterState.Trace:
                _anim.CrossFade("Trace", 0.1f);
                break;
            case MonsterState.Attack:
                _stat.Attack(_anim);
                break;
        }
    }

    void ChangeColor(Color color)
    {
        if (_meshs.Length > 0)
        {
            foreach (Renderer mesh in _meshs)
                mesh.material.color = color;
        }
    }

    public void ChangeLayer(eLayer layer)
    {
        int i = (int)layer;
        if (gameObject.layer != i)
            gameObject.layer = i;
    }

    public Vector3 GetRandomPos(float range = 5.0f)
    {
        Vector3 pos = Random.onUnitSphere;
        pos.y = 0;
        float r = 0;
        NavMeshPath path = new NavMeshPath();
        while (true)
        {
            r = Random.value * range;
            pos = _defPos + (pos * r);
            if (_agent.CalculatePath(pos, path))
                break;
        }
        return pos;               
    }

    public bool CheckCloseTarget(Vector3 pos, float range = 20.0f)
    {
        float dist = Vector3.SqrMagnitude(transform.position - pos);
        if (dist > range * range)
            return false;
        return true;
    }

    public void Move(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
    }

    public void TurnToPlayer()
    {
        if(player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
        }
    }

    public void Attack()
    {
        if(target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;
            return;
        }

        if (player.Bools[PlayerBools.ActDodge])
            return;

        _agent.avoidancePriority = 51;
        State = MonsterState.Attack;
        isAttack = true;
    }

    public void OnAttackEvent()
    {
        if (target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;
            return;
        }

        if(CheckCloseTarget(target.position, _stat.AttackRange))
            player.OnDamage(_stat);
    }

    public void OffAttackEvent()
    {
        _agent.avoidancePriority = 50;
        cntTime = 0;
        isAttack = false;
    }

    void OnDamage()
    {
        if (DamgedCoroutine != null)
            StopCoroutine(DamgedCoroutine);
        DamgedCoroutine = StartCoroutine(OnDamageEvent());
    }

    public bool OnDamage(BaseStat stat)
    {
        if (isDead)
            return true;
        isDead = _stat.GetHit(stat);

        OnDamage();
        return isDead;
    }

    public bool OnDamage(float damage)
    {
        if (isDead)
            return true;
        isDead = _stat.GetHit(damage);

        OnDamage();
        return isDead;
    }


    IEnumerator OnDamageEvent()
    {
        //사운드
        //피격 데미지 텍스트

        if (isDead)
        {
            Dead();
            yield break;
        }

        ChangeColor(Color.red);
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
    }

    void Dead()
    {
        _coll.enabled = false;
        _agent.SetDestination(transform.position);
        ChangeColor(Color.gray);
        //ChangeState(MonsterStateDie._inst);
        //if (player != null && player.State != PlayerState.Die)
        //    _stat.DeadFunc(player.Stat);
    }

    public void Spawn(Vector3 pos,BossField bf)
    {
        _defPos = pos;
    }

    public void Respawn()
    {
        if (isDead)
            OnResurrectEvent();
        else
        {
            //미니맵마크 생성
        }
    }

    public void OnDeadEvent()
    {
        SpawnManager.Instance.MonsterDespawn(gameObject);
        ChangeColor(Color.white);
        ChangeState(MonsterStateDisable._inst);
        _dropTable.ItemDrop(transform, _stat.Gold);
        _dropTable.ItemDrop(transform);
        
    }

    public void OnResurrectEvent()
    {
        if (_coll.enabled == false)
            _coll.enabled = true;
        ChangeLayer(eLayer.Monster);
        ChangeState(MonsterStateInitial._inst);
    }

    void OnTriggerEnter(Collider other)
    {
        
    }

    
}
