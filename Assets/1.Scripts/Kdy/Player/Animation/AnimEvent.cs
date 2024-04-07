using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent m_firstAtk;
    public UnityEvent m_secondAtk;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FirstComboCheck()
    {
        m_firstAtk?.Invoke();
    }

    public void SecondComboCheck()
    {
        m_secondAtk?.Invoke();
    }
}
