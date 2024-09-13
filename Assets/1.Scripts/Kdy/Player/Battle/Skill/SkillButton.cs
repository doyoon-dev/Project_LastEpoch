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
    public Player m_player;
    GameObject m_skillObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(m_skillKeyCode))
        {
            m_skillAct?.Invoke(m_skillKeyCode);
        }
    }

    public void GetSkillData(GameObject data)
    {
        m_skillObj = data;
    }
}
