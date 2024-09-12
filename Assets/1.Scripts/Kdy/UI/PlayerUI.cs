using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public interface IUsingPotionAct
{
    event UnityAction m_usingPotionAct;
}

public class PlayerUI : MonoBehaviour
{
    //public event UnityAction<float> m_usingPotionAct = null;
    public UnityEvent<float> m_usingPotionAct = null;
    public Player m_player;
    public Image m_hpUI;
    public Image m_mpUI;
    public Text m_hpText;
    public Text m_mpText;
    public SkillCoolTime m_skillCoolTime;
    public UsingPotion m_potionFlame;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (m_potionFlame.m_potion != null && m_player.m_curHp < m_player.m_stat.MaxHp)
            {
                m_usingPotionAct?.Invoke(m_potionFlame.m_potion.GetComponent<Item>().m_itemData.recoveryAmount);
                IUsePotion iup = m_potionFlame.GetComponent<IUsePotion>();
                if (iup != null)
                {
                    iup.UsePotion();
                }
            }
        }
    }


    // HP MP UI 이미지 2개로 만들어서 "{앞에 그려진 이미지는 한번에 줄어들게} {뒤에 그려진 이미지는 Time.deltaTime 써서 서서히 줄어들게 만들기}"
    void Initialize()
    {
        m_hpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxHp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxHp;
        m_mpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxMp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxMp;
    }

    public void HealthPoint(float value, float MaxHpValue, bool healCheck)
    {
        if (healCheck)
        {
            Debug.Log(value * 100);
            Recovery(value, true);
            //m_hpText.text = Mathf.CeilToInt(value * MaxHpValue).ToString() + " / " + Mathf.CeilToInt(MaxHpValue).ToString();
            m_hpText.text = (value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
        }
        else
        {
            Debug.Log(value * 100);
            StopAllCoroutines();
            StartCoroutine(DamagedResourcePoint(value, true));
            //m_hpText.text = Mathf.CeilToInt(value * MaxHpValue).ToString() + " / " + Mathf.CeilToInt(MaxHpValue).ToString();
            m_hpText.text = (value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
            //m_hpUI.fillAmount = value;
        }
    }

    public void ManaPoint(float value, float MaxMpValue, bool isUsingSkill)
    {
        if (isUsingSkill)
        {
            StopAllCoroutines();
            StartCoroutine(DamagedResourcePoint(value, false));
            m_mpText.text = Mathf.CeilToInt(value * MaxMpValue).ToString() + " / " + Mathf.CeilToInt(MaxMpValue).ToString();
        }
        else
        {
            Recovery(value, false);
            m_mpText.text = (value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
        }
    }

    // 실제 플레이어의 체력, 마나가 줄도록 만드는 코드 추가해야 함
    IEnumerator DamagedResourcePoint(float value, bool isHp)// value : 현재 데미지를 입은 후 플레이어의 체력
    {
        if (isHp)
        {
            float beforeHp = m_hpUI.fillAmount;
            float hp = m_hpUI.fillAmount - value;
            float val = 0;
            //while (!Mathf.Approximately(m_hpUI.fillAmount, value))
            //{

            //    m_hpUI.fillAmount = Mathf.Lerp(m_hpUI.fillAmount, value, Time.deltaTime * 2);
            //    yield return null;
            //}
            while (val <= hp)
            {
                val += Time.deltaTime * 0.5f;
                // m_hpUI.fillAmount = (데미지를 입기 전 hp) - val;
                m_hpUI.fillAmount = beforeHp - val;
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

    // 수정 필요
    public void Recovery(float value, bool isHp)
    {
        StopAllCoroutines();
        StartCoroutine(RecoveryResourcePoint(value, isHp));
    }
    IEnumerator RecoveryResourcePoint(float value, bool isHp)
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

    // 마나 채우는 함수
    // 스킬 사용했을 때 마나 감소하고, 중지했을 때 차도록 만들기
    IEnumerator RecoveryManaPoint(bool isUsingSkill)
    {
        while (!isUsingSkill || m_mpUI.fillAmount >= 1)
        {
            m_mpUI.fillAmount += Time.deltaTime * 0.5f;
            yield return null;
        }
        m_mpUI.fillAmount = 1;
    }
}
