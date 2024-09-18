using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent m_secondAtk;
    public UnityEvent m_comboStart;
    public UnityEvent m_comboEnd;
    public UnityEvent m_atkEvent;
    public UnityEvent m_skillWarPath;
    public UnityEvent m_skillLunge;
    public UnityEvent m_skillStrikeAtk;
    public UnityEvent m_skillStrikeEffectOn;
    public UnityEvent m_skillStrikeEffectOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SecondComboCheck()
    {
        m_secondAtk?.Invoke();
    }

    public void ComboStart()
    {
        m_comboStart?.Invoke();
    }

    public void ComboEnd()
    {
        m_comboEnd?.Invoke();
    }

    public void AtkEvent()
    {
        m_atkEvent?.Invoke();
    }

    public void Skill_WarPath()
    {
        m_skillWarPath?.Invoke();
    }

    public void Skill_Lunge()
    {
        m_skillLunge?.Invoke();
    }

    public void Skill_StrikeDmg()
    {
        m_skillStrikeAtk?.Invoke();
    }

    public void Skill_StrikeOn()
    {
        m_skillStrikeEffectOn?.Invoke();
    }

    public void Skill_StrikeOff()
    {
        m_skillStrikeEffectOff?.Invoke();
    }
}
