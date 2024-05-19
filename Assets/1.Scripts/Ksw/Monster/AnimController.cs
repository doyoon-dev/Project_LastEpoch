using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimController : MonoBehaviour
{
    protected Animator m_animator;
    string m_prevAnimName = string.Empty;

    public void SetRootMotion(bool isApply)
    {
        m_animator.applyRootMotion = isApply;
    }
    public void Play(string animName, bool isBlend)
    {
        if (!string.IsNullOrEmpty(m_prevAnimName))
        {
            m_animator.ResetTrigger(m_prevAnimName);
            m_prevAnimName = string.Empty;
        }
        if (isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_prevAnimName = animName;
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
}
