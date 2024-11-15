using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemData;

public interface IGetItemData
{
    void SetItemToInventory(GameObject itemPrefab);
}

public interface IMakeSlotEmpty
{
    void MakeSlotEmpty(Item item);
}

public interface IPlaceItem
{
    void PlaceItem(Item item, int posX, int posY);
}

public class Inventory : MonoBehaviour, IGetItemData, IMakeSlotEmpty, IPlaceItem, IFindEmptySlot
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
    [SerializeField]
    GameObject m_invenOnPos;
    [SerializeField]
    GameObject m_invenOffPos;
    bool m_isInvenOpen = false;
    bool m_isInvenPushKey = false;

    public Slot m_selectedItmeGrid;
    public EquipSlot m_equipSlot;
    public ItemInform m_itemInform;

    Item m_selectedItem;

    ItemData m_itemData;

    public Item[,] m_itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        Init(m_slotSizeWidth, m_slotSizeHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!m_isInvenOpen)
            {
                StopAllCoroutines();
                m_inventory.SetActive(true);
                StartCoroutine(MovingInventory(m_invenOnPos));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(MovingInventory(m_invenOffPos));
            }
            
        }
        if (m_selectedItmeGrid == null) { return; }

        //if(Input.GetMouseButtonDown(0))
        //{
        //    Vector2Int tileGridPosition = m_selectedItmeGrid.GetTileGridPosition(Input.mousePosition);
        //}
    }



    void Init(int width, int height)
    {
        m_itemSlot = new Item[width, height];
        //Vector2 size = new Vector2(width * m_tileSizeWidth, height * m_tileSizeHeight);
        Vector2 size = new Vector2(width * 47.0f, height * 47.0f);
        m_selectedItmeGrid.GetComponent<RectTransform>().sizeDelta = size;
        transform.position = m_invenOffPos.transform.position;
    }

    public void SetItemToInventory(GameObject itemImagePrefab)
    {
        //GameObject itemaa = ObjectPool.Inst.Pull<Item>(itemImagePrefab, null);
        //if (FindEmptySlot(itemaa.GetComponent<Item>()) == null)
        //{
        //    return;
        //}

        Item itemImage = Instantiate(itemImagePrefab).GetComponent<Item>();
        Vector2Int itemSlotSize = FindEmptySlot(itemImage).Value;
        if (itemSlotSize == null)
        {
            // 아이템 이동시키는 경우 : 원래 자리로 아이템 이동
            // 아이템을 획득한 경우 : 슬롯에 자리가 없으니 아이템 획득 안되게 만들기
        }
        PlaceItem(itemImage, itemSlotSize.x, itemSlotSize.y);
        //ObjectPool.Inst.Push<Item>(obj);
    }

    IEnumerator MovingInventory(GameObject obj)
    {
        Vector2 dir = obj.transform.position - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        while (dist > 0)
        {
            float delta = Time.deltaTime * 600.0f;
            if(delta > dist) delta = dist;
            transform.Translate(dir * delta);
            dist -= delta;
            yield return null;
        }
        m_isInvenOpen = !m_isInvenOpen;
        if(!m_isInvenOpen)
        {
            m_inventory.SetActive(false);
        }
    }

    #region 240712 수정중
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

    // posX : 현재 슬롯의 X 좌표    posY : 현재 슬롯의 Y 죄표    width : 아이템의 가로 길이    height : 아이템의 세로 길이
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

    public void PlaceItem(Item itemImage, int posX, int posY)
    {
        if (!BoundaryCheck(posX, posY, itemImage.m_itemData.itemWidth, itemImage.m_itemData.itemHeight))
        {
            return;
        }
        RectTransform itemPos = itemImage.GetComponent<RectTransform>();
        itemPos.SetParent(m_selectedItmeGrid.GetComponent<RectTransform>());        // 아이템 오브젝트를 Slot 오브젝트의 자식 오브젝트로 만듬
        ISetInventory isi = itemImage.GetComponent<ISetInventory>();
        if(isi != null)
        {
            isi.SetInventory(gameObject.transform);
        }

        // 슬롯에 아이템을 넣을 때 아이템 크기에 따라 차지하는 슬롯만큼 데이터 넣기
        for (int x = 0; x < itemImage.m_itemData.itemWidth; x++)
        {
            for (int y = 0; y < itemImage.m_itemData.itemHeight; y++)
            {
                m_itemSlot[posX + x, posY + y] = itemImage;
            }
        }

        itemImage.m_onGridPositionX = posX;
        itemImage.m_onGridPositionY = posY;

        Vector2 pos = new Vector2();
        if (itemImage.m_onGridPositionX == 0)
        {
            pos.x = posX * m_tileSizeWidth + 3;
            pos.y = -(posY * m_tileSizeHeight) - 2;
        }
        else
        {
            pos.x = posX * m_tileSizeWidth;
            pos.y = -(posY * m_tileSizeHeight) - 2;
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
