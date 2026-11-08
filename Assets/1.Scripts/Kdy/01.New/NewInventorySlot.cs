using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewInventorySlot
{
    public bool isOccupied = false;                 // 아이템이 이 칸을 점유하는지 여부
    public ItemInstance occupyingItem = null;       // 어떤 아이템이 차지 중인지 저장
}
