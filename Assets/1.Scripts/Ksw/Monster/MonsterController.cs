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
    protected float m_chaseDist;  // 추적 거리

    [Header("웨이포인트")]
    [SerializeField]
    WaypointController m_waypointCtr;

    [Header("공격범위")]
    [SerializeField]
    protected Vector3 boxSize = new Vector3(2f, 2f, 2f);  // 공격 범위 크기 설정
    [SerializeField]
    protected GameObject AttackArea;  // OverlapBox의 중심이 될 오브젝트 (몬스터의 위치)

    [Header("피흘리는 이펙트 프리팹")]
    [SerializeField]
    public GameObject bleedEffectPrefab; // 피흘리는 이펙트 프리팹

    public GameObject bloodSpletterPrefab;


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
    protected Coroutine hideHealthBarCoroutine;

    #region Blank Methods

    /* //레이캐스트로 타켓 찾는법
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
    #endregion
    #region Animation Event Methods

    void AnimEvent_Attack()
    {
        //Attack();
    }
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    void AnimEvent_HitFinished()
    {
        SetIdle(1f);
    }
    #endregion
    #region Mon Effect Methods

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
    void OnDrawGizmos() // Gizmos를 사용해 OverlapBox 범위를 시각적으로 확인
    {
        // 몬스터가 죽었을 때는 Gizmo를 그리지 않음
        if (IsDie) return;

        // 감지 범위 (m_detectDist)를 시각적으로 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_detectDist);

        //공격 범위 시각화를 유지
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackArea.transform.position, m_attackDist);

        // 추적 범위 (m_chaseDist)를 시각적으로 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_chaseDist);

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

    public void SetMonster(WaypointController waypoint)
    {
        gameObject.SetActive(true);
        m_waypointCtr = waypoint;

    }
    #endregion

 
    #region Mon Attack Methods
   
    public bool CanAttack()
    {
        // 타겟과의 거리 계산 및 공격 범위 확인
        var dist = transform.position - m_player.transform.position;

        // 항상 플레이어를 바라보도록 설정 (거리와 상관없이)
        StartCoroutine(LookAtPlayer());

        // 보스 몬스터일 경우 시야각을 적용
        if (this is BossMonster)
        {
            Vector3 directionToPlayer = (m_player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float maxViewAngle = 90f;

            // 공격 범위 안에 있고, 시야각 내에 있을 때만 공격 가능
            if (dist.sqrMagnitude < Mathf.Pow(m_attackDist, 2f) && angleToPlayer <= maxViewAngle)
            {
                return true;
            }
        }
        // 일반 몬스터는 시야각을 적용하지 않음
        else
        {
            // 공격 범위 안에 있을 때만 공격 가능
            if (dist.sqrMagnitude < Mathf.Pow(m_attackDist, 2f))
            {
                // 몬스터와 플레이어 사이의 방향을 계산
                Vector3 directionToPlayer = (m_player.transform.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                // 플레이어가 몬스터의 정면(예: 60도 이내)에 있을 때만 공격 가능
                float maxViewAngle = 60f;
                if (angleToPlayer <= maxViewAngle)
                {
                    return true;
                }
            }
        }
        return false;
    }


    // OverlapBox를 사용한 공격 범위 내 적 감지 및 데미지 입히기
    public override void Attack()
    {
        Collider[] playersInRange = Physics.OverlapSphere(AttackArea.transform.position, m_attackDist, m_playerMask);

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
        m_currentAttackIndex = (m_currentAttackIndex + 1) % 2; // 0, 1 를 반복
    }


    #endregion
    #region Mon Damage Methods

    protected IEnumerator DamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 일정 시간 대기 후
        Attack();  // 공격 실행
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

    public override void SetDamage(SkillData skillData)
    {
        if (IsDie) return;
        if (m_state == BehaviourState.Damaged) return;

        // 피 흘리는 이펙트 소환
        SpawnBleedEffect(transform.position);

        // 데미지 값 확인
        float damage = skillData.Dmg;

        // 체력 감소 처리
        m_curHealPoint -= damage;

        // MonsterManager에게 현재 공격받고 있는 몬스터를 설정
        MonsterManager.Instance.SetCurrentTargetMonster(this);

        // 몬스터는 흰색 데미지 텍스트 표시
        ShowDamageText(damage, Color.white);  // 색상으로 흰색 전달

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
        // 데미지 받음 몬스터를 안 죽였을 떄 n초 후 체력바를 비활성화하는 코루틴 실행
        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(3f));

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
            StartCoroutine(ResumeMovementAfterDamage());
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

        float rotationSpeed = 1.0f; // 회전 속도

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 현재 회전에서 목표 회전까지 부드럽게 회전
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;  // 최종적으로 완전히 회전시킴

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
                //StartCoroutine(LookAtPlayer());// 타겟을 찾으면 플레이어 쪽으로 고개 돌리기
                return true; // 타겟을 찾았으면 true 반환
            }
        }

        detectedPlayer = null; // 감지된 플레이어가 없으면 null로 설정
        return false;
    }


    protected bool CheckArea(Vector3 target, float area)
    {
        var dist = target - transform.position;
        if (dist.sqrMagnitude < Mathf.Pow(area, 2f))  // 공격 범위와 추적 거리 조건이 더 잘 맞도록 설정
        {
            return true;
        }
        return false;
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

    #endregion
    public void Initialize(MonsterManager manager, WaypointController waypoint, HealthBarUI healthBar, GameObject damageUIPrefab)
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
        // 데미지 UI 프리팹 설정
        this.damageUIPrefab = damageUIPrefab;

        if (this.damageUIPrefab == null)
        {
            Debug.LogWarning("DamageUIPrefab is not assigned during initialization.");
        }
    }

    // 피 흘리는 이펙트
    public void SpawnBleedEffect(Vector3 position)
    {
        // 보스 몬스터일 경우 높이를 추가
        if (this is BossMonster)
        {
            position.y += 2.0f;  
        }

        // ObjectPool에서 피 이펙트 가져오기
        GameObject bleedEffect = ObjectPool.Inst.Pull<GameObject>(bleedEffectPrefab);
        bleedEffect.transform.position = position;

        // 일정 시간이 지나면 피 이펙트를 다시 풀로 반환
        StartCoroutine(ReturnBleedEffectToPool(bleedEffect, 2.0f)); // 2초 뒤 반환
    }
    // 피 이펙트를 다시 풀로 반환하는 코루틴
    public IEnumerator ReturnBleedEffectToPool(GameObject bleedEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPool.Inst.Push<GameObject>(bleedEffect); // 피 이펙트를 풀에 반환
    }

    // 핏자국 이펙트
    public void SpawnBloodSplatter(Vector3 position)
    {
        GameObject bloodSpletter = ObjectPool.Inst.Pull<GameObject>(bloodSpletterPrefab);
        bloodSpletter.transform.position = position;

        StartCoroutine(ReturnBloodSplatter(bloodSpletter, 5.0f));
    }
    // 핏 자국을 다시 풀로 반환하는 코루틴 
    public IEnumerator ReturnBloodSplatter(GameObject bloodSpletter, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPool.Inst.Push<GameObject>(bloodSpletter);

    }

    // 일정 시간이 지나면 체력바를 숨기는 코루틴(몬스터를 떄리고 있다가 몇초 뒤 꺼지게 만들려고)
    protected IEnumerator HideHealthBarAfterDelay(float delay)
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
        SpawnBloodSplatter(transform.position);
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
                // 플레이어 위치로 추적
                m_navAgent.SetDestination(m_player.transform.position);
                // NavMeshAgent의 자동 회전 활성화
                m_navAgent.updateRotation = true;
                if (CheckArea(m_player.transform.position, m_navAgent.stoppingDistance))
                {
                    SetIdle(1f);
                }
                
                break;

            //경계 상태
            case BehaviourState.Patrol:
                
                if (!m_isPatrol)
                {
                    //순찰 시작
                    m_isPatrol = true;                
                    m_curWaypoint = Random.Range(0, m_waypointCtr.m_waypoints.Length);//랜덤으로
                    m_navAgent.updateRotation = true;
                    m_navAgent.ResetPath();
                    m_navAgent.SetDestination(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position);
                    // 회전 강제 설정: 순찰 웨이포인트를 향해 몬스터 회전
                    Quaternion targetRotation = Quaternion.LookRotation(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // 회전 속도를 조절하여 자연스럽게 회전
                    /* //웨이포인트 순차적으로
                    m_curWaypoint++;
                    if (m_curWaypoint >= m_waypointCtr.m_waypoints.Length)
                    {
                        m_curWaypoint = 0;
                    }
                    */
                }
                else
                {
                    //타켓을 찾으면 순찰 중지 idle상태로 감
                    if (FindTarget())
                    {
                        m_isPatrol = false;
                        m_navAgent.ResetPath();
                        SetIdle(1f);
                    }
                    //타겟을 찾지 못함, 현재 웨이포인트에 도착판단
                    else
                    {
                        if (CheckArea(m_waypointCtr.m_waypoints[m_curWaypoint].transform.position, m_navAgent.stoppingDistance))
                        {
                            m_isPatrol = false;
                            SetState(BehaviourState.Idle);
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