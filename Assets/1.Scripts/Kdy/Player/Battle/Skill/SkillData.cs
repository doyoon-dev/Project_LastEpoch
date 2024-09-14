using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    public string Name;
    public int Mp;
    public float Dmg;
    public float CoolTime;
    public int Channeling;
    public float knockback;
}
