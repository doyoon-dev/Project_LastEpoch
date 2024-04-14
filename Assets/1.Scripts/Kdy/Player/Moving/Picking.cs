using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Picking : MonoBehaviour
{
    public LayerMask m_moveMask;
    public LayerMask m_enemyMask;
    public UnityEvent<Vector3> m_moveAct;
    public UnityEvent<Vector3> m_attackAct;
    public UnityEvent<Transform> m_moveAttackAct;
    public Animator m_anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_anim.GetBool("IsAttacking"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask | m_enemyMask))
            {
                m_moveAct?.Invoke(hit.point);
            }
        }
        if(Input.GetMouseButtonDown(1) && !m_anim.GetBool("IsAttacking"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask | m_enemyMask))
            {
                // 몬스터 클릭 됐을 때 -> BattleSystem의 MoveToAttack 실행
                if ((1 << hit.transform.gameObject.layer & m_enemyMask) != 0)
                {
                    m_moveAttackAct?.Invoke(hit.transform);
                }
                // 배경 클릭 됐을 때 - 마우스 클릭 방향으로 회전 후 제자리에서 공격
                else
                {
                    m_attackAct?.Invoke(hit.point);
                }
            }
        }
    }
}
