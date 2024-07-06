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
    [SerializeField]
    GameObject m_inventory;
    bool m_isInvenOpen = false;

    public Slot m_selectedItmeGrid;
    public GameObject m_equipSlot;
    Item m_selectedItem;

    ItemData m_itemData;

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

        //if(Input.GetMouseButtonDown(0))
        //{
        //    Vector2Int tileGridPosition = m_selectedItmeGrid.GetTileGridPosition(Input.mousePosition);
        //}
    }

    // 240702 DropItem/Slot/Inventory Ω««Ë¡ﬂ
    public void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = m_selectedItmeGrid.PlaceItem(m_selectedItem, tileGridPosition.x, tileGridPosition.y);
        if (complete)
        {
            m_selectedItem = null;
        }
    }

    public void GetItemData(GameObject itemPrefab)
    {
        ICreateItem ici = m_selectedItmeGrid.GetComponent<ICreateItem>();
        if(ici != null)
        {
            ici.CreateItem(itemPrefab);
        }
        //m_itemData = itemData;
        //m_selectedItmeGrid.PlaceItem(item, itemData);
    }
}
