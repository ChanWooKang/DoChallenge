using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "DropTable", menuName = "Scriptable/DropTable")]
public class SODropTable : ScriptableObject
{
    public List<ItemWithWeight> list_Items = new List<ItemWithWeight>();
    public List<int> pickCnt_Weigthts = new List<int>();
    public SOItem GoldItem;

    SOItem PickItem()
    {
        int sum = 0;
        int i = 0;
        for (; i < list_Items.Count; i++)
            sum += list_Items[i].weight;

        int randValue = Random.Range(0, sum);
        for(i = 0; i < list_Items.Count; i++)
        {
            if (list_Items[i].weight > randValue)
                return list_Items[i].item;
            else
                randValue -= list_Items[i].weight;
        }
        return null;
    }

    public void ItemDrop(Transform parent , int gold = 0)
    {
        if(gold > 0)
        {
            SpawnManager.Instance.Spawn(GoldItem, parent, gold);
            return;
        }

        List<SOItem> dropList = new List<SOItem>();
        int sum = 0;
        int cnt = 0;
        int i = 0;
        for (; i < pickCnt_Weigthts.Count; i++)
            sum += pickCnt_Weigthts[i];
        int randValue = Random.Range(0, sum);
        for(i = 0; i < pickCnt_Weigthts.Count; i++)
        {
            if (pickCnt_Weigthts[i] > randValue)
            {
                cnt = i +1;
                break;
            }
            else
                randValue -= pickCnt_Weigthts[i];
        }

        for (i = 0; i < cnt; i++)
        {
            SOItem item = PickItem();
            if (item == null || item.itype == eItem.Unknown)
                continue;
            dropList.Add(item);
        }

        for (i = 0; i < dropList.Count; i++)
            SpawnManager.Instance.Spawn(dropList[i], parent);
    }
}
