using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterSample : BattleSystem
{
    // Start is called before the first frame update
    void Start()
    {
        IDeadAlarm da = GetComponent<IDeadAlarm>();
        if (da != null)
        {
            da.m_deadAlarm += () =>
            {
                gameObject.SetActive(false);
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
