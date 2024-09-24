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
    int m_currentAttackIndex;
    int m_curWaypoint;
    public HealthBarUI healthBarUI;
    public HeadHealthBar headHealthBar; // 머리 위 체력바 참조
    public string monsterName;
    public bool m_isPatrol; //patrol 여부확인
    public bool IsDie { get { return m_state == BehaviourState.Die; } } // Die 여부확인
    private Coroutine hideHealthBarCoroutine;


   
    #region Animation Event Methods

    void AnimEvent_Attack()
    {
        SetIdle(0.2f);
    }
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    void AnimEvent_HitFinished()
    {
        SetIdle(0.2f);
    }
    #endregion
    #region SetMethods
    protected void SetState(BehaviourState state)
    {
        m_state = state;
    }
    void SetIdleDuration(float duration)//idle 대기시간
    {
        m_idleTime = m_idleDuration - duration;
    }
    public void SetHeadHealthBar(HeadHealthBar healthBar)
    {
        headHealthBar = healthBar;
    }

    protected virtual void SetIdle(float duration)
    {

        SetState(BehaviourState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);

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
    #endregion

    #region Mon Effect Draw

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
    void OnDrawGizmos() // Gizmos를 사용해 OverlapBox 범위를 시각적으로 확인
    {
        // 몬스터가 죽었을 때는 Gizmo를 그리지 않음
        if (IsDie) return;

        // 감지 범위 (m_detectDist)를 시각적으로 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_detectDist);

        //공격 범위 시각화를 유지
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttackArea.transform.position, boxSize);
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
    #endregion

    #region Mon Attack methods
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
                targetPlayer.SetDamage(SkillDataManager.m_skillDataDic["MonsterDmg"]); // 플레이어에게 데미지 입힘

            }
        }
    }
    // 몬스터 차례대로 공격 모션
    protected void MonAttackCombo()
    {
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
        StartCoroutine(LookAtPlayer()); // 공격 전에 항상 플레이어를 바라보게 함
        // 다음 공격 모션으로 인덱스를 증가시킴 퍼센트 뒤에 숫자를 바꿀수록 공격 모션 추가
        m_currentAttackIndex = (m_currentAttackIndex + 1) % 2; // 0, 1 를 반복
    }


    // 넉백 처리 메서드
    protected void ApplyKnockback(SkillData skillData)
    {
        if (skillData.knockback > 0f)
        {
            var dir = (transform.position - m_player.transform.position).normalized;
            dir.y = 0f; // 수평 방향으로만 넉백 처리
            var duration = SkillDataManager.MaxKnockbackDuration * (skillData.knockback / SkillDataManager.MaxKnockbackDist);
            m_moveTween.Play(transform.position, transform.position + dir * skillData.knockback, duration);
        }
    }
    #endregion

    #region Mon Find Methods

    // 플레이어 방향으로 고개 돌리기 메서드
    protected IEnumerator LookAtPlayer()
    {
        if (m_player == null) yield break;

        Vector3 direction = (m_player.transform.position - transform.position).normalized;
        direction.y = 0f; // 수평면에서만 회전하도록 Y축을 0으로 설정

        //회전 각도 계산
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > 180.0f) angle -= 360.0f;

        float rotDir = Vector3.Dot(transform.right, direction) < 0.0f ? -1.0f : 1.0f;

        while (angle > 0.1f) // 너무 작은 각도일 때는 무시
        {
            float delta = Time.deltaTime * 360.0f * m_moveStat.rotSpeed;
            if (delta > angle) delta = angle;

            angle -= delta;
            transform.Rotate(Vector3.up * rotDir * delta, Space.World);

            yield return null;
        }

    }

    protected bool FindTarget()
    {
        // 플레이어가 없으면 바로 false 반환
        if (m_player == null)
        {
            return false;
        }

        // 감지 범위 내에 플레이어가 있는지 확인
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, m_detectDist, m_playerMask);

        foreach (Collider player in playersInRange)
        {
            Player targetPlayer = player.GetComponent<Player>();
            if (targetPlayer != null)
            {
                detectedPlayer = player.gameObject; // 감지된 플레이어 저장
                StartCoroutine(LookAtPlayer());// 타겟을 찾으면 플레이어 쪽으로 고개 돌리기
                return true; // 타겟을 찾았으면 true 반환
            }
        }

        detectedPlayer = null; // 감지된 플레이어가 없으면 null로 설정
        return false;
    }

    /*
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
    */

    protected bool CheckArea(Vector3 target, float area)
    {
        var dist = target - transform.position;
        if (Mathf.Approximately(dist.sqrMagnitude, area) || dist.sqrMagnitude < area)
        {
            return true;
        }
        return false;
    }

    #endregion




    // 일정 시간이 지나면 체력바를 숨기는 코루틴(몬스터를 떄리고 있다가 몇초 뒤 꺼지게 만들려고)
    private IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 중앙 체력바 숨기기
        healthBarUI.HideHealthBar();

        // 머리 위 체력바가 있을 경우 숨기기
        if (headHealthBar != null)
        {
            headHealthBar.HideHeadHealthBar();
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
            // NavMeshAgent가 활성 상태인지 확인
            if (m_navAgent != null && m_navAgent.isActiveAndEnabled)
            {
                // 플레이어의 현재 위치를 목표로 설정
                m_navAgent.SetDestination(m_player.transform.position);
            }
            else
            {
                // NavMeshAgent가 비활성화되었으면 코루틴 중지
                yield break;
            }
        }
    }

    // 데미지 후 다시 돌아오기
    protected IEnumerator ResumeMovementAfterDamage(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay); // Hit 모션이 끝날 시간을 대기
                                                // NavMeshAgent가 활성화되어 있고, NavMesh 위에 배치되어 있는지 확인
        if (m_navAgent != null && m_navAgent.isOnNavMesh)
        {
            m_navAgent.isStopped = false; // 이동 재개
            SetState(BehaviourState.Patrol); // Patrol 상태로 전환하여 웨이포인트로 돌아가게 함
            m_isPatrol = false; // Patrol을 재시작하도록 설정
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not on a NavMesh or is null.");
        }
    }
   
    public void Initialize(MonsterManager manager, WaypointController waypoint, HealthBarUI healthBar)
    {
        m_manager = manager;
        SetMonster(waypoint);

        healthBarUI = healthBar; // 체력바 초기화


        if (healthBarUI != null)
        {
            healthBarUI.HideHealthBar();  // 체력바를 강제로 숨기기
            healthBarUI.Initialize(monsterName, m_stat.MaxHp);// 몬스터 이름과 최대 체력 설정
        }
        else
        {
            Debug.LogWarning("HealthBarUI is null during initialization.");
        }

    }
   

    public override void SetDamage(SkillData skillData)
    {
        if (IsDie) return;
        if (m_state == BehaviourState.Damaged) return;

        m_curHealPoint -= skillData.Dmg;

       
        if (IsDie) return;  // 이미 죽어있다면 처리하지 않음
        if (m_state == BehaviourState.Damaged) return;  // 중복 Hit 애니메이션 방지

        // 현재 체력 감소
        m_curHealPoint -= skillData.Dmg;

        // MonsterManager에게 현재 공격받고 있는 몬스터를 설정
        MonsterManager.Instance.SetCurrentTargetMonster(this);

        // **머리 위 체력바가 있을 경우 업데이트**
        if (headHealthBar != null)
        {
            headHealthBar.UpdateHeadHealth((int)m_curHealPoint, m_stat.MaxHp); // 머리 위 체력바 갱신
            headHealthBar.ShowHeadHealthBar(); // 머리 위 체력바 활성화
        }
        
        // 중앙 체력바 업데이트
        healthBarUI.UpdateHealth((int)m_curHealPoint, m_stat.MaxHp);

        // 중앙 체력바를 활성화
        healthBarUI.ShowHealthBar();

        // 이미 실행 중인 체력바 숨기기 코루틴이 있다면 정지
        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }
        // 데미지 받음 몬스터를 안 죽였을 떄 10초 후 체력바를 비활성화하는 코루틴 실행
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(10f));
        
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
            // 넉백 처리
            ApplyKnockback(skillData);
            StartCoroutine(ResumeMovementAfterDamage()); // 일반 몬스터는 기본 복귀 시간
        }
    }

    protected virtual void HandleDeath()//죽음 상태 처리
    {
        
        if (IsDie) return;// 이미 죽은 상태에서 다시 처리하지 않도록 함
        m_manager.HandleMonsterDeath(transform.position);// 매니저에게 몬스터가 죽었다고 알림
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과                                                 
        m_navAgent.isStopped = true;  // 네비게이션 에이전트 중지
        // Collider 비활성화 (필요한 Collider를 전부 비활성화)
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정                                                                                       
        SetState(BehaviourState.Die);
        ShutDownHealthBars();
        // 공격 범위 비활성화 (박스가 보이지 않도록 설정)
        AttackArea.SetActive(false);
        
    }

    
    protected void ShutDownHealthBars()//헬스바 전체 숨기기
    {
        // 중앙 체력바 숨기기
        if (healthBarUI != null)
        {
            healthBarUI.HideHealthBar();
        }

        // 머리 위 체력바 숨기기
        if (headHealthBar != null)
        {
            headHealthBar.HideHeadHealthBar();
            headHealthBar.gameObject.SetActive(false);  // 캔버스 자체도 비활성화
        }
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
                            SetIdle(1f);
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