using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    // 슬롯 한 칸 사이즈
    public const float m_tileSizeWidth = 47.0f;
    public const float m_tileSizeHeight = 47.0f;

    [SerializeField]
    int m_slotSizeWidth = 14;       // 슬롯 가로 개수
    [SerializeField]
    int m_slotSizeHeight = 8;       // 슬롯 세로 개수
    [SerializeField]
    GameObject m_itemPrefab;        // 슬롯에 들어갈 아이템

    RectTransform m_rectTransform;
    Vector2 m_positionOnTheGrid = new Vector2();            // 스크린 좌표 기준 슬롯 한 칸 좌표
    Vector2Int m_tileGridPosition = new Vector2Int();       // 슬롯 기준 슬롯 한 칸 좌표

    Item[,] m_itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        Init(m_slotSizeWidth, m_slotSizeHeight);
        Item item = Instantiate(m_itemPrefab).GetComponent<Item>();
        Vector2Int itemSlotSize = FindEmptySlot(item).Value;
        PlaceItem(item, itemSlotSize.x, itemSlotSize.y);
        
    }

    // Update is called once per frame
    void Update()
    {
        // 실험중
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (CheckSlot())
            {
                Item item = Instantiate(m_itemPrefab).GetComponent<Item>();
                Vector2Int itemSlotSize = FindEmptySlot(item).Value;
                PlaceItem(item, itemSlotSize.x, itemSlotSize.y);
            }
        }
    }

    // 실험중
    bool CheckSlot()
    {
        int size = 0;
        for (int i = 0; i < m_slotSizeWidth; i++)
        {
            for (int j = 0; j < m_slotSizeHeight; j++)
            {
                if (m_itemSlot[i, j] != null)
                {
                    size++;
                }
            }
        }
        if (size >= 112)
        {
            return false;
        }
        return true;
    }

    void Init(int width, int height)
    {
        m_itemSlot = new Item[width, height];
        Vector2 size = new Vector2(width * m_tileSizeWidth, height * m_tileSizeHeight);
        m_rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        m_positionOnTheGrid.x = mousePosition.x - m_rectTransform.position.x;
        m_positionOnTheGrid.y = m_rectTransform.position.y - mousePosition.y;
        Debug.Log(m_positionOnTheGrid);

        m_tileGridPosition.x = (int)(m_positionOnTheGrid.x / m_tileSizeWidth);
        m_tileGridPosition.y = (int)(m_positionOnTheGrid.y / m_tileSizeHeight);

        return m_tileGridPosition;
    }

    // 아이템 슬롯에 넣기
    // 나중에 불린 함수로 바꿔서 Inventory 스크립트에서 호출해서 true일 때 아이템 들어가도록 만듬(영상에서)
    public void PlaceItem(Item item, int posX, int posY)
    {
        if (!BoundaryCheck(posX, posY, item.m_itemData.itemWidth, item.m_itemData.itemHeight))
        {
            return;
        }
        RectTransform itemPos = item.GetComponent<RectTransform>();
        itemPos.SetParent(m_rectTransform);

        // 슬롯에 아이템을 넣을 때 아이템 크기에 따라 차지하는 슬롯만큼 데이터 넣기
        for (int x = 0; x < item.m_itemData.itemWidth; x++)
        {
            for (int y = 0; y < item.m_itemData.itemHeight; y++)
            {
                m_itemSlot[posX + x, posY + y] = item;
            }
        }

        item.m_onGridPositionX = posX;
        item.m_onGridPositionY = posY;

        Vector2 pos = new Vector2();
        if (item.m_onGridPositionX == 0)
        {
            pos.x = posX * m_tileSizeWidth + 3;
            pos.y = -(posY * m_tileSizeHeight) - 2;
        }
        else
        {
            pos.x = posX * m_tileSizeWidth;
            pos.y = -(posY * m_tileSizeHeight);
        }
        itemPos.localPosition = pos;
        //return true;
    }

    // 아이템을 옮기거나 슬롯에서 빼낼 때 m_itemSlot[x, y] = null 로 초기화 해주기
    public Item PickUpItem(int x, int y)
    {
        Item item = m_itemSlot[x, y];

        if(item == null) { return null; };

        for (int i = 0; i < item.m_itemData.itemWidth; i++)
        {
            for (int j = 0; j < item.m_itemData.itemHeight; j++)
            {
                m_itemSlot[item.m_onGridPositionX + i, item.m_onGridPositionY + j] = null;
            }
        }
        return item;
    }

    // 아이템의 크기가 슬롯보다 클 때 예외처리 -> true일 때만 아이템 옮기기 가능
    bool PositionCheck(int posX, int posY)
    {
        if (m_itemSlot[posX, posY] != null) { return false; }

        if (posX < 0 || posY < 0 || posX >= m_slotSizeWidth || posY >= m_slotSizeHeight)
        {
            return false;
        }
        return true;
    }
    bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (!PositionCheck(posX, posY)) { return false; }

        posX += width - 1;
        posY += height - 1;

        if (!PositionCheck(posX, posY)) { return false; }

        return true;
    }

    // 찾은 슬롯의 빈 공간의 좌표 가져오기
    Vector2Int? FindEmptySlot(Item item)
    {
        // item : 획득한 아이템
        for (int y = 0; y < m_slotSizeHeight; y++)
        {
            for (int x = 0; x < m_slotSizeWidth; x++)
            {
                int nextSlotWidth = m_slotSizeWidth - x;
                int nextSlotHeight = m_slotSizeHeight - y;
                if (nextSlotWidth < item.m_itemData.itemWidth || nextSlotHeight < item.m_itemData.itemHeight)
                {
                    break;
                }
                if (CheckAvailableSpace(x, y, item.m_itemData.itemWidth, item.m_itemData.itemHeight))  // 빈 슬롯에 해당 아이템이 들어갈 수 있을 때
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    // 슬롯에 아이템을 넣을 수 있는 공간 찾기
    bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for(int y =  0; y < height; y++)
            {
                if (m_itemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
