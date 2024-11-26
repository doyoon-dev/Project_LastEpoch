using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using static ItemData;
using TMPro;
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
    public Sprite m_minimapIcon;
    public BattleStat m_stat;
    
    public event UnityAction m_deadAlarm;
    public event UnityAction<float, float, bool> m_changeHp;
    public event UnityAction<float, float, bool> m_changeMp;
    protected IBattle m_target = null;
    public Item m_item;
    public bool m_recoveryCheck = false;
    public Skill m_skillObj;
    public Transform damageTextPosition; 
    public GameObject damageUIPrefab;
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
            m_changeMp?.Invoke(m_curMp / m_stat.MaxMp, m_stat.MaxMp, m_skillObj.m_usingSkill);
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

    public virtual void AttackAnim()
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

 
    // 데미지를 받을 때 호출되는 함수
    public virtual void SetDamage(SkillData skillData)
    {
        
        m_recoveryCheck = false;

        // 데미지 계산
        float damage = Mathf.Max(0, skillData.Dmg * (1 - (m_stat.Defense * 0.01f)));
        m_curHealPoint -= damage;

        // 데미지 텍스트 표시
        ShowDamageText(damage, Color.white);

        // 체력이 0 이하일 때 사망 처리
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            Dead(); // 사망 처리
        }
    }

    public void ShowDamageText(float damage, Color color)
    {
        if (ObjectPool.Inst != null && SceneData.Inst.damageUIPrefab != null && damageTextPosition != null)
        {
            // ObjectPool에서 damageUIPrefab을 가져와서 사용
            GameObject damageUIInstance = ObjectPool.Inst.Pull<DamageUI>(SceneData.Inst.damageUIPrefab, SceneData.Inst.canvasTransform);

            // 초기화 후 위치와 데미지 설정
            DamageUI damageUIScript = damageUIInstance.GetComponent<DamageUI>();
            damageUIScript.ResetState(color);
            // 월드 좌표에서 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(damageTextPosition.position);

            // SceneData에서 가져온 damageUIPrefab을 생성하고 캔버스의 자식으로 설정
            damageUIInstance.transform.position = screenPosition;

            // damageUIInstance에서 DamageUI 스크립트를 가져와서 데미지 값을 설정
            //DamageUI damageUIScript = damageUIInstance.GetComponent<DamageUI>();

            damageUIScript.DMUISetDamage(damage);
            // 데미지 위치 설정
            damageUIScript.DMUISetPosition(damageTextPosition);
            // 색상 설정
            damageUIScript.SetDamageTextColor(color);
            // 일정 시간 후 오브젝트 풀로 되돌리기 (DamageUI 내에서 ReturnToPoolAfter 사용)
            damageUIScript.DestroyAfter(5.0f);
        }
        else
        {
            if (SceneData.Inst.damageUIPrefab == null)
                Debug.LogError("damageUIPrefab이 SceneData에 할당되지 않았습니다.");

            if (damageTextPosition == null)
                Debug.LogError("DamageTextPosition이 할당되지 않았습니다.");
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

    //public void RecoveryManaPoint(bool isUsingSkill)
    //{
    //    //m_recoveryMpCheck = isUsingSkill;
    //    //m_curMagicPoint += manapoint;
    //    //if (m_curMagicPoint >= m_stat.MaxHp)
    //    //{
    //    //    m_curMagicPoint = m_stat.MaxHp;
    //    //}
    //    StopAllCoroutines();
    //    StartCoroutine(ManaPointCoroutine(isUsingSkill));
    //}

    //IEnumerator ManaPointCoroutine(bool isUsingSkill)
    //{
    //    //m_recoveryMpCheck = isUsingSkill;
    //    while (!isUsingSkill && m_curMagicPoint < m_stat.MaxHp)
    //    {
    //        m_curMagicPoint += Time.deltaTime * 10;
    //        if (m_curMagicPoint >= m_stat.MaxHp)
    //        {
    //            m_curMagicPoint = m_stat.MaxHp;
    //            break;
    //        }
    //        yield return null;
    //    }
    //}

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
