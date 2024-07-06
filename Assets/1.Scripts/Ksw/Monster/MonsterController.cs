using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{

    public enum BehaviourState
    {
        Idle,
        Attack,
        Chase,
        Patrol,
        Damaged,
        //Die,
        Max
    }
    [SerializeField]
    BehaviourState m_state; //상태
    [Header("타겟 인식 범위")]
    [SerializeField]
    protected float m_dectectDist = 100f;
    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist = 1.5f;
    [Header("플레이어 인식 ")]
    [SerializeField]
    Player m_player;
    [SerializeField]
    WaypointController m_waypointCtr;
    MoveTween m_moveTween;
    NavMeshAgent m_navAgent;
    MonsterAnimController m_monAnimCtr;
    bool m_isPatrol;
    int m_curWaypoint;
    float m_idleDuration = 1f;
    float m_idleTime = 0;
    public LayerMask m_playerMask;
    public LayerMask m_BackgroundMask;

   
    
    //public bool IsDie { get { return m_state == BehaviourState.Die1; } }
    public MonsterAnimController.Motion GetMotion {  get { return m_monAnimCtr.CurrentMotion; } }
    #region Animation Event Methods
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    void AnimEvent_HitFinished()
    {
        SetIdle(0f);
    }
    #endregion

    void SetState(BehaviourState state)
    {
        m_state = state;
    }
    void SetIdleDuration(float duration)//idle 대기시간
    {
        m_idleTime = m_idleDuration - duration;
    }

    void SetIdle(float duration)
    {

        SetState(BehaviourState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);

    }

    //임시 데미지 입었을떄 
    public void SetDamage(Transform attacker, SkillData skillData)
    {
        SetState(BehaviourState.Damaged);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Hit);
        if (skillData.knockback > 0f)
        {
            var dir = (transform.position - attacker.position).normalized;
            dir.y = 0f;
            var duration = SkillData.MaxKnockbackDuration * (skillData.knockback / SkillData.MaxKnockbackDist);
            m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
        }
    }
    bool CanAttack()
    {
        var dist = transform.position - m_player.transform.position;
        if (Mathf.Approximately(dist.sqrMagnitude, Mathf.Pow(m_attackDist, 2f)) || dist.sqrMagnitude < Mathf.Pow(m_attackDist, 2f))
        {
            return true;
        }
        return false;
    }

    bool FindTarget()
    {
        RaycastHit hit;
        var originPos = transform.position + Vector3.up * 0.5f;
        var targetPos = m_player.transform.position + Vector3.up * 0.5f;
        var dir = targetPos - originPos;
        Debug.DrawRay(originPos, dir.normalized * m_dectectDist, Color.green);
        /*
        if (Physics.Raycast(originPos, dir.normalized, out hit, m_dectectDist, 1 << LayerMask.NameToLayer("Background") | 1 << LayerMask.NameToLayer("Player")))
        {
            if (hit.collider.CompareTag("Background"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
        */

        if (Physics.Raycast(originPos, dir.normalized, out hit, m_dectectDist, m_playerMask | m_BackgroundMask))
        {
            if ((m_playerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                return true;
            }
  
        }
        return false;
    }

    //행동 프로세스
    void BehaviourProcess()
    {
        switch(m_state)
        {
            //Idle 상태
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime > m_idleDuration)
                {
                    if (FindTarget())
                    {
                        if (CanAttack())
                        {
                            SetState(BehaviourState.Attack);
                            m_monAnimCtr.Play(MonsterAnimController.Motion.Attack1);
                            return;
                        }
                        else
                        {
                            SetState(BehaviourState.Chase);
                            m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                            m_navAgent.stoppingDistance = m_attackDist;
                            m_idleTime = 0;
                        }
                    }
                    else
                    {
                        SetState(BehaviourState.Patrol);
                        m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                        m_navAgent.stoppingDistance = m_navAgent.radius; //navagent radius만큼 정지

                    }
                }
                break;

            //공격 상태
            case BehaviourState.Attack:
                break;
            //추적 상태
            case BehaviourState.Chase:
                m_navAgent.SetDestination(m_player.transform.position);
                var dist = m_player.transform.position - transform.position;            
                if (Mathf.Approximately(dist.sqrMagnitude, Mathf.Pow(m_navAgent.stoppingDistance, 2f)) || dist.sqrMagnitude < Mathf.Pow(m_navAgent.stoppingDistance, 2f))
                {
                    SetIdle(1f);
                }               
                break;

            //경계 상태
            case BehaviourState.Patrol:
                if (!m_isPatrol)
                {
                    m_isPatrol = true;
                    m_curWaypoint++;
                    if (m_curWaypoint >= m_waypointCtr.m_waypoints.Length)
                    {
                        m_curWaypoint = 0;
                    }
                    m_navAgent.SetDestination(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position);
                }
                else
                {
                    if (FindTarget())
                    {
                        m_isPatrol = false;
                        m_navAgent.ResetPath();
                        SetIdle(0f);
                    }
                    else
                    {
                        dist = transform.position - m_waypointCtr.m_waypoints[m_curWaypoint].transform.position;
                        if (Mathf.Approximately(dist.sqrMagnitude, Mathf.Pow(m_navAgent.stoppingDistance, 2f)) || dist.sqrMagnitude < Mathf.Pow(m_navAgent.stoppingDistance, 2f))
                        {
                            m_isPatrol = false;
                            SetIdle(2f);
                        }
                    }
                }
                break;
                
                //데미지 상태
            case BehaviourState.Damaged:
                break;

                /*
                //죽은 상태
            case BehaviourState.Die:
                break;
                */
        }
    }




    void Start()
    {
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_moveTween = GetComponent<MoveTween>();
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        BehaviourProcess();
    }



}

