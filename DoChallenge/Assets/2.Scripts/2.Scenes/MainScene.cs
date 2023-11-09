using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        CurrScene = eScene.MainScene;
    }

    public override void Clear()
    {
        base.Clear();
    }
}

