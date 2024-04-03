using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MovePath
{
    float curAttackDelay = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_myAnim.GetBool("IsAttacking"))
        {

        }
    }

    public void OnAttack()
    {
        m_myAnim.SetTrigger("Attack");
    }
}
