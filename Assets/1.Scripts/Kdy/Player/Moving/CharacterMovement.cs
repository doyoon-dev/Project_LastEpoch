using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : CharacterProperty
{
    public float m_moveSpeed = 3.0f;
    public float m_rotSpeed = 2.0f;
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

    // 이동
    public IEnumerator Moving(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        m_myAnim.SetBool("Move", true);
        StartCoroutine(Rotating(dir));
        while (dist > 0.01f)
        {
            float delta = Time.deltaTime * m_moveSpeed;
            if(delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        m_myAnim.SetBool("Move", false);
    }

    // 회전
    IEnumerator Rotating(Vector3 pos)
    {
        float angle = Vector3.Angle(transform.forward, pos);
        if (angle > 180.0f) angle -= 360.0f;
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, pos) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (angle > 0.0f)
        {
            float delta = Time.deltaTime * 360.0f * m_rotSpeed;
            if (delta > angle) delta = angle;
            angle -= delta;
            transform.Rotate(Vector3.up * rotDir * delta, Space.World);
            yield return null;
        }
    }

    public void MoveToEnemy(Transform target, float range)
    {
        StopAllCoroutines();
        StartCoroutine(MovingToEnemy(target, range));
    }

    // 적과 거리가 있을 때 적 앞까지 이동 후 공격
    public IEnumerator MovingToEnemy(Transform target, float range)
    {
        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude - range;
        dir.Normalize();
        m_myAnim.SetBool("Move", true);
        StartCoroutine(Rotating(dir));
        while (!Mathf.Approximately(dist, 0.0f))
        {
            float delta = Time.deltaTime * m_moveSpeed;
            if (delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        m_myAnim.SetBool("Move", false);
    }
}
