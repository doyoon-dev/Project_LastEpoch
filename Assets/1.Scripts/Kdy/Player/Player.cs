using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : BattleSystem
{
    [SerializeField]
    Transform m_weaponStartPoint;
    [SerializeField]
    Transform m_weaponEndPoint;
    [SerializeField]
    Transform weaponPoint;
    public Inventory m_inventory;
    public LayerMask m_enemyMask;
    int m_clickCnt = 0;
    bool m_isComboCheck = false;
    public int attackDamage = 20;
    public float attackRange = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Initalize();
        m_deadAlarm += () =>
        {
            Debug.Log("죽음");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isComboCheck)
        {
            if (Input.GetMouseButtonDown(1))
            {
                m_clickCnt++;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnDamaged(50);
        }
    }


    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void AttackTarget(Transform target)//(Transform target)
    {
        m_target = target.GetComponent<IBattle>();
        IDeadAlarm alarm = target.GetComponent<IDeadAlarm>();
        if (alarm != null)
        {
            alarm.m_deadAlarm += () =>
            {
                StopAllCoroutines();
                m_target = null;
            };
        }
        //FollowingEnemy(target.position, m_stat.attackRange, null);
        MoveToEnemy(m_target.transform, m_stat.AttackRange, AttackAnim);
    }

    // 첫 공격 이후 다음 공격 모션 바뀜
    void FirstAttack()
    {
        m_myAnim.SetBool("ComboOn", true);
        m_myAnim.SetBool("Attack", false);
    }

    // 두 번째 공격 이후 첫 번째 공격 모션으로 바뀜
    public void SecondAttack()
    {
        m_myAnim.SetBool("ComboOn", false);
        m_myAnim.SetBool("Attack", false);
    }

    public void ComboCheckStart()
    {
        m_isComboCheck = true;
        FirstAttack();
        m_myAnim.SetBool("ComboCheck", false);
        m_clickCnt = 0;
    }

    public void ComboCheckEnd()
    {
        m_isComboCheck = false;
        m_myAnim.SetBool("Attack", false);
        if (m_clickCnt > 0)
        {
            m_myAnim.SetBool("ComboCheck", true);
        }
    }

    public override void OnAttack(Vector3 pos)
    {
        base.OnAttack(pos);
        Collider[] list = Physics.OverlapSphere(m_weaponEndPoint.position, 0.06f, m_enemyMask);
        foreach (Collider col in list)
        {
            // 충돌한 col에 BattleSystem 컴포넌트가 없기 때문에 bat이 null이됨
            // 충돌한 col에 BattleSystem 컴포넌트 넣으면 해결
            IOnDamaged ms = col.GetComponent<IOnDamaged>();
            if (ms != null)
            {
                ms.OnDamaged(m_stat.AttackDmg);
            }
        }

    }
    /* //몬스터 피격 연결코드 임시 (안됨)
    void Attack()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, m_enemyMask))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // 가상의 SkillData 객체를 생성하여 전달
                SkillData skillData = new SkillData { knockback = 5f }; // 실제로는 스킬 데이터 설정
                damageable.SetDamage(transform, skillData);
            }
        }
    }
    */
    
    public override void Attack()
    {
        Collider[] enemy = Physics.OverlapCapsule(m_weaponStartPoint.position, m_weaponEndPoint.position, 0.06f, m_enemyMask);
        Collider[] list = Physics.OverlapSphere(m_weaponEndPoint.position, 0.7f, m_enemyMask);
        foreach (Collider col in list)
        {
            
            // 충돌한 col에 BattleSystem 컴포넌트가 없기 때문에 bat이 null이됨
            // 충돌한 col에 BattleSystem 컴포넌트 넣으면 해결
            IOnDamaged ms = col.GetComponent<IOnDamaged>();
            if (ms != null)
            {
                ms.OnDamaged(m_stat.AttackDmg);
            }
        }
    }
    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(m_weaponEndPoint.position, 0.7f);
    //}

}
