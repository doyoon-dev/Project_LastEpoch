using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillInform
{
    public int Mp;
    public int Dmg;
    public int CoolTime;
};

public class SkillData : MonoBehaviour
{
    public static Dictionary<string, SkillInform> m_skillData = new Dictionary<string, SkillInform>();

    private void Start()
    {
        m_skillData.Add("WindMill", new SkillInform() { Mp = 10, Dmg = 100, CoolTime = 0 });
        m_skillData.Add("Lunge", new SkillInform() { Mp = 10, Dmg = 100, CoolTime = 5 });
    }
}
