using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BattleSystem
{

    public enum BehaviourState
    {
        Idle,
        Attack,
        Chase,
        Patrol,
        Damaged,
        Die,
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
    [Header("임시 몬스터 체력 ")]
    //[SerializeField]
   //int m_hp = 10;
    MoveTween m_moveTween;
    NavMeshAgent m_navAgent;
    MonsterAnimController m_monAnimCtr;
    Renderer[] m_renderers;
    bool m_isPatrol; //patrol 여부확인
    int m_curWaypoint;
    float m_idleDuration = 1f;
    float m_idleTime = 0;
    Coroutine m_hitColorCoroutine;
    MaterialPropertyBlock m_mpBlock;
    public LayerMask m_playerMask;
    public LayerMask m_BackgroundMask;
    public bool IsDie {get { return m_state == BehaviourState.Die; } }
   
    public MonsterAnimController.Motion GetMotion { get { return m_monAnimCtr.CurrentMotion; } }// 어느 포인트를 가고 있는지 체크
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
    void SetHitColor(float duration)
    {
        if(m_hitColorCoroutine != null)
        {
            StopCoroutine(m_hitColorCoroutine);
            m_hitColorCoroutine = null;
        }
        m_hitColorCoroutine = StartCoroutine(Coroutine_SetHitColor(duration));//동작 여러개 들어올수 있음

    }
    IEnumerator Coroutine_SetHitColor(float duration)
    {
        m_mpBlock.SetColor("_RimColor", Color.white);
        m_mpBlock.SetFloat("_RimPower", 1);
        for(int i = 0; i< m_renderers.Length; i++) 
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
        yield return new WaitForSeconds(duration);
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_RimPower", 10);
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].SetPropertyBlock(m_mpBlock);
        }
    }
   
    IEnumerator Coroutine_SetDissolve(float duration)
    {
        float time = 0f;
        float result = 0f;
        while(true)
        {
            time += Time.deltaTime;
            result = Mathf.Lerp(-1.5f, 0.7f, time / duration);
            m_mpBlock.SetFloat("_Duration",result);
            for (int i = 0; i < m_renderers.Length; i++)
            {
                m_renderers[i].SetPropertyBlock(m_mpBlock);
            }           
            if(time > duration)
            {
                yield break;
            }
            yield return null;
        }

    }
    //데미지 입었을떄 
    public void SetDamage(Transform attacker, SkillData skillData)
    {
        /*
        m_hp--;
        if(m_hp <=0)
        {
            if (IsDie) return;
            m_hp = 0;
            SetState(BehaviourState.Die);
            m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);
            StartCoroutine(Coroutine_SetDissolve(4f));
            return;
        }
        */
        SetState(BehaviourState.Damaged);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        SetHitColor(0.5f);
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
                        SetIdle(1f);
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
            //죽은 상태  
            case BehaviourState.Die:
                break;
                 
        }
    }




    void Start()
    {
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_mpBlock = new MaterialPropertyBlock();
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_RimPower", 10);
        m_moveTween = GetComponent<MoveTween>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_renderers = GetComponentsInChildren<Renderer>();
        /*
        Initalize();
        IDeadAlarm da = GetComponent<IDeadAlarm>();
        if (da != null)
        {
            da.m_deadAlarm += () =>
            {
                gameObject.SetActive(false);
            };
        }
        */
    }

    void Update()
    {
        BehaviourProcess();
    }



}

