using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SentinelSkill : Skill
{
    [SerializeField]
    Animator m_myAnim;
    Player m_player;    // 나중에 인터페이스로 바꿔야 될 수 있음
    bool m_usingSkill = false;
    // Start is called before the first frame update
    void Start()
    {
        m_player = transform.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Skill_WarPath(KeyCode.Q);
        Skill_Lunge(KeyCode.W);
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
        if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillData.m_skillData["WindMill"].Mp)
        {
            UsingSkillMp(SkillData.m_skillData["WindMill"].Mp * Time.deltaTime);
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
        if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillData.m_skillData["WindMill"].Mp)
        {
            m_myAnim.SetBool("SkillWarPath", false);
            m_usingSkill = false;
        }
    }

    // 돌격 스킬
    void Skill_Lunge(KeyCode inputKey)
    {
        if (Input.GetKeyDown(inputKey))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                StopAllCoroutines();
                Vector3 dir = hit.point - transform.position;
                transform.forward = dir;
                StartCoroutine(LungeMove(dir));
            }
        }
        // 스킬 키 한 번 누르면 스킬 발동
        // 마우스 방향으로 일정거리 돌진
    }

    public void DamageBox()
    {
        Collider[] enemy = Physics.OverlapSphere(transform.position, 1.0f);
        foreach (Collider col in enemy)
        {
            IBattle ib = col.GetComponent<IBattle>();
            if (ib != null)
            {
                ib.OnDamaged(SkillData.m_skillData["WindMill"].Dmg);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }

    // 돌격 스킬 이동 함수
    IEnumerator LungeMove(Vector3 dir)
    {
        m_myAnim.SetBool("SkillLunge", true);
        float dist = 2;
        dir.Normalize();
        dir.y = 0;
        while (dist > 0)
        {
            float delta = 5.0f * Time.deltaTime;
            if (delta > dist) delta = dist;
            dist -= delta;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        m_myAnim.SetBool("SkillLunge", false);
    }
}
