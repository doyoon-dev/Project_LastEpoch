using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Picking : MonoBehaviour
{
    public LayerMask m_moveMask;
    public UnityEvent<Vector3> m_moveAct;
    public UnityEvent m_attackAct;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_moveMask))
            {
                m_moveAct?.Invoke(hit.point);
            }
        }
        if(Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, Mathf.Infinity, m_moveMask))
            {
                m_attackAct?.Invoke();
            }
        }
    }
}
