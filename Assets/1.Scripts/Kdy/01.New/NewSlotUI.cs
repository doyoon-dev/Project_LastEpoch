using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int x;
    public int y;

    NewInventoryUI inventoryUI;

    public void Init(NewInventoryUI ui, int x, int y)
    {
        inventoryUI = ui;
        this.x = x;
        this.y = y;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (NewDragItem.Instance.draggingItem != null)
        {
            NewDragItem.Instance.currentHoverX = x;
            NewDragItem.Instance.currentHoverY = y;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NewDragItem.Instance.currentHoverX = -1;
        NewDragItem.Instance.currentHoverY = -1;
    }

    // 클릭 시 아이템 정보 확인 이벤트 추가
    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryUI.OnSlotClick(x, y);
    }
}
