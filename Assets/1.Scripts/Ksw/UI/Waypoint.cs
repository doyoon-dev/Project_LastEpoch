using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Color m_color = Color.yellow;
    void OnDrawGizmos()
    {
        Gizmos.color = m_color;
        Gizmos.DrawWireSphere(transform.position, 1.0f);

    }
}
