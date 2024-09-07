using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ItemData;

public interface ISetEquipItem
{
    void SetEquipItem(Item item);
}

public interface IIsEquiped
{
    bool m_isEquiped { get; }
}

public class EquipSlot : MonoBehaviour, ISetEquipItem, IIsEquiped
{
    public enum Equip
    {
        Head = 0,
        Necklace,
        Weapon,
        Armor,
        Sheild,
        Belt,
        Ring1,
        Ring2,
        Shoes,
        Hand
    }

    [SerializeField]
    Image m_bgImage;

    public ItemType m_itemType;
    public Item m_item = null;
    public BattleSystem m_battleSystem;
    public bool m_isEquiped { get; private set; }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEquipItem(Item item)
    {
        // 아이템 교체할 때 m_item을 null로 만들고 교체할 아이템을 m_item에 넣기
        m_item = item;
        m_isEquiped = true;
        m_bgImage.gameObject.SetActive(false);
        m_item.transform.position = Vector3.zero;
        m_item.GetComponent<RectTransform>().localScale = Vector3.one;
        m_item.m_unEquipItem += UnEquipedItem;
    }

    void UnEquipedItem()
    {
        //ISetStatus iss = m_battleSystem.GetComponent<ISetStatus>();
        //if (iss != null)
        //{
        //    // 아이템 교체시 에러나는 부분
        //    m_item.m_equipItemStat += iss.SetStatus;
        //}
        m_item.m_equipedItem = false;
        m_item.m_frameImage.SetActive(true);
        m_bgImage.gameObject.SetActive(true);
        m_isEquiped = false;
    }
}
