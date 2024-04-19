using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Q_SkillInputKey();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            W_SkillInputKey();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            E_SkillInputKey();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            R_SkillInputKey();
        }
    }

    protected virtual void Q_SkillInputKey()
    {
        // Q 스킬 창에 어떤 스킬이 있는지 확인
        // 애니메이션 실행
        // 데미지 주기
        // 스킬 바뀌었을 때 바뀐 스킬로 실행
    }

    protected virtual void W_SkillInputKey()
    {

    }

    protected virtual void E_SkillInputKey()
    {

    }

    protected virtual void R_SkillInputKey()
    {

    }
}
