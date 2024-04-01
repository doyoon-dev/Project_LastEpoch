using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperty : MonoBehaviour
{
    Animator m_anim = null;
    protected Animator m_myAnim
    {
        get
        {
            if (m_anim == null)
            {
                m_anim = GetComponentInChildren<Animator>();
            }
            return m_anim;
        }
    }
}
