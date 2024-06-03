using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemData;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public int m_onGridPositionX;       // 인벤토리 내의 아이템 위치 x좌표
    public int m_onGridPositionY;       // 인벤토리 내의 아이템 위치 y좌표

    Slot m_slotSize;

    // string에 아이템 이름 -> 나중에 ItemData 만들면 그걸로 바꿔야함
    Dictionary<string, int[]> m_itemSlotSize = new Dictionary<string, int[]>();
    EquipSlot m_equipSlot;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Item item = eventData.pointerClick.GetComponent<Item>();
            EquipItem(item);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 아이템 사이즈 구하는 부분 만드는중(영상 없는 부분)
    void ItemSize(string name, int itemSizeX, int itemSizeY)
    {
        int[,] slotSize = new int[itemSizeX, itemSizeY];
        for (int i = 0; i < itemSizeX; i++)
        {
            for(int j = 0; j < itemSizeY; j++)
            {
                int slotSizeX = (int)transform.localPosition.x + i;
                int slotSizeY = (int)transform.localPosition.y + j;
                //slotSize = new int[] { slotSizeX, slotSizeY };
                
            }
        }
        //m_itemSlotSize.Add(name, slotSize[,]);
    }

    void EquipItem(Item item)
    {
        // 장착 전에 슬롯이 비어있는지 확인하고
        // 비어있다면 아래 코드 실행
        // 꽉 찼다면 리턴
        //if (m_equipSlot.m_item != null) { return; }
        //CheckEmptyEquipSlot(item);
        IMakeSlotEmpty imse = transform.parent.GetComponent<IMakeSlotEmpty>();
        if (imse != null)
        {
            imse.MakeSlotEmpty(item);
        }
        EquipItemSetParent(item);
    }

    void EquipItemSetParent(Item item)
    {
        switch (item.m_itemData.itemType)
        {
            case ItemType.Head:
                SetEquip(0);
                break;
            case ItemType.Necklace:
                SetEquip(1);
                break;
            case ItemType.Weapon:
                SetEquip(2);
                break;
            case ItemType.Armor:
                SetEquip(3);
                break;
            case ItemType.Sheild:
                SetEquip(4);
                Debug.Log("실드 장착");
                break;
            case ItemType.Belt:
                SetEquip(5);
                Debug.Log("벨트 장착");
                break;
            case ItemType.Ring:
                SetEquip(6);
                break;
            case ItemType.Shoes:
                SetEquip(8);
                break;
            case ItemType.Hand:
                SetEquip(9);
                break;
        }
    }

    void CheckEmptyEquipSlot(Item item)
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
    }

    void CheckSlot(int i)
    {
        m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[i];

        if (m_equipSlot.m_itemType == ItemType.Ring)
        {
            if (m_equipSlot.m_item != null)
            {
                m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[7];
            }
        }
        if (m_equipSlot.m_item == null)
        {

        }
    }

    void SetEquip(int i)
    {
        m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[i];
        
        if (m_equipSlot.m_itemType == ItemType.Ring)
        {
            if (m_equipSlot.m_item != null)
            {
                m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[7];
            }
        }

        EquipItem(m_equipSlot);
        transform.SetParent(m_equipSlot.transform);
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        //transform.position = Vector3.zero;
        //transform.localPosition = Vector3.zero;
    }

    void EquipItem(EquipSlot es)
    {
        ISetEquipItem sei = es.GetComponent<ISetEquipItem>();
        if (sei != null)
        {
            sei.SetEquipItem(this);
        }
    }
}
