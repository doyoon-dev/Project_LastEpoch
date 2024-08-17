using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerUI : MonoBehaviour
{
    public Player m_player;
    public Image m_hpUI;
    public Image m_mpUI;
    public Text m_hpText;
    public Text m_mpText;
    public SkillCoolTime m_skillCoolTime;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // HP MP UI 이미지 2개로 만들어서 "{앞에 그려진 이미지는 한번에 줄어들게} {뒤에 그려진 이미지는 Time.deltaTime 써서 서서히 줄어들게 만들기}"
    void Initialize()
    {
        m_hpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxHp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxHp;
        m_mpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxMp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxMp;
    }

    public void HealthPoint(float value, float MaxHpValue)
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseHp(value, true));
        m_hpText.text = Mathf.CeilToInt(value * MaxHpValue).ToString() + " / " + Mathf.CeilToInt(MaxHpValue).ToString();
        //m_hpUI.fillAmount = value;
    }

    public void ManaPoint(float value, float MaxMpValue)
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseHp(value, false));
        m_mpText.text = Mathf.CeilToInt(value * MaxMpValue).ToString() + " / " + Mathf.CeilToInt(MaxMpValue).ToString();
    }

    IEnumerator DecreaseHp(float value, bool isHp)
    {
        if (isHp)
        {
            while (!Mathf.Approximately(m_hpUI.fillAmount, value))
            {
                m_hpUI.fillAmount = Mathf.Lerp(m_hpUI.fillAmount, value, Time.deltaTime * 2);
                yield return null;
            }
            m_hpUI.fillAmount = value;
        }
        else
        {
            while (!Mathf.Approximately(m_mpUI.fillAmount, value))
            {
                m_mpUI.fillAmount = Mathf.Lerp(m_mpUI.fillAmount, value, Time.deltaTime * 2);
                yield return null;
            }
            m_mpUI.fillAmount = value;
        }
    }
}
