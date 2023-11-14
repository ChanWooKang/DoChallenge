using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class GoldItem : BaseItem, IItemSpawn , IInteractive
{
    public void Interact()
    {
        GameManagerEX.Game.player.ClearNearObject();
        GameManagerEX.Game.player.EarnMoney(Gold);
        Despawn();
    }

    public void Out()
    {
        GameManagerEX.Game.player.ClearNearObject();
    }

    public void Spawn(SOItem item, int gold = 0)
    {
        Gold = gold;
        SpawnObject();
    }
}
