using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPoint : MonoBehaviour
{
    public Color m_color = Color.blue;
    void OnDrawGizmos()
    {
        Gizmos.color = m_color;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
