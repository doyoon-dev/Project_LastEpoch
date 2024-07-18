using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemData;
using static UnityEditor.Progress;

public interface IChangePos
{
    void ChangePos(Vector2 pos);
}

public class Item : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IChangePos
{
    public event UnityAction m_unEquipItem = null;
    public Transform m_parentSlot = null;
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public int m_onGridPositionX;       // 인벤토리 내의 아이템 위치 x좌표
    public int m_onGridPositionY;       // 인벤토리 내의 아이템 위치 y좌표
    

    bool m_isEquiped = false;

    EquipSlot m_equipSlot;              // 장착할 아이템이 들어갈 장비 슬롯

    Vector3 m_orgPos = Vector3.zero;
    Vector2 m_dragOffset = Vector2.zero;
    Image m_image;


    public void OnBeginDrag(PointerEventData eventData)
    {
        m_orgPos = transform.position;
        m_dragOffset = (Vector2)transform.position - eventData.position;
        m_image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + m_dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = m_orgPos;
        m_image.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!m_isEquiped)
            {
                Item item = eventData.pointerClick.GetComponent<Item>();    // 슬롯에 있던 아이템
                CheckItemSlotType(item);
                // 아이템 장착할 때 아이템이 있던 슬롯 비우기
                IMakeSlotEmpty imse = m_parentSlot.GetComponent<IMakeSlotEmpty>();
                if (imse != null)
                {
                    imse.MakeSlotEmpty(item);
                }
                if (m_equipSlot.m_item != null)   // 아이템 교체 함수 넣기 (m_equipSlot.m_item : 장비슬롯에 장착된 아이템)
                {
                    // 장착아이템 교체
                    ChangeEquipItem(m_equipSlot.m_item);
                }
                //EquipItem(item);
                SetEquip();
            }
            else
            {
                UnEquipeItem(m_equipSlot.m_item);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("아이템 위치 : " + eventData.position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_parentSlot = transform.parent;    // 슬롯 변수
        m_image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 아이템이 장착 될 슬롯 찾는 함수
    EquipSlot CheckItemSlotType(Item item)
    {
        switch (item.m_itemData.itemType)
        {
            case ItemType.Head:
                CheckSlot(0);
                break;
            case ItemType.Necklace:
                CheckSlot(1);
                break;
            case ItemType.Weapon:
                CheckSlot(2);
                break;
            case ItemType.Armor:
                CheckSlot(3);
                break;
            case ItemType.Sheild:
                CheckSlot(4);
                break;
            case ItemType.Belt:
                CheckSlot(5);
                break;
            case ItemType.Ring:
                CheckSlot(6); 
                break;
            case ItemType.Shoes:
                CheckSlot(8);
                break;
            case ItemType.Hand:
                CheckSlot(9);
                break;
        }
        return m_equipSlot;
    }

    EquipSlot CheckSlot(int i)
    {
        m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[i];

        if (m_equipSlot.m_itemType == ItemType.Ring)
        {
            if (m_equipSlot.m_item != null)
            {
                m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[7];
            }
        }
        return m_equipSlot; 
    }

    // 장비 장착했을 때 아이템 위치 설정
    void SetEquip()
    {
        m_isEquiped = true;
        EquipSlotItem(m_equipSlot);
        transform.SetParent(m_equipSlot.transform);
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        //transform.position = Vector3.zero;
        //transform.localPosition = Vector3.zero;
    }

    // 장비슬롯에 장착한 아이템 장비슬롯에 저장하는 함수
    void EquipSlotItem(EquipSlot es)
    {
        ISetEquipItem sei = es.GetComponent<ISetEquipItem>();
        if (sei != null)
        {
            sei.SetEquipItem(this);
        }
    }

    // 장착 아이템 교체 함수
    void ChangeEquipItem(Item equipItem)
    {
        int equipX = equipItem.m_onGridPositionX;   // 장착중인 아이템이 슬롯에 있었을 때의 위치
        int equipY = equipItem.m_onGridPositionY;   // 장착중인 아이템이 슬롯에 있었을 때의 위치

        // 장착 해제 아이템 슬롯으로 이동
        IPlaceItem pi = m_parentSlot.transform.GetComponent<IPlaceItem>();
        if(pi != null)
        {
            pi.PlaceItem(equipItem, m_onGridPositionX, m_onGridPositionY);
            equipItem.m_isEquiped = false;
        }

        // 장착할 아이템 위치
        m_onGridPositionX = equipX;
        m_onGridPositionY = equipY;
    }

    // 장비 장착 해제 함수
    void UnEquipeItem(Item equipItem)
    {
        m_unEquipItem?.Invoke();
        m_unEquipItem = null;

        m_equipSlot.m_item = null;
        IFindEmptySlot fes = m_parentSlot.transform.GetComponent<IFindEmptySlot>();
        if (fes != null)
        {
            equipItem.m_onGridPositionX = fes.FindEmptySlot(equipItem).Value.x;
            equipItem.m_onGridPositionY = fes.FindEmptySlot(equipItem).Value.y;
        }
        IPlaceItem pi = m_parentSlot.transform.GetComponent<IPlaceItem>();
        if (pi != null)
        {
            pi.PlaceItem(equipItem, equipItem.m_onGridPositionX, equipItem.m_onGridPositionY);
            equipItem.m_isEquiped = false;
        }
    }

    public void ChangePos(Vector2 pos)
    {
        // 이동한 아이템의 위치 지정
        m_orgPos = pos;
    }
}
