using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTime : MonoBehaviour
{
    [SerializeField]
    GameObject m_gamClearUI;
    public bool m_isEnd = false;
    int m_hour = 0;
    int m_min = 0;
    int m_sec = 0;
    public string m_time;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeCheck());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public MonsterManager m;
    IEnumerator TimeCheck()
    {
        float time = 0.0f;
        while (!m_isEnd)
        {
            time += Time.deltaTime;
            if (time >= 60)
            {
                m_min += 1;
                time = 0;
                if (m_min >= 60)
                {
                    m_hour += 1;
                    m_min = 0;
                }
            }
            m_sec = (int)time;
            Debug.Log(string.Format("{0:D2}:{1:D2}:{2:D2}", m_hour, m_min, m_sec));
            yield return null;
        }
        //m_sec = (int)time;
        
        ITimeResult itr = m_gamClearUI.GetComponent<ITimeResult>();
        if (itr != null)
        {
            itr.TimeResult(TimeToText(m_hour, m_min, m_sec));
        }
    }

    public string TimeToText(int h, int m, int s)
    {
        m_time = string.Format("{0:D2}:{1:D2}:{2:D2}", m_hour, m_min, m_sec);
        return m_time;
    }
}
