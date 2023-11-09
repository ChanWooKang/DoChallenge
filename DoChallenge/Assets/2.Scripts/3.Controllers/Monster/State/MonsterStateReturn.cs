using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateReturn : TSingleton<MonsterStateReturn>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.Agent.ResetPath();
        m.Agent.updatePosition = false;
        m.Agent.velocity = Vector3.zero;
        m.Agent.speed = m.Stat.MoveSpeed;
        m.State = MonsterState.Patrol;
        m.BaseNavSetting();
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.CheckCloseTarget(m._offSet, 0.5f))
        {
            if (m.target != null)
            {
                if (m.CheckCloseTarget(m.target.position, m.Stat.TraceRange))
                    m.ChangeState(MonsterStateTrace._inst);
                else
                    m.ChangeState(MonsterStatePatrol._inst);
            }
            else
                m.ChangeState(MonsterStatePatrol._inst);
        }
        else
        {
            if (m.State != MonsterState.Patrol)
                m.State = MonsterState.Patrol;
            m.Move(m._offSet);
        }
    }

    public void Exit(MonsterCtrl m)
    {
        
    }
}
