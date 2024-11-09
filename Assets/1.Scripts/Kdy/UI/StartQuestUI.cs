using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartQuestUI : ShowUI
{
    [Header("StartQuestUI Parameter")]
    public Image m_bg;
    public TextMeshProUGUI m_text;

    public Color m_initQuestUIImageColor;
    public Color m_initQuestUITextColor;
    bool m_isStart = true;

    // Start is called before the first frame update
    void Start()
    {
        ShowQuestUI();
        //CoroutineShowUI(m_bg, m_text, StartFadeOutQuest);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowQuestUI();
        }
    }

    public override void CoroutineShowUI(Image bg, TextMeshProUGUI text, UnityAction act)
    {
        base.CoroutineShowUI(bg, text, act);
    }

    public void ShowQuestUI()
    {
        ShowText();
        CoroutineShowUI(m_bg, m_text, StartFadeOutQuest);
    }

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

    public void InitColor()
    {
        m_initQuestUIImageColor = m_bg.color;
        m_initQuestUITextColor = m_text.color;
    }

    public string ShowText()
    {
        if (m_isStart)
        {
            m_text.text = "던전에 설치된 오브젝트들을 모두 부수고 부활한 좀비왕을 처치하세요.";
            m_isStart = false;
        }
        else
        {
            m_text.text = "모든 오즈벡트가 부서져 좀비왕이 부활합니다.";
        }
        return m_text.text;
    }
}
