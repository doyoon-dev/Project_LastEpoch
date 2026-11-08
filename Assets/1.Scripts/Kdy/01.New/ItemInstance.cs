using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance
{
    public NewItemData data;

    public int posX;
    public int posY;

    public ItemInstance(NewItemData data)
    {
        this.data = data;
    }
}
