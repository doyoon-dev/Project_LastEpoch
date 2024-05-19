using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



//넉백 할떄 쓸만한 DotWEEN
public class MoveTween : MonoBehaviour
{
    NavMeshAgent m_navAgent;
    [SerializeField]
    AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField]
    Vector3 m_from;
    [SerializeField]
    Vector3 m_to;
    [SerializeField]
    float m_duration = 1f;


    IEnumerator Coroutine_MoveCurve()
    {
        float time = 0f;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        while (true)
        {
            time += Time.deltaTime / m_duration;
            var value = m_curve.Evaluate(time);
            m_navAgent.Move((m_from * (1f - value) + m_to * value) - transform.position);
            if (time > 1f)
            {
                m_navAgent.isStopped = false;
                yield break;
            }
            yield return null;
        }
    }
    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine("Coroutine_MoveCurve");
    }
    public void Play(Vector3 from, Vector3 to, float duration)
    {
        m_from = from;
        m_to = to;
        m_duration = duration;
        Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            m_from = transform.position;
            m_to = m_from + Vector3.forward * 8f;
            m_duration = 0.5f;
            Play();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_from = transform.position;
            m_to = m_from + Vector3.back * 8f;
            m_duration = 0.5f;
            Play();
        }
    }
}