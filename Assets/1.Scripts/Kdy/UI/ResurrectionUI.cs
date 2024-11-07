using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IInitializeUI
{
    void InitializeUI();
}

public class ResurrectionUI : ShowUI, IInitializeUI
{
    //[Header("부활 오브젝트")]
    //[SerializeField]
    //public Image m_bgImage;
    //[SerializeField]
    //public GameObject m_dieTextImage;
    //[SerializeField]
    //public GameObject m_lineImage;
    //[Header("사망하였습니다 출력 오브젝트")]
    //[SerializeField]
    //public Image m_bgDieTextImage;
    //[SerializeField]
    //public TextMeshProUGUI m_dieText;
    //[Header("처치당했습니다 출력 오브젝트")]
    //[SerializeField]
    //public Image m_bgLineUIImage;
    //[SerializeField]
    //public TextMeshProUGUI m_dieLineUIText;
    //[Header("버튼")]
    //[SerializeField]
    //public GameObject m_button;
    // Start is called before the first frame update
    void Start()
    {
        m_dieTextImage.SetActive(true);
        StartCoroutine(ShowResurrectionUI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowResurrectionUI()
    {
        yield return new WaitForSeconds(3.0f);
        m_lineImage.SetActive(true);
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
        m_dieTextImage.SetActive(false);
        m_lineImage.SetActive(false);
        m_button.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetUI()
    {
        //m_dieTextImage.InitializeShowUI()
    }
}
