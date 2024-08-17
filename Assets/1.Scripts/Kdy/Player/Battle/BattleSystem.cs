using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public struct BattleStat
{
    public float MaxHp;
    public float MaxMp;
    public float AttackDmg;
    public float AttackRange;
}

public interface IDeadAlarm
{
    event Action m_deadAlarm;
}

// 인터페이스 필요 없어서 지워야 될 수 있음
public interface IUsedSkill
{
    void UsedSkill(float skillMp);
}
public interface IDamageable
{
    void SetDamage(Transform attacker, SkillInform skillData);
}
public interface IOnDamaged
{
    void OnDamaged(float damage);
}

public interface ITransform
{
    Transform transform { get; }
}

public interface IBattle : IOnDamaged, ITransform, IUsedSkill
{

}

// 공격하고, 데미지 받는 스크립트
public class BattleSystem : MovePath, IDeadAlarm, IBattle, IDamageable
{
    public BattleStat m_stat;
    public event Action m_deadAlarm;
    protected IBattle m_target = null;
    public float m_curHp = 0.0f;
    public float m_curMp = 0.0f;
    protected float m_curHealPoint
    {
        get { return m_curHp; }
        set
        {
            m_curHp = Mathf.Clamp(value, 0.0f, m_stat.MaxHp);
        }
    }
    public float m_curMagicPoint
    {
        get { return m_curMp; }
        set
        {
            m_curMp = Mathf.Clamp(value, 0.0f, m_stat.MaxMp);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void Initalize()
    {
        m_curHealPoint = m_stat.MaxHp;
        m_curMagicPoint = m_stat.MaxMp;
    }

    // 공격
    // 마우스 우클릭 한 방향으로 회전 후 공격
    // 몬스터 우클릭(계속 클릭할 때 도) 시 몬스터한테 이동 후 공격 범위 안에 몬스터가 들어오면 공격

    // 제자리 공격
    public virtual void OnAttack(Vector3 pos)
    {
        if (m_myAnim.GetBool("Move")) m_myAnim.SetBool("Move", false);
        Vector3 dir = pos - transform.position;
        dir.Normalize();
        dir.y = 0;
        transform.forward = dir;
        StopAllCoroutines();
        //Rotate(pos);
        m_myAnim.SetBool("Attack", true);
        if(m_target != null) m_target.OnDamaged(m_stat.AttackDmg);
    }

    public void AttackAnim()
    {
        m_myAnim.SetBool("Attack", true);
    }

    public virtual void Attack()
    {
        if (m_target != null) m_target.OnDamaged(m_stat.AttackDmg);
    }

    public void Dead()
    {
        m_deadAlarm?.Invoke();
    }

    // 데미지 받음
    public void OnDamaged(float damage)
    {
        m_curHealPoint -= damage;
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            Dead();
        }
    }

    public virtual void SetDamage(Transform attacker, SkillInform skillData)
    {
        //체력 깎이는거 
        m_curHealPoint -= skillData.Dmg;
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            
        }
    }

    public void UsedSkill(float skillMp)
    {
        m_curMagicPoint -= skillMp;
        if (m_curMagicPoint <= 0)
        {
            m_curMagicPoint = 0;
        }
    }
}
