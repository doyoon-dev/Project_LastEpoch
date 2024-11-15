using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttack : MonoBehaviour
{
    public SkillData m_skillData;
    public Transform m_parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetKeyParent()
    {
        m_parent = transform.parent;
        IGetSkillData igsd = m_parent.GetComponent<IGetSkillData>();
        if (igsd != null)
        {
            igsd.GetSkillData(gameObject);
        }
    }
}
