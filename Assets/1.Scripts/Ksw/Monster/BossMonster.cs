using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
public class BossMonster : MonsterController
{
    [Header("게더링 이펙트 프리팹")]
    [SerializeField]
    public GameObject GatheringEffectPrefab; // 피흘리는 이펙트 프리팹
    private Vector3 startPos; // 시작 위치 저장
    private bool isGathering = false;
    private float gatheringDuration = 4.0f; // 힘을 모으는 시간  
    private bool isSpecialAttackActive = false;
    private float lastHealthThreshold = 1.0f; // 마지막 체크한 체력 비율
    private bool isForceGathering = false; // 강제 Gathering 상태 플래그
    private float forceGatheringDuration = 2.0f; // 강제로 Gathering 상태를 유지할 시간
    private float specialAttackCooldown = 10.0f; // 스페셜 어택 쿨다운
    private float lastSpecialAttackTime = -10f;  // 마지막 스페셜 어택 시간을 초기화
    private float specialAttackMoveDistance = 7.0f; // 스페셜 어택 시 이동할 거리
    private float specialAttackSpeed = 5.0f; // 스페셜 어택 시 이동 속도
    protected override void Start()
    {
        base.Start();
        startPos = transform.position; // 시작 위치 초기화
       
    }

    //행동 프로세스
    public override void BehaviourProcess()
    {
        // 체력 손실을 체크하여 30%마다 Gathering 상태로 전환
        CheckHealthThreshold();

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
                StartCoroutine(LookAtPlayer()); // 공격 상태에서 플레이어를 바라보도록 호출
                break;


            //추적 상태
            case BehaviourState.Chase:
                
                StartCoroutine(LookAtPlayer()); // 추적 상태에서도 플레이어를 바라보도록 호출
                // 플레이어 위치로 추적
                m_navAgent.SetDestination(m_player.transform.position);

                // 플레이어와의 거리를 계산
                float distanceToPlayer = Vector3.Distance(m_player.transform.position, transform.position);

                // 추적 거리를 벗어나면 추적 중지
                if (distanceToPlayer > m_chaseDist)
                {
                    SetIdle(1f);  // Idle 상태로 전환
                }
                else if (CheckArea(m_player.transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                {
                    SetIdle(1f);  // 플레이어 근처에 도착하면 Idle 상태로 전환
                }

                break;

            //Patrol 상태 로직 수정
            case BehaviourState.Patrol:
                if (!m_isPatrol) // 패트롤 시작
                {
                    m_isPatrol = true;
                    StartRoaming();
                }
                else
                {
                    if (FindTarget()) // 플레이어를 발견하면
                    {
                        m_isPatrol = false;
                        m_navAgent.ResetPath();
                        SetIdle(1f);
                    }
                    else
                    {
                        // NavMeshAgent의 remainingDistance가 stoppingDistance 이하인지 확인
                        if (m_navAgent.remainingDistance <= m_navAgent.stoppingDistance && !m_navAgent.pathPending)
                        {
                            m_isPatrol = false; // 패트롤 종료 플래그
                            SetIdle(2f); // Idle 상태로 전환하여 잠시 대기
                        }
                    }
                }
                break;

            case BehaviourState.Damaged:
                break;
            // 힘을 모으는 상태 (Gathering)
            case BehaviourState.Gathering:
                StartCoroutine(LookAtPlayer()); // 게더링 상태에서도 플레이어를 바라보도록 호출
                if (!isGathering)
                {
                    // 코루틴 시작
                    StartCoroutine(GatheringCoroutine());
                }
                break;
            // 스페셜 어택 상태
            case BehaviourState.SpecialAttack:
                // 스페셜 어택 실행
                ExecuteSpecialAttack();
                break;
            case BehaviourState.Die:
                if (!IsDie) // IsDie가 false인 경우에만 처리
                {
                    HandleDeath();
                    IsDie = true; // IsDie를 true로 설정하여 다시 호출되지 않도록 방지
                }
                break;
        }
        
    }

    // 체력이 30% 감소할 때마다 Gathering 상태로 전환하는 메서드
    private void CheckHealthThreshold()
    {
        float currentHealthRatio = m_curHealPoint / m_stat.MaxHp;

        // 강제 Gathering 상태일 때는 트리거 방지
        if (isForceGathering) return;

        // 체력 감소가 30% 이상일 때만 Gathering 트리거
        if (lastHealthThreshold - currentHealthRatio >= 0.3f)
        {
            lastHealthThreshold = currentHealthRatio; // 현재 체력 비율 저장
            SetState(BehaviourState.Gathering); // Gathering 상태로 전환
        }
    }

    // 플레이어 방향으로 고개 돌리기 메서드(보스)
    protected override IEnumerator LookAtPlayer()
    {
        if (isLookingAtPlayer) yield break; // 이미 실행 중이면 종료
        isLookingAtPlayer = true;

        while (m_state == BehaviourState.Gathering || m_state == BehaviourState.Attack || m_state == BehaviourState.Chase)
        {
            if (m_player == null) break;

            Vector3 direction = (m_player.transform.position - transform.position).normalized;
            direction.y = 0f; // 수평면에서만 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 게더링 상태에서는 즉각적으로 바라보도록 회전 속도 증가
            float rotationSpeed = m_state == BehaviourState.Gathering ? 1000f : 300f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null; // 매 프레임마다 실행
        }

        isLookingAtPlayer = false; // 상태 종료 후 플래그 리셋
    }





    // 힘 모으는 코루틴
    public IEnumerator GatheringCoroutine()
    {
        if (m_state != BehaviourState.Gathering || isGathering) yield break;

        isGathering = true;
        isForceGathering = true; // 강제 Gathering 상태 활성화
        Debug.Log("Gathering 시작");
        m_navAgent.enabled = false; // NavMeshAgent 비활성화
        // Gathering 모션 재생
        m_monAnimCtr.Play(MonsterAnimController.Motion.Gathering);
        // 플레이어를 바라보는 메서드 호출
        StartCoroutine(LookAtPlayer());

        float elapsed = 0f; //경과
        while (elapsed < gatheringDuration)
        {
           

            // 체력이 0이 되어 Gathering 중단 후 죽음
            if (m_curHealPoint <= 0)
            {
                Debug.Log("체력이 0이 되어 Gathering 중단 후 죽음 상태로 전환");
                SetState(BehaviourState.Die);
                yield break;
            }
            // 강제 Gathering 시간이 종료되면 강제 상태 해제
            if (elapsed > forceGatheringDuration)
            {
                isForceGathering = false;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Gathering 완료 - SpecialAttack 상태로 전환");
        // 스페셜 어택 전 지연 시간 추가
        yield return new WaitForSeconds(0.2f); //  (원하는 시간으로 설정 가능)

        m_navAgent.enabled = true;
        SetState(BehaviourState.SpecialAttack);
        isGathering = false;
        isForceGathering = false;  // Gathering과 강제 상태 모두 해제
        ExecuteSpecialAttack();
    }

    private void ExecuteSpecialAttack()
    {
        if (isSpecialAttackActive) return;
        isSpecialAttackActive = true;

        // 스페셜 어택 쿨다운이 지났을 경우
        if (Time.time - lastSpecialAttackTime >= specialAttackCooldown)
        {
          
            // 스페셜 어택 애니메이션 재생
            m_monAnimCtr.Play(MonsterAnimController.Motion.SpAttack);
            lastSpecialAttackTime = Time.time;  // 마지막 스페셜 어택 시간 갱신

            //이동하면서 공격
            StartCoroutine(MoveForwardSpecialAttack(specialAttackMoveDistance, specialAttackSpeed));
        }
    }

    // 플레이어가 범위에 들어왔을 때만 데미지 적용
    private IEnumerator MoveForwardSpecialAttack(float totalMoveDistance, float moveSpeedPerSecond)
    {
        float accumulatedDistance = 0f;
        bool hasDealtDamage = false;

        while (accumulatedDistance < totalMoveDistance)
        {
            float distanceThisFrame = moveSpeedPerSecond * Time.deltaTime;
            Vector3 moveDirection = transform.forward * distanceThisFrame;

            // NavMeshAgent의 Move 메서드를 사용하여 벽에 막힐 때 자동으로 멈추게 함
            m_navAgent.Move(moveDirection);
            accumulatedDistance += distanceThisFrame;

            if (!hasDealtDamage)
            {
                Collider[] playersInRange = Physics.OverlapSphere(transform.position, m_attackDist, m_playerMask);
                foreach (Collider player in playersInRange)
                {
                    Player targetPlayer = player.GetComponent<Player>();
                    if (targetPlayer != null)
                    {
                        targetPlayer.SetDamage(SkillDataManager.m_skillDataDic["BossSpAttk"]);
                        hasDealtDamage = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        SetIdle(1f);
        isSpecialAttackActive = false;
    }



    /*
    public void SpawnGatheringEffect(Vector3 position)
    {
        if (m_state != BehaviourState.Gathering) return;  // Gathering 상태가 아니면 실행하지 않음
        // 보스 몬스터일 경우 높이를 추가
        if (this is BossMonster)
        {
            position.y += 1.0f;
        }

        // ObjectPool에서 이펙트 가져오기
        GameObject gatherEffect = ObjectPool.Inst.Pull<GameObject>(GatheringEffectPrefab);
        gatherEffect.transform.position = position;

        StartCoroutine(ReturnGatherEffectToPool(gatherEffect));
    }

    // 게더링 이펙트를 다시 풀로 반환하는 코루틴
    public IEnumerator ReturnGatherEffectToPool(GameObject gatherEffect)
    {
        yield return new WaitForSeconds(gatheringDuration);
        ObjectPool.Inst.Push<GameObject>(gatherEffect);
    }
    */



    protected override void HandleDeath()//죽음 상태 처리
    {
        if (IsDie) return;// 이미 죽은 상태에서 다시 처리하지 않도록 함
        IsDie = true;                        // 
        StopAllCoroutines();// 모든 코루틴 중지
        isGathering = false;
        isSpecialAttackActive = false;
        m_navAgent.isStopped = true;  // 네비게이션 에이전트 중지
        m_manager.HandleMonsterDeath(transform.position);// 매니저에게 몬스터가 죽었다고 알림
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과                                                  
        DisableColiders();
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정                                                                                       
        //SetState(BehaviourState.Die);
        ShutDownHealthBars();    
        AttackArea.SetActive(false);  // 공격 범위 비활성화 (박스가 보이지 않도록 설정)
        SpawnBloodSplatter(transform.position);
    }

    // 랜덤 이동 시작
    void StartRoaming()
    {
        m_monAnimCtr.Play(MonsterAnimController.Motion.Run);
        float rnd = Random.Range(0.0f, 360.0f);  // 360도 방향 중 무작위 선택
        Vector3 rndDir = Quaternion.Euler(0, rnd, 0) * Vector3.forward * Random.Range(0.0f, 7.0f);  // 랜덤 방향과 거리 설정
        Vector3 rndPos = startPos + rndDir;  // 시작 위치에서 이동할 목표 위치 계산
        MoveToPos(rndPos, () => { StartCoroutine(DelayChangeIdle(Random.Range(1.5f, 4.0f))); });  // 이동 후 일정 시간 후에 다시 Idle 상태로
    }



    // 일정 시간 후 다시 Normal 상태로 전환하는 코루틴
    IEnumerator DelayChangeIdle(float t)
    {
        yield return new WaitForSeconds(t);  // t초 대기
        SetIdle(1f);  // 다시 Idle 상태로
    }

    // 이동 완료 후 호출되는 콜백 처리
    void MoveToPos(Vector3 pos, System.Action onComplete)
    {
        m_navAgent.SetDestination(pos);
        StartCoroutine(CheckArrival(onComplete));
    }

    // 목표 지점에 도달했는지 확인하는 코루틴
    IEnumerator CheckArrival(System.Action onComplete)
    {
        while (m_navAgent.pathPending || m_navAgent.remainingDistance > m_navAgent.stoppingDistance)
        {
            yield return null;
        }

        onComplete?.Invoke(); // 목표 도달 시 콜백 호출
    }

    // 상태를 Idle로 전환하는 메서드
    protected override void SetIdle(float duration)
    {
        SetState(BehaviourState.Idle);  // Idle 상태로 전환
        m_idleTime = 0;  // Idle 시간 초기화
        m_idleDuration = duration;  // 새로운 Idle 대기 시간 설정
    }

    protected override bool CanAttack()
    {
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
        return false;
    }


}