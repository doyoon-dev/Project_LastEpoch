using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int m_itemSizeWidth = 1;
    public int m_itemSizeHeight = 1;

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
        if (Input.GetKeyDown(KeyCode.J))
        {
            ItemSize("Knife", 2, 4);
            Debug.Log(m_itemSlotSize.Values);
        }
    }

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
}
