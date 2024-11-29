using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_optionObj;
    [SerializeField]
    GameObject m_statObj;
    bool m_isActive = false;
    bool m_isStatActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if(!m_isActive)
            {
                m_optionObj.SetActive(true);
            }
            else
            {
                m_optionObj.SetActive(false);
            }
            m_isActive = !m_isActive;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!m_isStatActive)
            {
                m_statObj.SetActive(true);
            }
            else
            {
                m_statObj.SetActive(false);
            }
            m_isStatActive = !m_isStatActive;
        }
    }
}
