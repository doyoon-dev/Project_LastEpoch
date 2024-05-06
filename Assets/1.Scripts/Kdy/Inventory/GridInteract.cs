using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Inventory m_inventory;
    Slot m_slot;
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_inventory.m_selectedItmeGrid = m_slot;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_inventory.m_selectedItmeGrid = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_slot = GetComponent<Slot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
