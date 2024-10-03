using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface ISkill_Lunge
{
    void Skill_Lunge(KeyCode inputKey);
}

public class SentinelSkill : Skill, ISkill_Lunge
{
    [SerializeField]
    Animator m_myAnim;
    [SerializeField]
    Transform m_warPathStartPos;
    [SerializeField]
    Transform m_warPathEndPos;

    public UnityEvent m_stopMovingAct;
    public GameObject m_effectPos;
    public GameObject m_erasingStrikeEffect;
    public GameObject m_warpathEffect;
    public GameObject m_lungeEffect;
    public LayerMask m_enemyMask;
    public LayerMask m_backgroundMask;
    public Dictionary<string, SkillButton> m_skillBtns = new Dictionary<string, SkillButton>();
    bool m_warPathUse = false;
    bool m_lungeUse = false;
    bool m_strikeUse = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Skill_ErasingStrike(KeyCode.Q);
        }

        Skill_WarPath(KeyCode.W);
        if (Input.GetKey(KeyCode.E))
        {
            Skill_Lunge(KeyCode.E);
        }
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

    // Q 스킬
    public void Skill_ErasingStrike(KeyCode inputKey)
    {
        if (!m_usingSkill)
        {
            if (m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["ErasingStrike"].Mp && !m_strikeUse)
            {
                m_myAnim.SetBool("Move", false);
                StopAllCoroutines();
                m_player.StopAllCoroutines();
                m_myAnim.SetTrigger("SkillStrike");
                m_strikeUse = true;
                m_usingSkill = true;
                UsingSkillMp(SkillDataManager.m_skillDataDic["ErasingStrike"].Mp);
                

                IUsableSkillAct iusa = m_playerUI.m_skillCoolTime.GetComponent<IUsableSkillAct>();
                if (iusa != null)
                {
                    iusa.m_usableSkillAct += () => { m_strikeUse = false; };
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
                {
                    Vector3 dir = hit.point - m_player.transform.position;
                    dir.y = 0;
                    m_player.transform.forward = dir;
                }
                ICoolTime ict = m_playerUI.m_skillCoolTime.GetComponent<ICoolTime>();
                if (ict != null)
                {
                    ict.CoolTime(inputKey, SkillDataManager.m_skillDataDic["ErasingStrike"].CoolTime);
                }

            }
        }
    }

    public void SKill_ErasingStrike_Damage()
    {
        Collider[] list = Physics.OverlapBox(m_warPathStartPos.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, m_enemyMask);
        foreach (Collider col in list)
        {
            IDamageable ib = col.GetComponent<IDamageable>();
            if (ib != null)
            {
                ib.SetDamage(SkillDataManager.m_skillDataDic["ErasingStrike"]);
            }
        }
    }
    public void Skill_ErasingStrike_EffectOn()
    {
        m_erasingStrikeEffect.SetActive(true);
    }
    public void Skill_ErasingStrike_EffectOff()
    {
        m_erasingStrikeEffect.SetActive(false);
        m_usingSkill = false;
        RecoverMp(m_usingSkill);
    }


    // 출정 스킬(W 스킬 : 윈드밀)
    //public void Skill_WarPath(KeyCode inputKey)
    //{
    //    // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
    //    // 마우스 방향으로 이동가능
    //    if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //    {
    //        //StopAllCoroutines();
    //        m_myAnim.SetBool("Move", false);
    //        m_usingSkill = true;
    //        m_stopMovingAct?.Invoke();
    //        #region 실험 코드
    //        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
    //        //{
    //        //    Vector3 skillDir = hit.point - m_player.transform.position;
    //        //    skillDir.y = 0;
    //        //    skillDir.Normalize();
    //        //    transform.forward = skillDir;
    //        //    transform.Translate(skillDir * Time.deltaTime * 2.0f);
    //        //}
    //        #endregion
    //        RecoverMp(m_usingSkill);
    //        UsingSkillMp(SkillDataManager.m_skillDataDic["Warpath"].Mp * Time.deltaTime * SkillDataManager.m_skillDataDic["Warpath"].Channeling);
    //        if (!m_warPathUse)
    //        {
    //            m_warpathEffect.SetActive(true);

    //            m_warPathUse = true;
    //            m_myAnim.SetBool("SkillWarPath", true);
    //        }

    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
    //        {
    //            StopAllCoroutines();

    //            Vector3 skillDir = hit.point - m_player.transform.position;

    //            skillDir.y = 0;
    //            Debug.DrawRay(transform.position, skillDir, Color.red);
    //            skillDir.Normalize();

    //            transform.Translate(skillDir * Time.deltaTime * 2.0f);
    //        }
    //    }
    //    if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //    {
    //        m_warpathEffect.SetActive(false);
    //        m_myAnim.SetBool("SkillWarPath", false);
    //        m_warPathUse = false;
    //        m_usingSkill = false;
    //        RecoverMp(m_usingSkill);
    //    }
    //}

    public void Skill_WarPath(KeyCode inputKey)
    {
        // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
        // 마우스 방향으로 이동가능
        if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            if (m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
            {
                m_stopMovingAct?.Invoke();
                return;
            }

            //StopAllCoroutines();
            m_myAnim.SetBool("Move", false);
            m_usingSkill = true;
            m_stopMovingAct?.Invoke();
            RecoverMp(m_usingSkill);
            UsingSkillMp(SkillDataManager.m_skillDataDic["Warpath"].Mp * Time.deltaTime * SkillDataManager.m_skillDataDic["Warpath"].Channeling);
            if (!m_warPathUse)
            {
                m_warpathEffect.SetActive(true);

                m_warPathUse = true;
                m_myAnim.SetBool("SkillWarPath", true);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
            {
                SkillMove(hit.point);
            }
        }
        if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            m_warpathEffect.SetActive(false);
            m_myAnim.SetBool("SkillWarPath", false);
            m_warPathUse = false;
            m_usingSkill = false;
            RecoverMp(m_usingSkill);
        }
    }

    public void SkillMove(Vector3 target)
    {
        StopAllCoroutines();
        StartCoroutine(SkillMoving(target));
    }

    public IEnumerator SkillMoving(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        dir.y = 0;

        while (gameObject.GetComponent<SentinelSkill>().m_usingSkill && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            float delta = Time.deltaTime * m_player.m_moveStat.moveSpeed;
            transform.Translate(dir * delta, Space.World);
            yield return null;
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
                ib.SetDamage(SkillDataManager.m_skillDataDic["Warpath"]);
                // 나중에 합칠 때 아래 코드 씀
                //ib.OnDamaged(SkillDataManager.m_skillData["WindMill"].Dmg, SkillDataManager.m_skillData["WindMill"]);
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
    public void Skill_Lunge(KeyCode inputKey)
    {
        if (!m_usingSkill)
        {
            //if (Input.GetKeyDown(inputKey) && !m_lungeUse && m_player.m_curMagicPoint >= SkillData.m_skillData["Lunge"].Mp)
            if (!m_lungeUse && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Lunge"].Mp)
            {
                m_lungeEffect.SetActive(true);
                m_usingSkill = true;
                UsingSkillMp(SkillDataManager.m_skillDataDic["Lunge"].Mp);
                
                IUsableSkillAct iusa = m_playerUI.m_skillCoolTime.GetComponent<IUsableSkillAct>();
                if (iusa != null)
                {
                    iusa.m_usableSkillAct += () => { m_lungeUse = false; };
                }
                m_lungeUse = true;
                ICoolTime ict = m_playerUI.m_skillCoolTime.GetComponent<ICoolTime>();
                if (ict != null)
                {
                    ict.CoolTime(inputKey, SkillDataManager.m_skillDataDic["Lunge"].CoolTime);
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
                {
                    m_player.StopAllCoroutines();
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
        m_myAnim.SetBool("Move", false);
        m_myAnim.SetBool("SkillLunge", true);
        float dist = 2;
        dir.Normalize();
        dir.y = 0;
        Collider[] list;
        //List<IBattle> enemyList = new List<IBattle>();
        IBattle ib;
        while (dist > 0)
        {
            list = Physics.OverlapBox(m_warPathStartPos.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, m_enemyMask);
            foreach (Collider col in list)
            {
                ib = col.GetComponent<IBattle>();
                if (ib != null)
                {
                    ib.SetDamage(SkillDataManager.m_skillDataDic["Lunge"]);
                }
            }
            float delta = 5.0f * Time.deltaTime;
            if (delta > dist) delta = dist;
            dist -= delta;
            m_player.transform.Translate(dir * delta, Space.World);
            yield return null;
        }
        m_lungeEffect.SetActive(false);
        m_usingSkill = false;
        m_myAnim.SetBool("SkillLunge", false);
        m_player.GetComponent<Collider>().isTrigger = false;
        m_player.GetComponent<Rigidbody>().isKinematic = false;
        RecoverMp(m_usingSkill);
    }

    void RecoverMp(bool isUsingSkill)
    {
        IRecoveryManaPoint ib = m_player.GetComponent<IRecoveryManaPoint>();
        if (ib != null)
        {
            ib.RecoveryManaPoint(isUsingSkill);
        }
    }


    // 돌진 스킬 데미지 박스
    // OnTrigger 함수로 무기 앞에 콜라이더 만들고 충돌 적 무시, 충돌 적 데미지 주기로 해야 할 지도
    //public void LungeDamageBox()
    //{
    //    // 무기 콜라이더 지우고 박스로 플레이어 앞에 생성
    //    Collider[] list = Physics.OverlapCapsule(m_warPathStartPos.position, m_warPathEndPos.position, 0.1f, m_enemyMask);
    //    List<IBattle> enemyList = new List<IBattle>();
    //    foreach (Collider col in list)
    //    {
    //        IBattle ib = col.GetComponent<IBattle>();
    //        enemyList.Add(ib);
    //        Debug.Log("추가 : " + enemyList.Count);
    //        if (ib != null)
    //        {
    //            ib.OnDamaged(SkillDataManager.m_skillData["Lunge"].Dmg);
    //            enemyList.Remove(ib);
    //            Debug.Log("제거 : " + enemyList.Count);
    //        }
    //    }
    //}
}
