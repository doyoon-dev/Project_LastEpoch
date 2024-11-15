using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISkillAction
{
    event UnityAction<KeyCode> m_skillAct;
}

public interface IGetSkillData
{
    void GetSkillData(GameObject data);
}

public class SkillButton : TestSkill, IGetSkillData
{
    public GameObject m_skillObj;
    public PlayerUI m_pUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(m_skillKey))
        {
            Skill_ErasingStrike(m_skillKey);
        }
    }

    public void GetSkillData(GameObject data)
    {
        m_skillObj = data;
    }
}
