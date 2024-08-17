using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface ICheckDropItem
{
    void CheckDropItem(Inventory inven);
}

public class DropItem : MonoBehaviour, ICheckDropItem
{
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public float dropChance;//드랍 확률

    public UnityAction<string> m_getItemAct;
    public GameObject m_slotItemPrefab;
    Inventory m_inventory;
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
        m_inventory = inven;
        IGetItemData igd = m_inventory.GetComponent<IGetItemData>();
        if(igd != null)
        {
            igd.GetItemData(m_slotItemPrefab);
        }
        // 오브젝트 풀링으로 몬스터에서 아이템 소환하고 여기서 아이템 다시 풀에 넣기
    }
    public void Initialize(ItemData itemData)
    {
        m_itemData = itemData;
        // 필요한 초기화 코드 추가
    }



}
