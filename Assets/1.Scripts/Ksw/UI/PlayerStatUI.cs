using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEquipItemStatUI
{
    void EquipItemStat(ItemData itemData);
}

public interface IUnEquipItemStatUI
{
    void UnequipItemStat(ItemData itemData);
}

public class PlayerStatUI : MonoBehaviour, IEquipItemStatUI, IUnEquipItemStatUI
{
    [SerializeField]
    private Player s_player;
    [SerializeField]
    private GameObject statsPanel;   // 스탯 UI 패널 오브젝트
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Text manaText;
    [SerializeField]
    private Text attackDmgText;
    [SerializeField]
    private Text defenseText;
    // 여러 아이템을 관리하는 리스트
    [SerializeField]
    private List<ItemData> itemDataList;
    private bool isStatsVisible = false;  // 스탯창 표시 여부
    private bool hasEquippedItem = false; // 아이템 장착 여부

    // 장착 아이템으로 증가한 스탯을 따로 저장
    private float additionalAttackDmg = 0.0f;
    private float additionalDefense = 0.0f;

    // 초기 스탯 저장
    private float initialhealth;
    private float initialmana;
    private float initialAttackDmg;
    private float initialDefense;


    // 색상 정의
    private Color normalColor = Color.white;  // 기본 색상
    private Color increaseColor = Color.yellow;  // 증가 시 노란색
    

    void Start()
    {
        // 플레이어의 초기 스탯 저장
        initialAttackDmg = s_player.m_stat.AttackDmg;
        initialDefense = s_player.m_stat.Defense;

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
                    attackDmgText.text = $"{s_player.m_stat.AttackDmg} (+{additionalAttackDmg})";
                }
                else
                {
                    attackDmgText.color = normalColor;  // 기본색
                    attackDmgText.text = $"{s_player.m_stat.AttackDmg}";
                }

                if (additionalDefense > 0)
                {
                    defenseText.color = increaseColor;  // 노란색
                    defenseText.text = $"{s_player.m_stat.Defense} (+{additionalDefense})";
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
    // 임시로 아이템 장착 후 스탯 증가시키는 함수
    public void EquipItemStat(ItemData itemData)
    {
        if (itemData != null && s_player != null)
        {
            // 추가된 공격력과 방어력을 별도로 저장
            additionalAttackDmg += itemData.atkPower;
            additionalDefense += itemData.defense;

            // 장착 시 공격력과 방어력 증가
            s_player.m_stat.AttackDmg = initialAttackDmg + additionalAttackDmg;
            s_player.m_stat.Defense = initialDefense + additionalDefense;
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

            // 장착 해제 후 스탯을 다시 초기 값에 추가된 값으로 설정
            s_player.m_stat.AttackDmg = initialAttackDmg + Mathf.Max(additionalAttackDmg, 0);
            s_player.m_stat.Defense = initialDefense + Mathf.Max(additionalDefense, 0);

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
        if (Input.GetKeyDown(KeyCode.K))
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