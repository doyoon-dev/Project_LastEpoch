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
        Gathering,
        SpecialAttack,
        Die,
        Max
    }
    public enum MonsterType
    {
        Dog,
        Rat,
        Worm,
        ZombieKing
    }
    [Header("몬스터 타입")]
    public MonsterType monsterType; // 몬스터 타입 설정

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
    public List<WaypointController> m_waypointCtr;

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
    protected Renderer[] m_renderers;
    private GameObject detectedPlayer;
    public LayerMask m_playerMask;
    public LayerMask m_BackgroundMask;
    Transform m_attacker;
    Coroutine m_hitColorCoroutine;
    int m_currentAttackIndex;
    int m_curWaypoint;
    public HealthBarUI healthBarUI;
    public HeadHealthBar headHealthBar; // 머리 위 체력바 참조
    protected Coroutine hideHealthBarCoroutine;
    public string monsterName;
    public bool m_isPatrol; //patrol 여부확인
    public bool IsDie = false;
    public bool isTransitioning = false;// 상태 전환 중인지 확인하는 플래그\
    private bool hasAttacked = false;
    public bool isLookingAtPlayer = false;
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
        Attack();
    }

    void AnimEvent_Attack2()
    {
        Attack2();
    }

    void AnimEvent_AttackFinished()
    {
        SetIdle(0.5f);
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
    protected virtual void SetState(BehaviourState state)
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

        m_waypointCtr.Clear(); // 기존 리스트를 초기화하여 중복 방지
        m_waypointCtr.Add(waypoint); // 리스트에 단일 WaypointController 추가

    }
    #endregion


    #region Mon Attack Methods

    // 플레이어한테 공격
    public override void Attack()
    {
        if (hasAttacked) return; // 이미 공격이 호출된 경우 실행하지 않음
        hasAttacked = true; // 공격이 호출되었음을 설정
        //Debug.Log("Attack1 호출됨");
        Collider[] playersInRange = Physics.OverlapSphere(AttackArea.transform.position, m_attackDist, m_playerMask);

        foreach (Collider player in playersInRange)
        {
            Player targetPlayer = player.GetComponent<Player>();
            if (targetPlayer != null)
            {
                targetPlayer.SetDamage(SkillDataManager.m_skillDataDic["MonsterDmg"]); // 플레이어에게 데미지 입힘

            }
        }

        StartCoroutine(ResetAttackFlag()); // 공격 플래그 초기화 코루틴 실행
    }
    public void Attack2()
    {
        if (hasAttacked) return; // 이미 공격이 호출된 경우 실행하지 않음
        hasAttacked = true; // 공격이 호출되었음을 설정
        //Debug.Log("Attack2 호출됨"); // 호출 횟수 확인을 위한 로그
        Collider[] playersInRange = Physics.OverlapSphere(AttackArea.transform.position, m_attackDist, m_playerMask);

        foreach (Collider player in playersInRange)
        {
            Player targetPlayer = player.GetComponent<Player>();
            if (targetPlayer != null)
            {
                targetPlayer.SetDamage(SkillDataManager.m_skillDataDic["MonsterDmg2"]); // 플레이어에게 데미지 입힘

            }
        }
        StartCoroutine(ResetAttackFlag()); // 공격 플래그 초기화 코루틴 실행
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1f); // 원하는 시간만큼 대기 후
        hasAttacked = false; // 공격 플래그 초기화
    }


    //공격 가능여부

    protected virtual bool CanAttack()
    {
        //거리
        var dist = transform.position - m_player.transform.position;

        //방향
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized); // 목표 회전값

        // 현재 회전에서 목표 회전까지 서서히 회전
        float rotationSpeed = 5f; // 회전 속도 (조정 가능)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

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
        return false;
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
                StartCoroutine(DamageAfterDelay(1f)); // 두 번째 공격 후 0.5초 후 데미지 적용
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


    //플레이어한테 몬스터 공격을 받았을떄임
    public override void SetDamage(SkillData skillData)
    {
        if (IsDie || isTransitioning) return; // 무적 상태면 데미지 무시) return;
        if (m_state == BehaviourState.Gathering || m_state == BehaviourState.SpecialAttack || m_state == BehaviourState.Damaged) return;

        // 피 흘리는 이펙트 소환
        SpawnBleedEffect(transform.position);

        // 맞는 사운드
        SoundManager.Inst.PlaySfx("Hit_Sound");

        // 체력 감소 처리
        m_curHealPoint -= skillData.Dmg;

        // MonsterManager에게 현재 공격받고 있는 몬스터를 설정
        MonsterManager.Instance.SetCurrentTargetMonster(this);

        // 몬스터는 흰색 데미지 텍스트 표시
        ShowDamageText(skillData.Dmg, Color.white);  // 색상으로 흰색 전달

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

            // 처치 수 증가
            MonsterManager.Instance.IncreaseKillCount();
            SetState(BehaviourState.Die);
            return;
        }

        if (this is BossMonster)
        {
            SetHitColor(0.2f);
        }
        else
        {
            // 일반 몬스터의 데미지 처리 (기존 로직)
            SetState(BehaviourState.Damaged);
            m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
            m_navAgent.ResetPath();
            m_navAgent.isStopped = true;  // 데미지 받았을 때 이동 중지
            SetHitColor(0.2f);
            ApplyKnockback(skillData);// 넉백 처리
            StartCoroutine(ResumeMovementAfterDamage());

        }


    }

    // 데미지 후 다시 돌아오기
    protected IEnumerator ResumeMovementAfterDamage(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay); // Hit 모션이 끝날 시간을 대기

        if (m_navAgent != null && m_navAgent.isOnNavMesh) // NavMeshAgent가 활성화되어 있고, NavMesh 위에 배치되어 있는지 확인
        {
            m_navAgent.isStopped = false; // 이동 재개           

        }
    }



    #endregion
    #region Mon Find Methods

    // 플레이어 방향으로 고개 돌리기 메서드
    protected virtual IEnumerator LookAtPlayer()
    {
        Vector3 direction = (m_player.transform.position - transform.position).normalized;
        direction.y = 0f; // 수평면에서만 회전하도록 Y축을 0으로 설정

        float rotationSpeed = 10f; // 회전 속도

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

    // Chase 상태 종료 시 LookAtPlayer 코루틴 중지
    void StopChase()
    {
        if (isLookingAtPlayer)
        {
            StopCoroutine(LookAtPlayer());
            isLookingAtPlayer = false;  // 플래그 초기화
        }
        m_navAgent.updateRotation = true; // NavMeshAgent 회전 활성화
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

        // EffectManager를 통해 피 이펙트를 생성하고 위치 설정
        EffectManager.Instance.GetEffect("Bleed", position, Quaternion.identity);
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

    public void DisableColiders()
    {
        // Collider 비활성화 (필요한 Collider를 전부 비활성화)
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    public virtual void HandleDeath()//죽음 상태 처리
    {
        if (IsDie) return;// 이미 죽은 상태에서 다시 처리하지 않도록 함
        IsDie = true;
        StopAllCoroutines();// 모든 코루틴 중지
        m_manager.HandleMonsterDeath(transform.position);// 매니저에게 몬스터가 죽었다고 알림
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과
        if (m_waypointCtr != null)
        {
            foreach (var waypoint in m_waypointCtr)
            {
                waypoint.DecrementMonsterCount(); // 웨이포인트의 몬스터 카운트 감소
            }
        }
        if (m_navAgent != null && m_navAgent.isActiveAndEnabled && m_navAgent.isOnNavMesh)
        {
            m_navAgent.isStopped = true;  // 네비게이션 에이전트 중지
            m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정    
        }      
        DisableColiders();                                                                                         
        ShutDownHealthBars();
        AttackArea.SetActive(false);// 공격 범위 비활성화 (박스가 보이지 않도록 설정)                              
        GameObject bloodstainEffect = EffectManager.Instance.GetEffect("BloodSplatter02", transform.position, Quaternion.identity);// 핏자국 이펙트 생성
                                                                                                                                   // 몬스터 종류에 따라 죽음 소리 재생
        switch (monsterType)
        {
            case MonsterType.Dog:
                SoundManager.Inst.PlaySfx("Dog_Death");
                break;
            case MonsterType.Rat:
                SoundManager.Inst.PlaySfx("Rat_Death");
                break;

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

                //LookAtPlayer 떄문에 자동회전 비활성화
                m_navAgent.updateRotation = true; // 자동 회전 활성화
                
                // 플레이어 위치로 추적
                m_navAgent.SetDestination(m_player.transform.position);

                // 플레이어와의 거리를 계산
                float distanceToPlayer = Vector3.Distance(m_player.transform.position, transform.position);

                // 추적 거리를 벗어나면 추적 중지
                if (distanceToPlayer > m_chaseDist)
                {
                    StopChase();
                    SetIdle(1f);  // Idle 상태로 전환
                }
                else if (CheckArea(m_player.transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                {
                    StopChase();
                    SetIdle(1f);  // 플레이어 근처에 도착하면 Idle 상태로 전환
                }
                break;

            //경계 상태
            case BehaviourState.Patrol:
                StopChase(); // Chase 종료 설정 초기화
                m_navAgent.updateRotation = true; // 자동 회전 활성화
                if (!m_isPatrol)
                { 
                    m_isPatrol = true;

                    // 여러 개의 웨이포인트 컨트롤러 리스트에서 랜덤으로 선택
                    if (m_waypointCtr != null && m_waypointCtr.Count > 0)
                    {
                        var selectedWaypointCtr = m_waypointCtr[Random.Range(0, m_waypointCtr.Count)];

                        // 선택된 웨이포인트 컨트롤러의 웨이포인트 배열에서 랜덤으로 웨이포인트 선택
                        if (selectedWaypointCtr != null && selectedWaypointCtr.m_waypoints.Length > 0)
                        {
                            m_curWaypoint = Random.Range(0, selectedWaypointCtr.m_waypoints.Length);
                            m_navAgent.SetDestination(selectedWaypointCtr.m_waypoints[m_curWaypoint].transform.position);
                            //m_navAgent.updateRotation = true;
                        }
                        
                    }             
                }
                else
                {
                    if (FindTarget())
                    {
                        m_isPatrol = false;
                        m_navAgent.ResetPath();
                        StopChase();
                        SetIdle(1f);
                    }
                    else
                    {
                        // 현재 할당된 웨이포인트 컨트롤러에서 목표 웨이포인트에 도달했는지 확인
                        if (m_waypointCtr != null && m_waypointCtr.Count > 0)
                        {
                            var currentWaypointCtr = m_waypointCtr[Random.Range(0, m_waypointCtr.Count)];
                            if (currentWaypointCtr != null && m_curWaypoint >= 0 && m_curWaypoint < currentWaypointCtr.m_waypoints.Length)
                            {
                                if (CheckArea(currentWaypointCtr.m_waypoints[m_curWaypoint].transform.position, m_navAgent.stoppingDistance))
                                {
                                    m_isPatrol = false;
                                    SetIdle(1f);
                                }
                            }
                            
                        }
                    }
                }
                break;
            //데미지 상태
            case BehaviourState.Damaged:
                break;
            //죽은 상태  
            case BehaviourState.Die:
                if (!IsDie) // IsDie가 false인 경우에만 처리
                {
                    HandleDeath();
                    IsDie = true; // IsDie를 true로 설정하여 다시 호출되지 않도록 방지
                }
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