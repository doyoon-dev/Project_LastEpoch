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
        for (int i = 0; i < m_skillDatas.Length; i++)
        {
            m_skillDataDic.Add(m_skillDatas[i].name, m_skillDatas[i]);
        }
        //m_skillDataDic.Add(m_skillDatas[0].name, m_skillDatas[0]);
        //m_skillDataDic.Add(m_skillDatas[1].name, m_skillDatas[1]);
        //m_skillDataDic.Add(m_skillDatas[2].name, m_skillDatas[2]);
        //m_skillDataDic.Add(m_skillDatas[3].name, m_skillDatas[3]);
        //m_skillDataDic.Add(m_skillDatas[4].name, m_skillDatas[4]);
        //m_skillDataDic.Add(m_skillDatas[5].name, m_skillDatas[5]);
        //m_skillDataDic.Add(m_skillDatas[6].name, m_skillDatas[6]);
    }
}
