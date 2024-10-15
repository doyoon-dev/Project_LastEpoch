using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //public MeshRenderer[] m_mr;
    public List<MeshRenderer> m_mr = new List<MeshRenderer>();
    Color m_initColor;

    // Start is called before the first frame update
    void Start()
    {
        //m_mr = GetComponentsInChildren<MeshRenderer>();
        SoundManager.Inst.PlayBgm("BGM");
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(transform.position, m_player.transform.position);
    //    Gizmos.color = Color.yellow;
    //}

    public void CameraShakeFunc()
    {
        StopAllCoroutines();
        StartCoroutine(CameraShake());
    }

    IEnumerator CameraShake()
    {
        Vector3 initPos = transform.position;
        float shakeTime = 0;
        while (shakeTime < 0.12f)
        {
            shakeTime += Time.deltaTime;
            transform.position = Random.onUnitSphere * shakeTime + transform.position;
            yield return null;
        }
        transform.position = initPos;
    }
}
