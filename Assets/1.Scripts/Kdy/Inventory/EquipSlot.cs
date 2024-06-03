using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;

public interface ISetEquipItem
{
    void SetEquipItem(Item item);
}

public class EquipSlot : MonoBehaviour, ISetEquipItem
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

    public ItemType m_itemType;
    public Item m_item = null;
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
        if (m_item == null)
        {
            m_item = item;
        }
    }
}
