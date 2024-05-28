using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Potion = -1,
        Head,
        Necklace,
        Weapon,
        Armor,
        Sheild,
        Belt,
        Ring,
        Shoes,
        Hand
    }

    public int itemWidth;
    public int itemHeight;
    public ItemType itemType;
}
