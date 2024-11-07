using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartQuestUI : ShowUI
{
    //public Image m_bg;
    //public TextMeshProUGUI m_text;
    // Start is called before the first frame update
    void Start()
    {
        CoroutineShowUI(m_bg, m_text, StartFadeOutQuest);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public override void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    //{
    //    base.CoroutineShowUI(bg, text, act);
    //}

    public void StartFadeOutQuest()
    {
        StartCoroutine(FadeOutQuest());
    }

    IEnumerator FadeOutQuest()
    {
        yield return new WaitForSeconds(2.0f);
        float time = 1.0f;
        Color bgColor = m_bg.color;
        Color textColor = m_text.color;
        while (time > 0.0f)
        {
            time -= Time.deltaTime * 0.5f;
            bgColor.a = time;
            textColor.a = time;
            m_bg.color = bgColor;
            m_text.color = textColor;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
