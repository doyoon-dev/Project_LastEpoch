using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecoveryManaPoint
{
    void RecoveryManaPoint(bool isUsingSkill);
}

public class RecoverResource : MonoBehaviour, IRecoveryManaPoint
{
    [SerializeField]
    Player m_player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecoveryHealPoint(float healpoint)
    {
        m_player.m_recoveryCheck = true;
        m_player.m_curHealPoint += healpoint;
        if (m_player.m_curHealPoint >= m_player.m_stat.MaxHp)
        {
            m_player.m_curHealPoint = m_player.m_stat.MaxHp;
        }
    }

    public void RecoveryManaPoint(bool isUsingSkill)
    {
        StopAllCoroutines();
        StartCoroutine(ManaPointCoroutine(isUsingSkill));
    }

    IEnumerator ManaPointCoroutine(bool isUsingSkill)
    {
        //m_recoveryMpCheck = isUsingSkill;
        while (!isUsingSkill && m_player.m_curMagicPoint < m_player.m_stat.MaxHp)
        {
            m_player.m_curMagicPoint += Time.deltaTime * 10;
            if (m_player.m_curMagicPoint >= m_player.m_stat.MaxHp)
            {
                m_player.m_curMagicPoint = m_player.m_stat.MaxHp;
                break;
            }
            yield return null;
        }
    }
}