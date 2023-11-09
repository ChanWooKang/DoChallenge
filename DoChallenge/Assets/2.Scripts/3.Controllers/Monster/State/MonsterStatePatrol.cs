using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStatePatrol : TSingleton<MonsterStatePatrol>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.BaseNavSetting();
        m.targetPos = m._offSet;
        m.cntTime = 0;
        m.Agent.speed = m.Stat.MoveSpeed;
        m.State = MonsterState.Patrol;
    }

    public void Execute(MonsterCtrl m)
    {

        if (m.CheckFarFromOffSet())
            m.ChangeState(MonsterStateReturn._inst);
        else
        {
            if(m.target != null)
            {
                if (m.CheckCloseTarget(m.target.position, m.Stat.TraceRange))
                    m.ChangeState(MonsterStateTrace._inst);
                else
                {
                    if (m.CheckCloseTarget(m.targetPos, 0.5f))
                    {
                        m.cntTime += Time.deltaTime;
                        if (m.cntTime > m.delayTime)
                        {
                            m.cntTime = 0;
                            m.targetPos = m.GetRandomPos();
                        }
                        else
                        {
                            if (m.State != MonsterState.Sense)
                                m.State = MonsterState.Sense;
                        }
                    }
                    else
                    {
                        if (m.State != MonsterState.Patrol)
                            m.State = MonsterState.Patrol;
                        m.Move(m.targetPos);
                    }
                }
            }
            else
            {
                if (m.CheckCloseTarget(m.targetPos, 0.5f))
                {
                    m.cntTime += Time.deltaTime;
                    if (m.cntTime > m.delayTime)
                    {
                        m.cntTime = 0;
                        m.targetPos = m.GetRandomPos();
                    }
                    else
                    {
                        if (m.State != MonsterState.Sense)
                            m.State = MonsterState.Sense;
                    }
                }
                else
                {
                    if (m.State != MonsterState.Patrol)
                        m.State = MonsterState.Patrol;
                    m.Move(m.targetPos);
                }
            }
        }
    }

    public void Exit(MonsterCtrl m)
    {
        
    }
}
