using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : BattleSystem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1) && !m_myAnim.GetBool("IsAttacking"))
        {

        }
    }

    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void MoveToAttack(Transform target)
    {
        MoveToEnemy(target, m_stat.attackRange, Attack);
    }

    // 첫 공격 이후 다음 공격 모션 바뀜
    public void FirstAttack()
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
}
