using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    // ½½·Ô ÇÑ Ä­ »çÀÌÁî
    const float m_tileSizeWidth = 47.0f;
    const float m_tileSizeHeight = 47.0f;

    [SerializeField]
    int m_gridSizeWidth = 14;       // ½½·Ô °¡·Î °³¼ö
    [SerializeField]
    int m_gridSizeHeight = 8;       // ½½·Ô ¼¼·Î °³¼ö
    [SerializeField]
    GameObject m_itemPrefab;        // ½½·Ô¿¡ µé¾î°¥ ¾ÆÀÌÅÛ

    RectTransform m_rectTransform;
    Vector2 m_positionOnTheGrid = new Vector2();            // ½ºÅ©¸° ÁÂÇ¥ ±âÁØ ½½·Ô ÇÑ Ä­ ÁÂÇ¥
    Vector2Int m_tileGridPosition = new Vector2Int();       // ½½·Ô ±âÁØ ½½·Ô ÇÑ Ä­ ÁÂÇ¥

    Item[,] m_itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();
        Init(m_gridSizeWidth, m_gridSizeHeight);
        Item item = Instantiate(m_itemPrefab).GetComponent<Item>();
        PlaceItem(item, 3, 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init(int width, int height)
    {
        m_itemSlot = new Item[width, height];
        Vector2 size = new Vector2(width * m_tileSizeWidth, height * m_tileSizeHeight);
        m_rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        m_positionOnTheGrid.x = mousePosition.x - m_rectTransform.position.x;
        m_positionOnTheGrid.y = m_rectTransform.position.y - mousePosition.y;

        m_tileGridPosition.x = (int)(m_positionOnTheGrid.x / m_tileSizeWidth);
        m_tileGridPosition.y = (int)(m_positionOnTheGrid.y / m_tileSizeHeight);

        return m_tileGridPosition;
    }

    public void PlaceItem(Item item, int posX, int posY)
    {
        RectTransform itemPos = item.GetComponent<RectTransform>();
        itemPos.SetParent(m_rectTransform);
        m_itemSlot[posX, posY] = item;

        Vector2 pos = new Vector2();
        pos.x = posX * m_tileSizeWidth;
        pos.y = -(posY * m_tileSizeHeight);

        itemPos.localPosition = pos;
    }
}
