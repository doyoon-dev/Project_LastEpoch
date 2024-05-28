using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData m_itemData;
    public int m_onGridPositionX;       // 인벤토리 내의 아이템 위치 x좌표
    public int m_onGridPositionY;       // 인벤토리 내의 아이템 위치 y좌표

    Slot m_slotSize;

    // string에 아이템 이름 -> 나중에 ItemData 만들면 그걸로 바꿔야함
    Dictionary<string, int[]> m_itemSlotSize = new Dictionary<string, int[]>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EquipItem();
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
        if (Input.GetMouseButtonDown(1))
        {
            IMakeSlotEmpty imse = transform.parent.GetComponent<IMakeSlotEmpty>();
            if(imse != null)
            {
                imse.MakeSlotEmpty(this);
            }
            // 현재 아이템의 Layer나 이름을 비교해서 부모 오브젝트로 만들 게임오브젝트 골라야함
            transform.SetParent(transform.parent.parent);       // 아이템의 부모 오브젝트를 Slot -> EquipSlot으로 변경
            // 
            transform.localPosition = Vector3.zero;
        }
    }
}
