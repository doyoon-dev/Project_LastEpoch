using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : BattleSystem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void MoveToAttack(Transform target)
    {
        MoveToEnemy(target, m_stat.attackRange, Attack);
    }
}
