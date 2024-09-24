using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : SingletonMonoBehaviour<MonsterManager>
{
    [Header("몬스터 소환 수")]
    [SerializeField]
    public int initialMonsterCount;

    [Header("소환 간격(초)")]
    [SerializeField]
    public float spawnInterval; // 몬스터 소환 간격 (초 단위)

    [Header("몬스터 소환 거리")]
    [SerializeField]
    private float spawnOffset; // 몬스터 간 거리 오프셋

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

    public WaypointController waypointController;
    private Vector3 lastSpawnPosition = Vector3.zero; // 마지막 소환 위치 저장
    private MonsterController currentTargetMonster;  // 현재 공격받고 있는 몬스터


    void Start()
    {
        // 코루틴 시작: 일정 간격마다 몬스터 소환
        StartCoroutine(AutoSpawnMonsters());
    }

    // 일정 시간마다 몬스터를 자동으로 소환하는 코루틴
    IEnumerator AutoSpawnMonsters()
    {
        while (true)
        {
            // 설정된 수만큼 몬스터를 연속적으로 소환
            for (int i = 0; i < initialMonsterCount; i++)
            {
                SpawnMonster();
            }
            yield return new WaitForSeconds(spawnInterval); // 설정된 간격만큼 대기 후 다시 소환
        }
    }

    // 몬스터 소환 메서드
    void SpawnMonster()
    {
       
        // 랜덤하게 몬스터 프리팹을 선택
        int monsterIndex = Random.Range(0, m_monsterPrefabs.Length);
        GameObject monsterPrefab = m_monsterPrefabs[monsterIndex];

        // 오브젝트 풀에서 몬스터를 가져옴
        GameObject monster = ObjectPool.Inst.Pull<MonsterController>(monsterPrefab);
        MonsterController monsterController = monster.GetComponent<MonsterController>();
        NavMeshAgent navAgent = monster.GetComponent<NavMeshAgent>();

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
        monsterController.Initialize(this, waypointController, healthBarUI);  // 매니저를 초기화 시 전달


        // HeadHealthBar도 오브젝트 풀에서 GameObject로 가져옴
        GameObject headHealthBarObj = ObjectPool.Inst.Pull<GameObject>(headHealthBarPrefab);  // HeadHealthBar 프리팹 가져오기

        // **머리 위 체력바 초기화**
        HeadHealthBar headHealthBar = monster.GetComponentInChildren<HeadHealthBar>();  // 몬스터에서 HeadHealthBar 컴포넌트 찾기
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
        
        // 웨이포인트에서 랜덤 위치를 얻어옴
        Vector3 spawnPosition = waypointController.GetRandomWaypointPosition();
        monster.transform.position = spawnPosition;


        // 이전 소환 위치와 동일한지 확인
        if (spawnPosition == lastSpawnPosition)
        {
            // 동일하다면 다른 위치 선택
            int nextIndex = GetNextWaypointIndex(spawnPosition);
            spawnPosition = waypointController.m_waypoints[nextIndex].transform.position;
        }

        // 소환 위치에 약간의 오프셋을 추가하여 겹침 방지
        spawnPosition += new Vector3(Random.Range(-spawnOffset, spawnOffset), 0, Random.Range(-spawnOffset, spawnOffset));


        // 웨이포인트에 몬스터 배치
        monster.transform.position = spawnPosition;

        // 마지막 소환 위치 업데이트
        lastSpawnPosition = spawnPosition;

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
    // 보스 몬스터 스폰 메서드 추가
    public void SpawnBossMonster()
    {
        GameObject boss = ObjectPool.Inst.Pull<BossMonster>(bossMonsterPrefab);
        BossMonster bossController = boss.GetComponent<BossMonster>();
        NavMeshAgent navAgent = boss.GetComponent<NavMeshAgent>();

        // 보스 몬스터 이름을 인스펙터에서 설정한 값으로 적용
        bossController.monsterName = bossMonsterName;

        bossController.Initialize(this, waypointController, healthBarUI);

        bossController.healthBarUI.HideHealthBar();  // 보스 소환 시 체력바 숨기기

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        // 보스는 특정 위치에 소환되게 할 수 있음
        Vector3 bossSpawnPosition = waypointController.GetBossSpawnPoint(); // 보스 전용 스폰 지점
        boss.transform.position = bossSpawnPosition;

       
    }


    // 몬스터가 죽을 때 호출되는 메서드
    public void HandleMonsterDeath(Vector3 position)
    {
        // 아이템 드롭 매니저에서 아이템 드롭 메서드 호출
        itemDropManager.DropItems(position);
    }

    // 특정 위치의 다음 웨이포인트 인덱스를 얻는 메서드
    int GetNextWaypointIndex(Vector3 currentPosition)
    {
        for (int i = 0; i < waypointController.m_waypoints.Length; i++)
        {
            if (waypointController.m_waypoints[i].transform.position == currentPosition)
            {
                // 현재 위치의 다음 인덱스를 반환, 배열의 끝에 도달하면 0으로 순환  원형(순환) 배열과 같은 효과
                return (i + 1) % waypointController.m_waypoints.Length;
            }
        }
        // 위치를 찾지 못한 경우 0번 인덱스를 기본으로 반환
        return 0;
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