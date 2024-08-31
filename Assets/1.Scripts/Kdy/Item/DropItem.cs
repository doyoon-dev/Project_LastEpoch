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

    public void Launch(Vector3 launchForce)
    {
        // Rigidbody 컴포넌트를 가져와서 힘을 가합니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(launchForce, ForceMode.Impulse);
        }
    }
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
            igd.SetItemToInventory(m_itemImagePrefab, gameObject);
        }
        // 오브젝트 풀링으로 몬스터에서 아이템 소환하고 여기서 아이템 다시 풀에 넣기
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
