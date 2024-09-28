using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : BattleSystem
{
    #region 아이템 드랍 실험

    public GameObject[] m_dropItemPrefabs;
    public ItemData m_itemData;
    public Dictionary<string, SkillData[]> m_skillData = new Dictionary<string, SkillData[]>();

    #endregion

    [SerializeField]
    Transform m_weaponStartPoint;
    [SerializeField]
    Transform m_weaponEndPoint;
    [SerializeField]
    Transform weaponPoint;
    [SerializeField]
    GameObject obj;

    public GameObject m_hitEffect;
    public int attackDamage = 20;
    public float attackRange = 3f;
    public Inventory m_inventory;
    public PlayerUI m_playerUI;
    public LayerMask m_enemyMask;

    int m_clickCnt = 0;
    bool m_isComboCheck = false;
    Vector3 boxSize = new Vector3(1, 1, 1);
    
    // Start is called before the first frame update
    void Start()
    {
        Initalize();
        m_changeHp += SceneData.Inst.m_playerHpMpUI.HealthPoint;
        m_changeMp += SceneData.Inst.m_playerHpMpUI.ManaPoint;
        m_deadAlarm += () =>
        {
            gameObject.GetComponent<Collider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.layer = 0;
            m_myAnim.SetTrigger("Die");
            
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            //ExDropItemOnDeath();
            ExCheckDropItem(m_inventory);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_curMagicPoint = 30;
        }
        #endregion
    }

    public override void Initalize()
    {
        base.Initalize();
        SkillDataManager.m_skillDataDic["Normal"].Dmg = m_stat.AttackDmg;
    }

    public override void SetDamage(SkillData damage)
    {
        //Debug.Log($"플레이어가 {damage}의 데미지를 받았습니다.");  // 데미지 로그
        m_curHealPoint -= damage.Dmg;  // 현재 체력에서 데미지를 뺌
        // 수정필요
        GameObject obj = Instantiate(m_hitEffect);
        obj.transform.position = transform.position;
        ParticleSystem ps = obj.GetComponentInChildren<ParticleSystem>();
        ps.Play();
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
        IDeadAlarm alarm = target.GetComponent<IDeadAlarm>();
        if (alarm != null)
        {
            alarm.m_deadAlarm += () =>
            {
                StopAllCoroutines();
                m_target = null;
            };
        }
        //FollowingEnemy(target.position, m_stat.attackRange, null);
        MoveToEnemy(m_target.transform, m_stat.AttackRange, AttackAnim);
    }

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(obj.transform.position, boxSize);
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
    /* //몬스터 피격 연결코드 임시 (안됨)
    void Attack()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, m_enemyMask))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // 가상의 SkillData 객체를 생성하여 전달
                SkillData skillData = new SkillData { knockback = 5f }; // 실제로는 스킬 데이터 설정
                damageable.SetDamage(transform, skillData);
            }
        }
    }
    */


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
    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(m_weaponEndPoint.position, 0.7f);
    //}

}
