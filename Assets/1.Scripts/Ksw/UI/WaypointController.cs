using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class WaypointController : MonoBehaviour
{
    public Waypoint[] m_waypoints;

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
}
