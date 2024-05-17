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
        Death1,
        Max
    }
    [SerializeField]
    BehaviourState m_state;
    MoveTween m_moveTween;
    NavMeshAgent m_navAgent;
    MonsterAnimController m_monAnimCtr;
    [SerializeField]
    protected float m_idleDuration = 5f;
    protected float m_idleTime = 0;

    [Header("타겟 인식 범위")]
    [SerializeField]
    protected float m_dectectDist = 10f;
    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist = 3f;
    [Header("임시 플레이어 인식 하는지")]
    [SerializeField]
    Player m_player;
    
    public bool IsDie { get { return m_state == BehaviourState.Death1; } }
    public MonsterAnimController.Motion GetMotion {  get { return m_monAnimCtr.CurrentMotion; } }
    void SetState(BehaviourState state)
    {
        m_state = state;
    }
    //행동 프로세스
    void BehaviourProcess()
    {
        switch(m_state)
        {
            //Idle 상태
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;

                if(m_idleTime > m_idleDuration)
                {
                    SetState(BehaviourState.Chase);
                    m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                    m_idleTime = 0f;
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
                    SetState(BehaviourState.Idle);
                    m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
                    m_idleTime = m_idleDuration - 1f;
                }
                break;

                //경계 상태
            case BehaviourState.Patrol:
                break;
                /*
            case BehaviourState.Damaged:
                break;
            case BehaviourState.Die:
                break;
                */
        }
    }
    //임시 데미지 입었을떄 
    public void SetDamage(Transform attacker, SkillData skillData)
    {
        if(skillData.knockback > 0f)
        {
            var dir = (transform.position - attacker.position).normalized;
            dir.y = 0f;
            var duration = SkillData.MaxKnockbackDuration * (skillData.knockback / SkillData.MaxKnockbackDist);
            m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
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

