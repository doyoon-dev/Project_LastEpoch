using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;

public interface ISetParentEquipItem
{
    void SetParentEquipItem();
}

public class EquipSlot : MonoBehaviour, ISetParentEquipItem
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParentEquipItem()
    {

    }
}
