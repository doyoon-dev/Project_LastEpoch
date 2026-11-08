using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewInventoryUI : MonoBehaviour
{
    public NewInventoryGrid grid;
    public GameObject slotPrefab;
    public RectTransform slotParent;

    // Start is called before the first frame update
    void Start()
    {
        CreateSlots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 인벤토리 슬롯 생성
    void CreateSlots()
    {
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                GameObject obj = ObjectPool.Inst.Pull<GameObject>(slotPrefab, slotParent);
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * 64, y * 64);
                NewSlotUI slotUI = obj.GetComponent<NewSlotUI>();
                slotUI.Init(this, x, y);
            }
        }
    }

    // 슬롯 클릭 -> 드래그 시작 또는 드래그 종료
    public void OnSlotClick(int x, int y)
    {
        NewInventorySlot slot = grid.slots[x, y];

        // 드래그 중이 아닐 때 클릭 -> 아이템이 있으면 드래그 시작
        if (NewDragItem.Instance.draggingItem == null)
        {
            if (slot.isOccupied)
            {
                ItemInstance item = slot.occupyingItem;
                grid.RemoveItem(item);
                NewDragItem.Instance.StartDrag(item);
                RefreshUI();
            }
            return;
        }

        // 드래그 중이면 -> 드랍시도
        TryDropItem();
    }

    // 아이템 아이콘을 해당 슬롯에 놓을 수 있으면 놓기
    public void TryDropItem()
    {
        NewDragItem drag = NewDragItem.Instance;

        if (drag.currentHoverX == -1 || drag.currentHoverY == -1) return;

        ItemInstance item = drag.draggingItem;

        // 배치 가능 여부 확인
        if (grid.CanPlaceItem(item.data, drag.currentHoverX, drag.currentHoverY))
        {
            grid.PlaceItem(item, drag.currentHoverX, drag.currentHoverY);
            drag.EndDrag();
            RefreshUI();
        }
        else
        {
            // 실패 -> 원래 자리로 돌아가게
            drag.EndDrag();
            RefreshUI();
        }
    }

    // UI 갱신 (아이콘 표시)
    public void RefreshUI()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        CreateSlots();

        //foreach (ItemInstance item in grid.GetAllItems())
        //{
        //    CreateItemIcon(item);
        //}
    }

    void CreateItemIcon(ItemInstance item)
    {
        GameObject obj = new GameObject("ItemIcon");
        obj.transform.SetParent(slotParent, false);

        Image img = obj.AddComponent<Image>();
        img.sprite = item.data.itemImagePrefab;

        RectTransform rect = img.rectTransform;
        rect.sizeDelta = new Vector2(item.data.itemWidth * 64, item.data.itemHeight * 64);

        // 아이템이 차지하는 첫 슬롯 기준 위치
        RectTransform baseSlot = GetSlotUIRect(item.posX, item.posY);
        rect.anchoredPosition = baseSlot.anchoredPosition;
    }

    RectTransform GetSlotUIRect(int x, int y)
    {
        int index = y + x * grid.height;
        return slotParent.GetChild(index).GetComponent<RectTransform>();
    }
}
