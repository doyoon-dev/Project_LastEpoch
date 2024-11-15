using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface ISetItemInform
{
    void SetItemInform(ItemData data);
}

public class ItemInform : MonoBehaviour, ISetItemInform
{
    [SerializeField]
    public GameObject m_informUIObj;
    [SerializeField]
    TextMeshProUGUI m_itemName;
    [SerializeField]
    TextMeshProUGUI m_healthValue;
    [SerializeField]
    TextMeshProUGUI m_AtkValue;
    [SerializeField]
    TextMeshProUGUI m_DefValue;

    // Start is called before the first frame update
    void Start()
    {
        #region 스킬 데미지 증가 예시
        // 아이템 장착시 스킬 데미지도 올리기 아래처럼 하면 될 듯
        //Debug.Log("장착전 데미지 : " + SkillDataManager.m_skillDataDic["Lunge"].Dmg);
        //SkillDataManager.m_skillDataDic["Lunge"].Dmg += 10;
        //Debug.Log("장착후 데미지 : " + SkillDataManager.m_skillDataDic["Lunge"].Dmg);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetItemInform(ItemData data)
    {
        m_itemName.text = data.name;
        m_healthValue.text = data.recoveryAmount.ToString();
        m_AtkValue.text = data.atkPower.ToString();
        m_DefValue.text = data.defense.ToString();
    }
}
