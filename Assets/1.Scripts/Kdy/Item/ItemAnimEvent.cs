using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemAnimEvent : MonoBehaviour
{
    public UnityEvent m_animAct;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimAct()
    {
        m_animAct?.Invoke();
    }
}
