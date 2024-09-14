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

public class SkillButton : MonoBehaviour, IGetSkillData, ISkillAction
{
    public event UnityAction<KeyCode> m_skillAct;
    public KeyCode m_skillKeyCode;
    public GameObject m_player;
    public GameObject m_skillObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(m_skillKeyCode))
        {
            ISkill_Lunge isl = m_player.GetComponent<ISkill_Lunge>();
            if(isl != null)
            {
                isl.Skill_Lunge(m_skillKeyCode);
            }
        }
    }

    public void GetSkillData(GameObject data)
    {
        m_skillObj = data;
    }
}
