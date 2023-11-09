using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateTrace : TSingleton<MonsterStateTrace>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.Agent.speed = m.Stat.TraceSpeed;
        m.State = MonsterState.Trace;
        m.BaseNavSetting();
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.CheckFarFromOffSet())
            m.ChangeState(MonsterStateReturn._inst);
        else
        {
            if (m.target != null)
            {
                if (m.CheckCloseTarget(m.target.position, m.Stat.TraceRange))
                {
                    m.Move(m.target.position);
                    if (m.CheckCloseTarget(m.target.position, m.Stat.AttackRange))
                        m.ChangeState(MonsterStateAttack._inst);
                }
                else
                    m.ChangeState(MonsterStatePatrol._inst);
            }
            else
                m.ChangeState(MonsterStatePatrol._inst);
        }

    }

    public void Exit(MonsterCtrl m)
    {
        
    }
}
