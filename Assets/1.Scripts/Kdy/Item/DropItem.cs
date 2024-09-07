using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface ICheckDropItem
{
    void CheckDropItem(Inventory inven, PlayerUI ui);
}

// 획득 아이템 인벤토리에 List에 저장하는 코드 테스트 중
public interface ICheckDropItemTest
{
    void CheckDropItemTest(Inventory inven);
}

public class DropItem : MonoBehaviour, ICheckDropItem//, ICheckDropItemTest
{
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public float dropChance;//드랍 확률

    public UnityAction<string> m_getItemAct;
    public GameObject m_itemImagePrefab;


    public float lifetime = 10f; // 인벤토리에 들어가지 않은 아이템이 사라지기 전까지의 시간(성원)
    private Coroutine lifetimeCoroutine; //(성원)
    private bool isPickedUp = false; // 아이템이 인벤토리에 들어갔는지 여부 확인(성원)


    // Start is called before the first frame update
    //아이템 발사하는 메서드
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
        // 아이템이 활성화될 때 생명주기 타이머 시작(성원)
        lifetimeCoroutine = StartCoroutine(StartLifetimeTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 아이템의 생명주기를 관리하는 코루틴
    private IEnumerator StartLifetimeTimer()
    {
        yield return new WaitForSeconds(lifetime); // 일정 시간 대기
        if (!isPickedUp) // 아이템이 인벤토리에 들어가지 않은 경우에만 처리
        {
            ObjectPool.Inst.Push<DropItem>(gameObject); // 객체 풀로 아이템 반환
        }
    }

    public void CheckDropItem(Inventory inven, PlayerUI ui)
    {
        if(m_itemData.itemType == ItemData.ItemType.Potion)
        {
            IGetPotion igp = ui.m_potionFlame.GetComponent<IGetPotion>();
            if (igp != null)
            {
                igp.GetPotion(m_itemImagePrefab);
                IGetItemData igd = inven.GetComponent<IGetItemData>();
                if (igd != null)
                {
                    igd.SetItemToInventory(m_itemImagePrefab);
                }

                isPickedUp = true; // 아이템이 인벤토리에 들어갔음을 표시(성원)
                ObjectPool.Inst.Push<Item>(gameObject); // 객체 풀로 아이템 반환
                if (lifetimeCoroutine != null) StopCoroutine(lifetimeCoroutine); // 타이머 정지(성원)
            }
            else
            {
                // 슬롯 공간이 없을 경우 처리
            }
        }
        else
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

    }

    // 획득 아이템 인벤토리에 List에 저장하는 코드 테스트 중
    //public void CheckDropItemTest(Inventory inven)
    //{
    //    IGetItemToList igitl = inven.GetComponent<IGetItemToList>();
    //    if(igitl != null)
    //    {
    //        igitl.GetItemToList(m_itemData);
    //    }
    //}

    public void Initialize(ItemData itemData)
    {
        m_itemData = itemData;
    }

    

}
