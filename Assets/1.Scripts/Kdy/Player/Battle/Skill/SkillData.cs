using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    public string Name;
    public int Mp;
    public float InitDmg;               // 수정 안함
    public float Dmg;                   // 수정 가능
    public float CoolTime;
    public int Channeling;
    public float knockback;
}
