using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public interface IEquipItemStatUI
{
    void EquipItemStat(ItemData itemData);
}

public interface IUnEquipItemStatUI
{
    void UnequipItemStat(ItemData itemData);
}

public interface IAdditionalStats
{
    Dictionary<string, float> AdditionalStats { get; set; }
}

public class PlayerStatUI : MonoBehaviour, IEquipItemStatUI, IUnEquipItemStatUI, IAdditionalStats
{
    [SerializeField]
    private Player s_player;
    [SerializeField]
    private GameObject statsPanel;   // 스탯 UI 패널 오브젝트
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private TextMeshProUGUI manaText;
    [SerializeField]
    private TextMeshProUGUI attackDmgText;
    [SerializeField]
    private TextMeshProUGUI defenseText;
    // 여러 아이템을 관리하는 리스트
    [SerializeField]
    private List<ItemData> itemDataList;

    private bool isStatsVisible = false;  // 스탯창 표시 여부
    private bool hasEquippedItem = false; // 아이템 장착 여부

    // 장착 아이템으로 증가한 스탯을 따로 저장
    private float additionalAttackDmg = 0.0f;
    private float additionalDefense = 0.0f;

    // AdditionalStats 인터페이스 구현
    public Dictionary<string, float> AdditionalStats { get; set; } = new Dictionary<string, float>();

    // 초기 스킬 데미지 저장
    private Dictionary<string, float> initialSkillDmgDict = new Dictionary<string, float>();


    // 초기 스탯 저장
    private float initialhealth;
    private float initialmana;
    private float initialAttackDmg;
    private float initialDefense;
    private float[] initialSkillDmg;

    // 색상 정의
    private Color normalColor = Color.white;  // 기본 색상
    private Color increaseColor = Color.yellow;  // 증가 시 노란색
    

    void Awake()
    {
        // 플레이어의 초기 스탯 저장
        initialAttackDmg = s_player.m_stat.AttackDmg;
        initialDefense = s_player.m_stat.Defense;

        // SkillDataManager에서 스킬 데이터를 가져와서 배열로 설정
        int skillCount = SkillDataManager.m_skillDataDic.Count;
        initialSkillDmg = new float[skillCount];



        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        if (s_player != null)
        {  // 기본 스탯은 항상 표시
            healthText.text = $"{s_player.m_stat.MaxHp}";
            manaText.text = $"{s_player.m_stat.MaxMp}";

            // 공격력과 방어력은 기본값 + 추가값으로 표시 (아이템 장착 상태에 따라 색상 변경)
            if (hasEquippedItem)
            {
                if (additionalAttackDmg > 0)
                {
                    attackDmgText.color = increaseColor;  // 노란색
                    attackDmgText.text = $"{s_player.m_stat.AttackDmg} (+{CalculateStat(initialAttackDmg, additionalAttackDmg)})";
                }
                else
                {
                    attackDmgText.color = normalColor;  // 기본색
                    attackDmgText.text = $"{s_player.m_stat.AttackDmg}";
                }

                if (additionalDefense > 0)
                {
                    defenseText.color = increaseColor;  // 노란색
                    defenseText.text = $"{s_player.m_stat.Defense} (+{CalculateStat(initialDefense,additionalDefense)})";
                }
                else
                {
                    defenseText.color = normalColor;  // 기본색
                    defenseText.text = $"{s_player.m_stat.Defense}";
                }
            }
            else
            {
                // 장착 아이템이 없을 경우 기본 색상으로 스탯만 표시
                attackDmgText.color = normalColor;
                attackDmgText.text = $"{s_player.m_stat.AttackDmg}";

                defenseText.color = normalColor;
                defenseText.text = $"{s_player.m_stat.Defense}";
            }
        }
    }

    // 증가(감소)율
    float CalculateStat(float initStat, float addStat)
    {
        return initStat * (addStat * 0.01f);
    }

    // 최종값
    float CalculateFinalStat(float initStat, float addStat)
    {
        return initStat + CalculateStat(initStat, addStat);
    }

    void ChangeStat()
    {
        s_player.m_stat.AttackDmg = CalculateFinalStat(initialAttackDmg, additionalAttackDmg);
        s_player.m_stat.Defense = CalculateFinalStat(initialDefense, additionalDefense);

        SkillDataManager.m_skillDataDic["Warpath"].Dmg = CalculateFinalStat(SkillDataManager.m_skillDataDic["Warpath"].InitDmg, additionalAttackDmg);
        SkillDataManager.m_skillDataDic["ErasingStrike"].Dmg = CalculateFinalStat(SkillDataManager.m_skillDataDic["ErasingStrike"].InitDmg, additionalAttackDmg);
        SkillDataManager.m_skillDataDic["Lunge"].Dmg = CalculateFinalStat(SkillDataManager.m_skillDataDic["Lunge"].InitDmg, additionalAttackDmg);
    }

    // 임시로 아이템 장착 후 스탯 증가시키는 함수
    public void EquipItemStat(ItemData itemData)
    {
        if (itemData != null && s_player != null)
        {
          
            // additional 값은 최종 계산된 값
            additionalAttackDmg += itemData.atkPower;
            additionalDefense += itemData.defense;

    
            ChangeStat();

            // 아이템이 장착되었다는 설정
            hasEquippedItem = true;

            // 장착 후 스탯 UI 업데이트
            UpdateStatUI();
        }
    }

    // 아이템 해제 시 스탯을 다시 감소시키는 함수
    public void UnequipItemStat(ItemData itemData)
    {
        if (itemData != null && s_player != null && hasEquippedItem)
        {
            // 장착 해제 시 추가된 스탯을 감소
            additionalAttackDmg -= itemData.atkPower;
            additionalDefense -= itemData.defense;


            ChangeStat();

            // 추가 스탯이 모두 해제되면 장착 상태 해제
            if (additionalAttackDmg <= 0 && additionalDefense <= 0)
            {
                hasEquippedItem = false;
            }

            // 해제 후 스탯 UI 업데이트
            UpdateStatUI();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isStatsVisible)
            {
                statsPanel.SetActive(true);
            }
            else
            {
                statsPanel.SetActive(false);
            }
            isStatsVisible = !isStatsVisible;
        }

        // 임시로 1번 아이템을 장착하는 예시
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (itemDataList.Count > 0)
            {
                EquipItemStat(itemDataList[0]);  // 첫 번째 아이템 장착
            }
        }

        // 임시로 2번 아이템을 장착하는 예시 
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (itemDataList.Count > 1)
            {
                EquipItemStat(itemDataList[1]);  // 두 번째 아이템 장착
            }
        }

        // 임시로 1번 아이템을 해제하는 예시 
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (itemDataList.Count > 0)
            {
                UnequipItemStat(itemDataList[0]);  // 첫 번째 아이템 해제
            }
        }

        // 임시로 2번 아이템을 해제하는 예시 
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (itemDataList.Count > 1)
            {
                UnequipItemStat(itemDataList[1]);  // 두 번째 아이템 해제
            }
        }



    }

 
}