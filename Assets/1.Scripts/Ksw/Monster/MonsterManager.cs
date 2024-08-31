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

    [Header("아이템 프리팹 목록")]
    [SerializeField]
    private GameObject[] m_dropItemPrefabs;


    [Header("아이템 드롭 확률 목록")]
    [SerializeField]
    private float[] m_dropChances;

    [Header("몬스터 프리팹 목록")]
    [SerializeField]
    private GameObject[] m_monsterPrefabs;  

    public WaypointController waypointController;
    private Vector3 lastSpawnPosition = Vector3.zero; // 마지막 소환 위치 저장
  


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
        GameObject monster = ObjectPool.Inst.Pool<MonsterController>(monsterPrefab);
        MonsterController monsterController = monster.GetComponent<MonsterController>();
        NavMeshAgent navAgent = monster.GetComponent<NavMeshAgent>();


        // 몬스터 초기화
        monsterController.Initialize(this, waypointController);  // 매니저를 초기화 시 전달

        // NavMeshAgent의 장애물 회피 비활성화
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

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

    // 몬스터가 죽을 때 호출되는 메서드
    public void HandleMonsterDeath(Vector3 position)
    {
        DropItem(position);
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

    // 아이템 드롭 메서드
    private void DropItem(Vector3 position)
    {
        if (m_dropItemPrefabs.Length != m_dropChances.Length)
        {
            Debug.LogError("아이템 프리팹, 드롭 확률, 아이템 데이터 배열의 길이가 일치하지 않습니다.");
            return;
        }

        for (int i = 0; i < m_dropItemPrefabs.Length; i++)
        {
            float dropChance = m_dropChances[i];
            if (UnityEngine.Random.Range(0f, 100f) <= dropChance)
            {
                GameObject dropItemObject = ObjectPool.Inst.Pool<DropItem>(m_dropItemPrefabs[i]);
                DropItem dropItem = dropItemObject.GetComponent<DropItem>();
                dropItem.transform.position = position;
                dropItem.gameObject.SetActive(true);
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
    }
}