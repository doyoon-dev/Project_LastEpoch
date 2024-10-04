using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class BossMonster : MonsterController
{
    private Vector3 startPos; // 시작 위치 저장

    protected override void Start()
    {
        base.Start();
        startPos = transform.position; // 시작 위치 초기화
    }

    //행동 프로세스
    public override void BehaviourProcess()
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
                            m_navAgent.isStopped = true;  // 공격 시 이동 중지\
                            StartCoroutine(LookAtPlayer());
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
                            m_navAgent.isStopped = false;  // 추적 시 이동 재개
                            m_navAgent.stoppingDistance = m_attackDist;
                            m_idleTime = 0;
                        }
                    }
                    // 타켓을 못 찾는 경우 
                    else
                    {
                        SetState(BehaviourState.Patrol);
                        m_navAgent.isStopped = false;  // 추적 시 이동 재개
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
                else
                {
                    // 플레이어를 향해 고개를 돌림 (현재 이동 방향 기준)
                    Vector3 directionToPlayer = (m_player.transform.position - transform.position).normalized;
                    directionToPlayer.y = 0f;  // 수평 회전만 적용
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);  // 회전 속도는 필요에 따라 조정
                }
                break;

            //Patrol 상태 로직 수정
            case BehaviourState.Patrol:
                if (!m_isPatrol)
                {
                    m_isPatrol = true;
                    StartRoaming();
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
                        // 순찰 중 일정 범위에 도착했는지 확인하고 Idle로 전환
                        if (CheckArea(transform.position, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                        {
                            m_isPatrol = false;
                            SetIdle(2f);
                        }
                    }
                }
                break;

            case BehaviourState.Damaged:
                break;

            case BehaviourState.Die:
                HandleDeath();
                return;
        }
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
        while (m_navAgent.remainingDistance > m_navAgent.stoppingDistance)
        {
            yield return null;
        }
        onComplete?.Invoke();
    }

    // 상태를 Idle로 전환하는 메서드
    protected override void SetIdle(float duration)
    {
        SetState(BehaviourState.Idle);  // Idle 상태로 전환
        m_idleTime = 0;  // Idle 시간 초기화
        m_idleDuration = duration;  // 새로운 Idle 대기 시간 설정
    }
    
    // 죽음 처리 기능 추가
    protected override void HandleDeath()
    {
        if (IsDie) return;// 이미 죽은 상태에서 다시 처리하지 않도록 함
        m_manager.HandleMonsterDeath(transform.position);// 매니저에게 몬스터가 죽었다고 알림
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생  
        StopAllCoroutines();  // 현재 실행 중인 모든 코루틴 정지
        m_navAgent.isStopped = true;  // 네비게이션 에이전트 중지
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과
        // 모든 콜라이더 비활성화
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;  // 네비게이션 에이전트 설정        
        SetState(BehaviourState.Die);  // 상태를 Die로 변경
        ShutDownHealthBars();
        AttackArea.SetActive(false);

    }

}