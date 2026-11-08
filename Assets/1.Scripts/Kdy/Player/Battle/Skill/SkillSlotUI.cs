using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ISetSkillIcon
{
    void SetSkillIcon(SkillIcon icon);
}

public class SkillSlotUI : MonoBehaviour, IDropHandler, ISetSkillIcon
{
    public SkillIcon m_skillIcon;
    public int index;
    public SkillData m_skillData;

    void Start()
    {
        m_skillData = SkillManager.instance.slots[index].data;
        ISetSkillData ssd = m_skillIcon.GetComponent<ISetSkillData>();
        if (ssd != null)
        {
            ssd.SetSkillData(m_skillData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 드랍된 위치에 있는 스킬 아이콘(m_skillIcon)을 드래그 중인 스킬 아이콘(eventData.pointerDrag)이 있던 슬롯으로 위치 이동
        if (m_skillIcon != null)
        {
            IChangeSlotParent icsp = m_skillIcon.GetComponent<IChangeSlotParent>();
            if (icsp != null)
            {
                icsp.ChangeSlotParent(eventData.pointerDrag.GetComponent<IOrgParentPos>().m_orgParentPos);
            }
        }

        // 드랍된 위치에 있는 스킬아이콘(m_skillIcon)을 드래그 중인 아이콘(eventData.pointerDrag)으로 변경
        m_skillIcon = eventData.pointerDrag.GetComponent<SkillIcon>();

        #region 설명
        // 드래그 중인 스킬 아이콘을 드랍된 위치로 변경
        // 드래그 중인 오브젝트는 드랍 이벤트가 끝나면 OnEndDrag 호출이 됨
        // OnEndDrag에서 m_orgParentPos을 부모 오브젝트로 만들기 때문에
        // SkillIcon 스크립트의 m_orgParentPos를 현재 드랍된 SkillSlotUI 오브젝트로 변경
        #endregion
        IChangeOrgParentPos icopp = m_skillIcon.GetComponent<IChangeOrgParentPos>();
        if (icopp != null)
        {
            icopp.ChangeOrgParentPos(transform);
        }
    }

    public void SetSkillIcon(SkillIcon icon)
    {
        m_skillIcon = icon;
    }
}
