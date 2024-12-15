using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IMonsterCountResult
{
    void MonsterCountResult(int cnt);
}

public interface ITimeResult
{
    void TimeResult(string s);
}

public interface IResult
{
    void Result();
}

public class GameClearUI : MonoBehaviour, ITimeResult, IMonsterCountResult, IResult
{
    // 카메라 나중에 지움
    [SerializeField]
    Camera m_cam;
    [SerializeField]
    GameObject m_playTimeObj;
    [SerializeField]
    GameObject m_monCntObj;
    [SerializeField]
    TextMeshProUGUI m_playTime;
    [SerializeField]
    TextMeshProUGUI m_monCntText;
    int m_monCnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayTime pt = m_cam.GetComponent<PlayTime>();
            pt.m_isEnd = true;
            Result();
        }
    }

    public void TimeResult(string s)
    {
        m_playTime.text = s;
    }

    

    public void MonsterCountResult(int cnt)
    {
        m_monCnt = cnt;
        m_monCntText.text = m_monCnt.ToString();
    }

    public void Result()
    {
        ActiveObjects();
    }

    public void ActiveObjects()
    {
        Invoke("OnPlayTimeObj", 1.5f);
        Invoke("OnMonsterCountObj", 3.0f);
    }

    void OnPlayTimeObj()
    {
        m_playTimeObj.SetActive(true);
    }
    void OnMonsterCountObj()
    {
        m_monCntObj.SetActive(true);
    }
}
