using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineUI : ResurrectionUI
{
    //[SerializeField]
    //Image m_bgLineUIImage;
    //[SerializeField]
    //TextMeshProUGUI m_dieLineUIText;
    // Start is called before the first frame update
    void Start()
    {
        CoroutineShowUI(m_bgLineUIImage, m_dieLineUIText, ShowButtonUI);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowButtonUI()
    {
        Invoke("ButtonActive", 2.0f);
        
    }

    void ButtonActive()
    {
        m_button.SetActive(true);
    }
}
