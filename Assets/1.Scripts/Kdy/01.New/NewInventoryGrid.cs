using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewInventoryGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 6;

    public NewInventorySlot[,] slots;


    private void Awake()
    {
        // NewInventorySlot MonoBehaviour가 아닌 일반 클래스로 생성했는데 다른 스크립트에서 new 로 생성할 수 있는 이유?
        slots = new NewInventorySlot[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                slots[x, y] = new NewInventorySlot();
            }
        }
    }

    // 배치 가능한지 체크
    public bool CanPlaceItem(NewItemData item, int posX, int posY)
    {
        for (int x = 0; x < item.itemWidth; x++)
        {
            for (int y = 0; y < item.itemHeight; y++)
            {
                int checkX = posX + x;
                int checkY = posY + y;

                // 그리드 벗어남
                if(checkX >= width || checkY >= height) return false;

                // 이미 점유됨
                if (slots[checkX, checkY].isOccupied) return false;
            }
        }

        return true;
    }

    // 실제로 아이템 배치
    public void PlaceItem(ItemInstance item, int posX, int posY)
    {
        item.posX = posX;
        item.posY = posY;

        // 아이템의 가로 크기
        for (int x = 0; x < item.data.itemWidth; x++)
        {
            // 아이템의 세로 크기
            for (int y = 0; y < item.data.itemWidth; y++)
            {
                slots[posX + x, posY + y].isOccupied = true;
                slots[posX + x, posY + y].occupyingItem = item;
            }
        }
    }

    // 아이템 제거
    public void RemoveItem(ItemInstance item)
    {
        // 아이템의 가로 크기
        for (int x = 0; x < item.data.itemWidth; x++)
        {
            // 아이템의 세로 크기
            for (int y = 0; y < item.data.itemWidth; y++)
            {
                slots[item.posX + x, item.posY + y].isOccupied = false;
                slots[item.posX + x, item.posY + y].occupyingItem = null;
            }
        }
    }

}