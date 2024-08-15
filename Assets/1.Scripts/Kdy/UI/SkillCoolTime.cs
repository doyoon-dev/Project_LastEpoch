using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public interface ICoolTime
{
    void CoolTime(KeyCode keyCode, float coolTime);
}

public class SkillCoolTime : MonoBehaviour, ICoolTime
{
    public Image m_QImage;
    public Image m_WImage;
    public Image m_EImage;
    public Image m_RImage;
    Image m_pushSkillKey;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Initialize()
    {
        m_QImage.fillAmount = 1.0f;
        m_WImage.fillAmount = 1.0f;
        m_EImage.fillAmount = 1.0f;
        m_RImage.fillAmount = 1.0f;
    }

    // StopAllCoroutines 하면 스킬 하나가 쿨 돌아가고 있을 때 다른 스킬 사용하면 쿨이 돌아가던 스킬이 안돌아갈 수 있음
    public void CoolTime(KeyCode keyCode, float coolTime)
    {
        if (PushSkillKey(keyCode) != null)
        {
            m_pushSkillKey = PushSkillKey(keyCode);
            m_pushSkillKey.fillAmount = 0.0f;
            StartCoroutine(Cooling(m_pushSkillKey, coolTime));
        }
    }

    IEnumerator Cooling(Image pushSkillKey, float coolTime)
    {
        while (pushSkillKey.fillAmount < 1.0f)
        {
            pushSkillKey.fillAmount += Time.deltaTime * (1 / coolTime);
            yield return null;
        }
        pushSkillKey.fillAmount = 1.0f;
    }

    Image PushSkillKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Q:
                return m_QImage;
            case KeyCode.W:
                return m_WImage;
            case KeyCode.E:
                return m_EImage;
            case KeyCode.R:
                return m_RImage;
        }
        return null;
    }
}
