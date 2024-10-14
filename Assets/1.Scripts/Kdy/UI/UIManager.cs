using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_optionObj;
    bool m_isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }
}
