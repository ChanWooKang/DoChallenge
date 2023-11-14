using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class BossField : MonoBehaviour
{
    BossCtrl boss = null;

    public void SettingBoss(BossCtrl bc)
    {
        boss = bc;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConstData.Tag_Player))
        {
            if (boss != null)
                boss.RecognizePlayer(other.transform);

            if(other.TryGetComponent(out PlayerCtrl pc))
            {
                if (pc.Bools[PlayerBools.Dead] == false)
                    pc.SetInBossField(boss.gameObject, true);
            }            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ConstData.Tag_Player))
        {
            if (other.TryGetComponent(out PlayerCtrl pc))
            {
                if (pc.Bools[PlayerBools.Dead] == false)
                    pc.SetInBossField(boss.gameObject, true);
            }
        }

        if (other.CompareTag(ConstData.Tag_Boss))
        {
            if (boss != null)
            {
                boss.IsOutField();
            }
        }
    }

}
