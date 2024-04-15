using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct MoveStat
{
    public float moveSpeed;
    public float rotSpeed;
}

public class CharacterMovement : CharacterProperty
{
    [SerializeField] MoveStat m_moveStat;
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
        dir.y = 0;
        m_myAnim.SetBool("Move", true);
        StartCoroutine(Rotating(dir));
        while (dist > 0.0f)
        {
            float delta = Time.deltaTime * m_moveStat.moveSpeed;
            if(delta > dist) delta = dist;
            transform.Translate(dir * delta, Space.World);
            dist -= delta;
            yield return null;
        }
        m_myAnim.SetBool("Move", false);
    }

    public void Rotate(Vector3 pos)
    {
        StopAllCoroutines();
        StartCoroutine(Rotating(pos));
    }

    // 회전
    public IEnumerator Rotating(Vector3 pos)
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
            float delta = Time.deltaTime * 360.0f * m_moveStat.rotSpeed;
            if (delta > angle) delta = angle;
            angle -= delta;
            transform.Rotate(Vector3.up * rotDir * delta, Space.World);
            yield return null;
        }
    }

    public void MoveToEnemy(Transform target, float range, UnityAction act)
    {
        StopAllCoroutines();
        StartCoroutine(MovingToEnemy(target, range, act));
    }

    // 적과 거리가 있을 때 적 앞까지 이동 후 공격
    public IEnumerator MovingToEnemy(Transform target, float range, UnityAction act)
    {
        while (target != null)
        {
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude - range;
            dir.Normalize();
            dir.y = 0;
            
            StartCoroutine(Rotating(dir));
            if (!Mathf.Approximately(dist, 0.0f))
            {
                m_myAnim.SetBool("Move", true);
                float delta = Time.deltaTime * m_moveStat.moveSpeed;
                if (delta > dist) delta = dist;
                if(!m_myAnim.GetBool("IsAttacking")) transform.Translate(dir * delta, Space.World);
            }
            else
            {
                m_myAnim.SetBool("Move", false);
                act?.Invoke();
                target = null;
            }
            yield return null;
        }
    }

    public void FollowingEnemy(Vector3 target, float range, UnityAction act)
    {
        StopAllCoroutines();
        StartCoroutine(Following(target, range, act));
    }

    public IEnumerator Following(Vector3 target, float range, UnityAction act)
    {
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude - range;
        dir.Normalize();
        dir.y = 0;
        m_myAnim.SetBool("Move", true);
        StartCoroutine(Rotating(dir));
        while (dist > 0.0f)
        {
            float delta = Time.deltaTime * m_moveStat.moveSpeed;
            if (delta > dist) delta = dist;
            transform.Translate(dir * delta, Space.World);
            dist -= delta;
            yield return null;
        }
        m_myAnim.SetBool("Move", false);
        act?.Invoke();
    }
}
