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
}
