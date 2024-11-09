using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IInitDieTextImageColor
{
    void InitDieTextImageColor(Color dieBgColor, Color dieTextColor);
}

public interface IDieTextImageActive
{
    void DieTextImageActive();
}

public class DieTextImage : ResurrectionUI, IInitDieTextImageColor, IDieTextImageActive
{
    public Color m_initDieTextImageColor;
    public Color m_initDieTextColor;

    // Start is called before the first frame update
    void Start()
    {
        InitColor();
        CoroutineShowUI(m_bgDieTextImage, m_dieText, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DieTextImageActive()
    {
        if (gameObject.activeSelf)
        {
            CoroutineShowUI(m_bgDieTextImage, m_dieText, null);
        }
    }

    public override void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        base.CoroutineShowUI(bg, text, act);
    }

    public void InitColor()
    {
        m_initDieTextImageColor = m_bgDieTextImage.color;
        m_initDieTextColor = m_dieText.color;
    }

    public void InitDieTextImageColor(Color dieBgColor, Color dieTextColor)
    {
        dieBgColor = m_initDieTextImageColor;
        dieTextColor = m_initDieTextColor;
    }

}
