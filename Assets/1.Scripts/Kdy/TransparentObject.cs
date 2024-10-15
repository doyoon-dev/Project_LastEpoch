using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    [SerializeField]
    LayerMask m_wallMask;
    [SerializeField]
    Player m_player;
    public float m_offsetX = 4.0f;
    public float m_offsetY = 4.0f;
    public float m_offsetZ = 4.0f;
    public float m_camSpeed = 5.0f;
    public List<MeshRenderer> m_mr = new List<MeshRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_player.transform.position.x + m_offsetX, m_player.transform.position.y + m_offsetY, m_player.transform.position.z - m_offsetZ);
        Vector3 dir = m_player.transform.position - transform.position;
        dir.Normalize();
        Debug.DrawRay(transform.position, dir, Color.yellow);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, m_wallMask))
        {
            if (!m_mr.Contains(hit.transform.GetComponentInChildren<MeshRenderer>()))
            {
                for (int i = 0; i < m_mr.Count; i++)
                {
                    m_mr[i].enabled = true;
                    m_mr.Remove(m_mr[i]);
                }
                m_mr.Add(hit.transform.GetComponentInChildren<MeshRenderer>());
                for (int i = 0; i < m_mr.Count; i++)
                {
                    m_mr[i].enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < m_mr.Count; i++)
            {
                m_mr[i].enabled = true;
                m_mr.Remove(m_mr[i]);
            }
        }
    }
}
