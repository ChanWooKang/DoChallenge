using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Mushroom : MonsterStat
{
    enum AnimationIndex
    {
        Sense = 0,
        HeadAttack = 1,
        KickAttack = 2,
        BombAttack = 3,
    }

    public override void Sense(Animator anim)
    {
        anim.SetTrigger((int)AnimationIndex.Sense);
    }

    public override void Attack(Animator anim)
    {
        int index = PickPattern();
        switch ((AnimationIndex)index)
        {
            case AnimationIndex.HeadAttack:
                anim.SetTrigger((int)AnimationIndex.HeadAttack);
                break;
            case AnimationIndex.KickAttack:
                anim.SetTrigger((int)AnimationIndex.KickAttack);
                break;
            case AnimationIndex.BombAttack:
                break;
        }
    }
    
    void BombAttack()
    {

    }
}
