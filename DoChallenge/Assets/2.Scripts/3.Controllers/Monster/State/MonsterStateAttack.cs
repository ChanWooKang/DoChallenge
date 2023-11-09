using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateAttack : TSingleton<MonsterStateAttack>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.AttackNavSetting();
        m.NowCombo = MonsterCombo.Hit1;
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.target == null)
            m.ChangeState(MonsterStatePatrol._inst);
        else
        {
            m.TurnTowardPlayer();
            if (m.CheckCloseTarget(m.target.position, m.Stat.AttackRange))
            {
                m.cntTime += Time.deltaTime;
                if (m.cntTime > m.Stat.AttackDelay && m.isAttack == false)
                    m.Attack();
            }
            else
            {
                if (m.isAttack == false)
                    m.ChangeState(MonsterStateTrace._inst);
            }                
        }
    }

    public void Exit(MonsterCtrl m)
    {
        throw new System.NotImplementedException();
    }
}
