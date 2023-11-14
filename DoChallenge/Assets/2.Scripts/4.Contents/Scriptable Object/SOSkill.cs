using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Skill")]
public class SOSkill : ScriptableObject
{
    public PlayerSkill type;
    public string skillName;
    public KeyCode key;
    public float useMp;
    public float cool;
    public float duration;
    public float effectValue;
    public List<STAT> sList = new List<STAT>();

    //UIs
    public Sprite icon;
    public string krName;
    [Multiline]
    public string description;
    [Multiline]
    public string effect;
}
