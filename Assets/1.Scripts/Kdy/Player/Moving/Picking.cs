using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Picking : MonoBehaviour
{
    public GameObject m_clickEffect;
    public LayerMask m_moveMask;
    public LayerMask m_enemyMask;
    public LayerMask m_itemMask;
    public UnityEvent<Vector3, GameObject> m_moveAct;
    public UnityEvent<Vector3> m_attackAct;
    public UnityEvent<Transform> m_moveAttackAct;
    public Animator m_anim;
    public Player m_player;
    IUsingSkill m_skillUsed;
    GameObject m_clickEffectObj;
    Dictionary<string, GameObject> m_effectDic = new Dictionary<string, GameObject>();

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
            if (Input.GetMouseButtonDown(0) && !m_anim.GetBool("IsAttacking") && !m_skillUsed.UsingSkill())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask | m_enemyMask))
                {
                    m_clickEffectObj = ObjectPool.Inst.Pull<GameObject>(m_clickEffect);

                    if (m_effectDic.ContainsKey(m_clickEffect.name))
                    {
                        ObjectPool.Inst.Push<GameObject>(m_effectDic[m_clickEffect.name]);
                        m_effectDic.Remove(m_clickEffect.name);
                    }
                    m_effectDic.Add(m_clickEffect.name, m_clickEffectObj);
                    ISetClickEffect isce = gameObject.GetComponent<ISetClickEffect>();
                    if (isce != null)
                    {
                        isce.SetClickEffect(m_effectDic[m_clickEffect.name]);
                    }
                    m_clickEffectObj.transform.position = hit.point;
                    m_moveAct?.Invoke(hit.point, m_effectDic[m_clickEffect.name]);
                }
            }
            if (Input.GetMouseButtonDown(1) && !m_anim.GetBool("IsAttacking") && !m_skillUsed.UsingSkill())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask | m_enemyMask))
                {
                    // 몬스터 우클릭 됐을 때 -> BattleSystem의 MoveToAttack 실행
                    if ((1 << hit.transform.gameObject.layer & m_enemyMask) != 0)
                    {
                        //SetOffClickEffect();
                        m_moveAttackAct?.Invoke(hit.transform);
                    }
                    // 배경 클릭 됐을 때 - 마우스 클릭 방향으로 회전 후 제자리에서 공격
                    else
                    {
                        //SetOffClickEffect();
                        m_attackAct?.Invoke(hit.point);
                    }
                }
            }
        }

        #region 아이템 획득할 때 원래 쓰던 코드
        // 드랍된 아이템 클릭
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_itemMask))
        //    {
        //        ICheckDropItem icp = hit.transform.GetComponent<ICheckDropItem>();
        //        if (icp != null)
        //        {
        //            icp.CheckDropItem(m_player.m_inventory, m_player.m_playerUI);
        //        }
        //    }
        //}
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

    public void SetOffClickEffect()
    {
        ObjectPool.Inst.Push<GameObject>(m_effectDic[m_clickEffect.name]);
        m_effectDic.Remove(m_clickEffect.name);
    }
}
