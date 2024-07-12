using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGetItemData
{
    void GetItemData(GameObject itemPrefab);
}

public class Inventory : MonoBehaviour, IGetItemData
{
    // Slot : ItemGrid 
    // Item : InventoryItem
    #region 수정중
    // 슬롯 한 칸 사이즈
    public const float m_tileSizeWidth = 47.0f;
    public const float m_tileSizeHeight = 47.0f;
    [SerializeField]
    int m_slotSizeWidth = 14;       // 슬롯 가로 개수
    [SerializeField]
    int m_slotSizeHeight = 8;       // 슬롯 세로 개수
    #endregion


    [SerializeField]
    GameObject m_inventory;
    bool m_isInvenOpen = false;

    public Slot m_selectedItmeGrid;
    public GameObject m_equipSlot;
    Item m_selectedItem;

    ItemData m_itemData;

    public Item[,] m_itemSlot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!m_isInvenOpen)
            {
                m_inventory.SetActive(true);
            }
            else
            {
                m_inventory.SetActive(false);
            }
            m_isInvenOpen = !m_isInvenOpen;
        }
        if (m_selectedItmeGrid == null) { return; }

        //if(Input.GetMouseButtonDown(0))
        //{
        //    Vector2Int tileGridPosition = m_selectedItmeGrid.GetTileGridPosition(Input.mousePosition);
        //}
    }

    // 240702 DropItem/Slot/Inventory 실험중
    //public void PlaceItem(Vector2Int tileGridPosition)
    //{
    //    bool complete = m_selectedItmeGrid.PlaceItem(m_selectedItem, tileGridPosition.x, tileGridPosition.y);
    //    if (complete)
    //    {
    //        m_selectedItem = null;
    //    }
    //}
    

    public void GetItemData(GameObject itemPrefab)
    {
        //ICreateItem ici = m_selectedItmeGrid.GetComponent<ICreateItem>();
        //if(ici != null)
        //{
        //    ici.CreateItem(itemPrefab);
        //}
        //m_itemData = itemData;
        //m_selectedItmeGrid.PlaceItem(item, itemData);
        Item item = Instantiate(itemPrefab).GetComponent<Item>();
        Vector2Int itemSlotSize = FindEmptySlot(item).Value;
        PlaceItem(item, itemSlotSize.x, itemSlotSize.y);
    }

    #region 240712 수정중
    public void PlaceItem(Item item, int posX, int posY)
    {
        if (!BoundaryCheck(posX, posY, item.m_itemData.itemWidth, item.m_itemData.itemHeight))
        {
            return;
        }
        RectTransform itemPos = item.GetComponent<RectTransform>();
        itemPos.SetParent(m_selectedItmeGrid.GetComponent<RectTransform>());

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

    public Vector2Int? FindEmptySlot(Item item)
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

    bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // ************ m_itemSlot이 null이여서 에러 뜸
                if (m_itemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 아이템을 장착 했을 때 아이템이 있던 슬롯 null로 만들기
    public void MakeSlotEmpty(Item item)
    {
        for (int y = item.m_onGridPositionY; y < item.m_itemData.itemHeight + item.m_onGridPositionY; y++)
        {
            for (int x = item.m_onGridPositionX; x < item.m_itemData.itemWidth + item.m_onGridPositionX; x++)
            {
                m_itemSlot[x, y] = null;
            }
        }
    }
    #endregion
}
