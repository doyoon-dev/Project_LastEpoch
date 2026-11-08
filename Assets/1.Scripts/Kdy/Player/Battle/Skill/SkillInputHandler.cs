using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SkillInputHandler : MonoBehaviour
{
    public SkillManager m_skillManager;
    KeyCode key;
    // Update is called once per frame
    void Update()
    {
        if (!m_skillManager.IsUsingSkill)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                key = KeyCode.Q;
                m_skillManager.InputKey(KeyCode.Q);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                key = KeyCode.W;
                m_skillManager.InputKey(KeyCode.W);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                key = KeyCode.E;
                m_skillManager.InputKey(KeyCode.E);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                key = KeyCode.R;
                m_skillManager.InputKey(KeyCode.R);
            }
        }
        // 여기서 사용 중인 키를 뗏을 때로 조건 바꿔야함
        if (Input.GetKeyUp(key))
        {
            key = KeyCode.None;
            m_skillManager.SetPressedKey(false);
        }
    }
}
