using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface IHighlightUI
{
    void HighlightUI(bool on);
}

public class ItemNameUI : MonoBehaviour, IHighlightUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Transform m_targetNamePos;
    GameObject m_itemObj;
    ItemData m_itemData;
    GameObject m_dropItem;
    public Text m_itemName;
    public Image m_mouseAlpha;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightUI(true);
        Debug.Log(m_itemName.text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightUI(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ICheckDropItem icp = m_dropItem.transform.GetComponent<ICheckDropItem>();
        if (icp != null)
        {
            icp.CheckDropItem(SceneData.Inst.m_inventory, SceneData.Inst.m_playerHpMpUI, gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_itemName.text = m_itemData.itemName;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_targetNamePos != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(m_targetNamePos.position);
            transform.position = pos;
        }
    }

    public void Initialize(Transform target, ItemData data, GameObject dropItem)
    {
        m_mouseAlpha.transform.gameObject.SetActive(false);
        m_dropItem = dropItem;
        m_targetNamePos = target;
        m_itemData = data;
        m_itemName.text = m_itemData.itemName;
    }

    public void HighlightUI(bool on)
    {
        if (on)
        {
            m_mouseAlpha.transform.gameObject.SetActive(true);
        }
        else
        {
            m_mouseAlpha.transform.gameObject.SetActive(false);
        }
        
    }
}
