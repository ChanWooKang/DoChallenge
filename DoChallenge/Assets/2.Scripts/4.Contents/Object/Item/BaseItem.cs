using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BaseItem : MonoBehaviour
{
    public SOItem itemSO;
    protected Rigidbody _rigid;
    SphereCollider _collider;

    [SerializeField] float _rSpeed = 30.0f;
    protected bool isShoot;
    bool isCall;
    bool isStop;

    public int Gold { get; set; }

    protected void Init()
    {
        _rigid = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
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

    IEnumerator Turnning()
    {
        while(isStop == false)
        {
            transform.Rotate(_rSpeed * Vector3.up * Time.deltaTime);
            yield return null;
        }
    }

    protected void SpawnObject(float power = 5.0f)
    {
        Init();
        Vector3 dir = GetRandomPoint();
        _rigid.AddForce(dir * power, ForceMode.Impulse);
        isShoot = true;
    }

    protected void Despawn()
    {
        gameObject.DestroyAPS();
        _rigid.isKinematic = false;
        _collider.enabled = true;
        isStop = false;
        isCall = false;
        isShoot = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (isShoot)
        {
            if (collision.gameObject.CompareTag(ConstData.Tag_Ground))
            {
                _rigid.isKinematic = true;
                _collider.enabled = false;
                if (isCall == false)
                {
                    StartCoroutine(Turnning());
                    isCall = true;
                }
            }
        }
    }
}
