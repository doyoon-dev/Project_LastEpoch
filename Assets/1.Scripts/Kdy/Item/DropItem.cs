using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface ICheckDropItem
{
    void CheckDropItem(Inventory inven);
}

// 획득 아이템 인벤토리에 List에 저장하는 코드 테스트 중
public interface ICheckDropItemTest
{
    void CheckDropItemTest(Inventory inven);
}

public class DropItem : MonoBehaviour, ICheckDropItem, ICheckDropItemTest
{
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public float dropChance;//드랍 확률

    public UnityAction<string> m_getItemAct;
    public GameObject m_itemImagePrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckDropItem(Inventory inven)
    {
        // 아이템을 인벤토리에 넣기 전에 공간이 있는지 확인하고
        // 공간이 있으면 인벤토리에 넣고
        // 공간이 없으면 아이템 획득 불가능하게 만들기
        IFindEmptySlot ifes = inven.GetComponent<IFindEmptySlot>();
        if (ifes != null)
        {
            if (ifes.FindEmptySlot(m_itemImagePrefab.GetComponent<Item>()) != null)
            {
                IGetItemData igd = inven.GetComponent<IGetItemData>();
                if (igd != null)
                {
                    igd.SetItemToInventory(m_itemImagePrefab);
                }
                ObjectPool.Inst.Push<Item>(gameObject);
            }
            else
            {
                // 슬롯 공간이 없을 경우 처리
            }
        }
    }

    // 획득 아이템 인벤토리에 List에 저장하는 코드 테스트 중
    public void CheckDropItemTest(Inventory inven)
    {
        IGetItemToList igitl = inven.GetComponent<IGetItemToList>();
        if(igitl != null)
        {
            igitl.GetItemToList(m_itemData);
        }
    }

    public void Initialize(ItemData itemData)
    {
        m_itemData = itemData;
    }
}
