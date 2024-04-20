using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelSkill : Skill
{
    [SerializeField]
    Animator m_myAnim;
    bool m_usingSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Skill_WarPath(KeyCode.Q);
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

    // 출정 스킬(윈드밀)
    void Skill_WarPath(KeyCode inputKey)
    {
        // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
        // 마우스 방향으로 이동가능
        if (Input.GetKey(inputKey))
        {
            if(!m_usingSkill)
            {
                m_usingSkill = true;
                m_myAnim.SetBool("SkillWarPath", true);
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 dir = hit.point - transform.position;
                dir.y = 0;
                dir.Normalize();
                transform.Translate(dir * Time.deltaTime * 2.0f);
            }
            
        }
        if (Input.GetKeyUp(inputKey))
        {
            m_myAnim.SetBool("SkillWarPath", false);
            m_usingSkill = false;
        }
    }

    // 돌격 스킬
    void Skill_Lunge()
    {
        // 스킬 키 한 번 누르면 스킬 발동
        // 마우스 방향으로 일정거리 돌진
    }
}
