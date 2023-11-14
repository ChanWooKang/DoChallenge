using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class Item : BaseItem, IItemSpawn, IInteractive
{
    public void Interact()
    {
        Despawn();
    }

    public void Out()
    {
        GameManagerEX.Game.player.ClearNearObject();
    }

    public void Spawn(SOItem item, int gold = 0)
    {
        SpawnObject();
    }
}
