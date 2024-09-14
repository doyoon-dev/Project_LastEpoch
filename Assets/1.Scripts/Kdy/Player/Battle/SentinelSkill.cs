using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SentinelSkill : Skill
{
    [SerializeField]
    Animator m_myAnim;
    [SerializeField]
    Transform m_warPathStartPos;
    [SerializeField]
    Transform m_warPathEndPos;
    public LayerMask m_enemyMask;
    public Dictionary<string, SkillButton> m_skillBtns = new Dictionary<string, SkillButton>();
    bool m_warPathUse = false;
    bool m_lungeUse = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Skill_WarPath(KeyCode.W);
        //Skill_Lunge(KeyCode.E);
    }

    protected override void Q_SkillInputKey()
    {
        base.Q_SkillInputKey();
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

    void SkillCheck()
    {

    }

    // 출정 스킬(윈드밀)
    public virtual void Skill_WarPath(KeyCode inputKey)
    {
        // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
        // 마우스 방향으로 이동가능
        if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillDataManager.m_skillData["WindMill"].Mp)
        {
            UsingSkillMp(SkillDataManager.m_skillData["WindMill"].Mp * Time.deltaTime * SkillDataManager.m_skillData["WindMill"].Channeling);
            if(!m_warPathUse)
            {
                m_usingSkill = true;
                m_warPathUse = true;
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
        if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillDataManager.m_skillData["WindMill"].Mp)
        {
            m_myAnim.SetBool("SkillWarPath", false);
            m_warPathUse = false;
            m_usingSkill = false;
        }
    }

    // 출정 스킬 데미지 박스
    public void DamageBox()
    {
        //Collider[] enemy = Physics.OverlapSphere(m_warPathSkillRange.position, 0.07f);

        //foreach (Collider col in enemy)
        //{
        //    IBattle ib = col.GetComponent<IBattle>();
        //    if (ib != null)
        //    {
        //        ib.OnDamaged(SkillData.m_skillData["WindMill"].Dmg);
        //    }
        //}

        // 충돌 문제 생기면 위에 걸로 아니면 이거 써도 됨
        Collider[] list = Physics.OverlapCapsule(m_warPathStartPos.position, m_warPathEndPos.position, 0.07f, m_enemyMask);
        foreach (Collider col in list)
        {
            IBattle ib = col.GetComponent<IBattle>();
            if (ib != null)
            {
                ib.OnDamaged(SkillDataManager.m_skillData["WindMill"].Dmg);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(m_warPathStartPos.position, 0.07f);
        //Gizmos.DrawWireSphere(m_warPathEndPos.position, 0.07f);
        //Gizmos.DrawWireCube(m_warPathEndPos.position, new Vector3(1, 1, 1));
    }

    // 돌격 스킬
    public virtual void Skill_Lunge(KeyCode inputKey)
    {
        if (!m_usingSkill)
        {
            //if (Input.GetKeyDown(inputKey) && !m_lungeUse && m_player.m_curMagicPoint >= SkillData.m_skillData["Lunge"].Mp)
            if (!m_lungeUse && m_player.m_curMagicPoint >= SkillDataManager.m_skillData["Lunge"].Mp)
            {
                if (m_player != null)
                {
                    Debug.Log("Player!!");
                }
                UsingSkillMp(SkillDataManager.m_skillData["Lunge"].Mp);
                m_usingSkill = true;
                IUsableSkillAct iusa = m_playerUI.m_skillCoolTime.GetComponent<IUsableSkillAct>();
                if (iusa != null)
                {
                    iusa.m_usableSkillAct += () => { m_usingSkill = false; };
                }
                m_lungeUse = true;
                ICoolTime ict = m_playerUI.m_skillCoolTime.GetComponent<ICoolTime>();
                if (ict != null)
                {
                    ict.CoolTime(inputKey, SkillDataManager.m_skillData["Lunge"].CoolTime);
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    StopAllCoroutines();
                    Vector3 dir = hit.point - m_player.transform.position;
                    m_player.transform.forward = dir;
                    StartCoroutine(LungeMove(dir));
                }

            }
        }
        
        // 스킬 키 한 번 누르면 스킬 발동
        // 마우스 방향으로 일정거리 돌진
    }

    // 돌격 스킬 이동 함수
    IEnumerator LungeMove(Vector3 dir)
    {
        m_player.GetComponent<Collider>().isTrigger = true;
        m_player.GetComponent<Rigidbody>().isKinematic = true;
        m_myAnim.SetBool("SkillLunge", true);
        float dist = 2;
        dir.Normalize();
        dir.y = 0;
        Collider[] list;
        List<IBattle> enemyList = new List<IBattle>();
        IBattle ib;
        while (dist > 0)
        {
            // 무기 콜라이더 지우고 박스로 플레이어 앞에 생성
            list = Physics.OverlapCapsule(m_warPathStartPos.position, m_warPathEndPos.position, 0.1f, m_enemyMask);
            // 아래 오버랩으로 사용
            Collider[] alist = Physics.OverlapBox(m_warPathStartPos.position, new Vector3(0.5f, 0.5f, 0.5f));
            foreach (Collider col in list)
            {
                ib = col.GetComponent<IBattle>();
                if (!enemyList.Contains(ib))
                {
                    enemyList.Add(ib);
                }
            }
            float delta = 5.0f * Time.deltaTime;
            if (delta > dist) delta = dist;
            dist -= delta;
            m_player.transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].OnDamaged(SkillDataManager.m_skillData["Lunge"].Dmg);
            enemyList.Remove(enemyList[i]);
        }
        m_player.GetComponent<Collider>().isTrigger = false;
        m_player.GetComponent<Rigidbody>().isKinematic = false;
        m_lungeUse = false;
        m_myAnim.SetBool("SkillLunge", false);
    }

    // 돌진 스킬 데미지 박스
    // OnTrigger 함수로 무기 앞에 콜라이더 만들고 충돌 적 무시, 충돌 적 데미지 주기로 해야 할 지도
    public void LungeDamageBox()
    {
        // 무기 콜라이더 지우고 박스로 플레이어 앞에 생성
        Collider[] list = Physics.OverlapCapsule(m_warPathStartPos.position, m_warPathEndPos.position, 0.1f, m_enemyMask);
        List<IBattle> enemyList = new List<IBattle>();
        foreach (Collider col in list)
        {
            IBattle ib = col.GetComponent<IBattle>();
            enemyList.Add(ib);
            Debug.Log("추가 : " + enemyList.Count);
            if (ib != null)
            {
                ib.OnDamaged(SkillDataManager.m_skillData["Lunge"].Dmg);
                enemyList.Remove(ib);
                Debug.Log("제거 : " + enemyList.Count);
            }
        }
    }
}
