using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BattleStat
{
    public float hp;
    public float attackDmg;

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
    public void OnAttack()
    {
        m_myAnim.SetTrigger("Attack");
    }

    // 데미지 받음
    public void OnDamaged(float damage)
    {
        m_stat.hp -= damage;
    }
}
