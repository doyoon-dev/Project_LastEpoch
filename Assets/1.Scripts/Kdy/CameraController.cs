using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_offsetX = 4.0f;
    public float m_offsetY = 4.0f;
    public float m_offsetZ = 4.0f;
    public float m_camSpeed = 5.0f;
    public GameObject m_player;
    public LayerMask m_wallMask;

    public MeshRenderer[] m_mr;
    Color m_initColor;

    // Start is called before the first frame update
    void Start()
    {
        //m_mr = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnDrawGizmo()
    {
        Gizmos.DrawLine(transform.forward, m_player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_player.transform.position.x + m_offsetX, m_player.transform.position.y + m_offsetY, m_player.transform.position.z - m_offsetZ);

        Vector3 dir = m_player.transform.position - transform.position;
        dir.Normalize();
        Ray ray = Camera.main.ScreenPointToRay(dir);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_wallMask))
        {
            m_mr = hit.transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < m_mr.Length; i++)
            {
                for (int j = 0; j < m_mr[i].materials.Length; j++)
                {
                    m_mr[i].enabled = false;
                    //Color color = m_mr[i].materials[j].color;
                    //m_initColor.a = color.a;
                    //color.a -= Time.deltaTime;
                    //m_mr[i].materials[j].color = color;
                }
            }
        }
        else
        {
            for (int i = 0; i < m_mr.Length; i++)
            {
                for (int j = 0; j < m_mr[i].materials.Length; j++)
                {
                    //Color color = m_mr[i].materials[j].color;

                    //if (255.0f > color.a)
                    //{
                    //    color.a += Time.deltaTime;
                    //}
                    //else
                    //{
                    //    color.a = 255.0f;
                    //}
                    //m_mr[i].materials[j].color = color;
                    m_mr[i].enabled = true;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_wallMask)
        {
            m_mr = collision.transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < m_mr.Length; i++)
            {
                for (int j = 0; j < m_mr[i].materials.Length; j++)
                {
                    m_mr[i].enabled = false;
                    //Color color = m_mr[i].materials[j].color;
                    //m_initColor.a = color.a;
                    //color.a -= Time.deltaTime;
                    //m_mr[i].materials[j].color = color;
                }
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == m_wallMask)
        {
            m_mr = collision.transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < m_mr.Length; i++)
            {
                for (int j = 0; j < m_mr[i].materials.Length; j++)
                {
                    m_mr[i].enabled = true;
                }
            }
        }
    }
}
