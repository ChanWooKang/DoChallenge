using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable/Item")]
public class SOItem : ScriptableObject
{
    //Datas
    public string Name;
    public eItem itype;
    public eEquipment eType;
    public ePotion pType;
    public int maxStack;
    public List<STAT> sList = new List<STAT>();

    //UIs
    public Sprite icon;
    public string krName;
    [Multiline]
    public string desc;
    public int price;
}
