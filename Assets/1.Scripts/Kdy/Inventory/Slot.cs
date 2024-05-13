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
    int m_gridSizeWidth = 14;       // 슬롯 가로 개수
    [SerializeField]
    int m_gridSizeHeight = 8;       // 슬롯 세로 개수
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
        Init(m_gridSizeWidth, m_gridSizeHeight);
        Item item = Instantiate(m_itemPrefab).GetComponent<Item>();
        PlaceItem(item, 3, 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        m_tileGridPosition.x = (int)(m_positionOnTheGrid.x / m_tileSizeWidth);
        m_tileGridPosition.y = (int)(m_positionOnTheGrid.y / m_tileSizeHeight);

        return m_tileGridPosition;
    }

    public bool PlaceItem(Item item, int posX, int posY)
    {
        // 아이템 데이터 만들고 주석 해제
        /*if(!BoundaryCheck(posX, posY, item.itemData.width, item.itemData.height))
        {
            return false;
        }

        RectTransform itemPos = item.GetComponent<RectTransform>();
        itemPos.SetParent(m_rectTransform);

        // 슬롯에 아이템을 넣을 때 아이템 크기에 따라 차지하는 슬롯만큼 데이터 넣기
        for (int x = 0; x < item.itemData.width; x++)
        {
            for (int y = 0; y < item.itemData.height; y++)
            {
                m_itemSlot[posX + x, posY + y] = item;
            }
        }*/

        item.m_onGridPositionX = posX;
        item.m_onGridPositionY = posY;

        Vector2 pos = new Vector2();
        pos.x = posX * m_tileSizeWidth;
        pos.y = -(posY * m_tileSizeHeight);

        //itemPos.localPosition = pos;

        return true;
    }

    // 아이템을 옮기거나 슬롯에서 빼낼 때 m_itemSlot[x, y] = null 로 초기화 해주기
    /*public Item PickUpItem(int x, int y)
    {
        Item item = m_itemSlot[x, y];

        if(item == null) { return; };

        for (int i = 0; i < item.itemData.width; i++)
        {
            for (int j = 0; j < item.itemData.height; j++)
            {
                m_itemSlot[item.m_onGridPositionX + i, item.m_onGridPositionY + j] = null;
            }
        }
        return item;
    }*/

    // 아이템의 크기가 슬롯보다 클 때 예외처리 -> true일 때만 아이템 옮기기 가능
    bool PositionCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0 || posX >= m_gridSizeWidth || posY >= m_gridSizeHeight)
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
        for (int y = 0; y < m_gridSizeHeight; y++)
        {
            for (int x = 0; x < m_gridSizeWidth; x++)
            {
                int nextSlotWidth = m_gridSizeWidth - x;
                int nextSlotHeight = m_gridSizeHeight - y;
                /*if (nextSlotWidth < item.itemData.width || nextSlotHeight < item.itemData.height)
                {
                    break;
                }*/
                /*if (CheckAvailableSpace(x, y, itemData.width, itemData.height))  // 빈 슬롯에 해당 아이템이 들어갈 수 있을 때
                {
                    return new Vector2Int(x, y);
                }*/
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
