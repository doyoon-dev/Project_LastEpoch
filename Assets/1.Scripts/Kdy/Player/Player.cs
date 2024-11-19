using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public interface ISetClickEffect
{
    void SetClickEffect(GameObject obj);
}

public class Player : BattleSystem, ISetClickEffect
{
    #region 아이템 드랍 실험

    public GameObject[] m_dropItemPrefabs;
    public ItemData m_itemData;
    public Dictionary<string, SkillData[]> m_skillData = new Dictionary<string, SkillData[]>();

    #endregion

    #region 미니맵 카메라

    public GameObject m_minimapCam;
    public GameObject m_minimapCamPos;

    #endregion


    [SerializeField]
    Transform m_weaponStartPoint;
    [SerializeField]
    Transform m_weaponEndPoint;
    [SerializeField]
    Transform weaponPoint;
    [SerializeField]
    GameObject obj;
    [SerializeField]
    GameObject m_atkParticle;

    public event UnityAction<string> m_clickEffectPush;
    public UnityEvent m_camShake;
    public GameObject m_resurrectionObj;
    public GameObject m_hitEffect;
    public int attackDamage = 20;
    public float attackRange = 3f;
    public Inventory m_inventory;
    public PlayerUI m_playerUI;
    public LayerMask m_enemyMask;
    public GameObject m_clickEffect;
    public GameObject m_resurrectionEffect;

    int m_deathCnt = 0;
    int m_clickCnt = 0;
    bool m_isComboCheck = false;
    bool m_isDie = false;
    Vector3 boxSize = new Vector3(1, 1, 1);
    
    // Start is called before the first frame update
    void Start()
    {
        Initalize();
        m_changeHp += SceneData.Inst.m_playerHpMpUI.HealthPoint;
        m_changeMp += SceneData.Inst.m_playerHpMpUI.ManaPoint;
        m_deadAlarm += () =>
        {
            m_isDie = true;
            gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.layer = 0;
            m_myAnim.SetTrigger("Die");
            IResurrectionUIActive irua = m_resurrectionObj.GetComponent<IResurrectionUIActive>();
            if (irua != null)
            {
                m_resurrectionObj.SetActive(true);
                irua.ResurrectionUIActive();
            }
            //Debug.Log("죽음");
        };
    }

    void ExDropItemOnDeath()
    {
        int rnd = Random.Range(0, m_dropItemPrefabs.Length);
        GameObject dropItemObject = ObjectPool.Inst.Pull<DropItem>(m_dropItemPrefabs[rnd]);
        DropItem dropItem = dropItemObject.GetComponent<DropItem>();
        ItemData dropItemData = dropItemObject.GetComponent<Item>().m_itemData;
        dropItem.Initialize(dropItemData); //드롭할 아이템 데이터 설정
        dropItem.transform.position = transform.position;  // 드롭 위치 설정
        dropItem.gameObject.SetActive(true); // 드롭 아이템 활성화
    }

    public void ExCheckDropItem(Inventory inven)
    {
        int rnd = Random.Range(0, m_dropItemPrefabs.Length);
        GameObject obj = m_dropItemPrefabs[rnd];

        IFindEmptySlot ifes = inven.GetComponent<IFindEmptySlot>();
        if (ifes != null)
        {
            if (ifes.FindEmptySlot(obj.GetComponent<Item>()) != null)
            {
                IGetItemData igd = inven.GetComponent<IGetItemData>();
                if (igd != null)
                {
                    igd.SetItemToInventory(obj);
                }
                //ObjectPool.Inst.Push<Item>(gameObject);
            }
            else
            {
                // 슬롯 공간이 없을 경우 처리
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isComboCheck)
        {
            if (Input.GetMouseButtonDown(1))
            {
                m_clickCnt++;
            }
        }
        
        #region 실험코드 나중에 지워야함
        if (Input.GetKeyDown(KeyCode.T))
        {
            SkillDataManager.m_skillDataDic["Normal"].Dmg = 30;
            
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetDamage(SkillDataManager.m_skillDataDic["Normal"]);
        }
        #endregion
        m_minimapCam.transform.position = m_minimapCamPos.transform.position;
    }

    public GameObject m_icon;
    // 첫 시작, 부활 했을 때 초기화
    public override void Initalize()
    {
        base.Initalize();
        m_icon = Instantiate(Resources.Load("UI\\MinimapIcon") as GameObject, SceneData.Inst.m_minimap);
        m_icon.GetComponent<MinimapIcon>().Initialize(transform);
        if (m_isDie)
        {
            m_resurrectionEffect.SetActive(true);
            SoundManager.Inst.PlaySfx("ResurrectionSound");
            // 처음 스폰 위치 (부활 위치)
            transform.position = Vector3.zero;
            gameObject.GetComponent<Collider>().isTrigger = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.layer = 9;
            m_myAnim.SetBool("Resurrection", true);
            IInitializeUI iiu = m_resurrectionObj.GetComponent<IInitializeUI>();
            if (iiu != null)
            {
                iiu.InitializeUI();
            }
            m_isDie = false;
            Invoke("SetIdle", 1.0f);
        }
        else
        {
            SkillDataManager.m_skillDataDic["ErasingStrike"].Dmg = SkillDataManager.m_skillDataDic["ErasingStrike"].InitDmg;
            SkillDataManager.m_skillDataDic["Lunge"].Dmg = SkillDataManager.m_skillDataDic["Lunge"].InitDmg;
            SkillDataManager.m_skillDataDic["Warpath"].Dmg = SkillDataManager.m_skillDataDic["Warpath"].InitDmg;
        }
        
    }

    void SetIdle()
    {
        m_myAnim.SetBool("Resurrection", false);
    }

    public override void SetDamage(SkillData damage)
    {
        SoundManager.Inst.PlaySfx("Hit_Sound");
        m_camShake?.Invoke();
        //Debug.Log($"플레이어가 {damage}의 데미지를 받았습니다.");  // 데미지 로그
        m_curHealPoint -= damage.Dmg;  // 현재 체력에서 데미지를 뺌
        // 수정필요
        //GameObject obj = Instantiate(m_hitEffect);
        //obj.transform.position = transform.position;
        //ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();
        //ps.Play();
        ShowDamageText(damage.Dmg, Color.red);
        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            //Debug.Log("플레이어가 죽었습니다.");
            Dead();  // 플레이어 사망 처리
        }
        else
        {
            //Debug.Log($"플레이어의 남은 체력: {m_curHealPoint}");
        }
    }

    // Enemy한테 이동 후 공격
    // 공격 범위에 들어왔을 때 멈추고 공격
    public void AttackTarget(Transform target)//(Transform target)
    {
        m_target = target.GetComponent<IBattle>();
        //IDeadAlarm alarm = target.GetComponent<IDeadAlarm>();
        //if (alarm != null)
        //{
        //    alarm.m_deadAlarm += () =>
        //    {
        //        StopAllCoroutines();
        //        m_target = null;
        //    };
        //}
        //FollowingEnemy(target.position, m_stat.attackRange, null);
        MoveToEnemy(m_target.transform, m_stat.AttackRange, AttackAnim);
    }

    #region 콤보체크
    // 첫 공격 이후 다음 공격 모션 바뀜
    void FirstAttack()
    {
        m_myAnim.SetBool("ComboOn", true);
        m_myAnim.SetBool("Attack", false);
    }

    // 두 번째 공격 이후 첫 번째 공격 모션으로 바뀜
    public void SecondAttack()
    {
        m_myAnim.SetBool("ComboOn", false);
        m_myAnim.SetBool("Attack", false);
    }

    public void ComboCheckStart()
    {
        m_isComboCheck = true;
        FirstAttack();
        m_myAnim.SetBool("ComboCheck", false);
        m_clickCnt = 0;
    }

    public void ComboCheckEnd()
    {
        m_isComboCheck = false;
        m_myAnim.SetBool("Attack", false);
        if (m_clickCnt > 0)
        {
            m_myAnim.SetBool("ComboCheck", true);
        }
    }
    #endregion

    public void AttackEffectOn()
    {
        SoundManager.Inst.PlaySfx("ATTACK1");
        ParticleSystem ps = m_atkParticle.GetComponentInChildren<ParticleSystem>();
        m_atkParticle.SetActive(true);
        ps.Play();
    }

    public void AttackEffectOff()
    {
        ParticleSystem ps = m_atkParticle.GetComponentInChildren<ParticleSystem>();
        ps.Stop();
        m_atkParticle.SetActive(false);
    }

    public override IEnumerator Moving(Vector3 target, GameObject obj)
    {
        Vector3 dir = target - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        dir.y = 0;
        m_myAnim.SetBool("Move", true);
        StartCoroutine(Rotating(dir));
        // 미니맵 아이콘 회전 추가
        m_icon.GetComponent<MinimapIcon>().Rotate(target);
        while (dist > 0.0f)
        {
            float delta = Time.deltaTime * m_moveStat.moveSpeed;
            if (delta > dist) delta = dist;
            transform.Translate(dir * delta, Space.World);
            //m_cam.transform.Translate(dir * delta, Space.World);
            dist -= delta;
            yield return null;
        }
        m_clickEffectPush?.Invoke(typeof(ClickEffectPool).Name);
        m_myAnim.SetBool("Move", false);
        
    }

    public void SetClickEffect(GameObject obj)
    {
        m_clickEffect = obj;
    }

    public void StopCoroutineFunc()
    {
        if (m_clickEffect != null)
        {
            ObjectPool.Inst.Push<GameObject>(m_clickEffect);
            m_clickEffect = null;
        }
        StopAllCoroutines();
    }

    //public override void OnAttack(Vector3 pos)
    //{
    //    base.OnAttack(pos);
    //    Collider[] list = Physics.OverlapSphere(m_weaponEndPoint.position, 0.06f, m_enemyMask);
    //    Collider[] box = Physics.OverlapBox(obj.transform.position, boxSize * 0.5f, gameObject.transform.rotation ,m_enemyMask);
    //    foreach (Collider col in box)
    //    {
    //        // 충돌한 col에 BattleSystem 컴포넌트가 없기 때문에 bat이 null이됨
    //        // 충돌한 col에 BattleSystem 컴포넌트 넣으면 해결
    //        IOnDamaged ms = col.GetComponent<IOnDamaged>();
    //        if (ms != null)
    //        {
    //            ms.OnDamaged(m_stat.AttackDmg);
    //        }
    //    }
    //}

    public override void Attack()
    {
        Collider[] box = Physics.OverlapBox(obj.transform.position, boxSize * 0.5f, gameObject.transform.rotation, m_enemyMask);
        foreach (Collider col in box)
        {
            IDamageable id = col.GetComponent<IDamageable>();
            if (id != null)
            {
                id.SetDamage(SkillDataManager.m_skillDataDic["Normal"]);
            }
        }
    }
    
    public void FirstMoveSound()
    {
        SoundManager.Inst.PlaySfx("move1");
    }

    public void SecondMoveSound()
    {
        SoundManager.Inst.PlaySfx("move2");
    }
}
