using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public interface IRecoveryMP
{
    void RecoveryMP(bool isUsingSkill);
}

// 현재 안씀
public interface IUsingPotionAct
{
    event UnityAction m_usingPotionAct;
}

public class PlayerUI : MonoBehaviour, IRecoveryMP
{
    //public event UnityAction<float> m_usingPotionAct = null;
    public UnityEvent<float> m_usingPotionAct = null;
    public Player m_player;
    public Image m_hpUI;
    public Image m_mpUI;
    //public Text m_hpText;
    //public Text m_mpText;
    public TextMeshProUGUI m_hpText;
    public TextMeshProUGUI m_mpText;
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
        //m_hpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxHp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxHp;
        //m_mpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxMp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxMp;

        m_hpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxHp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxHp;
        m_mpText.text = m_player.GetComponent<BattleSystem>().m_stat.MaxMp + " / " + m_player.GetComponent<BattleSystem>().m_stat.MaxMp;
    }

    // 플레이어의 체력이 회복, 감소함에 따라 함수 실행
    public void HealthPoint(float value, float MaxHpValue, bool healCheck)
    {
        // 체력 회복 UI 함수 실행
        if (healCheck)
        {
            RecoveryHp(value);
            //m_hpText.text = (value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
            m_hpText.text = Mathf.FloorToInt(value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
        }
        // 체력 감소 UI 함수 실행
        else
        {
            StopAllCoroutines();
            StartCoroutine(DamagedHealPoint(value));
            //m_hpText.text = (value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
            m_hpText.text = Mathf.FloorToInt(value * MaxHpValue).ToString() + " / " + (MaxHpValue).ToString();
        }
    }

    // 플레이어의 마나가 회복, 감소함에 따라 함수 실행
    // -> Player 스크립트 m_changeMp 이벤트 함수에서 m_curMagicPoint 변화하면 실행
    public void ManaPoint(float value, float MaxMpValue, bool isUsingSkill)
    {
        // 마나 감소 UI 함수 실행
        if (isUsingSkill)
        {
            StopAllCoroutines();
            StartCoroutine(UsingManaPoint(value));
            //m_mpText.text = (value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
            //m_mpText.text = Mathf.FloorToInt(value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
            m_mpText.text = Mathf.FloorToInt(value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
        }
        // 마나 회복 UI 함수 실행
        else
        {
            RecoveryMP(isUsingSkill);
            //m_mpText.text = (value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
            //m_mpText.text = Mathf.FloorToInt(value * MaxMpValue).ToString() + " / " + (MaxMpValue).ToString();
        }
    }


    #region UI 자원 감소 함수
    // 플레이어의 체력 UI 감소
    IEnumerator DamagedHealPoint(float value)// value : 현재 데미지를 입은 후 플레이어의 체력 || 스킬 사용 마나
    {
        float beforeHp = m_hpUI.fillAmount;
        float hp = m_hpUI.fillAmount - value;
        float val = 0;
        while (val <= hp)
        {
            val += Time.deltaTime * 0.5f;
            m_hpUI.fillAmount = beforeHp - val;
            yield return null;
        }

        m_hpUI.fillAmount = value;
    }

    // 플레이어 마나 UI 감소
    IEnumerator UsingManaPoint(float value)
    {
        float beforeMp = m_mpUI.fillAmount;
        float mp = m_mpUI.fillAmount - value;
        float val = 0;
        while (val <= mp)
        {
            val += Time.deltaTime * 0.5f;
            m_mpUI.fillAmount = beforeMp - val;
            yield return null;
        }
        m_mpUI.fillAmount = value;
    }
    #endregion


    #region UI 자원 회복 함수
    // 체력 회복 UI
    public void RecoveryHp(float value)
    {
        StopAllCoroutines();
        StartCoroutine(RecoveryHealPoint(value));
    }
    // 체력 회복 UI 코루틴
    IEnumerator RecoveryHealPoint(float value)
    {
        while (!Mathf.Approximately(m_hpUI.fillAmount, value))
        {
            m_hpUI.fillAmount = Mathf.Lerp(m_hpUI.fillAmount, value, Time.deltaTime * 2);
            yield return null;
        }
        m_hpUI.fillAmount = value;
    }


    // 마나 회복 UI
    public void RecoveryMP(bool isUsingSkill)
    {
        // 체력 회복 문제있으면 StopCoroutine(RecoveryManaPoint(isUsingSkill)); 로 바꾸기
        StopAllCoroutines();
        StartCoroutine(RecoveryManaPoint(isUsingSkill));
    }

    // 마나 회복 UI 코루틴
    IEnumerator RecoveryManaPoint(bool isUsingSkill)
    {
        while (!isUsingSkill && m_mpUI.fillAmount < 1)
        {
            m_mpUI.fillAmount += Time.deltaTime * 0.1f;
            //m_mpText.text = Mathf.FloorToInt(m_onRecoverMana.fillAmount * m_player.m_stat.MaxMp).ToString() + " / " + (m_player.m_stat.MaxMp).ToString();
            m_mpText.text = Mathf.FloorToInt(m_mpUI.fillAmount * m_player.m_stat.MaxMp).ToString() + " / " + (m_player.m_stat.MaxMp).ToString();
            yield return null;
        }
        if (m_mpUI.fillAmount >= 1)
        {
            m_mpUI.fillAmount = 1;
            //m_mpText.text = Mathf.FloorToInt(m_onRecoverMana.fillAmount * m_player.m_stat.MaxMp).ToString() + " / " + (m_player.m_stat.MaxMp).ToString();
            m_mpText.text = Mathf.FloorToInt(m_mpUI.fillAmount * m_player.m_stat.MaxMp).ToString() + " / " + (m_player.m_stat.MaxMp).ToString();
        }
    }
    #endregion

    public void MPUI(float amount)
    {
        m_mpUI.fillAmount = amount / m_player.m_stat.MaxMp;
        m_mpText.text = Mathf.FloorToInt(m_mpUI.fillAmount * m_player.m_stat.MaxMp).ToString() + " / " + (m_player.m_stat.MaxMp).ToString();
    }
}
