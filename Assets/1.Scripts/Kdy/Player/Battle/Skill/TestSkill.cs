using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestSkill : Skill
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
    public KeyCode m_skillKey;
    bool m_warPathUse = false;
    bool m_lungeUse = false;
    bool m_strikeUse = false;
    bool m_isSoundPlay = false;

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
        SoundManager.Inst.PlaySfx("ErasingStrike_Sound");
        m_erasingStrikeEffect.SetActive(true);
    }
    public void Skill_ErasingStrike_EffectOff()
    {
        m_erasingStrikeEffect.SetActive(false);
        m_usingSkill = false;
        RecoverMp(m_usingSkill);
    }


    public void Skill_WarPath(KeyCode inputKey)
    {
        // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
        // 마우스 방향으로 이동가능
        if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            if (!m_usingSkill)
            {
                if (!m_isSoundPlay)
                {
                    SoundManager.Inst.PlaySfx("WarPath_Playing_Sound");
                    m_isSoundPlay = true;
                }
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
            }

            SkillMove();
        }


        if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            m_isSoundPlay = false;
            SoundManager.Inst.StopSfxSound("WarPath_Playing_Sound");
            m_warpathEffect.SetActive(false);
            m_myAnim.SetBool("SkillWarPath", false);
            m_warPathUse = false;
            m_usingSkill = false;
            RecoverMp(m_usingSkill);
        }
    }

    public void SkillMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
        {
            StopAllCoroutines();
            StartCoroutine(SkillMoving(hit.point));
        }
    }

    public IEnumerator SkillMoving(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.Normalize();
        dir.y = 0;

        while (m_warPathUse && m_usingSkill && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
        {
            UsingSkillMp(SkillDataManager.m_skillDataDic["Warpath"].Mp * Time.deltaTime * SkillDataManager.m_skillDataDic["Warpath"].Channeling);
            float delta = Time.deltaTime * m_player.m_moveStat.moveSpeed;
            transform.Translate(dir * delta, Space.World);
            yield return null;
        }
    }



    // 출정 스킬 데미지 박스
    public void DamageBox()
    {
        // 충돌 문제 생기면 위에 걸로 아니면 이거 써도 됨
        Collider[] list = Physics.OverlapBox(m_warPathStartPos.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, m_enemyMask);
        foreach (Collider col in list)
        {
            IBattle ib = col.GetComponent<IBattle>();
            if (ib != null)
            {
                ib.SetDamage(SkillDataManager.m_skillDataDic["Warpath"]);
            }
        }
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
                SoundManager.Inst.PlaySfx("Lunge_Sound");
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
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_backgroundMask))
                {
                    m_player.StopAllCoroutines();
                    Vector3 dir = hit.point - m_player.transform.position;
                    m_player.transform.forward = dir;
                    StartCoroutine(LungeMove(dir));
                }

            }
        }
    }

    #region 실험중
    IEnumerator LungeMove(Vector3 dir)
    {
        m_player.GetComponent<Collider>().isTrigger = true;
        m_player.GetComponent<Rigidbody>().isKinematic = true;
        m_myAnim.SetBool("Move", false);
        m_myAnim.SetBool("SkillLunge", true);
        SoundManager.Inst.PlaySfx("Lunge_Sound");
        float time = 0;
        dir.Normalize();
        dir.y = 0;
        Collider[] list;
        //List<IBattle> enemyList = new List<IBattle>();
        IBattle ib;
        while (time < 0.6f)
        {
            time += Time.deltaTime;
            float delta = 5.0f * Time.deltaTime;
            list = Physics.OverlapBox(m_warPathStartPos.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, m_enemyMask);
            foreach (Collider col in list)
            {
                ib = col.GetComponent<IBattle>();
                if (ib != null)
                {
                    ib.SetDamage(SkillDataManager.m_skillDataDic["Lunge"]);
                }
            }

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
    #endregion

    void RecoverMp(bool isUsingSkill)
    {
        IRecoveryManaPoint ib = m_player.GetComponent<IRecoveryManaPoint>();
        if (ib != null)
        {
            ib.RecoveryManaPoint(isUsingSkill);
        }
    }
}
