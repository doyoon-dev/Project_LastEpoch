using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetSkillData
{
    void GetSkillData(GameObject data);
}

public class SkillButton : MonoBehaviour, IGetSkillData
{
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
        
    }

    public void GetSkillData(GameObject data)
    {
        m_skillObj = data;
    }
}
