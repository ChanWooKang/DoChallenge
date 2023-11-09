using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class GameScene : BaseScene
{
    
    protected override void Init()
    {
        base.Init();
        CurrScene = eScene.GameScene;
    }

    public override void Clear()
    {
        base.Clear();
    }

}
