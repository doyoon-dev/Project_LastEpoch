using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IInitializeUI
{
    void InitializeUI();
}

public interface IResurrectionUIActive
{
    void ResurrectionUIActive();
}

public class ResurrectionUI : ShowUI, IInitializeUI, IResurrectionUIActive
{
    [Header("ResurrctionUI Parameter")]

    [Header("ResurrectionUI")]
    public Image m_bgImage;
    public GameObject m_dieTextImage;
    public GameObject m_lineImage;

    [Header("DieTextImage")]
    public Image m_bgDieTextImage;
    public TextMeshProUGUI m_dieText;
    // 이미지 텍스트 알파값 0 -> 1

    [Header("LineUI")]
    public Image m_bgLineUIImage;
    public TextMeshProUGUI m_dieLineUIText;
    // 이미지 텍스트 알파값 0 -> 1

    [Header("버튼")]
    public GameObject m_button;

    // Start is called before the first frame update
    void Start()
    {
        //ResurrectionUIActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResurrectionUIActive()
    {
        if (gameObject.activeSelf)
        {
            StartResurrectionUI();
        }
    }

    public void StartResurrectionUI()
    {
        m_dieTextImage.SetActive(true);
        IDieTextImageActive idtia = m_dieTextImage.GetComponent<IDieTextImageActive>();
        if (idtia != null)
        {
            idtia.DieTextImageActive();
        }
        
        StopAllCoroutines();
        StartCoroutine(ShowResurrectionUI());
    }

    IEnumerator ShowResurrectionUI()
    {
        yield return new WaitForSeconds(3.0f);
        m_lineImage.SetActive(true);
        ILineUIActive ilua = m_lineImage.GetComponent<ILineUIActive>();
        if (ilua != null)
        {
            ilua.LineUIActive();
        }
        float time = 1.0f;
        Color bgColor = m_bgImage.color;
        while (time > 0.0f)
        {
            time -= Time.deltaTime * 0.3f;
            bgColor.r = bgColor.g = bgColor.b = time;
            m_bgImage.color = bgColor;
            yield return null;
        }
    }

    public void InitializeUI()
    {
        Color bgColor = m_bgImage.color;
        bgColor.r = bgColor.g = bgColor.b = 1;
        m_bgImage.color = bgColor;

        InitColor(m_bgDieTextImage, m_dieText);
        InitColor(m_bgLineUIImage, m_dieLineUIText);

        m_dieTextImage.SetActive(false);
        m_lineImage.SetActive(false);
        m_button.SetActive(false);
        gameObject.SetActive(false);
    }

    void InitColor(Image image, TextMeshProUGUI text)
    {
        Color imageColor = image.color;
        Color textColor = text.color;
        imageColor.a = 0;
        textColor.a = 0;
        image.color = imageColor;
        text.color = textColor;
    }
}
