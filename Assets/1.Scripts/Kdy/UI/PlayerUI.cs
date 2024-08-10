using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void HealthPoint(float damage)
    {
        float hp = m_player.m_curHp - damage;
        m_hpUI.fillAmount -= (hp / m_player.m_stat.MaxHp);
    }
}
