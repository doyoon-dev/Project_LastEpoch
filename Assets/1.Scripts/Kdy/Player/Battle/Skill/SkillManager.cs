using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class SkillSlot
{
    public KeyCode key;
    public SkillData data;
    public float lastCastTime = -999f;

    public bool IsOnCooldown => Time.time < lastCastTime + data.CoolTime;
    public float CooldownRemaining => Mathf.Max(0f, lastCastTime + data.CoolTime - Time.time);
}

[Serializable]
public class SkillBinding
{
    public SkillData skillData;
    public Transform vfxSpawnPoint;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance { get; private set; }
    [Header("이펙트 위치 설정")]
    public List<SkillBinding> m_vfxBindings = new List<SkillBinding>();

    [Header("스킬 데이터 리스트")]
    public List<SkillSlot> slots = new List<SkillSlot>();

    [Header("스킬 슬롯")]
    public SkillSlotUI[] m_skillSlots;

    [Header("플레이어")]
    public Player m_player;

    Dictionary<SkillData, SkillSlot> m_slotMap = new Dictionary<SkillData, SkillSlot>();

    bool isPressedKey = false;
    public bool IsPressedKey => isPressedKey;

    bool isUsingSkill = false;
    public bool IsUsingSkill => isUsingSkill;

    Dictionary<KeyCode, ISkill> activeSkills = new Dictionary<KeyCode, ISkill>();
    Dictionary<SkillData, Transform> m_vfxPoints = new Dictionary<SkillData, Transform>();

    KeyCode[] m_keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < slots.Count && i < m_keys.Length; i++)
        {
            slots[i].key = m_keys[i];
        }
        SetVFXPoints();
        RefreshSlots();
    }

    private void Update()
    {
        //foreach (var slot in slots)
        //{
        //    if (slot.data == null)
        //    {
        //        continue;
        //    }

        //    if (slot.data.triggerType == SkillTriggerType.Instant)
        //    {
        //        if (Input.GetKeyDown(slot.key))
        //        {
        //            UsingSkill(gameObject, slot.key);
        //        }
        //    }
        //    else
        //    {
        //        if (Input.GetKey(slot.key))
        //        {
        //            UsingSkill(gameObject, slot.key);
        //        }
        //        if (Input.GetKeyUp(slot.key))
        //        {
        //            StopSkill(slot.key);
        //        }
        //    }
        //}
    }

    public void SetPressedKey(bool value)
    {
        isPressedKey = value;
    }

    public void SetUsingSkill(bool value)
    {
        isUsingSkill = value;
    }

    Dictionary<ISkill, Coroutine> m_skillCorDic = new Dictionary<ISkill, Coroutine>();

    public void UsingSkill(GameObject caster, KeyCode key)
    {
        if (activeSkills.TryGetValue(key, out ISkill skill))
        {
            // ISkill 인터페이스에는 없고, SkillBase에는 있는 기능을 사용하기 위해
            // ISkill로 데이터를 가져오고 ISkill 타입을 SkillBase 타입으로 변경
            // skill이 SkillBase 타입(또는 그 자식 타입)인지 확인 + 맞다면 skill을 SkillBase 타입 변수 skillBase로 변환
            if (skill is SkillBase skillBase)
            {
                skillBase.SetUseManaCallBack(m_player.m_onUseMana);
                skillBase.vfxSpawnPoint = GetVFXSpawnPoint(skillBase.data);
            }
            skill.Cast(caster);
            SkillSlot slot = m_slotMap[skill.data];
            slot.lastCastTime = Time.time;
            StartCheckSkillCooldown(skill, slot);
        }
    }

    void StartCheckSkillCooldown(ISkill skill, SkillSlot slot)
    {
        if (m_skillCorDic.ContainsKey(skill) && m_skillCorDic[skill] != null)
        {
            StopCoroutine(m_skillCorDic[skill]);
        }
        Coroutine c = StartCoroutine(CheckSkillCooldown(slot));
        m_skillCorDic[skill] = c;
    }

    IEnumerator CheckSkillCooldown(SkillSlot slot)
    {
        SkillIcon skillIcon = null;
        for (int i = 0; i < m_skillSlots.Length; i++)
        {
            if (m_skillSlots[i].m_skillData == slot.data)
            {
                skillIcon = m_skillSlots[i].m_skillIcon;
                break;
            }
        }
        skillIcon.m_cooldownImage.fillAmount = 0;
        while (slot.CooldownRemaining > 0)
        {
            skillIcon.CooldownUI(slot.CooldownRemaining);
            yield return null;
        }

        skillIcon.CooldownUI(slot.CooldownRemaining);
        skillIcon.m_cooldownImage.fillAmount = 1;
    }

    //public void StopSkill(KeyCode key)
    //{
    //    if (activeSkills.TryGetValue(key, out ISkill skill))
    //    {
    //        skill.StopCast();
    //    }
    //}

    //public float GetCooldownRemaining(int slotIndex)
    //{
    //    if (slotIndex < 0 || slotIndex >= slots.Count) return 0f;
    //    return slots[slotIndex].CooldownRemaining;
    //}

    public void RefreshSlots()
    {
        m_slotMap.Clear();
        activeSkills.Clear();
        foreach (SkillSlot slot in slots)
        {
            if (slot.data != null)
            {
                ISkill skill = SkillFactory.Create(slot.data, GetComponent<MonoBehaviour>());
                activeSkills[slot.key] = skill;
                m_slotMap[slot.data] = slot;
            }
        }
    }

    public void SetVFXPoints()
    {
        m_vfxPoints.Clear();
        foreach (SkillBinding binding in m_vfxBindings)
        {
            if (binding.skillData != null && binding.vfxSpawnPoint != null)
            {
                m_vfxPoints[binding.skillData] = binding.vfxSpawnPoint;
            }
        }
    }

    public Transform GetVFXSpawnPoint(SkillData data)
    {
        if (data != null && m_vfxPoints.TryGetValue(data, out Transform point))
        {
            return point;
        }
        return transform;
    }

    public void InputKey(KeyCode key)
    {
        PickSkill(key);
    }

    void PickSkill(KeyCode k)
    {
        foreach (var slot in slots)
        {
            if (slot.IsOnCooldown) continue;
            if (slot.key == k)
            {
                if (slot.data.triggerType == SkillTriggerType.Instant)
                {
                    UsingSkill(gameObject, slot.key);
                }
                else
                {
                    isPressedKey = true;
                    UsingSkill(gameObject, slot.key);
                }
            }
        }
    }

    public void SetSkillSlotsData()
    {
        for (int i = 0; i < m_skillSlots.Length; i++)
        {
            m_skillSlots[i].m_skillData = m_skillSlots[i].m_skillIcon.m_data;
        }
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].data = m_skillSlots[i].m_skillData;
        }
        RefreshSlots();
    }
}
