using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class BossCtrl : FSM<BossCtrl>, IHitAble, IMonsterSpawn
{
    [SerializeField] SODropTable _dropTable;
    [SerializeField] MonsterStat _stat;
    Animator _anim;
    Rigidbody _rigid;
    SkinnedMeshRenderer _mesh;
    NavMeshAgent _agent;
    BoxCollider _collider;
    AudioSource _source;

    BossState _nowState = BossState.Idle;
    [SerializeField]eMonster mType;    

    [SerializeField] Transform headTR;
    [HideInInspector] public Transform target = null;
    [HideInInspector] public Vector3 _offSet = Vector3.zero;
    [HideInInspector] public Vector3 _defPos = Vector3.zero;
    [HideInInspector] public float delayTime;
    [HideInInspector] public float cntTime;
    [HideInInspector] public float hitDamage;
    [SerializeField] float _rSpeed = 10.0f;
    [SerializeField] float _hpPerRate = 0.15f;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool isImotal;

    Coroutine DamagedCoroutine = null;
    Coroutine RegenerateCoroutine = null;

    #region [ Property ]
    public float HandDamage { get; set; }
    public MonsterStat Stat { get { return _stat; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public BossState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;
            ChangeAnim(_nowState);
        }
    }
    public eMonster MonsterType { get { return mType; } }
    public PlayerCtrl player { get { return GameManagerEX.Game.player; } }
    #endregion [ Property ]

    void Start()
    {
        InitComponent();
        InitState(this, BossStateInitial._inst);
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
        _rigid = GetComponent<Rigidbody>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<BoxCollider>();
        _source = gameObject.GetOrAddComponent<AudioSource>();
        _agent.updateRotation = false;
    }

    public void Init()
    {
        _stat.HP = _stat.MaxHP;
        isDead = false;
        isAttack = false;
        isImotal = false;
        SetCollider(true);
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

    public void IsOutField()
    {
        ChangeState(BossStateReturnHome._inst);
    }

    public void SetTarget(Transform targets)
    {
        if (player != null && player.State != PlayerState.Die)
            target = targets;
        else
            target = null;
    }

    void FreezeRotation()
    {
        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;
    }

    void SetCollider(bool isOn)
    {
        isImotal = !isOn;
        _collider.enabled = isOn;
    }

    public void ChangeColor(Color color)
    {
        _mesh.material.color = color;
    }

    public void ChangeLayer(eLayer layer)
    {
        int i = (int)layer;
        if (gameObject.layer != i)
            gameObject.layer = i;
    }

    public void ChangeAnim(BossState type)
    {
        switch (type)
        {
            case BossState.Die:
                _anim.CrossFade("Die", 0.1f);
                break;
            case BossState.Sleep:
                _anim.CrossFade("Sleep", 0.1f);
                break;
            case BossState.Idle:
                _anim.CrossFade("Idle", 0.1f);
                break;
            case BossState.Scream:
                _anim.CrossFade("Scream", 0.1f, -1, 0);
                break;
            case BossState.Trace:
            case BossState.Return:
                _anim.CrossFade("Walk", 0.1f);
                break;
            case BossState.Attack:
                _anim.CrossFade("Attack", 0.1f, -1, 0);
                break;
            case BossState.HandAttack:
                isImotal = true;
                _anim.CrossFade("Hand", 0.1f, -1, 0);
                break;
            case BossState.FlameAttack:
                _anim.CrossFade("Flame", 0.1f, -1, 0);
                break;
        }
    }

    public void Move(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        _agent.SetDestination(pos);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _rSpeed * Time.deltaTime);
        //transform.Rotate(dir * 20.0f * Time.deltaTime);
    }

    public void TurnToPlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, _rSpeed * Time.deltaTime);
            //transform.Rotate(dir * 20.0f * Time.deltaTime);
        }
    }

    public bool CheckCloseTarget(Vector3 pos, float range = 20.0f)
    {
        float dist = Vector3.SqrMagnitude(transform.position - pos);
        if (dist > range * range)
            return false;
        return true;
    }

    public void RecognizePlayer(Transform tr)
    {
        if(State == BossState.Sleep || State == BossState.Idle)
        {
            //사운드 세팅
            State = BossState.Scream;
            SetTarget(tr);
        }
    }

    public void EndScreamEvent() { ChangeState(BossStateTrace._inst); }

    public void Attack()
    {
        if (target == null || player.State == PlayerState.Die)
        {
            if (target != null)
                target = null;
            return;
        }

        if (player.Bools[PlayerBools.ActDodge])
            return;

        _agent.avoidancePriority = 51;
        State = BossState.Attack;
        isAttack = true;
    }

    public void OnAttackEvent()
    {

        if (target != null && player.State != PlayerState.Die)
        {

            Vector3 dir = target.position - transform.position;
            dir = dir.normalized;

            Ray ray = new Ray(transform.position + Vector3.up, dir * _stat.AttackRange);

            if (Physics.Raycast(ray, _stat.AttackRange, (1 << (int)eLayer.Player)))
            {
                //Sound 삽입

                player.OnDamage(_stat);
            }
        }
    }


    //이동 공격
    public void OnHandAttackEvent()
    {
        //SetClip(eSoundList.Boss_Hand);
        if (target != null && player.State != PlayerState.Die)
        {
            HandDamage = _stat.Damage;
            _collider.center = Vector3.forward * 8;
        }
    }

    //화염 발사 공격
    public void OnFlameAttackEvent()
    {
        //SetClip(eSoundList.Boss_Flame, true);
        //_flameEffect.OnEffect(headTR, _stat.Damage);
    }

    public void OffHandAttackEvent()
    {
        _collider.center = Vector3.zero;
        isImotal = false;
        //HandDamage = 0;

        Invoke("OffAttackEvent", 0.25f);
    }

    public void OffFlameEvent()
    {
        //StopAudio();
        //_flameEffect.OffEffect();
        Invoke("OffAttackEvent", 0.25f);
    }

    public void OffAttackEvent()
    {

        _agent.avoidancePriority = 50;
        cntTime = 0;
        isAttack = false;

    }

    void OnDamage()
    {
        if (DamagedCoroutine != null)
        {
            StopCoroutine(DamagedCoroutine);
        }
        DamagedCoroutine = StartCoroutine(OnDamageEvent());
    }

    public bool OnDamage(BaseStat stat)
    {
        if(isDead)
            return true;
        if (isImotal)
            return false;

        isDead = _stat.GetHit(stat);
        OnDamage();
        return isDead;
    }

    public bool OnDamage(float damage)
    {
        if (isDead)
            return true;
        if (isImotal)
            return false;

        isDead = _stat.GetHit(damage);
        OnDamage();
        return isDead;
    }

    IEnumerator OnDamageEvent()
    {
        //SetClip(eSoundList.GetHit);
        //FloatText.Create("FloatText", true, transform.position + Vector3.up, (int)_stat.AttackedDamage);

        if (isDead)
        {
            //사운드 삽입
            //_flameEffect.OffEffect();
            SetCollider(false);
            _agent.SetDestination(transform.position);
            _stat.DeadFunc(player.Stat);
            ChangeColor(Color.gray);
            ChangeState(BossStateDie._inst);
            yield break;
        }
        ChangeColor(Color.red);
        isImotal = true;
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
        isImotal = false;
    }
    public void OnRegenerate()
    {
        if (RegenerateCoroutine != null)
            StopCoroutine(RegenerateCoroutine);
        RegenerateCoroutine = StartCoroutine(RegenerateHP());
    }

    public void OffRegenerate()
    {
        if (RegenerateCoroutine != null)
            StopCoroutine(RegenerateCoroutine);
    }

    IEnumerator RegenerateHP()
    {
        float hpRate = _stat.MaxHP * _hpPerRate;
        while (_stat.HP <= _stat.MaxHP)
        {
            _stat.HP += hpRate;
            yield return new WaitForSeconds(1);
        }

        _stat.HP = _stat.MaxHP;
    }

    public void OnDeadEvent()
    {
        AttackNavSetting();
        SpawnManager.Instance.MonsterDespawn(gameObject);
    }

    public void Spawn(Vector3 pos, BossField bf)
    {
        _defPos = pos;
        //보스필드 세팅
        bf.SettingBoss(this);
    }

    public void Respawn()
    {
        if(isDead == false)
        {
            //미니맵 마크 생성
        }
        else
        {
            //게임 클리어
        }
    }
}
