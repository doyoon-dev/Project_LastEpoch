using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IInitLineUIColor
{
    void InitLineUIColor(Color lineBgColor, Color lineTextColor);
}

public interface ILineUIActive
{
    void LineUIActive();
}

public class LineUI : ResurrectionUI, IInitLineUIColor, ILineUIActive
{
    public Color m_initLineUIImageColor;
    public Color m_initLineUITextColor;
    // Start is called before the first frame update
    void Start()
    {
        InitColor();
        LineUIActive();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LineUIActive()
    {
        if(gameObject.activeSelf)
        {
            CoroutineShowUI(m_bgLineUIImage, m_dieLineUIText, ShowButtonUI);
        }
    }

    public override void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        StopAllCoroutines();
        base.CoroutineShowUI(bg, text, act);
    }

    public void InitColor()
    {
        m_initLineUIImageColor = m_bgLineUIImage.color;
        m_initLineUITextColor = m_dieLineUIText.color;
    }

    public void InitLineUIColor(Color lineBgColor, Color lineTextColor)
    {
        lineBgColor = m_initLineUIImageColor;
        lineTextColor = m_initLineUITextColor;
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
