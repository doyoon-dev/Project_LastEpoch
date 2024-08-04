using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetItemToList
{
    void GetItemToList(ItemData itemData);
}

public class InventoryTest : MonoBehaviour, IGetItemToList
{
    List<Item> m_itemList = new List<Item>();
    Dictionary<string, List<Item>> m_itemDic = new Dictionary<string, List<Item>>();
    Item[,] m_itemSlot;

    public const float m_tileSizeWidth = 47.0f;
    public const float m_tileSizeHeight = 47.0f;
    [SerializeField]
    int m_slotSizeWidth = 14;       // 슬롯 가로 개수
    [SerializeField]
    int m_slotSizeHeight = 8;       // 슬롯 세로 개수

    Vector2 m_positionOnTheGrid = new Vector2();
    Vector2Int m_tileGridPosition = new Vector2Int();
    RectTransform m_rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_itemSlot = new Item[m_slotSizeWidth, m_slotSizeHeight];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        m_positionOnTheGrid.x = mousePosition.x - m_rectTransform.position.x;
        m_positionOnTheGrid.y = m_rectTransform.position.y - mousePosition.y;

        m_tileGridPosition.x = (int)(m_positionOnTheGrid.x / m_tileSizeWidth);
        m_tileGridPosition.y = (int)(m_positionOnTheGrid.y / m_tileSizeHeight);

        return m_tileGridPosition;
    }

    int count = 0;
    public void GetItemToList(ItemData itemData)
    {
        // 1. 리스트에 아이템 데이터만 넣고 인벤토리가 열리면 그 때 아이템 생성하고 슬롯에 아이템 넣음
        // 2. 아이템 생성해서 리스트에 넣고 인벤토리가 열리면 슬롯에 아이템 넣음
        // 아이템을 슬롯에 넣기 전에 아이템이 슬롯에 들어갈 자리가 있는지 체크 먼저 해야함
        Debug.Log("딕셔너리 개수 : " + m_itemDic.Count);
        if (FindEmptySlot(itemData.itemImagePrefab.GetComponent<Item>()) == null)
        {
            Debug.Log("슬롯 꽉참");
            return;
        }
        int x = FindEmptySlot(itemData.itemImagePrefab.GetComponent<Item>()).Value.x;
        int y = FindEmptySlot(itemData.itemImagePrefab.GetComponent<Item>()).Value.y;
        for (int i = x; i < itemData.itemWidth + x; i++)
        {
            for (int j = y; j < itemData.itemHeight + y; j++)
            {
                m_itemSlot[i, j] = itemData.itemImagePrefab.GetComponent<Item>();
                m_itemList.Add(m_itemSlot[i, j]);
            }
        }
        if (m_itemDic.ContainsKey(itemData.name))
        {
            count++;
            m_itemDic.Add(itemData.name + ("%d", count), m_itemList);
        }
        else
        {
            m_itemDic.Add(itemData.name, m_itemList);
        }
        

        //Debug.Log(m_itemDic["Zweihander"].Count);
        //Item itemImage = Instantiate(itemImagePrefab).GetComponent<Item>();
        //int itemWidth = itemImage.m_itemData.itemWidth;
        //int itemHeight = itemImage.m_itemData.itemHeight;
        //if (CheckSlot())
        //{
        //    m_itemList.Add(itemImage);
        //}

    }

    public Vector2Int? FindEmptySlot(Item itemImage)
    {
        // itemImage : 슬롯에 들어갈 획득한 아이템 이미지
        // 빈 슬롯 있는지 전체 슬롯 검사
        for (int y = 0; y < m_slotSizeHeight; y++)
        {
            for (int x = 0; x < m_slotSizeWidth; x++)
            {
                int nextSlotWidth = m_slotSizeWidth - x;
                int nextSlotHeight = m_slotSizeHeight - y;
                if (nextSlotWidth < itemImage.m_itemData.itemWidth || nextSlotHeight < itemImage.m_itemData.itemHeight)
                {
                    break;
                }
                if (CheckAvailableSpace(itemImage, x, y, itemImage.m_itemData.itemWidth, itemImage.m_itemData.itemHeight))  // 빈 슬롯에 해당 아이템이 들어갈 수 있을 때
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    public bool CheckAvailableSpace(Item itemImage, int posX, int posY, int width, int height)
    {
        if (!BoundaryCheck(posX, posY, itemImage.m_itemData.itemWidth, itemImage.m_itemData.itemHeight))
        {
            return false;
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // ************ 여러번 이동시키면? 어떤 위치로 이동시키면? index 에러 뜸
                if (m_itemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (!PositionCheck(posX, posY)) { return false; }

        posX += width - 1;
        posY += height - 1;

        if (!PositionCheck(posX, posY)) { return false; }

        return true;
    }

    bool PositionCheck(int posX, int posY)
    {
        if (m_itemSlot[posX, posY] != null) { return false; }

        if (posX < 0 || posY < 0 || posX >= m_slotSizeWidth || posY >= m_slotSizeHeight)
        {
            return false;
        }
        return true;
    }
}
