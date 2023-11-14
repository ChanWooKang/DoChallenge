using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class PlayerCtrl : MonoBehaviour,IHitAble
{
    [SerializeField] PlayerStat _stat;
    Animator _anim;
    Rigidbody _rigid;
    NavMeshAgent _agent;
    Renderer[] _meshs;

    //LayerMask
    int _clickMask;
    int _blockMask;
    int _groundMask;
    int _monsterMask;

    Vector3 _offSetPos;
    Vector3 _destPos;
    Vector3 _mouseWorldPoint;

    float hitCntTime = 0;
    float hitDamage = 0;
    [SerializeField, Range(0.2f, 0.5f)] float hitRate;
    [SerializeField, Range(0.2f, 0.5f)] float hitTime;
    [SerializeField, Range(2.0f, 4.0f)] float attackRange;
    [SerializeField] float _hpPerRate;
    [SerializeField] float _mpPerRate;
    GameObject _lockTarget;
    GameObject _nearObject;
    PlayerState _nowState = PlayerState.Idle;
    PlayerSkill _nowSkill = PlayerSkill.Unknown;
    Dictionary<PlayerBools, bool> dict_bool;

    Coroutine RegenerateCoroutine = null;
    Coroutine DamagedCoroutine = null;
    

    public PlayerStat Stat { get { return _stat; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public GameObject LockTarget { get { return _lockTarget; } set { _lockTarget = value; } }
    public PlayerSkill Skill { get { return _nowSkill; } }
    public PlayerState State
    {
        get { return _nowState; }
        set
        {
            _nowState = value;

            //사운드

            switch (_nowState)
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
                    //{
                    //    switch (_sType)
                    //    {
                    //        case eSkill.Dodge:
                    //            _anim.CrossFade("Dodge", 0.1f, -1, 0);
                    //            break;
                    //        case eSkill.Slash:
                    //            _anim.CrossFade("Slash", 0.1f, -1, 0);
                    //            break;
                    //        case eSkill.Spin:
                    //            _anim.CrossFade("Spin", 0.1f);
                    //            break;
                    //        case eSkill.Cry:
                    //            _anim.CrossFade("Cry", 0.1f, -1, 0);
                    //            break;
                    //    }
                    //}
                    break;
            }
        }
    }    
    public Dictionary<PlayerBools, bool> Bools { get { return dict_bool; } }

    void Awake()
    {
        InitComponent();
        InitData();
    }

    void Start()
    {
        Managers.Input.KeyAction -= OnKeyBoardEvent;
        Managers.Input.KeyAction += OnKeyBoardEvent;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
    }

    void Update()
    {
        switch (_nowState)
        {
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Attack:
                UpdateAttack();
                break;
            case PlayerState.Skill:
                {
                    switch(_nowSkill)
                    {
                        case PlayerSkill.Slash:
                        case PlayerSkill.Dodge:
                            UpdateSkill();
                            break;
                        case PlayerSkill.Spin:
                            UpdateMoveDuringSkill();
                            break;
                    }
                }
                break;
        }
    }

    void FixedUpdate()
    {
        FreezeRotate();
    }

    void InitComponent()
    {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _meshs = GetComponentsInChildren<Renderer>();


    }
    
    void InitData()
    {
        _offSetPos = transform.position;
        _destPos = transform.position;
        _mouseWorldPoint = transform.position;
        _clickMask = (1 << (int)eLayer.Ground) | (1 << (int)eLayer.Monster);
        _blockMask = (1 << (int)eLayer.Block) | (1 << (int)eLayer.TransBlock);
        _groundMask = (1 << (int)eLayer.Ground);
        _monsterMask = (1 << (int)eLayer.Monster);
        _lockTarget = null;
        _nearObject = null;

        dict_bool = new Dictionary<PlayerBools, bool>();
        for (int i = 0; i < (int)PlayerBools.Max_Cnt; i++)
            dict_bool.Add((PlayerBools)i, false);
    }

    public void StartStatSetting(bool isNewGame = true)
    {
        if (isNewGame)
            _stat.Init();
        else
            _stat.LoadPlayer();        
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

    void FreezeRotate()
    {
        if (_nowSkill == PlayerSkill.Dodge)
            return;

        _rigid.velocity = Vector3.zero;
        _rigid.angularVelocity = Vector3.zero;
    }

    public void ChangeColor(Color color)
    {
        foreach (Renderer mesh in _meshs)
            mesh.material.color = color;
    }

    bool CheckDistance(float range)
    {
        if (_lockTarget == null)
            return false;

        _destPos = _lockTarget.transform.position;
        Vector3 dir = _destPos - transform.position;
        dir = dir.normalized;

        Ray ray = new Ray(transform.position + Vector3.up, dir * range);
        if (Physics.Raycast(ray, out RaycastHit rhit, range, _monsterMask))
            return true;
        else
            return false;
    }

    void CheckAttackAble(float range = 2.0f)
    {
        if(dict_bool[PlayerBools.ClickMonster] == false || _lockTarget == null)
        {
            return;
        }

        if (CheckDistance(range))
            State = PlayerState.Attack;
    }

    public void ClearNearObject()
    {
        _nearObject = null;
    }

    bool CheckMousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit rhit, 100, _groundMask))
        {
            _mouseWorldPoint = rhit.point - transform.position;
            _mouseWorldPoint.y = 0;
            _mouseWorldPoint = _mouseWorldPoint.normalized;
            return true;
        }
        _mouseWorldPoint = transform.position;
        return false;
    }

    #region [ Key Action && Mouse Action ]

    void OnKeyBoardEvent()
    {
        if (dict_bool[PlayerBools.Dead] || dict_bool[PlayerBools.ActDodge])
            return;

        OnInteractEvent(Input.GetKeyDown(KeyCode.F));
    }

    void OnMouseEvent(MouseEvent evt)
    {
        if (dict_bool[PlayerBools.Dead] || dict_bool[PlayerBools.ActDodge])
            return;

        switch (_nowState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                OnMouseEvent_IDLEMOVE(evt);
                break;
            case PlayerState.Attack:
                OnMouseEvent_ATTACK(evt);
                break;
            case PlayerState.Skill:
                if (_nowSkill == PlayerSkill.Spin)
                    OnMouseEvent_SKILLSPIN(evt);
                break;
        }
    }

    void OnMouseEvent_IDLEMOVE(MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit rhit, 100, _clickMask);
        switch (evt)
        {
            case MouseEvent.PointerDown:
                {
                    if (hit)
                    {
                        _destPos = rhit.point;
                        if (State != PlayerState.Move)
                            State = PlayerState.Move;
                        dict_bool[PlayerBools.ContinueAttack] = true;
                        if(rhit.collider.gameObject.layer == (int)eLayer.Monster)
                        {
                            dict_bool[PlayerBools.ClickMonster] = true;
                            _lockTarget = rhit.transform.gameObject;
                        }
                        else
                        {
                            dict_bool[PlayerBools.ClickMonster] = false;
                            if (dict_bool[PlayerBools.InBossField] == false)
                                _lockTarget = null;
                        }
                    }
                    else
                    {
                        if (State == PlayerState.Move)
                            State = PlayerState.Idle;
                    }                        
                }
                break;
            case MouseEvent.Press:
                {
                    if(dict_bool[PlayerBools.InBossField] == false)
                    {
                        if (_lockTarget == null && hit)
                            _destPos = rhit.point;
                    }
                    else
                    {
                        if (hit)
                            _destPos = rhit.point;
                    }
                }
                break;
            case MouseEvent.PointerUp:
                {
                    dict_bool[PlayerBools.ClickMonster] = false;
                    dict_bool[PlayerBools.ContinueAttack] = false;
                }
                break;
        }
    }

    void OnMouseEvent_ATTACK(MouseEvent evt)
    {
        if (evt == MouseEvent.PointerUp)
            dict_bool[PlayerBools.ContinueAttack] = false;
        else
        {
            if (CheckDistance(attackRange) == false)
                dict_bool[PlayerBools.ContinueAttack] = false;
        }
    }

    void OnMouseEvent_SKILLSPIN(MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out RaycastHit rhit, 100, 1 << (int)eLayer.Ground);
        switch (evt)
        {
            case MouseEvent.PointerDown:
            case MouseEvent.Press:
                {
                    if (hit)
                    {
                        _destPos = rhit.point;
                    }
                }
                break;
        }
    }
    #endregion [ Key Action && Mouse Action ]

    #region [ Update By State ]
    void UpdateMove()
    {
        CheckAttackAble(attackRange);
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
        {
            float dist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.sqrMagnitude);
            _agent.Move(dir.normalized * dist);

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _blockMask))
            {
                //벽앞에 있더라도 마우스를 계속 누르고 있을경우 뛰는 애니메이션 재생
                if (Input.GetMouseButton(1))
                    return;
            }

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 20.0f * Time.deltaTime);
            }
        }
    }

    void UpdateMoveDuringSkill()
    {
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude >= 0.01f)
        {
            float dist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.sqrMagnitude);
            _agent.Move(dir.normalized * dist);

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, _blockMask))
                return;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 20.0f * Time.deltaTime);
            }
        }
    }

    void UpdateAttack()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f * Time.deltaTime);
            }
        }
    }

    void UpdateSkill()
    {
        if (_mouseWorldPoint != transform.position && _mouseWorldPoint != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(_mouseWorldPoint);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 20.0f * Time.deltaTime);
        }
    }

    #endregion [ Update By State ]

    #region [ KeyBoard Event ]
    void OnInteractEvent(bool btnDown)
    {
        if (_nearObject == null)
            return;

        if (btnDown)
        {
            if (_nearObject.TryGetComponent(out IInteractive obj))
                obj.Interact();
            else
                ClearNearObject();
        }
    }
    #endregion [ KeyBoard Event ]

    #region [ State Event ]
    public void LevelUP()
    {

    }

    public void OnAttackEvent()
    {
        if(_lockTarget != null)
        {
            if(_lockTarget.TryGetComponent(out IHitAble hit))
            {
                if(hit.OnDamage(_stat))
                {
                    _lockTarget = null;
                    dict_bool[PlayerBools.ContinueAttack] = false;
                }
            }
        }
    }

    public void OffAttackEvent()
    {
        if (dict_bool[PlayerBools.ContinueAttack])
            State = PlayerState.Attack;
        else
            State = PlayerState.Idle;
    }

    public void OnDamage()
    {
        if (DamagedCoroutine != null)
            StopCoroutine(DamagedCoroutine);
        //DamagedCoroutine = StartCoroutine();
    }

    public bool OnDamage(BaseStat stat)
    {
        if (dict_bool[PlayerBools.Dead])
            return true;
        if (dict_bool[PlayerBools.ActDodge])
            return false;

        dict_bool[PlayerBools.Dead] = _stat.GetHit(stat);
        OnDamage();

        return dict_bool[PlayerBools.Dead];
    }

    public bool OnDamage(float damage)
    {
        if (dict_bool[PlayerBools.Dead])
            return true;
        if (dict_bool[PlayerBools.ActDodge])
            return false;

        dict_bool[PlayerBools.Dead] = _stat.GetHit(damage);
        OnDamage();

        return dict_bool[PlayerBools.Dead];
    }

    public void OnDeadEvent()
    {
        AttackNavSetting();
    }   

    public void SetInBossField(GameObject boss = null, bool Inside = false)
    {
        dict_bool[PlayerBools.InBossField] = Inside;
        _lockTarget = boss;
    }
    #endregion [ State Event ]

    #region [ Stat Event ]
    public void ResetStat() { _stat.Init(); }    
    public void EarnMoney(int gold) { _stat.Gold += gold; }
    public bool TryUseMoney(int gold) { return _stat.TryUseMoney(gold); }
    public bool UseMP(float value) { return _stat.UseMP(value); }
    public void UsePotion(eStat type, float value) { _stat.UsePotion(type, value); }
    #endregion [ Stat Event ]

    #region [ Skill ]
    public void SkillEvent() 
    {
    }

    void SlashEvent(SOSkill skill)
    {

    }
    void HealEvent(SOSkill skill)
    {
       
    }

    void SpinEvent(SOSkill skill)
    {
        
    }

    void CryEvent(SOSkill skill)
    {
       
    }

    void DodgeEvent(SOSkill skill)
    {
        
    }

    public void EndSkillAnim()
    {
        _nowSkill = PlayerSkill.Unknown;
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            State = PlayerState.Idle;
        else
            State = PlayerState.Move;
    }

    public void PotionEvent(ePotion type)
    {

    }

    public void CancelSlash(GameObject go)
    {
        
    }

    #endregion [ Skill ]

    #region [ Coroutine ]
    IEnumerator RegenerateStat()
    {
        float hpRate = _stat.MaxHP * _hpPerRate;
        float mpRate = _stat.MaxMP * _mpPerRate;

        while(dict_bool[PlayerBools.Dead] == false)
        {
            if (_stat.HP < _stat.MaxHP)
                _stat.HP = Mathf.Min(_stat.HP + hpRate, _stat.MaxHP);
            if (_stat.MP < _stat.MaxMP)
                _stat.MP = Mathf.Min(_stat.MP + mpRate, _stat.MaxMP);
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator OnDamageEvent()
    {
        //플로팅 텍스트
        if (dict_bool[PlayerBools.Dead])
        {
            State = PlayerState.Die;
            ChangeColor(Color.gray);
            yield return new WaitForSeconds(2.0f);
            OnDeadEvent();
            yield break;
        }
        ChangeColor(Color.red);
        yield return new WaitForSeconds(0.3f);
        ChangeColor(Color.white);
    }
    #endregion [ Coroutine ]

    void OnTriggerEnter(Collider other)
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ConstData.Tag_Interact))
        {
            if (other.TryGetComponent(out IInteractive item))
                item.Out();
        }
    }
}
