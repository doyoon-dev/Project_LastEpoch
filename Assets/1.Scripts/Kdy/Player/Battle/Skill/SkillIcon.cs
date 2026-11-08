using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ISetSkillData
{
    void SetSkillData(SkillData data);
}

public interface IChangeOrgParentPos
{
    void ChangeOrgParentPos(Transform target);
}

public interface IChangeSlotParent
{
    void ChangeSlotParent(Transform target);
}

public interface IOrgParentPos
{
    Transform m_orgParentPos { get; }
}

public class SkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IOrgParentPos, IChangeSlotParent, IChangeOrgParentPos, ISetSkillData
{
    public Image m_icon;
    public Image m_cooldownImage;
    public TextMeshProUGUI m_remainingTimeText;

    public Transform m_orgParentPos { get; private set; }

    public SkillData m_data;

    Vector2 m_dragOffset = Vector2.zero;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_orgParentPos = transform.parent;
        transform.SetParent(transform.parent.parent.parent);
        m_dragOffset = (Vector2)transform.position - eventData.position;
        m_icon.raycastTarget = false;

        m_orgParentPos.GetComponent<ISetSkillIcon>()?.SetSkillIcon(null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + m_dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(m_orgParentPos);
        transform.localPosition = Vector3.zero;
        m_icon.raycastTarget = true;
        m_orgParentPos.GetComponent<ISetSkillIcon>()?.SetSkillIcon(this);
        SkillManager.instance.SetSkillSlotsData();
    }

    public void ChangeSlotParent(Transform target)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        target.GetComponent<ISetSkillIcon>()?.SetSkillIcon(this);
    }

    public void ChangeOrgParentPos(Transform target)
    {
        m_orgParentPos = target;
    }


    public void SetSkillData(SkillData data)
    {
        m_data = data;
    }

    public void CooldownUI(float cooldown)
    {
        if (cooldown > 0)
        {
            m_cooldownImage.fillAmount = 1 - (cooldown / m_data.CoolTime);
            m_remainingTimeText.text = cooldown.ToString("F1");
        }
        else
        {
            m_remainingTimeText.text = "";
        }
    }
}
