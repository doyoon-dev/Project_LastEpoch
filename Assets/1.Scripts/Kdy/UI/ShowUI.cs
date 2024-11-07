using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShowUI : MonoBehaviour
{
    [Header("StartQuestUI Parameter ----------------------")]
    public Image m_bg;
    public TextMeshProUGUI m_text;

    [Header("ResurrctionUI Parameter ----------------------")]

    [Header("ResurrectionUI")]
    public Image m_bgImage;
    public GameObject m_dieTextImage;
    public GameObject m_lineImage;

    [Header("DieTextImage")]
    public Image m_bgDieTextImage;
    public TextMeshProUGUI m_dieText;

    [Header("LineUI")]
    public Image m_bgLineUIImage;
    public TextMeshProUGUI m_dieLineUIText;

    [Header("¹öÆ°")]
    public GameObject m_button;
    public float m_speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        //StopAllCoroutines();
        StartCoroutine(Show(bg, text, act));
    }

    public IEnumerator Show(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0.0f;
        Color bgColor = bg.color;
        Color textColor = text.color;
        while (time < 1.0f)
        {
            time += Time.deltaTime * m_speed;
            bgColor.a = time;
            textColor.a = time;
            bg.color = bgColor;
            text.color = textColor;
            yield return null;
        }
        act?.Invoke();
    }

    public virtual void InitializeShowUI(Image bg, TextMeshProUGUI text)
    {
        Color bgColor = bg.color;
        Color textColor = text.color;
        bgColor.a = 0;
        textColor.a = 0;
        bg.color = bgColor;
        text.color = textColor;
    }
}
