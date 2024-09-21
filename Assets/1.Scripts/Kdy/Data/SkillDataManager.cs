using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillInform
{
    public string Name;
    public int Mp;
    public float Dmg;
    public float CoolTime;
    public int Channeling;
    public float knockback;
};

public class SkillDataManager : MonoBehaviour
{
    public static Dictionary<string, SkillInform> m_skillData = new Dictionary<string, SkillInform>();
    public static Dictionary<string, SkillData> m_skillDataDic = new Dictionary<string, SkillData>();
    [SerializeField]
    SkillData[] m_skillDatas;
    public static float MaxKnockbackDuration = 0.7f;
    public static float MaxKnockbackDist = 5f;
    [HideInInspector]
    public int attackArea;
    [HideInInspector]
    public float attack;
    [HideInInspector]
    public float knockback;
    [HideInInspector]
    public float delayTime;

    private void Awake()
    {
        m_skillData.Add("Normal", new SkillInform() { Name = "Normal", Mp = 0, Dmg = 29, CoolTime = 0, Channeling = 0, knockback = 0 });
        m_skillData.Add("ErasingStrike", new SkillInform() { Name = "ErasingStrike", Mp = 10, Dmg = 29, CoolTime = 3.0f, Channeling = 0, knockback = 0 });
        m_skillData.Add("WindMill", new SkillInform() { Name = "WindMill", Mp = 1, Dmg = 29, CoolTime = 0.5f, Channeling = 10, knockback = 0 });
        m_skillData.Add("Lunge", new SkillInform() { Name = "Lunge", Mp = 8, Dmg = 15, CoolTime = 4.0f, Channeling = 0, knockback = 0 });

        m_skillDataDic.Add(m_skillDatas[0].name, m_skillDatas[0]);
        m_skillDataDic.Add(m_skillDatas[1].name, m_skillDatas[1]);
        m_skillDataDic.Add(m_skillDatas[2].name, m_skillDatas[2]);
        m_skillDataDic.Add(m_skillDatas[3].name, m_skillDatas[3]);
    }
}
