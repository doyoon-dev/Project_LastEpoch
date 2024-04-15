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
    public LayerMask m_enemyMask;
    int m_clickCnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            m_clickCnt++;
        }
    }

    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void MoveToAttack(Vector3 target)//(Transform target)
    {
        //MoveToEnemy(target, m_stat.attackRange, Attack);
        FollowingEnemy(target, m_stat.attackRange, Attack);
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
        FirstAttack();
        m_myAnim.SetBool("ComboCheck", false);
        m_clickCnt = 0;
    }

    public void ComboCheckEnd()
    {
        m_myAnim.SetBool("Attack", false);
        if (m_clickCnt > 0)
        {
            m_myAnim.SetBool("ComboCheck", true);
        }
    }

    public override void OnAttack(Vector3 pos)
    {
        base.OnAttack(pos);
        Collider[] list = Physics.OverlapSphere(m_weaponEndPoint.position, 1.0f, m_enemyMask);
        foreach (Collider col in list)
        {
            // 충돌한 col에 BattleSystem 컴포넌트가 없기 때문에 bat이 null이됨
            // 충돌한 col에 BattleSystem 컴포넌트 넣으면 해결
            IOnDamaged ms = col.GetComponent<IOnDamaged>();
            if (ms != null)
            {
                ms.OnDamaged(m_stat.attackDmg);
            }
        }
    }
    int count = 0;
    public override void Attack()
    {
        base.Attack();
        Collider[] enemy = Physics.OverlapCapsule(m_weaponStartPoint.position, m_weaponEndPoint.position, 0.06f, m_enemyMask);
        Collider[] list = Physics.OverlapSphere(m_weaponEndPoint.position, 0.06f, m_enemyMask);
        foreach (Collider col in list)
        {
            
            // 충돌한 col에 BattleSystem 컴포넌트가 없기 때문에 bat이 null이됨
            // 충돌한 col에 BattleSystem 컴포넌트 넣으면 해결
            IOnDamaged ms = col.GetComponent<IOnDamaged>();
            if (ms != null)
            {
                Debug.Log(++count);
                ms.OnDamaged(m_stat.attackDmg);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_weaponEndPoint.position, 0.06f);
    }

}
