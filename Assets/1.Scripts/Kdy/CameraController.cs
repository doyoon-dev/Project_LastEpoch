using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_camSpeed = 5.0f;
    public GameObject m_player;
    public LayerMask m_wallMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = m_player.transform.position - transform.position;
        Ray ray = Camera.main.ScreenPointToRay(dir);
        if (Physics.Raycast(ray, m_wallMask))
        {

        }
    }
}
