using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillInform
{
    public int Mp;
    public int Dmg;
    public float CoolTime;
};

public class SkillData : MonoBehaviour
{
    public static Dictionary<string, SkillInform> m_skillData = new Dictionary<string, SkillInform>();

    private void Start()
    {
        m_skillData.Add("WindMill", new SkillInform() { Mp = 1, Dmg = 29, CoolTime = 0.5f });
        m_skillData.Add("Lunge", new SkillInform() { Mp = 8, Dmg = 15, CoolTime = 4.0f });
    }
}
