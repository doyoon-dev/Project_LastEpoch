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
            Debug.Log(tileGridPosition);
            Debug.Log("------------------------------------------------------");
        }
        
        
    }
}
