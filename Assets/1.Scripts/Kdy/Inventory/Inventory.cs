using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Slot : ItemGrid 
    // Item : InventoryItem
    public Slot m_selectedItmeGrid;
    Item m_selectedItem;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(m_selectedItmeGrid == null) { return; }

        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int tileGridPosition = m_selectedItmeGrid.GetTileGridPosition(Input.mousePosition);
        }
        
    }

}
