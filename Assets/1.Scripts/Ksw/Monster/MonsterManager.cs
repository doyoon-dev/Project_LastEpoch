using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;


public interface KillCountAlarm
{
    int KillMonCount { get; }
}

public class MonsterManager : SingletonMonoBehaviour<MonsterManager> , KillCountAlarm
{
    [Header("몬스터 소환 수")]
    [SerializeField]
    public int initialMonsterCount;

    [Header("소환 간격(초)")]
    [SerializeField]
    public float spawnInterval; // 몬스터 소환 간격 (초 단위)

    [Header("보스 몬스터 프리팹")]
    [SerializeField]
    private GameObject bossMonsterPrefab; // 보스 몬스터 프리팹 추가

    [Header("몬스터 프리팹 목록")]
    [SerializeField]
    private GameObject[] m_monsterPrefabs;

    [Header("아이템 드롭 매니저")]
    public ItemDropManager itemDropManager; // 아이템 드롭 매니저 

    [Header("체력바 UI")]
    [SerializeField]
    private HealthBarUI healthBarUI;

    [Header("머리 위 체력바 프리팹")]
    [SerializeField]
    private GameObject headHealthBarPrefab;

    [Header("몬스터 이름 목록")]
    [SerializeField]
    private string[] monsterNames;  // 몬스터 이름 목록

    [Header("보스 몬스터 이름")]
    [SerializeField]
    private string bossMonsterName;

    [Header("데미지 UI 프리팹")]
    [SerializeField]
    private GameObject damageUIPrefab;  // 데미지 UI 프리팹 추가

    [Header("소환 이펙트 프리팹")]
    [SerializeField]
    private GameObject summonEffectPrefab;  // 소환 이펙트 프리팹

    [Header("보스 웨이포인트")]
    [SerializeField]
    private WaypointController bossWaypoint; // 보스 몬스터가 소환될 고정 웨이포인트

    [Header("웨이포인트 그룹 목록")]
    [SerializeField]
    private List<WaypointController> waypointGroups; // 여러 웨이포인트 그룹 관리



    [Header("보스 소환 지연 시간")]
    [SerializeField]
    private float bossSpawnDelay = 3f; // 보스 소환 지연 시간 (초)

    private Dictionary<WaypointController, bool> waypointStatus = new Dictionary<WaypointController, bool>(); // 웨이포인트 활성 상태 관리

    private Vector3 lastSpawnPosition = Vector3.zero; // 마지막 소환 위치 저장
    private MonsterController currentTargetMonster;  // 현재 공격받고 있는 몬스터
    private int destroyedTotemCount = 0; // 파괴된 토템 수를 저장하는 변수
    private const int totalTotems = 5; // 총 토템 수


    public int KillMonCount { get; private set; } = 0;



    void Start()
    {
        // 모든 웨이포인트를 활성화 상태로 초기화
        foreach (var waypoint in waypointGroups)
        {
            waypointStatus[waypoint] = true;
        }

        // 시작 시 각 웨이포인트에 몬스터 하나씩 소환
        foreach (var waypoint in waypointGroups)
        {
            SpawnMonster(waypoint);
        }

        // 설정된 지연 후에 자동 스폰 코루틴 시작
        StartCoroutine(StartAutoSpawnAfterDelay());
    }
 


    #region Spawn Methods
    // 일정 시간마다 몬스터를 자동으로 소환하는 코루틴
    IEnumerator AutoSpawnMonsters()
    {
        while (true)
        {
            // 활성화된 웨이포인트 그룹 목록을 가져오기
            List<WaypointController> activeWaypoints = new List<WaypointController>();
            foreach (var waypoint in waypointGroups)
            {
                if (waypointStatus[waypoint]) activeWaypoints.Add(waypoint);
            }

            // 활성화된 웨이포인트 그룹이 없으면 경고 로그 출력 후 대기
            if (activeWaypoints.Count == 0)
            {
                Debug.LogWarning("활성화된 웨이포인트 그룹이 없습니다. 몬스터를 소환할 수 없습니다.");
                yield return new WaitForSeconds(spawnInterval);
                continue;
            }

            // `initialMonsterCount`만큼 랜덤으로 웨이포인트 그룹 선택
            HashSet<int> selectedIndices = new HashSet<int>();
            while (selectedIndices.Count < Mathf.Min(initialMonsterCount, activeWaypoints.Count))
            {
                selectedIndices.Add(Random.Range(0, activeWaypoints.Count));
            }

            // 선택된 웨이포인트 그룹에 몬스터 소환
            foreach (int index in selectedIndices)
            {
                SpawnMonster(activeWaypoints[index]);
            }

            yield return new WaitForSeconds(spawnInterval); // 설정된 간격만큼 대기 후 다시 소환
        }


    }

    // 자동 스폰 코루틴을 지연 후 시작하는 메서드
    IEnumerator StartAutoSpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnInterval); // 초기 소환 후 spawnInterval만큼 대기
        StartCoroutine(AutoSpawnMonsters()); // 이후부터 자동 스폰 코루틴 시작
    }
  

    

    // 몬스터 랜덤 소환 메서드
    void SpawnMonster(WaypointController assignedWaypointGroup = null)
    {
      
        // 활성화된 웨이포인트 그룹 중에서 선택
        List<WaypointController> activeWaypoints = new List<WaypointController>();

        foreach (var waypoint in waypointGroups)
        {
            if (waypointStatus[waypoint]) activeWaypoints.Add(waypoint);
        }

        if (activeWaypoints.Count == 0)
        {
            Debug.LogWarning("활성화된 웨이포인트가 없습니다. 몬스터를 소환할 수 없습니다.");
            return;
        }

        // 웨이포인트가 지정되지 않았다면 랜덤으로 선택
        if (assignedWaypointGroup == null)
        {
            assignedWaypointGroup = activeWaypoints[Random.Range(0, activeWaypoints.Count)];// 활성화된 웨이포인트 중 랜덤으로 선택
        }
       

        // 랜덤하게 몬스터 프리팹을 선택
        int monsterIndex = Random.Range(0, m_monsterPrefabs.Length);
        GameObject monsterPrefab = m_monsterPrefabs[monsterIndex];

        // 오브젝트 풀에서 몬스터를 가져옴
        GameObject monster = ObjectPool.Inst.Pull<MonsterController>(monsterPrefab);
        monster.SetActive(true); // 오브젝트 활성화
        MonsterController monsterController = monster.GetComponent<MonsterController>();
        NavMeshAgent navAgent = monster.GetComponent<NavMeshAgent>();

        // NavMeshAgent의 이동을 멈춘 상태로 설정
        navAgent.isStopped = true;

        // NavMeshAgent의 장애물 회피 비활성화
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        // 몬스터 이름 설정 (MonsterManager 인스펙터에서 설정한 이름을 사용)
        if (monsterIndex < monsterNames.Length)
        {
            monsterController.monsterName = monsterNames[monsterIndex];
        }
        else
        {
            monsterController.monsterName = "Unknown Monster";
        }



        // 몬스터 초기화
        monsterController.Initialize(this, assignedWaypointGroup, healthBarUI, damageUIPrefab);  // 매니저를 초기화 시 전달

 
        // 헤드 헬스바 설정
        GameObject headHealthBarObj = ObjectPool.Inst.Pull<GameObject>(SceneData.Inst.headHealthBarPrefab);
        headHealthBarObj.transform.SetParent(SceneData.Inst.headHealthBarParent, false);
        HeadHealthBar headHealthBar = headHealthBarObj.GetComponent<HeadHealthBar>();

        if (headHealthBar != null)
        {
            headHealthBar.Initialize(monsterController.m_stat.MaxHp);  // 머리 위 체력바 초기화
            monsterController.SetHeadHealthBar(headHealthBar);  // 몬스터 컨트롤러에 머리 위 체력바 연결
            headHealthBar.HideHeadHealthBar();  // 체력바 숨기기 (초기 상태에서)
        }


        // 중앙 체력바 숨김
        if (monsterController.healthBarUI != null)
        {
            monsterController.healthBarUI.HideHealthBar();
        }

        // 스폰 위치를 해당 웨이포인트 그룹 내에서 결정
        Vector3 spawnPosition = assignedWaypointGroup.GetRandomWaypointPosition();

        // NavMeshAgent의 위치를 강제로 설정
        navAgent.Warp(spawnPosition);     
   
        // 소환 위치가 설정된 후, NavMeshAgent의 이동을 다시 활성화
        navAgent.isStopped = false;

        // 마지막 소환 위치 업데이트
        lastSpawnPosition = spawnPosition;

    }

    // 보스 몬스터 스폰 메서드 추가
    void SpawnBossMonster()
    {
        StartCoroutine(SpawnBossWithEffect());
    }

    IEnumerator SpawnBossWithEffect()
    {
        if (bossWaypoint == null)
        {
            Debug.LogWarning("보스 웨이포인트가 설정되지 않았습니다.");
            yield break;
        }

        // 고정된 보스 소환 위치를 설정
        Vector3 bossSpawnPosition = bossWaypoint.GetBossSpawnPoint(); // 보스 소환 위치 가져오기

        // **소환 이펙트 생성**
        GameObject summonEffect = Instantiate(summonEffectPrefab, bossSpawnPosition, Quaternion.identity);

        //보스 몬스터 스폰 사운드
        SoundManager.Inst.PlaySfx("Boss_Spawn");

        // 일정 시간 대기 (예: 3초 대기)
        yield return new WaitForSeconds(0.5f);

        // 소환 이펙트 제거
        Destroy(summonEffect);

        // 오브젝트 풀에서 보스 몬스터를 가져옴
        GameObject boss = ObjectPool.Inst.Pull<BossMonster>(bossMonsterPrefab);
        BossMonster bossController = boss.GetComponent<BossMonster>();
        NavMeshAgent navAgent = boss.GetComponent<NavMeshAgent>();

        // 보스 몬스터 이름을 인스펙터에서 설정한 값으로 적용
        bossController.monsterName = bossMonsterName;

        // 보스 몬스터 초기화
        bossController.Initialize(this, bossWaypoint, healthBarUI, damageUIPrefab);

        // 보스 소환 시 체력바 숨기기
        bossController.healthBarUI.HideHealthBar();

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        // 보스를 지정된 위치에 소환
        navAgent.Warp(bossSpawnPosition); // 위치 설정
        boss.transform.position = bossSpawnPosition; // 위치를 고정
    }

    // 지연 후 보스 몬스터 소환 코루틴
    private IEnumerator SpawnBossAfterDelay()
    {
        yield return new WaitForSeconds(bossSpawnDelay); // 설정된 시간만큼 대기
        SpawnBossMonster(); // 보스 몬스터 소환
    }


    #endregion
    // 특정 웨이포인트 비활성화 메서드 (토템이 파괴될 때 호출됨)
    public void DisableWaypoint(WaypointController waypoint)
    {
        if (waypointStatus.ContainsKey(waypoint))
        {
            waypointStatus[waypoint] = false; // 웨이포인트 비활성화
            Debug.Log("웨이포인트 비활성화: " + waypoint.name);
        }
    }
    // 처치 수를 증가시키는 메서드
    public void IncreaseKillCount()
    {
        KillMonCount++;
        Debug.Log("현재 처치한 몬스터 수: " + KillMonCount);
    }

    // 토템 파괴를 처리하는 메서드 추가
    public void OnTotemDestroyed()
    {
        destroyedTotemCount++; // 토템 파괴 시 카운트 증가
        if (destroyedTotemCount >= totalTotems) // 모든 토템이 파괴되었는지 확인
        {
            StartCoroutine(SpawnBossAfterDelay()); // 모든 토템이 파괴되면 지연 후 보스 소환
        }
    }
    // 현재 공격받고 있는 몬스터 설정 (중앙 체력바)
    public void SetCurrentTargetMonster(MonsterController monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("Trying to set null monster as current target.");
            return;
        }
        // 현재 타겟 몬스터와 새 타겟이 다를 때만 변경
        if (currentTargetMonster != monster)
        {
            // 기존 몬스터의 체력바 숨기기
            if (currentTargetMonster != null)
            {
                if (currentTargetMonster.healthBarUI != null)
                {
                    currentTargetMonster.healthBarUI.HideHealthBar();// 중앙 체력바 숨기기
                }

                if (currentTargetMonster.headHealthBar != null)
                {
                    currentTargetMonster.headHealthBar.HideHeadHealthBar();  // 머리 위 체력바 숨기기
                }
            }

            // 새 타겟 몬스터 설정
            currentTargetMonster = monster;

            if (currentTargetMonster.healthBarUI != null)
            {
                currentTargetMonster.healthBarUI.ShowHealthBar();// 중앙 체력바 활성화
                currentTargetMonster.healthBarUI.Initialize(monster.monsterName, monster.m_stat.MaxHp);// 중앙 체력바 갱신
            }
            // **머리 위 체력바 설정**
            if (currentTargetMonster.headHealthBar != null)
            {
                currentTargetMonster.headHealthBar.ShowHeadHealthBar();// 머리 위 체력바 활성화
                currentTargetMonster.headHealthBar.UpdateHeadHealth((int)monster.m_curHealPoint, monster.m_stat.MaxHp);// 머리 위 체력바 갱신
            }
        }
    }

    // 현재 타겟 몬스터 초기화 (공격 중지 or 사망 시)
    public void ClearCurrentTargetMonster()
    {
        if (currentTargetMonster != null)
        {
            currentTargetMonster.healthBarUI.HideHealthBar();
            currentTargetMonster = null;
        }
    }


    // 몬스터가 죽을 때 호출되는 메서드
    public void HandleMonsterDeath(Vector3 position)
    {
        // 아이템 드롭 매니저에서 아이템 드롭 메서드 호출
        itemDropManager.DropItems(position);
    }

    // 특정 위치의 다음 웨이포인트 인덱스를 얻는 메서드
    int GetNextWaypointIndex(Vector3 currentPosition, WaypointController waypointGroup)
    {
        for (int i = 0; i < waypointGroup.m_waypoints.Length; i++)
        {
            if (waypointGroup.m_waypoints[i].transform.position == currentPosition)
            {
                // 현재 위치의 다음 인덱스를 반환, 배열의 끝에 도달하면 0으로 순환
                return (i + 1) % waypointGroup.m_waypoints.Length;
            }
        }
        // 위치를 찾지 못한 경우 0번 인덱스를 기본으로 반환
        return 0;
    }

    public void RemoveMonstersAtWaypoint(WaypointController waypoint)
    {
        foreach (var monster in FindObjectsOfType<MonsterController>())
        {
            if (monster.m_waypointCtr.Contains(waypoint))
            {
                // 사망 처리 또는 비활성화
                monster.HandleDeath(); // 사망 처리 메서드 호출
                Destroy(monster.gameObject); // 또는 Destroy로 제거
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        // V 키를 눌렀을 때 몬스터 소환
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnMonster();
        }

        // B 키를 눌렀을 때 보스 몬스터 소환
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBossMonster();
        }
    }
}