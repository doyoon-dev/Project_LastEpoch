using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static ItemData;

[System.Serializable]
public struct BattleStat
{
    public float MaxHp;
    public float MaxMp;
    public float AttackDmg;
    public float AttackRange;
    public float Defense;
}

public interface IDeadAlarm
{
    event UnityAction m_deadAlarm;
}

// 인터페이스 필요 없어서 지워야 될 수 있음
public interface IUsedSkill
{
    void UsedSkill(float skillMp);
}
public interface IDamageable
{
    void SetDamage(SkillData skillData);
}

public interface ITransform
{
    Transform transform { get; }
}

public interface IEquipItemSetting
{
    void EquipItemSetting(Item item);
}

public interface ISetStatus
{
    void SetStatus(ItemData itemData, bool equip);
}

public interface IBattle : ITransform, IUsedSkill, IEquipItemSetting, ISetStatus, IDamageable
{

}

// 공격하고, 데미지 받는 스크립트
public class BattleSystem : MovePath, IDeadAlarm, IBattle
{
    public BattleStat m_stat;
    public event UnityAction m_deadAlarm;
    public event UnityAction<float, float, bool> m_changeHp;
    public event UnityAction<float, float, bool> m_changeMp;
    protected IBattle m_target = null;
    public Item m_item;
    

    bool m_recoveryCheck = false;

    public float m_curHp = 0.0f;
    public float m_curMp = 0.0f;
    public float m_curHealPoint
    {
        get { return m_curHp; }
        set
        {
            m_curHp = Mathf.Clamp(value, 0.0f, m_stat.MaxHp);
            m_changeHp?.Invoke(m_curHp / m_stat.MaxHp, m_stat.MaxHp, m_recoveryCheck);
        }
    }
    public float m_curMagicPoint
    {
        get { return m_curMp; }
        set
        {
            m_curMp = Mathf.Clamp(value, 0.0f, m_stat.MaxMp);
            m_changeMp?.Invoke(m_curMp / m_stat.MaxMp, m_stat.MaxMp, true);
        }
    }
    protected float m_curDamage
    {
        get { return m_stat.AttackDmg; }
        set
        {
            m_stat.AttackDmg = value;
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

    public virtual void Initalize()
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
        if(m_target != null) m_target.SetDamage(SkillDataManager.m_skillDataDic["Normal"]);
    }

    public void AttackAnim()
    {
        m_myAnim.SetBool("Attack", true);
    }

    public virtual void Attack()
    {
        if (m_target != null) m_target.SetDamage(SkillDataManager.m_skillDataDic["Normal"]);
    }

    public void Dead()
    {
        m_deadAlarm?.Invoke();
    }

    /*
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
    */



    public virtual void SetDamage(SkillData skillData)
    {
        m_recoveryCheck = false;
        if (m_stat.Defense > 0)
        {

        }
        // 체력 깎이는 로직
        m_curHealPoint -= skillData.Dmg;
        
        // 체력이 0 이하일 때 처리
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            Dead(); // 사망 처리
        }
    }

    public void RecoveryHealPoint(float healpoint)
    {
        m_recoveryCheck = true;
        m_curHealPoint += healpoint;
        if (m_curHealPoint >= m_stat.MaxHp)
        {
            m_curHealPoint = m_stat.MaxHp;
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

    // Item 스크림트에서 장비 장착했을 때 이벤트 함수에 추가할 함수
    public void SetStatus(ItemData itemData, bool equip)
    {
        //m_curDamage += itemData.atkPower;
        // 데미지와 방어력은 나중에 계산식 만들면 프로퍼티로 바꿔서 적용되게 만들 예정
        if (equip)
        {
            m_stat.AttackDmg += itemData.atkPower;
            m_stat.Defense += itemData.defense;
        }
        else
        {
            m_stat.AttackDmg -= itemData.atkPower;
            m_stat.Defense -= itemData.defense;
        }
        //Debug.Log("      공격력 :   " + m_stat.AttackDmg + "      방어력 :   " + m_stat.Defense);
    }

    public void EquipItemSetting(Item item)
    {
        m_item = item;
        switch (item.m_itemData.itemType)
        {
            case ItemType.Head:
                
                break;
            case ItemType.Necklace:
                
                break;
            case ItemType.Weapon:
                
                break;
            case ItemType.Armor:
                
                break;
            case ItemType.Sheild:
                
                break;
            case ItemType.Belt:
                
                break;
            case ItemType.Ring:
                
                break;
            case ItemType.Shoes:
                
                break;
            case ItemType.Hand:
                
                break;
        }
    }
}
