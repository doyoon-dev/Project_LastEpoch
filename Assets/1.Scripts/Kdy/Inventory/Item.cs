using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ItemData;

public class Item : MonoBehaviour
{
    public ItemData m_itemData;
    public int m_onGridPositionX;       // 인벤토리 내의 아이템 위치 x좌표
    public int m_onGridPositionY;       // 인벤토리 내의 아이템 위치 y좌표

    Slot m_slotSize;

    // string에 아이템 이름 -> 나중에 ItemData 만들면 그걸로 바꿔야함
    Dictionary<string, int[]> m_itemSlotSize = new Dictionary<string, int[]>();
    EquipSlot m_equipSlot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EquipItem();
        }
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

    void EquipItem()
    {
        IMakeSlotEmpty imse = transform.parent.GetComponent<IMakeSlotEmpty>();
        if (imse != null)
        {
            imse.MakeSlotEmpty(this);
        }
        EquipItemSetParent(this);
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
