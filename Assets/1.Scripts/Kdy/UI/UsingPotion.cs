using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGetPotion
{
    void GetPotion(GameObject potionImage);
}

public class UsingPotion : MonoBehaviour, IGetPotion
{
    public GameObject m_potion = null;
    public int m_potionCnt = 0;
    public Text m_countText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPotion(GameObject potionImage)
    {
        if (m_potion == null)
        {
            m_potion = potionImage;
            ObjectPool.Inst.Pool<GameObject>(potionImage);
            m_potionCnt++;
        }
        else
        {
            // 포션 개수(텍스트)만 증가
            m_potionCnt++;
            m_countText.text = m_potion.ToString();
        }
    }

    public void UsePotion()
    {
        if (m_potion == null) return;

        if (m_potionCnt == 0)
        {
            m_potion = null;
            ObjectPool.Inst.Push<GameObject>(m_potion);
        }
        else
        {
            m_potionCnt--;
            m_countText.text = m_potion.ToString();
        }
    }
}
