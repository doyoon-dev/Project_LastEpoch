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
        yield return new WaitForSeconds(0.5f);
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