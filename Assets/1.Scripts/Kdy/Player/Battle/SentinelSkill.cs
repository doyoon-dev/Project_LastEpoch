using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelSkill : Skill
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Q_SkillInputKey()
    {
        base.Q_SkillInputKey();
        Collider[] enemy = Physics.OverlapBox(transform.forward * 2, new Vector3(2, 2, 2), Quaternion.identity);
        foreach (Collider col in enemy)
        {
            IBattle ib = col.GetComponent<IBattle>();
            if (ib != null)
            {
                ib.OnDamaged(30.0f);
            }
        }
    }

    protected override void W_SkillInputKey()
    {
        base.W_SkillInputKey();
    }

    protected override void E_SkillInputKey()
    {
        base.E_SkillInputKey();
    }

    protected override void R_SkillInputKey()
    {
        base.R_SkillInputKey();
    }
}
