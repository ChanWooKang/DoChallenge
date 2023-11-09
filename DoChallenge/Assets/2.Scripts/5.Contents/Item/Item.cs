using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Item : MonoBehaviour
{
    #region [ Component ]
    public SOItem itemSO;
    Rigidbody _rigid;
    Collider _colider;
    #endregion [ Component ]

    #region [ Field Data ]
    [SerializeField] float _rSpeed = 30.0f;
    bool isShoot;
    bool isCall;
    bool isStop;
    Coroutine TurnCoroutine;
    #endregion [ Field Data ]

    #region [ Property ]
    public int Gold { get; set; }
    #endregion [ Property ]

    void Init()
    {
        _rigid = GetComponent<Rigidbody>();
        _colider = GetComponent<Collider>();
        isShoot = false;
        isCall = false;
        isStop = false;        
    }

    Vector3 GetRandomPoint()
    {
        Vector3 pos = Random.onUnitSphere;
        pos.y = 1;
        return pos;
    }

    IEnumerator TurnEvent()
    {
        while(isStop == false)
        {
            transform.Rotate(_rSpeed * Vector3.up * Time.deltaTime);
            yield return null;
        }
    }

    public void Spawn(float power = 5)
    {
        Init();
        Vector3 dir = GetRandomPoint();
        _rigid.AddForce(dir * power, ForceMode.Impulse);
        isShoot = true;
    }

    //수정 인벤토리 매니저 생성시
    public bool Root()
    {
        if(itemSO.iType == eItem.Gold)
        {
            Despawn();
            return true;
        }
        else
        {
            return false;
        }
    }

    void Despawn()
    {
        _rigid.isKinematic = false;
        _colider.enabled = true;
        gameObject.DestroyAPS();
    }

    void OnCollisionStay(Collision collision)
    {
        if (isShoot)
        {
            if (collision.gameObject.CompareTag(ConstData.Ground))
            {
                _rigid.isKinematic = true;
                _colider.enabled = false;
                if(isCall == false)
                {
                    if(TurnCoroutine != null)
                        StopCoroutine(TurnCoroutine);
                    TurnCoroutine = StartCoroutine(TurnEvent());

                    isCall = true;
                }
            }
        }
    }
}
