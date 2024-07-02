using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropItem : MonoBehaviour
{
    public LayerMask m_itemMask;
    public ItemData m_itemData;
    public UnityAction<string> m_getItemAct;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckDropItem();
    }

    void CheckDropItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_itemMask)) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                string itemName = m_itemData.itemName;
                m_getItemAct?.Invoke(itemName);
                m_getItemAct = null;
                // 오브젝트 풀링으로 몬스터에서 아이템 소환하고 여기서 아이템 다시 풀에 넣기
            }
        }
    }
}
