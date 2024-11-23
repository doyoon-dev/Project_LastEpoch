using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class WaypointController : MonoBehaviour
{
    public Waypoint[] m_waypoints;
    public Transform bossSpawnPoint; // 보스 몬스터 전용 스폰 지점

    [Header("웨이포인트 몬스터 제한")]
    public int maxMonsters = 2; // 웨이포인트당 최대 몬스터 수
    private int currentMonsters = 0; // 현재 소환된 몬스터 수

    void OnDrawGizmos()
    {
        m_waypoints = GetComponentsInChildren<Waypoint>();
        for (int i = 0; i < m_waypoints.Length; i++)
        {
            if (i == 0)
            {
                m_waypoints[i].m_color = Color.magenta;
            }
            else if (i == m_waypoints.Length - 1)
            {
                m_waypoints[i].m_color = Color.magenta;
            }
            else
            {
                m_waypoints[i].m_color = Color.yellow;
            }
        }
        for (int i = 0; i < m_waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
        }
    }
    public Vector3 GetRandomWaypointPosition()
    {
        if (m_waypoints.Length == 0) return Vector3.zero;
        int randomIndex = Random.Range(0, m_waypoints.Length);
        return m_waypoints[randomIndex].transform.position;
    }
    // 보스 몬스터 전용 스폰 지점 반환
    public Vector3 GetBossSpawnPoint()
    {
        if (bossSpawnPoint != null)
        {
            return bossSpawnPoint.position;
        }
        else
        {
            // 보스 스폰 지점이 설정되지 않은 경우 기본 위치를 반환하거나 예외 처리
            Debug.LogWarning("보스 스폰 지점이 설정되지 않았습니다. 기본 위치를 반환합니다.");
            return Vector3.zero;
        }
    }

    // 몬스터 소환 가능 여부 확인
    public bool CanSpawnMoreMonsters()
    {
        return currentMonsters < maxMonsters;
    }

    // 몬스터 수 증가
    public void IncrementMonsterCount()
    {
        currentMonsters++;
    }

    // 몬스터 수 감소
    public void DecrementMonsterCount()
    {
        currentMonsters = Mathf.Max(0, currentMonsters - 1);
    }
}
