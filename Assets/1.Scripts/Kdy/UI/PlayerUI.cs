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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealthPoint(float value)
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseHp(value, true));
        //m_hpUI.fillAmount = value;
    }

    public void ManaPoint(float value)
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseHp(value, false));
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
