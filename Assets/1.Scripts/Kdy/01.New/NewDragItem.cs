using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDragItem : MonoBehaviour
{
    public static NewDragItem Instance;
    public ItemInstance draggingItem;    // 현재 드래그 중인 아이템
    public int currentHoverX = -1;
    public int currentHoverY = -1;

    public RectTransform icon;           // 아이콘 이미지
    public Image iconImage;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }

        icon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (draggingItem != null)
        {
            icon.position = Input.mousePosition;
        }
    }

    public void StartDrag(ItemInstance item)
    {
        draggingItem = item;

        iconImage.sprite = item.data.itemImagePrefab;
        icon.sizeDelta = new Vector2(item.data.itemWidth * 64, item.data.itemHeight * 64);
        icon.gameObject.SetActive(true);
    }

    public void EndDrag()
    {
        draggingItem = null;
        icon.gameObject.SetActive(false);
    }
}
