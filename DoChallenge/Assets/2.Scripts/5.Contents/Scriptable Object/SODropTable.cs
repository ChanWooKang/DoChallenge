using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable")]
public class SODropTable : ScriptableObject
{
    public List<ItemWithWeight> list_Items = new List<ItemWithWeight>();
    public List<int> pickCnt_Weights = new List<int>();
    public SOItem GoldItem;

    public SOItem PickItem()
    {
        int sum = 0;
        foreach (var item in list_Items)
            sum += item.weight;

        int randValue = Random.Range(0, sum);
        for (int i = 0; i < list_Items.Count; i++)
        {
            var item = list_Items[i];
            if (item.weight > randValue)
                return list_Items[i].item;
            else
                randValue -= item.weight;
        }
        return null;
    }

    public void ItemDrop(Transform pos)
    {
        List<SOItem> dropList = new List<SOItem>();
        int sum = 0;
        int cnt = 0;
        int i = 0;
        for (; i < pickCnt_Weights.Count; i++)
            sum += pickCnt_Weights[i];

        int randValue = Random.Range(0, sum);
        for (i = 0; i < pickCnt_Weights.Count; i++)
        {
            if (pickCnt_Weights[i] > randValue)
            {
                cnt = i;
                break;
            }
            else
                randValue -= pickCnt_Weights[i];
        }

        for (i = 0; i < cnt; i++)
        {
            SOItem item = PickItem();
            if (item == null || item.iType == eItem.Unknown)
                continue;
            dropList.Add(item);
        }

        for (i = 0; i < dropList.Count; i++)
            SpawnManager.Inst.Spawn(dropList[i], pos);
    }

    public void ItemDrop(Transform pos, int gold)
    {
        SpawnManager.Inst.Spawn(GoldItem, pos, gold);
    }

}
