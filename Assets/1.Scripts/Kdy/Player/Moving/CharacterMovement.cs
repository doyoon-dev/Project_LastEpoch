using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : CharacterProperty
{
    public float m_moveSpeed = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 target)
    {
        StopAllCoroutines();
        StartCoroutine(Moving(target));
    }

    IEnumerator Moving(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        while (dist > 0.0f)
        {
            float delta = Time.deltaTime * m_moveSpeed;
            if(delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
    }
}
