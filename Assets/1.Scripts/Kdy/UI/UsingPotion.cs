using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IGetPotion
{
    void GetPotion(GameObject potionImage);
}

public interface IUsePotion
{
    void UsePotion();
}

public class UsingPotion : MonoBehaviour, IGetPotion, IUsePotion
{
    #region 실험용
    public GameObject m_potionImage;
    #endregion
    public PlayerUI m_playerUI;
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            GetPotion(m_potionImage);
        }
    }

    public void GetPotion(GameObject potionImage)
    {
        if (m_potion == null)
        {
            GameObject obj = ObjectPool.Inst.Pool<GameObject>(potionImage);
            m_potion = obj;
            m_potion.transform.SetParent(transform);
            RectTransform objRect = m_potion.GetComponent<RectTransform>();
            objRect.transform.localPosition = Vector3.zero;
            objRect.transform.localScale = Vector3.one;
            m_potionCnt++;
            m_countText.text = m_potionCnt.ToString();
            //m_playerUI.m_usingPotionAct += m_playerUI.Recovery;
        }
        else
        {
            // 포션 개수(텍스트)만 증가
            m_potionCnt++;
            m_countText.text = m_potionCnt.ToString();
        }
    }

    public void UsePotion()
    {
        if (m_potion == null) return;

        if (m_potionCnt <= 1)
        {
            m_potionCnt = 0;
            m_countText.text = m_potionCnt.ToString();
            ObjectPool.Inst.Push<GameObject>(m_potion);
            m_potion = null;
        }
        else
        {
            // 포션 사용
            m_potionCnt--;
            m_countText.text = m_potionCnt.ToString();
        }
    }
}
