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
        //StopAllCoroutines();
        //StartCoroutine(DecreaseHp(value));
        m_hpUI.fillAmount = value;
    }

    IEnumerator DecreaseHp(float value)
    {
        while (!Mathf.Approximately(m_hpUI.fillAmount, value))
        {
            //m_hpUI.fillAmount -= (1 - value) * Time.deltaTime * 0.5f;
            yield return null;
        }
    }
}
