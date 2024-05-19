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
        //Die,
        Max
    }
    [SerializeField]
    BehaviourState m_state; //상태
    [Header("타겟 인식 범위")]
    [SerializeField]
    protected float m_dectectDist = 10f;
    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist = 3f;
    [Header("플레이어 인식 ")]
    [SerializeField]
    Player m_player; 
    MoveTween m_moveTween;
    NavMeshAgent m_navAgent;
    MonsterAnimController m_monAnimCtr;
    float m_idleDuration = 5f;
    float m_idleTime = 0;
   
    
    //public bool IsDie { get { return m_state == BehaviourState.Die1; } }
    public MonsterAnimController.Motion GetMotion {  get { return m_monAnimCtr.CurrentMotion; } }
    #region Animation Event Methods
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
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

                /*
                //경계 상태
            case BehaviourState.Patrol:
                break;
                
                //데미지 상태
            case BehaviourState.Damaged:
                break;
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

