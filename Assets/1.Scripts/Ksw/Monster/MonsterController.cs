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
    protected float m_detectDist;
    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist;
    [Header("플레이어 인식 ")]
    [SerializeField]
    Player m_player;
    [SerializeField]
    WaypointController m_waypointCtr;
    [Header("아이템 프리팹")]
    [SerializeField]
    private GameObject m_dropItemPrefab;
    [Header("아이템 데이터")]
    [SerializeField]
    private ItemData m_itemData;
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
    private Transform m_attacker;
    private SkillInform m_skillData;

    public bool IsDie { get { return m_state == BehaviourState.Die; } } //죽음 상태인지 체크

    private Transform playerTransform;

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
    //몬스터 맞았을 떄 
    void SetHitColor(float duration)
    {
        if (m_hitColorCoroutine != null)
        {
            StopCoroutine(m_hitColorCoroutine);
            m_hitColorCoroutine = null;
        }
        m_hitColorCoroutine = StartCoroutine(Coroutine_SetHitColor(duration));//동작 여러개 들어올수 있음

    }
    //몬스터 맞았을 때 색상
    IEnumerator Coroutine_SetHitColor(float duration)
    {
        m_mpBlock.SetColor("_RimColor", Color.white);
        m_mpBlock.SetFloat("_RimPower", 1);
        for (int i = 0; i < m_renderers.Length; i++)
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
    //몬스터 사라지는거
    IEnumerator Coroutine_SetDissolve(float duration)
    {
        float time = 0f;
        float result = 0f;
        while (true)
        {
            time += Time.deltaTime;
            result = Mathf.Lerp(-1.5f, 0.7f, time / duration);
            m_mpBlock.SetFloat("_Duration", result);
            for (int i = 0; i < m_renderers.Length; i++)
            {
                m_renderers[i].SetPropertyBlock(m_mpBlock);
            }
            if (time > duration)
            {
                yield break;
            }
            yield return null;
        }

    }

    public void InitMonster(Player player)
    {
        m_player = player;

    }
    // 레이어 마스크 설정 메서드
    public void SetLayerMasks(LayerMask playerMask, LayerMask backgroundMask)
    {
        m_playerMask = playerMask;
        m_BackgroundMask = backgroundMask;
    }

    public void SetMonster(WaypointController waypoint)
    {
        gameObject.SetActive(true);
        m_waypointCtr = waypoint;
        //transform.position = waypoint.transform.position;
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
        //플레이어가 없으면 바로 false 반환
        if(m_player == null)
        {
            return false;
        }
        RaycastHit hit;
        var originPos = transform.position + Vector3.up * 0.5f;
        var targetPos = m_player.transform.position + Vector3.up * 0.5f;
        var dir = targetPos - originPos;
        Debug.DrawRay(originPos, dir.normalized * m_detectDist, Color.green); //레이 쏘는거 확인
        if (Physics.Raycast(originPos, dir.normalized, out hit, m_detectDist, m_playerMask | m_BackgroundMask))
        {
            if ((m_playerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                return true;
            }

        }
        return false;
    }

    protected bool CheckArea(Vector3 target, float area)
    {
        var dist = target - transform.position;
        if (Mathf.Approximately(dist.sqrMagnitude, area) || dist.sqrMagnitude < area)
        {
            return true;
        }
        return false;
    }
    //죽었을 떄 몬스터 드랍 확률 
    void DropItemOnDeath()
    {
        // 드롭 확률 (예: 50%)
        float dropChance = 0.5f;
        if (UnityEngine.Random.value <= dropChance)
        {
            GameObject dropItemObject = ObjectPool.Inst.Pool<DropItem>(m_dropItemPrefab);
            DropItem dropItem = dropItemObject.GetComponent<DropItem>();

            dropItem.Initialize(m_itemData); //드롭할 아이템 데이터 설정
            dropItem.transform.position = transform.position;  // 드롭 위치 설정
            dropItem.gameObject.SetActive(true); // 드롭 아이템 활성화
        }
    }

    public override void SetDamage(Transform attacker, SkillInform skillData)
    {
        if (IsDie) return;  // 이미 죽어있다면 처리하지 않음
        m_curHealPoint -= skillData.Dmg;
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            HandleDeath();
            return;
        }

        // 데미지 처리
        SetState(BehaviourState.Damaged);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        SetHitColor(0.5f);

        // 넉백 처리
        if (skillData.knockback > 0f)
        {
            var dir = (transform.position - attacker.position).normalized;
            dir.y = 0f;
            var duration = SkillData.MaxKnockbackDuration * (skillData.knockback / SkillData.MaxKnockbackDist);
            m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
        }
    }

    // 죽음 상태 처리
    void HandleDeath()
    {
        // 이미 죽은 상태에서 다시 처리하지 않도록 함
        if (IsDie) return;      
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정
        DropItemOnDeath(); // 아이템 드롭                          
        SetState(BehaviourState.Die);
    }



 
    //행동 프로세스
    public virtual void BehaviourProcess()
    {
        switch (m_state)
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
                if (CheckArea(m_player.transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
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
                        if (CheckArea(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
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
                HandleDeath();
                break;

        }

        
    }


     
    void Start()
    {
        Initalize();
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_mpBlock = new MaterialPropertyBlock();
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_RimPower", 10);
        m_moveTween = GetComponent<MoveTween>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        BehaviourProcess();
        
    }



}