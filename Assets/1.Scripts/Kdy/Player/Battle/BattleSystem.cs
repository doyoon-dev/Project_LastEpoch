using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public struct BattleStat
{
    public float hp;
    public float attackDmg;
    public float attackRange;

}

interface IOnDamaged
{
    void OnDamaged(float damage);
}

interface IDeadAlarm
{
    event Action m_deadAlarm;
}

// 공격하고, 데미지 받는 스크립트
public class BattleSystem : MovePath, IDeadAlarm, IOnDamaged
{
    public BattleStat m_stat;
    public event Action m_deadAlarm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        //Rotate(pos);
        m_myAnim.SetBool("Attack", true);
    }

    public virtual void Attack()
    {
        m_myAnim.SetBool("Attack", true);
    }

    public void Dead()
    {
        m_deadAlarm?.Invoke();
    }

    // 데미지 받음
    public void OnDamaged(float damage)
    {
        m_stat.hp -= damage;
        Debug.Log("현재 피 : " + m_stat.hp);
        if (m_stat.hp <= 0)
        {
            m_stat.hp = 0;
            Dead();
        }
    }

    
}
