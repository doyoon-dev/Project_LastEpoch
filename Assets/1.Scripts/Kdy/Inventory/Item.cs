using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemData;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.VolumeComponent;

public interface ISetInventory
{
    void SetInventory(Transform inven);
}

public interface IChangeParent
{
    void ChangeParent(Transform parent);
}

public interface IChangePos
{
    void ChangePos(Vector2 pos);
}

public interface IOrgPos
{
    Vector3 m_orgPos { get; }
}

public interface IEquipItemStat
{
    //event UnityAction<ItemData, bool> m_equipItemStat;
    event UnityAction<ItemData> m_equipItemStat;
}

public interface IItemInterface : IChangePos, IOrgPos, IEquipItemStat, ISetInventory
{

}

public class Item : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IItemInterface
{
    public event UnityAction m_unEquipItem = null;
    //public event UnityAction<ItemData, bool> m_equipItemStat = null;         // 아이템을 장착했을 때 유니티 이벤트 실행해서 BattleSystem에 있는 Stat 아이템 Stat에 따라 바꿔주기
    public event UnityAction<ItemData> m_equipItemStat = null;
    public Transform m_inventory = null;
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public int m_onGridPositionX;       // 인벤토리 내의 아이템 위치 x좌표
    public int m_onGridPositionY;       // 인벤토리 내의 아이템 위치 y좌표
    public GameObject m_frameImage;
    

    public Transform m_orgPosition { get; private set; }  // 원래 위치
    public Vector3 m_orgPos { get; private set; }

    //bool m_isEquiped = false;

    EquipSlot m_equipSlot;              // 장착할 아이템이 들어갈 장비 슬롯

    Item m_curItem;
    Vector2 m_dragOffset = Vector2.zero;
    public Image m_image;
    Transform m_parentPos;
    public bool m_equipedItem = false;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        m_orgPos = transform.position;
        m_parentPos = transform.parent;
        m_dragOffset = (Vector2)transform.position - eventData.position;
        transform.SetParent(transform.parent.parent);
        m_image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        transform.position = eventData.position + m_dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        transform.position = m_orgPos;
        transform.SetParent(m_parentPos);
        m_image.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ISetItemInform isii = m_inventory.GetComponent<Inventory>().m_itemInform.GetComponent<ISetItemInform>();
        if (isii != null)
        {
            isii.SetItemInform(m_itemData);
        }
        m_inventory.GetComponent<Inventory>().m_itemInform.m_informUIObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_inventory.GetComponent<Inventory>().m_itemInform.m_informUIObj.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 아이템 우클릭 했을 때
        // 1. 장착슬롯이 비어있고 슬롯에 있는 아이템을 장착하기 위해 우클릭 했을 경우 [ if(m_equipSlot.m_item == null) ]
        // 2. 장착슬롯에 있는 아이템을 해제하기 위해 장착 슬롯 아이템을 우클릭 했을 경우 [ if(m_equipSlot.m_item != null) || if(!m_equipSlot.GetComponent<IIsEquiped>().m_isEquiped)]
        // 3. 아이템이 장착되어 있고 다른 아이템으로 교체하기위해 슬롯에 있는 같은 종류의 아이템을 우클릭 했을 경우 [ if(m_equipSlot.GetComponent<IIsEquiped>().m_isEquiped) ]
        //     ==> 1.장착아이템 해제 후 빈슬롯에 넣음
        //         2.슬롯에 있던 우클릭한 아이템 장착
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // m_isEquiped 변수는 현재 스크립트에 해당하는 아이템에만 적용돼서 이미 아이템을 장착한 상태여도 다른 아이템을 장착하면 그 아이템의 변수 m_isEquiped는 false 이므로
            // 아래코드가 호출된다. -> 플레이어 쪽 또는 EquipSlot 쪽에 장착했을 때 변수를 만들어야함
            // 처음 아이템을 장착할 때 순서상 m_equipSlot이 할당이 안된 상태임
            // 장착 아이템 없을 때 아이템 장착
            IMakeSlotEmpty imse = m_inventory.GetComponent<IMakeSlotEmpty>();
            if (!m_equipSlot.GetComponent<IIsEquiped>().m_isEquiped)
            {
                Item item = eventData.pointerClick.GetComponent<Item>();    // 슬롯에 있던 아이템
                CheckItemSlotType(item);
                // 아이템 장착할 때 아이템이 있던 슬롯 비우기
                //IMakeSlotEmpty imse = m_inventory.GetComponent<IMakeSlotEmpty>();
                if (imse != null)
                {
                    imse.MakeSlotEmpty(item);
                }
                // 이부분 안쓰이는것 같아서 지워야 할 수 있음
                if (m_equipSlot.m_item != null)   // 아이템 교체 함수 넣기 (m_equipSlot.m_item : 장비슬롯에 장착된 아이템)
                {
                    // 장착아이템 교체
                    ChangeEquipItem(m_equipSlot.m_item);
                }
                //EquipItem(item);
                SetEquip();
            }
            // 장착 아이템만 해제
            else if(m_equipedItem)
            {
                if (imse != null)
                {
                    imse.MakeSlotEmpty(this);
                }
                UnEquipeItem(m_equipSlot.m_item);
            }
            // 아이템 교체
            else
            {
                if (imse != null)
                {
                    imse.MakeSlotEmpty(this);
                }
                // 장착 중인 아이템을 우클릭해서 장착 해제 했을 경우
                // 슬롯에 있는 다른아이템을 우클릭해서 교체를 위해 장착중인 아이템 해제 했을 경우 나뉘어야 함
                UnEquipeItem(m_equipSlot.m_item);
                SetEquip();
            }
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("아이템 위치 : " + eventData.position);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_itemData.itemType == ItemType.Potion)
        {
            return;
        }
        else
        {
            gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            //m_inventory = transform.parent.parent.parent;    // 인벤토리 변수
            m_image = gameObject.GetComponent<Image>();

            m_equipSlot = CheckItemSlotType(this);
            //ISetStatus iss = m_equipSlot.m_battleSystem.GetComponent<ISetStatus>();
            //if (iss != null)
            //{
            //    m_equipItemStat += iss.SetStatus;
            //}
            
            //ISetItemEquipSlot isies = m_inventory.GetComponent<ISetItemEquipSlot>();
            //if(isies != null)
            //{
            //    m_equipSlot = isies.SetItemEquipSlot(this);
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 아이템이 장착 될 슬롯 찾는 함수
    EquipSlot CheckItemSlotType(Item item)
    {
        switch (item.m_itemData.itemType)
        {
            case ItemType.Head:
                CheckSlot(0);
                break;
            case ItemType.Necklace:
                CheckSlot(1);
                break;
            case ItemType.Weapon:
                CheckSlot(2);
                break;
            case ItemType.Armor:
                CheckSlot(3);
                break;
            case ItemType.Sheild:
                CheckSlot(4);
                break;
            case ItemType.Belt:
                CheckSlot(5);
                break;
            case ItemType.Ring:
                CheckSlot(6); 
                break;
            case ItemType.Shoes:
                CheckSlot(8);
                break;
            case ItemType.Hand:
                CheckSlot(9);
                break;
        }
        return m_equipSlot;
    }

    EquipSlot CheckSlot(int i)
    {
        m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[i];
        if (m_equipSlot.m_itemType == ItemType.Ring)
        {
            if (m_equipSlot.m_item != null)
            {
                m_equipSlot = transform.parent.GetComponent<Slot>().m_equipSlot[7];
            }
        }
        return m_equipSlot; 
    }

    // 장비 장착했을 때 아이템 위치 설정
    void SetEquip()
    {
        SoundManager.Inst.PlaySfx("GetItem");
        IEquipItemStatUI iss = m_equipSlot.m_playerStatUI.GetComponent<IEquipItemStatUI>();
        if (iss != null)
        {
            m_equipItemStat += iss.EquipItemStat;
        }
        //m_isEquiped = true;
        m_frameImage.SetActive(false);
        m_equipedItem = true;
        EquipSlotItem(m_equipSlot);
        transform.SetParent(m_equipSlot.transform);

        //ISetStatus iss = m_equipSlot.m_battleSystem.GetComponent<ISetStatus>();
        //if (iss != null)
        //{
        //    m_equipItemStat += iss.SetStatus;
        //}
        //m_equipItemStat?.Invoke(m_itemData, true);            // BattleSystem 에서 캐릭터 Stat 바궈주는 이벤트
        m_equipItemStat?.Invoke(m_itemData);
        m_equipItemStat = null;

        //IEquipItemSetting ieis = m_equipSlot.m_battleSystem.GetComponent<IEquipItemSetting>();
        //if (ieis != null)
        //{
        //    ieis.EquipItemSetting(this);
        //}
        
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        //transform.position = Vector3.zero;
        //transform.localPosition = Vector3.zero;
    }

    // 장비슬롯에 장착한 아이템 장비슬롯에 저장하는 함수
    void EquipSlotItem(EquipSlot es)
    {
        ISetEquipItem sei = es.GetComponent<ISetEquipItem>();
        if (sei != null)
        {
            sei.SetEquipItem(this);
        }
    }

    // 장착 아이템 교체 함수
    void ChangeEquipItem(Item equipItem)
    {
        int equipX = equipItem.m_onGridPositionX;   // 장착중인 아이템이 슬롯에 있었을 때의 위치
        int equipY = equipItem.m_onGridPositionY;   // 장착중인 아이템이 슬롯에 있었을 때의 위치

        // 장착 해제 아이템 슬롯으로 이동
        IPlaceItem pi = m_inventory.transform.GetComponent<IPlaceItem>();
        if(pi != null)
        {
            pi.PlaceItem(equipItem, m_onGridPositionX, m_onGridPositionY);
            // 문제
            //equipItem.m_isEquiped = false;
        }

        // 장착할 아이템 위치
        m_onGridPositionX = equipX;
        m_onGridPositionY = equipY;
    }

    // 장비 장착 해제 함수
    void UnEquipeItem(Item equipItem)
    {
        IFindEmptySlot fes = m_inventory.transform.GetComponent<IFindEmptySlot>();
        if (fes != null)
        {
            if(fes.FindEmptySlot(equipItem) == null) { return; }
            else
            {
                equipItem.m_onGridPositionX = fes.FindEmptySlot(equipItem).Value.x;
                equipItem.m_onGridPositionY = fes.FindEmptySlot(equipItem).Value.y;
            }
        }
        // 슬롯에 있는 아이템을 우클릭해서 장착 중인 아이템을 교체할 시
        // 슬롯에 있는 아이템의 m_unEquipItem에 함수를 추가해주지 않아서 m_unEquipItem == null 이라 함수 실행 안됨
        equipItem.m_unEquipItem?.Invoke();
        equipItem.m_unEquipItem = null;

        m_equipSlot.m_item = null;
        //IFindEmptySlot fes = m_inventory.transform.GetComponent<IFindEmptySlot>();
        //if (fes != null)
        //{
        //    equipItem.m_onGridPositionX = fes.FindEmptySlot(equipItem).Value.x;
        //    equipItem.m_onGridPositionY = fes.FindEmptySlot(equipItem).Value.y;
        //}
        IPlaceItem pi = m_inventory.transform.GetComponent<IPlaceItem>();
        if (pi != null)
        {
            pi.PlaceItem(equipItem, equipItem.m_onGridPositionX, equipItem.m_onGridPositionY);
        }
    }

    public void ChangePos(Vector2 pos)
    {
        // 이동한 아이템의 위치 지정
        m_orgPos = pos;
    }

    void ChangeParent(Transform parent)
    {
        m_parentPos = parent;
    }

    public void SetInventory(Transform inven)
    {
        m_inventory = inven;
    }
}
