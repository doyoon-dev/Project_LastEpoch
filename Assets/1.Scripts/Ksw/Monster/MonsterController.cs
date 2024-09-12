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
    protected BehaviourState m_state; //상태

    [Header("타겟 인식 범위")]
    [SerializeField]
    protected float m_detectDist;

    [Header("공격 거리")]
    [SerializeField]
    protected float m_attackDist;

    [Header("추적 거리")]
    [SerializeField]
    protected float m_chaseDist = 10f;  // 추적을 멈추는 최대 거리

    [Header("웨이포인트")]
    [SerializeField]
    WaypointController m_waypointCtr;

    [Header("공격범위")]
    [SerializeField]
    protected Vector3 boxSize = new Vector3(2f, 2f, 2f);  // 공격 범위 크기 설정
    [SerializeField]
    protected GameObject AttackArea;  // OverlapBox의 중심이 될 오브젝트 (몬스터의 위치)

    protected Player m_player;   
    protected NavMeshAgent m_navAgent;
    protected MonsterAnimController m_monAnimCtr;
    protected MaterialPropertyBlock m_mpBlock;  
    protected float m_idleDuration = 1f;
    protected float m_idleTime = 0;
    protected MonsterManager m_manager;  // 매니저 참조
    protected float lastAttackTime = 0f;
    private MoveTween m_moveTween;
    private Renderer[] m_renderers;
    private GameObject detectedPlayer;
    public LayerMask m_playerMask;
    public LayerMask m_BackgroundMask;
    Transform m_attacker;
    Coroutine m_hitColorCoroutine;
    SkillInform m_skillData;
    int m_currentAttackIndex;
    int m_curWaypoint;
   

    public bool m_isPatrol; //patrol 여부확인
    public bool IsDie { get { return m_state == BehaviourState.Die; } } // Die 여부확인

    private Transform playerTransform;

    public MonsterAnimController.Motion GetMotion { get { return m_monAnimCtr.CurrentMotion; } }// 어느 포인트를 가고 있는지 체크
    #region Animation Event Methods

    void AnimEvent_Attack()
    {

    }
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    void AnimEvent_HitFinished()
    {
        SetIdle(0f);
    }
    #endregion

    protected void SetState(BehaviourState state)
    {
        m_state = state;
    }
    void SetIdleDuration(float duration)//idle 대기시간
    {
        m_idleTime = m_idleDuration - duration;
    }

    public void Initialize(MonsterManager manager, WaypointController waypoint)
    {
        m_manager = manager;
        SetMonster(waypoint);
    }
    protected virtual void SetIdle(float duration)
    {

        SetState(BehaviourState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);

    }
    //몬스터 맞았을 떄 
    protected void SetHitColor(float duration)
    {
        if (m_hitColorCoroutine != null)
        {
            StopCoroutine(m_hitColorCoroutine);
            m_hitColorCoroutine = null;
        }
        m_hitColorCoroutine = StartCoroutine(Coroutine_SetHitColor(duration));//동작 여러개 들어올수 있음

    }
    //몬스터 맞았을 때 색상
    protected IEnumerator Coroutine_SetHitColor(float duration)
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
    protected IEnumerator Coroutine_SetDissolve(float duration)
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

    protected IEnumerator DamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 일정 시간 대기 후
        Attack();  // 공격 실행
    }

    protected IEnumerator Coroutine_CalculateTargetPath(int frame)
    {
        while (m_state == BehaviourState.Chase) // 몬스터의 상태가 Chase일 때 계속 반복
        {
            for (int i = 0; i < frame; i++) // 'frame' 만큼의 프레임 동안 대기
            {
                yield return null; // 한 프레임 대기 (코루틴을 일시 중지하고 다음 프레임까지 기다림)
            }
            m_navAgent.SetDestination(m_player.transform.position); // 플레이어의 현재 위치를 목표로 설정
        }
    }

    // 데미지 후 다시 돌아오기
    protected IEnumerator ResumeMovementAfterDamage(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay); // Hit 모션이 끝날 시간을 대기
        m_navAgent.isStopped = false; // 이동 재개
        SetState(BehaviourState.Patrol); // Patrol 상태로 전환하여 웨이포인트로 돌아가게 함
        m_isPatrol = false; // Patrol을 재시작하도록 설정
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

    }

    public bool CanAttack()
    {
        // 타겟과의 거리 계산 및 공격 범위 확인
        var dist = transform.position - m_player.transform.position;
        return dist.sqrMagnitude < Mathf.Pow(m_attackDist, 2f);
    }

    // OverlapBox를 사용한 공격 범위 내 적 감지 및 데미지 입히기
    public override void Attack()
    {


        Collider[] playersInRange = Physics.OverlapBox(AttackArea.transform.position, boxSize * 0.5f, gameObject.transform.rotation, m_playerMask);

        foreach (Collider player in playersInRange)
        {
            Player targetPlayer = player.GetComponent<Player>();
            if (targetPlayer != null)
            {
                targetPlayer.OnDamaged(m_stat.AttackDmg); // 플레이어에게 데미지 입힘
                                                          // 만약 스킬 데미지로 데미지 입히고 싶으면 (m_skillData.AttackDmg)교체
            }
        }
    }



    void OnDrawGizmos() // Gizmos를 사용해 OverlapBox 범위를 시각적으로 확인
    {
        // 몬스터가 죽었을 때는 Gizmo를 그리지 않음
        if (IsDie) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttackArea.transform.position, boxSize);
    }


    protected bool FindTarget()
    {
        //플레이어가 없으면 바로 false 반환
        if (m_player == null)
        {
            return false;
        }

        RaycastHit hit;

        // 보스 몬스터는 레이를 더 높은 곳에서 쏘도록 하고, 일반 몬스터는 낮게 설정
        float rayHeight = this is BossMonster ? 2.0f : 0.5f;  // 보스는 높이를 2.0f, 일반 몬스터는 0.5f로 설정
        var originPos = transform.position + Vector3.up * rayHeight;  // 레이 시작점
        var targetPos = m_player.transform.position + Vector3.up * 1.0f;  // 플레이어 목표점 (머리 부분)

        var dir = targetPos - originPos;

        Debug.DrawRay(originPos, dir.normalized * m_detectDist, Color.green); //레이 쏘는거 확인

        if (Physics.Raycast(originPos, dir.normalized, out hit, m_detectDist, m_playerMask | m_BackgroundMask))
        {
            if ((m_playerMask & (1 << hit.collider.gameObject.layer)) != 0)
            {
                detectedPlayer = hit.collider.gameObject;
                return true;
            }
        }

        detectedPlayer = null;
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

    public override void SetDamage(Transform attacker, SkillInform skillData)
    {
        if (IsDie) return;  // 이미 죽어있다면 처리하지 않음
        if (m_state == BehaviourState.Damaged) return;  // 중복 Hit 애니메이션 방지

        // 현재 체력 감소
        m_curHealPoint -= skillData.Dmg;
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            HandleDeath();  // 체력이 0이 되면 사망 처리
            return;
        }

        // 보스 몬스터는 Hit 모션 시간을 짧게 설정
        if (this is BossMonster)
        {
            SetHitColor(0.2f);  // 보스가 0.2초간 흰색으로 변함                                                           
        }
        else
        {
            // 일반 몬스터의 데미지 처리 (기존 로직)
            SetState(BehaviourState.Damaged);
            m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
            m_navAgent.ResetPath();
            m_navAgent.isStopped = true;  // 데미지 받았을 때 이동 중지
            SetHitColor(0.2f);  // 일반 몬스터는 0.5초 Hit 모션

            // 넉백 처리 (일반 몬스터)
            if (skillData.knockback > 0f)
            {
                var dir = (transform.position - attacker.position).normalized;
                dir.y = 0f;
                var duration = SkillData.MaxKnockbackDuration * (skillData.knockback / SkillData.MaxKnockbackDist);
                m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
            }

            StartCoroutine(ResumeMovementAfterDamage()); // 일반 몬스터는 기본 복귀 시간
        }
    }

    void DamageToPlayer()
    {
        if (detectedPlayer != null)
        {
            Player player = detectedPlayer.GetComponent<Player>(); // 플레이어의 Player 컴포넌트를 가져오기

            if (player != null)
            {
                player.OnDamaged(m_skillData.Dmg); // 플레이어에게 데미지를 입힘
            }
        }
    }



    protected virtual void HandleDeath()//죽음 상태 처리
    {
        // 이미 죽은 상태에서 다시 처리하지 않도록 함
        if (IsDie) return;
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정                                                                                       
        m_manager.HandleMonsterDeath(transform.position);// 매니저에게 몬스터가 죽었다고 알림
        SetState(BehaviourState.Die);
        // 공격 범위 비활성화 (박스가 보이지 않도록 설정)
        AttackArea.SetActive(false);
    }

    //몬스터 차례대로 공격 모션
    protected void MonAttackCombo()
    {
        //플레이어 방향쪽으로 헤드 돌리기
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime); // 회전 속도를 360도로 설정 (필요에 따라 조절 가능)

        // m_currentAttackIndex에 따라 공격 모션 선택
        switch (m_currentAttackIndex)
        {
            case 0:
                m_monAnimCtr.Play(MonsterAnimController.Motion.Attack1);
                StartCoroutine(DamageAfterDelay(0.5f)); // 첫 공격 후 0.5초 후 데미지 적용
                break;
            case 1:
                m_monAnimCtr.Play(MonsterAnimController.Motion.Attack2);
                StartCoroutine(DamageAfterDelay(0.7f)); // 두 번째 공격 후 0.7초 후 데미지 적용
                break;
        }

        // 다음 공격 모션으로 인덱스를 증가시킴 퍼센트 뒤에 숫자를 바꿀수록 공격 모션 추가
        m_currentAttackIndex = (m_currentAttackIndex + 1) % 2; // 0, 1 를 반복
    }




    //행동 프로세스
    public virtual void BehaviourProcess()
    {
        switch (m_state)
        {
            //Idle 상태
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime > m_idleDuration * 0.5f)
                {
                    if (FindTarget())
                    {
                        // 타켓을 찾아 공격 가능하면 공격
                        if (CanAttack())
                        {
                            SetState(BehaviourState.Attack);
                            MonAttackCombo();
                            lastAttackTime = Time.time;  // 공격 시간 갱신
                            return;
                        }
                        // 타켓을 찾아 공격 가능하지 않으면 쫓아가기
                        else
                        {
                            SetState(BehaviourState.Chase);
                            StartCoroutine(Coroutine_CalculateTargetPath(30));
                            m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
                            m_navAgent.stoppingDistance = m_attackDist;
                            m_idleTime = 0;
                        }
                    }
                    // 타켓을 못 찾는 경우 
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



    protected virtual void Start()
    {
        Initalize();
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_mpBlock = new MaterialPropertyBlock();
        m_mpBlock.SetColor("_RimColor", Color.black);
        m_mpBlock.SetFloat("_RimPower", 10);
        m_moveTween = GetComponent<MoveTween>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_renderers = GetComponentsInChildren<Renderer>();
        m_player = FindObjectOfType<Player>();

    }

    void Update()
    {
        BehaviourProcess();

    }



}