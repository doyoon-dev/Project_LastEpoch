using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartQuestUI : MonoBehaviour
{
    public Image m_bg;
    public TextMeshProUGUI m_text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowQuest());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ShowQuest()
    {
        yield return new WaitForSeconds(0.5f);
        float time = 0.0f;
        Color bgColor = m_bg.color;
        Color textColor = m_text.color;
        while (time < 1.0f)
        {
            time += Time.deltaTime * 0.5f;
            bgColor.a = time;
            textColor.a = time;
            m_bg.color = bgColor;
            m_text.color = textColor;
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeOutQuest());
    }

    IEnumerator FadeOutQuest()
    {
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
