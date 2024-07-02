using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Slot : ItemGrid 
    // Item : InventoryItem
    [SerializeField]
    GameObject m_inventory;
    bool m_isInvenOpen = false;

    public Slot m_selectedItmeGrid;
    public GameObject m_equipSlot;
    Item m_selectedItem;

    [SerializeField]
    List<Item> m_items = new List<Item>();

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

        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int tileGridPosition = m_selectedItmeGrid.GetTileGridPosition(Input.mousePosition);
        }
    }

    // 드랍한 아이템 유니티 이벤트로 아래 함수 호출해서 아이템 저장 후 슬롯에 넣기
    void GetDropItem()
    {

    }

    // 240702 DropItem/Slot/Inventory 실험중
    public void PlaceItem(Item item, int posX, int posY)
    {
        if (!m_selectedItmeGrid.BoundaryCheck(posX, posY, item.m_itemData.itemWidth, item.m_itemData.itemHeight))
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
                m_selectedItmeGrid.m_itemSlot[posX + x, posY + y] = item;
            }
        }

        item.m_onGridPositionX = posX;
        item.m_onGridPositionY = posY;

        Vector2 pos = new Vector2();
        if (item.m_onGridPositionX == 0)
        {
            pos.x = posX * 47.0f + 3;
            pos.y = -(posY * 47.0f) - 2;
        }
        else
        {
            pos.x = posX * 47.0f;
            pos.y = -(posY * 47.0f);
        }
        itemPos.localPosition = pos;
        //return true;
    }
}
