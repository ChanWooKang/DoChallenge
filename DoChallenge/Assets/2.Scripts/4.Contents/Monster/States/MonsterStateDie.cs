using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateDie : TSingleton<MonsterStateDie>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m.AttackNavSetting();
        m.ChangeLayer(eLayer.Disable);
        m.Agent.destination = m.transform.position;
        m.State = MonsterState.Die;
        m.cntTime = 0;
    }

    public void Execute(MonsterCtrl m)
    {
        if (m.isActiveAndEnabled)
        {
            m.cntTime += Time.deltaTime;
            if (m.cntTime > m.delayTime)
                m.OnDeadEvent();
        }
    }

    public void Exit(MonsterCtrl m)
    {

    }
}
