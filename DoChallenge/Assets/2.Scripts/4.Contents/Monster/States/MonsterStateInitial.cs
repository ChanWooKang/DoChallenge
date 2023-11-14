using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MonsterStateInitial : TSingleton<MonsterStateInitial>, IFSMState<MonsterCtrl>
{
    public void Enter(MonsterCtrl m)
    {
        m._offSet = m.transform.position;
        m.Stat.SetStat(m.MonsterType);
        m.SetTarget();
        m.InitData();
        m.ChangeState(MonsterStatePatrol._inst);
        m.BaseNavSetting();
    }

    public void Execute(MonsterCtrl m)
    {

    }

    public void Exit(MonsterCtrl m)
    {

    }
}
