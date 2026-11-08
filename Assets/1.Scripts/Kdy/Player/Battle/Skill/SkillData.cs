using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTriggerType
{
    Instant,    // 단발형
    Channel     // 지속형 (키 입력 유지)
}

public enum SkillType
{
    ErasingStrike,
    Lunge,
    Normal,
    Warpath
}

[CreateAssetMenu]
public class SkillData : ScriptableObject
{
    public SkillTriggerType triggerType;
    public SkillType skillType;
    public string Name;
    public int Mp;
    public float InitDmg;               // 수정 안함
    public float Dmg;                   // 수정 가능
    public float CoolTime;
    public int Channeling;
    public float knockback;
    public float radius;                 // 스킬 범위
    public Transform effectPos;
    public GameObject vfxPrefab;
    public AudioClip sfx;
}
