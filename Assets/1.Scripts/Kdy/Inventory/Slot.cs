using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

//public interface IPlaceItem
//{
//    bool PlaceItem(Item item, int posX, int posY);
//}

//public interface IMakeSlotEmpty
//{
//    void MakeSlotEmpty(Item item);
//}

public interface IFindEmptySlot
{
    Vector2Int? FindEmptySlot(Item item);
}

public interface ICreateItem
{
    void CreateItem(GameObject dropItemPrefab);
}

public interface ISlotInterface : ICreateItem { } //IMakeSlotEmpty, IPlaceItem, IFindEmptySlot

public class Slot : MonoBehaviour, IDropHandler//, ISlotInterface
{
    // 슬롯 한 칸 사이즈 원래 47이였는데 문제생겨서 27로 바꿈
    public const float m_tileSizeWidth = 27.0f;
    public const float m_tileSizeHeight = 27.0f;

    [SerializeField]
    int m_slotSizeWidth = 14;       // 슬롯 가로 개수
    [SerializeField]
    int m_slotSizeHeight = 8;       // 슬롯 세로 개수
    [SerializeField]
    GameObject m_itemPrefab;        // 슬롯에 들어갈 아이템
    [SerializeField]
    Inventory m_inven;

    RectTransform m_rectTransform;
    Vector2 m_positionOnTheGrid = new Vector2();            // 스크린 좌표 기준 슬롯 한 칸 좌표
    Vector2Int m_tileGridPosition = new Vector2Int();       // 슬롯 기준 슬롯 한 칸 좌표

    //public Item[,] m_itemSlot;                  // 240702 public 실험중 원래 public 아님

    [SerializeField]
    public EquipSlot[] m_equipSlot;

    public void OnDrop(PointerEventData eventData)
    {
        // 아이템을 놓는 슬롯의 좌표 가져오기
        // 아이템을 해당 슬롯에 놓기
        // item.transform.position : 드랍했을 때의 아이템 위치
        Item item = eventData.pointerDrag.GetComponent<Item>();

        // 원래 아이템 위치
        //Vector3 itemPos = eventData.pointerDrag.GetComponent<IOrgPos>().m_orgPos;

        //Vector2Int pos = GetTileGridPosition(item.transform.position);
        Vector2Int pos = GetTileGridPosition(Input.mousePosition);
        int posX = pos.x;
        int posY = pos.y;
        //Debug.Log("좌표 : " + "( " + posX + " , " + posY + " )");

        // 아래 함수들 인벤토리 스크립트로 옮겨서 인덱스 에러뜸 수정필요
        IMakeSlotEmpty imsm = m_inven.GetComponent<IMakeSlotEmpty>();
        if (imsm != null)
        {
            imsm.MakeSlotEmpty(item);
        }

        // posX, posY 변수 : 아이템을 드랍한 슬롯의 위치의 x, y 좌표
        if (!m_inven.CheckAvailableSpace(item, posX, posY, item.m_itemData.itemWidth, item.m_itemData.itemHeight)) return;

        IPlaceItem ipi = m_inven.GetComponent<IPlaceItem>();
        if(ipi != null)
        {
            ipi.PlaceItem(item, posX, posY);
        }

        IChangePos cp = item.GetComponent<IChangePos>();
        if (cp != null)
        {
            cp.ChangePos(item.transform.position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        //Init(m_slotSizeWidth, m_slotSizeHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log("마우스 포지션 : " + Input.mousePosition);
            //Debug.Log("슬롯 포지션 : " + m_rectTransform.position);
            //Debug.Log("그리드 좌표 : " + "( " + (Input.mousePosition.x - m_rectTransform.position.x) + " , " + (Input.mousePosition.y - m_rectTransform.position.y) + " )");
            Debug.Log("최종 좌표 : " + GetTileGridPosition(Input.mousePosition).x + " , " + GetTileGridPosition(Input.mousePosition).y);
            //Debug.Log("-----------------------------------------");
        }
    }

    //void Init(int width, int height)
    //{
    //    m_itemSlot = new Item[width, height];
    //    Vector2 size = new Vector2(width * m_tileSizeWidth, height * m_tileSizeHeight);
    //    m_rectTransform.sizeDelta = size;
    //}

    // 슬롯 좌표
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        m_positionOnTheGrid.x = mousePosition.x - m_rectTransform.position.x;
        m_positionOnTheGrid.y = m_rectTransform.position.y - mousePosition.y;

        m_tileGridPosition.x = (int)(m_positionOnTheGrid.x / m_tileSizeWidth);
        m_tileGridPosition.y = (int)(m_positionOnTheGrid.y / m_tileSizeHeight);

        return m_tileGridPosition;
    }

    /*
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
    */
    
    #region 240712 인벤토리 스크립트로 이동중
    /*
    public void CreateItem(GameObject dropItemPrefab)
    {
        Item item = Instantiate(dropItemPrefab).GetComponent<Item>();
        Vector2Int itemSlotSize = FindEmptySlot(item).Value;
        PlaceItem(item, itemSlotSize.x, itemSlotSize.y);
    }

    // 아이템 슬롯에 넣기
    // 나중에 불린 함수로 바꿔서 Inventory 스크립트에서 호출해서 true일 때 아이템 들어가도록 만듬(영상에서)
    // Inventory 스크립트로 옮겨야 인벤토리가 꺼져있을 때도 아이템이 슬롯으로 들어감
    public bool PlaceItem(Item item, int posX, int posY)
    {
        if (!BoundaryCheck(posX, posY, item.m_itemData.itemWidth, item.m_itemData.itemHeight))
        {
            return false;
        }
        RectTransform itemPos = item.GetComponent<RectTransform>();
        itemPos.SetParent(m_rectTransform);

        // 슬롯에 아이템을 넣을 때 아이템 크기에 따라 차지하는 슬롯만큼 데이터 넣기
        for (int x = 0; x < item.m_itemData.itemWidth; x++)
        {
            for (int y = 0; y < item.m_itemData.itemHeight; y++)
            {
                //m_itemSlot[posX + x, posY + y] = item;
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
        return true;
    }

    // 아이템의 크기가 슬롯보다 클 때 예외처리 -> true일 때만 아이템 옮기기 가능
    bool PositionCheck(int posX, int posY)
    {
        // 슬롯 밖으로 아이템을 드랍하면 인덱스 에러 발생
        //if (m_itemSlot[posX, posY] != null) { return false; }

        if (posX < 0 || posY < 0 || posX >= m_slotSizeWidth || posY >= m_slotSizeHeight)
        {
            return false;
        }
        return true;
    }

    // 240702 public 실험중 원래 public 아님
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (!PositionCheck(posX, posY)) { return false; }

        posX += width - 1;
        posY += height - 1;

        if (!PositionCheck(posX, posY)) { return false; }

        return true;
    }

    // 찾은 슬롯의 빈 공간의 좌표 가져오기
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

    // 슬롯에 아이템을 넣을 수 있는 공간 찾기
    bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for(int y =  0; y < height; y++)
            {
                //if (m_itemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 아이템을 장착 했을 때 아이템이 있던 슬롯 null로 만들기
    //public void MakeSlotEmpty(Item item)
    //{
    //    for (int y = item.m_onGridPositionY; y < item.m_itemData.itemHeight + item.m_onGridPositionY; y++)
    //    {
    //        for (int x = item.m_onGridPositionX; x < item.m_itemData.itemWidth + item.m_onGridPositionX; x++)
    //        {
    //            m_itemSlot[x, y] = null;
    //        }
    //    }
    //}
    */
    #endregion
}
