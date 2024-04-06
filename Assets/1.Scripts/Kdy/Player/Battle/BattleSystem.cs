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
public class BattleSystem : MovePath
{
    public BattleStat m_stat;
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
    public void OnAttack(Vector3 pos)
    {
        Rotate(pos);
        m_myAnim.SetTrigger("Attack");
    }

    public void Attack()
    {
        m_myAnim.SetTrigger("Attack");
    }

    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void MoveToAttack(Transform target)
    {
        MoveToEnemy(target, m_stat.attackRange, null);
        m_myAnim.SetTrigger("Attack");
    }

    // 데미지 받음
    public void OnDamaged(float damage)
    {
        m_stat.hp -= damage;
    }
}
