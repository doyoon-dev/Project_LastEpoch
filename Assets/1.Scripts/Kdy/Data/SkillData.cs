using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillInform
{
    public int mp;
    public int dmg;
    public int coolTime;
};

public class SkillData : MonoBehaviour
{
    public static Dictionary<string, SkillInform> m_skillData = new Dictionary<string, SkillInform>();

    private void Start()
    {
        m_skillData.Add("WindMill", new SkillInform() { mp = 10, dmg = 100, coolTime = 0 });
        m_skillData.Add("Lunge", new SkillInform() { mp = 10, dmg = 100, coolTime = 5 });
    }
}
