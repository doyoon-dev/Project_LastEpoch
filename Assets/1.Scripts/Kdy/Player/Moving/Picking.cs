using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Picking : MonoBehaviour
{
    public LayerMask m_moveMask;
    public LayerMask m_enemyMask;
    public LayerMask m_itemMask;
    public UnityEvent<Vector3> m_moveAct;
    public UnityEvent<Vector3> m_attackAct;
    public UnityEvent<Transform> m_moveAttackAct;
    public Animator m_anim;
    public Player m_player;
    IUsingSkill m_skillUsed;
    // Start is called before the first frame update
    void Start()
    {
        m_skillUsed = transform.GetComponent<IUsingSkill>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && !m_anim.GetBool("IsAttacking"))
            {
                Debug.Log(m_skillUsed.UsingSkill());
            }
            if (Input.GetMouseButtonDown(0) && !m_anim.GetBool("IsAttacking") && !m_skillUsed.UsingSkill())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask | m_enemyMask))
                {
                    m_moveAct?.Invoke(hit.point);
                }
            }
            if (Input.GetMouseButtonDown(1) && !m_anim.GetBool("IsAttacking") && !m_skillUsed.UsingSkill())
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

        #region 아이템 획득할 때 원래 쓰던 코드
        // 드랍된 아이템 클릭
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_itemMask))
            {
                ICheckDropItem icp = hit.transform.GetComponent<ICheckDropItem>();
                if (icp != null)
                {
                    icp.CheckDropItem(m_player.m_inventory);
                }
            }
        }
        #endregion

        // 획득 아이템 인벤토리에 List에 저장하는 코드 테스트 중
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_itemMask))
        //    {
        //        ICheckDropItemTest icdit = hit.transform.GetComponent<ICheckDropItemTest>();
        //        if (icdit != null)
        //        {
        //            icdit.CheckDropItemTest(m_player.m_inventory);
        //        }
        //    }
        //}
    }
}
