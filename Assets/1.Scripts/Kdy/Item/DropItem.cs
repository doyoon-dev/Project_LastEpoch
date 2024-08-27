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
        IGetItemData igd = inven.GetComponent<IGetItemData>();
        if(igd != null)
        {
            igd.SetItemToInventory(m_itemImagePrefab);
        }
        ObjectPool.Inst.Push<Item>(gameObject);
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
