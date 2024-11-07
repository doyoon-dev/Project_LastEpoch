using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DieTextImage : ResurrectionUI
{
    //[SerializeField]
    //Image m_bgDieTextImageImage;
    //[SerializeField]
    //TextMeshProUGUI m_dieText;

    // Start is called before the first frame update
    void Start()
    {
        CoroutineShowUI(m_bgDieTextImage, m_dieText, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        base.CoroutineShowUI(bg, text, act);
    }

}
